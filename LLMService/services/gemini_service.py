import json
from groq import Groq
from config import GROQ_API_KEY, GROQ_MODEL
from services.tool_definitions import TOOLS
from services.tool_executor import execute_tool
from services.rag_service import retrieve_context

client = Groq(api_key=GROQ_API_KEY)

SYSTEM_INSTRUCTION = """You are the Let's Go Biking assistant, a bilingual (French/English) guide for planning bike-sharing routes in French and European cities. You help users find bike stations, calculate routes, and understand the JCDecaux bike-sharing network.

LANGUAGE RULES:
- Reply in the same language the user writes in. If they write in French, reply in French. If in English, reply in English.
- Understand city names, landmarks, and addresses in both languages.

TOOL USAGE:
- When a user asks about a route between two places, use the get_itinerary tool with their origin and destination as full addresses (e.g. "Gare Part-Dieu, Lyon, France").
- When they ask about bike availability or nearest stations, use get_stations with the city name.
- When a user mentions a landmark, neighborhood, or station name, infer the city and build a proper address for the tool call.
- If the user asks for "nearest station" or stations with specific availability (e.g. "at least 5 bikes"), call get_stations for the relevant city, then filter and present the best matches.

SUPPORTED CITIES: lyon, paris, rouen, toulouse, nancy, nantes, amiens, marseille, lille, bruxelles, valence, cergy-pontoise, creteil, luxembourg, mulhouse, besancon.

RESPONSE FORMATTING:
- Format responses clearly with route segments, distances, and times.
- Distinguish between walking and biking segments with clear labels.
- If a route is inter-city, warn that the walking gap between cities is not practical and suggest public transport (train, bus).
- For station queries, highlight station name, available bikes, available stands, and status.
- For questions about timing (e.g. "best time to bike"), give general advice about rush hours (7-9h, 17-19h) and station availability patterns.
- For terrain/hill questions, acknowledge the request and suggest the flattest route when possible, noting that the routing system finds optimal paths.

Be concise but helpful. Use simple, clear language. Use bullet points or numbered lists for readability."""

# Build OpenAI-compatible tool declarations for Groq
_groq_tools = [
    {
        "type": "function",
        "function": {
            "name": tool["name"],
            "description": tool["description"],
            "parameters": tool["parameters"],
        },
    }
    for tool in TOOLS
]


def _build_history(history: list[dict]) -> list[dict]:
    """Convert chat history to Groq message format."""
    messages = []
    for msg in history:
        role = "user" if msg["role"] == "user" else "assistant"
        messages.append({"role": role, "content": msg["content"]})
    return messages


async def generate_response(
    message: str, history: list[dict] = None
) -> tuple[str, list[str], dict | None, list[dict] | None]:
    """
    Generate a response using Groq with tool calling.
    Returns: (reply_text, tool_calls_made, itinerary_data, stations_data)
    """
    rag_context = retrieve_context(message)

    augmented_message = message
    if rag_context:
        augmented_message = (
            f"[Context from knowledge base]\n{rag_context}\n\n"
            f"[User question]\n{message}"
        )

    messages = [{"role": "system", "content": SYSTEM_INSTRUCTION}]
    messages.extend(_build_history(history or []))
    messages.append({"role": "user", "content": augmented_message})

    tool_calls_made = []
    itinerary_data = None
    stations_data = None

    # Handle tool calling loop
    max_iterations = 5
    iteration = 0
    while iteration < max_iterations:
        iteration += 1

        response = client.chat.completions.create(
            model=GROQ_MODEL,
            messages=messages,
            tools=_groq_tools,
            tool_choice="auto",
        )

        choice = response.choices[0]

        if choice.finish_reason == "tool_calls" or (
            choice.message.tool_calls and len(choice.message.tool_calls) > 0
        ):
            # Append the assistant message with tool calls
            messages.append(choice.message)

            for tool_call in choice.message.tool_calls:
                fn_name = tool_call.function.name
                fn_args = json.loads(tool_call.function.arguments)

                print(f"[Groq] Tool call: {fn_name}({fn_args})")
                tool_calls_made.append(fn_name)

                result = await execute_tool(fn_name, fn_args)

                # Track itinerary/station data for frontend
                if fn_name in ("get_itinerary", "get_itinerary_by_coords"):
                    itinerary_data = result
                elif fn_name == "get_stations":
                    stations_data = result

                # Send tool result back
                messages.append(
                    {
                        "role": "tool",
                        "tool_call_id": tool_call.id,
                        "content": json.dumps(result, default=str),
                    }
                )
        else:
            break

    # Extract final text
    reply = choice.message.content or ""

    return reply, tool_calls_made, itinerary_data, stations_data
