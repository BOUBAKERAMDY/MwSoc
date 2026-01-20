from pydantic import BaseModel


class StationInfo(BaseModel):
    Name: str | None = None
    Address: str | None = None
    Latitude: float = 0
    Longitude: float = 0
    AvailableBikes: int = 0
    AvailableStands: int = 0
    Status: str | None = None
    ContractName: str | None = None


class ItineraryDto(BaseModel):
    StartStation: str | None = None
    EndStation: str | None = None
    WalkToStartMeters: float = 0
    BikeMeters: float = 0
    WalkToEndMeters: float = 0
    TotalKm: float = 0
    TotalSeconds: float = 0
    Note: str | None = None
    OriginCity: str | None = None
    DestinationCity: str | None = None
    IsInterCity: bool = False
    IsWalkingOnly: bool = False
    OriginLat: float = 0
    OriginLon: float = 0
    DestLat: float = 0
    DestLon: float = 0
    StartStationDetails: StationInfo | None = None
    EndStationDetails: StationInfo | None = None


class ChatMessage(BaseModel):
    role: str
    content: str


class ChatRequest(BaseModel):
    message: str
    history: list[ChatMessage] = []


class ChatResponse(BaseModel):
    reply: str
    itinerary: dict | None = None
    stations: list[dict] | None = None
    tool_calls_made: list[str] = []
