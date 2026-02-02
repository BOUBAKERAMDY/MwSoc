from fastapi import APIRouter
from models.schemas import ChatRequest, ChatResponse
from services.gemini_service import generate_response

router = APIRouter()


@router.post("/chat", response_model=ChatResponse)
async def chat(request: ChatRequest):
    history = [{"role": m.role, "content": m.content} for m in request.history]

    reply, tool_calls, itinerary, stations = await generate_response(
        request.message, history
    )

    return ChatResponse(
        reply=reply,
        itinerary=itinerary,
        stations=stations,
        tool_calls_made=tool_calls,
    )
