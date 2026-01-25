TOOLS = [
    {
        "name": "get_itinerary",
        "description": (
            "Calculate a bike-sharing route between two addresses in France. "
            "Returns walking segments, biking segments, station details, "
            "total distance and estimated time. Works for intra-city and inter-city routes."
        ),
        "parameters": {
            "type": "object",
            "properties": {
                "origin": {
                    "type": "string",
                    "description": "Starting address or location, e.g. 'Gare Part-Dieu, Lyon'",
                },
                "destination": {
                    "type": "string",
                    "description": "Destination address or location",
                },
            },
            "required": ["origin", "destination"],
        },
    },
    {
        "name": "get_itinerary_by_coords",
        "description": "Calculate a bike-sharing route using exact GPS coordinates.",
        "parameters": {
            "type": "object",
            "properties": {
                "origin_lat": {"type": "number", "description": "Origin latitude"},
                "origin_lon": {"type": "number", "description": "Origin longitude"},
                "dest_lat": {"type": "number", "description": "Destination latitude"},
                "dest_lon": {"type": "number", "description": "Destination longitude"},
            },
            "required": ["origin_lat", "origin_lon", "dest_lat", "dest_lon"],
        },
    },
    {
        "name": "get_stations",
        "description": (
            "Get all bike-sharing stations in a city with real-time availability. "
            "Supported cities: lyon, paris, rouen, toulouse, nancy, nantes, amiens, "
            "marseille, lille, bruxelles, valence, cergy-pontoise, creteil, "
            "luxembourg, mulhouse, besancon."
        ),
        "parameters": {
            "type": "object",
            "properties": {
                "city": {
                    "type": "string",
                    "description": "City name in lowercase, e.g. 'lyon', 'paris'",
                },
            },
            "required": ["city"],
        },
    },
]
