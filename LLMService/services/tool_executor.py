import json
from services import backend_client


async def execute_tool(function_name: str, arguments: dict) -> dict:
    try:
        if function_name == "get_itinerary":
            return await backend_client.get_itinerary(
                arguments["origin"], arguments["destination"]
            )
        elif function_name == "get_itinerary_by_coords":
            return await backend_client.get_itinerary_by_coords(
                arguments["origin_lat"],
                arguments["origin_lon"],
                arguments["dest_lat"],
                arguments["dest_lon"],
            )
        elif function_name == "get_stations":
            return await backend_client.get_stations(arguments["city"])
        else:
            return {"error": f"Unknown tool: {function_name}"}
    except Exception as e:
        return {"error": str(e)}
