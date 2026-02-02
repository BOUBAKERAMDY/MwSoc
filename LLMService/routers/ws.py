import json
from fastapi import APIRouter, WebSocket, WebSocketDisconnect
from services.gemini_service import generate_response

router = APIRouter()


@router.websocket("/ws/chat")
async def websocket_chat(websocket: WebSocket):
    await websocket.accept()
    print("[WS] Client connected")

    try:
        while True:
            data = await websocket.receive_json()
            message = data.get("message", "")
            history = data.get("history", [])

            if not message:
                await websocket.send_json({"type": "error", "content": "Empty message"})
                continue

            await websocket.send_json({"type": "thinking", "content": "Processing..."})

            try:
                reply, tool_calls, itinerary, stations = await generate_response(
                    message, history
                )

                # Send tool calls info
                for tool_name in tool_calls:
                    await websocket.send_json({
                        "type": "tool_call",
                        "name": tool_name,
                    })

                # Send itinerary data if available (for map rendering)
                if itinerary:
                    await websocket.send_json({
                        "type": "tool_result",
                        "name": "get_itinerary",
                        "data": itinerary,
                    })

                # Send stations data if available
                if stations:
                    await websocket.send_json({
                        "type": "tool_result",
                        "name": "get_stations",
                        "data": stations,
                    })

                # Send final reply
                await websocket.send_json({
                    "type": "done",
                    "content": reply,
                    "itinerary": itinerary,
                    "stations": stations,
                })

            except Exception as e:
                print(f"[WS] Error: {e}")
                await websocket.send_json({
                    "type": "error",
                    "content": f"Error: {str(e)}",
                })

    except WebSocketDisconnect:
        print("[WS] Client disconnected")
