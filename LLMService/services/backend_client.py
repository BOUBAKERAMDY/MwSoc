import httpx
from config import ROUTING_SERVER_URL, PROXY_SERVER_REST_URL

_client = httpx.AsyncClient(timeout=30.0)


async def get_itinerary(origin: str, destination: str) -> dict:
    url = f"{ROUTING_SERVER_URL}/Itinerary"
    params = {"origin": origin, "destination": destination}
    response = await _client.get(url, params=params)
    response.raise_for_status()
    return response.json()


async def get_itinerary_by_coords(
    origin_lat: float, origin_lon: float, dest_lat: float, dest_lon: float
) -> dict:
    url = f"{ROUTING_SERVER_URL}/ItineraryByCoords"
    params = {
        "originLat": str(origin_lat),
        "originLon": str(origin_lon),
        "destLat": str(dest_lat),
        "destLon": str(dest_lon),
    }
    response = await _client.get(url, params=params)
    response.raise_for_status()
    return response.json()


async def get_stations(city: str) -> list[dict]:
    url = f"{PROXY_SERVER_REST_URL}/GetStations"
    params = {"city": city}
    response = await _client.get(url, params=params)
    response.raise_for_status()
    return response.json()
