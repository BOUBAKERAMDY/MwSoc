const API_CONFIG = {
    NOMINATIM_URL: 'https://nominatim.openstreetmap.org/search',
    ROUTING_URL: 'http://localhost:8090/MyService/Itinerary',
    OSRM_BASE_URL: 'https://router.project-osrm.org/route/v1',
    LLM_WS_URL: 'ws://localhost:8000/ws/chat',
    ACTIVEMQ_WS_URL: 'ws://127.0.0.1:61614',
    ACTIVEMQ_TOPIC: '/topic/stationAlerts',
    MAP_CENTER: [45.764043, 4.835659],
    MAP_ZOOM: 13
};

let map, currentRouteLayer, markersLayer;
let searchTimeout;
let selectedOriginIndex = -1;
let selectedDestinationIndex = -1;
let stompClient = null;
let currentPickupStation = null;
let currentCity = null;
let currentRouteData = null;

let chatWs = null;
let chatHistory = [];
let chatOpen = false;

let interCityStations = {
    originStart: null,
    originEdge: null,
    destEdge: null,
    destEnd: null
};

function connectWebSocket() {
    try {
        const ws = new WebSocket(API_CONFIG.ACTIVEMQ_WS_URL, ['stomp']);
        stompClient = Stomp.over(ws);
        stompClient.debug = null;
        stompClient.heartbeat.outgoing = 20000;
        stompClient.heartbeat.incoming = 20000;
        
        stompClient.connect({},
            function(frame) {
                updateWSStatus('connected');
                stompClient.subscribe(API_CONFIG.ACTIVEMQ_TOPIC, function(message) {
                    try {
                        const alert = JSON.parse(message.body);
                        showStationAlert(
                            `Station Alert: ${alert.Message}`,
                            `Station: ${alert.StationName}\nCity: ${alert.City}\nBikes: ${alert.AvailableBikes}\nStands: ${alert.AvailableStands}`
                        );
                    } catch (e) {}
                });
            },
            function(error) {
                updateWSStatus('disconnected');
                setTimeout(connectWebSocket, 5000);
            }
        );
        
        ws.onclose = function() {
            updateWSStatus('disconnected');
            setTimeout(connectWebSocket, 5000);
        };
    } catch (e) {
        updateWSStatus('disconnected');
        setTimeout(connectWebSocket, 5000);
    }
}

function updateWSStatus(status) {
    const el = document.getElementById('ws-status');
    if (!el) return;
    el.className = `ws-status ${status}`;
    el.querySelector('span').textContent = status === 'connected' ? 'Live Updates' : 'Disconnected';
}

function showStationAlert(title, message) {
    document.getElementById('alert-title').textContent = title;
    document.getElementById('alert-message').textContent = message;
    document.getElementById('station-alert-popup').classList.add('show');
    setTimeout(() => closeStationAlert(), 30000);
}

function closeStationAlert() {
    document.getElementById('station-alert-popup').classList.remove('show');
}

function handleRecalculate() {
    closeStationAlert();
    calculateRoute();
}

function extractCityFromAddress(address) {
    const cities = ['lyon', 'paris', 'marseille', 'toulouse', 'nice', 'nantes', 'strasbourg'];
    const lower = address.toLowerCase();
    return cities.find(city => lower.includes(city)) || 'lyon';
}

function extractStationId(name) {
    const match = name.match(/^(\d+)/);
    return match ? match[1] : null;
}

function showTestSection() {
    if (!currentPickupStation) return;
    document.getElementById('test-station-name').textContent = `📍 ${currentPickupStation.name}`;
    document.getElementById('test-station-details').innerHTML = `
        <div>City: ${currentPickupStation.city}</div>
        <div>Station ID: ${currentPickupStation.id}</div>
        <div>Available Bikes: ${currentPickupStation.bikes}</div>
    `;
    document.getElementById('test-section').style.display = 'block';
}

function hideTestSection() {
    document.getElementById('test-section').style.display = 'none';
}

async function forceStationUnavailable() {
    const stationId = document.getElementById('test-station-id')?.value ||
        (currentPickupStation ? currentPickupStation.id : null);
    const city = document.getElementById('test-city')?.value ||
        (currentPickupStation ? currentPickupStation.city : null);

    if (!stationId || !city) {
        showTestResult('error', '✗ No station to force. Calculate route first or enter values manually.');
        return;
    }

    const btn = document.getElementById('force-unavailable-btn');
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Forcing...';

    const url = `http://localhost:9000/ProxyServiceRest/ForceStationUnavailable?stationId=${stationId}&city=${city}`;

    try {
        const response = await fetch(url, { method: 'POST' });
        const data = await response.json();
        if (data.Success) {
            showTestResult('success', `✓ Success! Wait for alert popup...`);
        } else {
            showTestResult('error', `✗ ${data.Message}`);
        }
    } catch (e) {
        showTestResult('error', `✗ ${e.message}`);
    }

    btn.disabled = false;
    btn.innerHTML = '<i class="fas fa-exclamation-triangle"></i> Force This Station Unavailable';
}

function showTestResult(type, message) {
    const el = document.getElementById('test-result');
    el.className = `test-result ${type} show`;
    el.innerHTML = message;
}

const MapIcons = {
    origin: L.divIcon({
        html: '<div style="background: #10b981; width: 32px; height: 32px; border-radius: 50%; display: flex; align-items: center; justify-content: center; border: 3px solid white; box-shadow: 0 3px 10px rgba(0,0,0,0.3);"><i class="fas fa-location-dot" style="color: white; font-size: 16px;"></i></div>',
        iconSize: [32, 32],
        iconAnchor: [16, 32],
        popupAnchor: [0, -32]
    }),
    destination: L.divIcon({
        html: '<div style="background: #ef4444; width: 32px; height: 32px; border-radius: 50%; display: flex; align-items: center; justify-content: center; border: 3px solid white; box-shadow: 0 3px 10px rgba(0,0,0,0.3);"><i class="fas fa-flag-checkered" style="color: white; font-size: 14px;"></i></div>',
        iconSize: [32, 32],
        iconAnchor: [16, 32],
        popupAnchor: [0, -32]
    }),
    bikeStation: L.divIcon({
        html: '<div style="background: #f59e0b; width: 28px; height: 28px; border-radius: 50%; display: flex; align-items: center; justify-content: center; border: 3px solid white; box-shadow: 0 3px 10px rgba(0,0,0,0.3);"><i class="fas fa-bicycle" style="color: white; font-size: 14px;"></i></div>',
        iconSize: [28, 28],
        iconAnchor: [14, 28],
        popupAnchor: [0, -28]
    })
};

async function getWalkingRoute(startLat, startLon, endLat, endLon) {
    try {
        const url = `${API_CONFIG.OSRM_BASE_URL}/foot/${startLon},${startLat};${endLon},${endLat}?overview=full&geometries=geojson`;
        const response = await fetch(url);
        if (!response.ok) return null;
        const data = await response.json();
        if (data.code === 'Ok' && data.routes && data.routes[0]) {
            return data.routes[0].geometry.coordinates.map(coord => [coord[1], coord[0]]);
        }
        return null;
    } catch (e) {
        return null;
    }
}

async function getCyclingRoute(startLat, startLon, endLat, endLon) {
    try {
        const url = `${API_CONFIG.OSRM_BASE_URL}/bike/${startLon},${startLat};${endLon},${endLat}?overview=full&geometries=geojson`;
        const response = await fetch(url);
        if (!response.ok) return null;
        const data = await response.json();
        if (data.code === 'Ok' && data.routes && data.routes[0]) {
            return data.routes[0].geometry.coordinates.map(coord => [coord[1], coord[0]]);
        }
        return null;
    } catch (e) {
        return null;
    }
}

function initializeMap() {
    map = L.map('map', {
        center: API_CONFIG.MAP_CENTER,
        zoom: API_CONFIG.MAP_ZOOM,
        zoomControl: false
    });

    L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
        attribution: '© OpenStreetMap contributors © CARTO',
        subdomains: 'abcd',
        maxZoom: 20
    }).addTo(map);

    L.control.zoom({ position: 'bottomleft' }).addTo(map);

    currentRouteLayer = L.layerGroup().addTo(map);
    markersLayer = L.layerGroup().addTo(map);
}

function setupAutocomplete() {
    const originInput = document.getElementById('origin-input');
    const destinationInput = document.getElementById('destination-input');

    originInput.addEventListener('input', (e) => {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => fetchSuggestions(e.target.value, 'origin'), 300);
    });

    destinationInput.addEventListener('input', (e) => {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => fetchSuggestions(e.target.value, 'destination'), 300);
    });

    originInput.addEventListener('keydown', (e) => handleKeyNavigation(e, 'origin'));
    destinationInput.addEventListener('keydown', (e) => handleKeyNavigation(e, 'destination'));

    document.addEventListener('click', (e) => {
        if (!e.target.closest('.input-group')) {
            closeAllDropdowns();
        }
    });
}

async function fetchSuggestions(query, field) {
    if (query.length < 3) {
        document.getElementById(`${field}-suggestions`).classList.remove('active');
        return;
    }

    try {
        const response = await fetch(
            `${API_CONFIG.NOMINATIM_URL}?q=${encodeURIComponent(query + ', France')}&format=json&addressdetails=1&limit=5`
        );
        const data = await response.json();
        data.sort((a, b) => (b.importance || 0) - (a.importance || 0));
        displaySuggestions(data, field);
    } catch (e) {}
}

function displaySuggestions(suggestions, field) {
    const container = document.getElementById(`${field}-suggestions`);

    if (suggestions.length === 0) {
        container.classList.remove('active');
        return;
    }

    container.innerHTML = suggestions.map((item, index) => {
        const mainText = item.display_name.split(',')[0];
        const subText = item.display_name.split(',').slice(1, 3).join(',');

        return `
            <div class="autocomplete-item" onclick="selectSuggestion('${field}', '${item.display_name.replace(/'/g, "\\'")}')">
                <i class="fas fa-map-marker-alt"></i>
                <div class="autocomplete-text">
                    <div class="autocomplete-main">${mainText}</div>
                    <div class="autocomplete-sub">${subText}</div>
                </div>
            </div>
        `;
    }).join('');

    container.classList.add('active');

    if (field === 'origin') selectedOriginIndex = -1;
    else selectedDestinationIndex = -1;
}

function selectSuggestion(field, value) {
    document.getElementById(`${field}-input`).value = value;
    document.getElementById(`${field}-suggestions`).classList.remove('active');
}

function handleKeyNavigation(event, field) {
    const container = document.getElementById(`${field}-suggestions`);
    const items = container.querySelectorAll('.autocomplete-item');
    let currentIndex = field === 'origin' ? selectedOriginIndex : selectedDestinationIndex;

    if (!container.classList.contains('active')) return;

    switch (event.key) {
        case 'ArrowDown':
            event.preventDefault();
            currentIndex = Math.min(currentIndex + 1, items.length - 1);
            break;
        case 'ArrowUp':
            event.preventDefault();
            currentIndex = Math.max(currentIndex - 1, -1);
            break;
        case 'Enter':
            event.preventDefault();
            if (currentIndex >= 0) {
                items[currentIndex].click();
            } else {
                calculateRoute();
            }
            return;
        case 'Escape':
            container.classList.remove('active');
            return;
        default:
            return;
    }

    items.forEach((item, index) => {
        item.classList.toggle('selected', index === currentIndex);
    });

    if (field === 'origin') selectedOriginIndex = currentIndex;
    else selectedDestinationIndex = currentIndex;
}

function closeAllDropdowns() {
    document.querySelectorAll('.autocomplete-dropdown').forEach(dropdown => {
        dropdown.classList.remove('active');
    });
}

async function calculateRoute() {
    const origin = document.getElementById('origin-input').value.trim();
    const destination = document.getElementById('destination-input').value.trim();

    if (!origin || !destination) {
        showError('Please enter both origin and destination');
        return;
    }

    showLoading(true);
    hideError();
    hideResults();
    clearMap();

    try {
        const response = await fetch(
            `${API_CONFIG.ROUTING_URL}?origin=${encodeURIComponent(origin)}&destination=${encodeURIComponent(destination)}`
        );

        if (!response.ok) throw new Error('Failed to calculate route');

        const data = await response.json();
        currentRouteData = data;

        if (data.IsInterCity && data.Note && data.Note.includes('Inter-city route from')) {
            extractInterCityStationsFromNote(data.Note, data);
        }

        currentCity = extractCityFromAddress(origin);

        if (data.StartStationDetails && !data.IsWalkingOnly) {
            currentPickupStation = {
                id: extractStationId(data.StartStationDetails.Name),
                name: data.StartStationDetails.Name,
                city: currentCity,
                bikes: data.StartStationDetails.AvailableBikes,
                stands: data.StartStationDetails.AvailableStands
            };
            showTestSection();
        } else {
            hideTestSection();
        }

        displayRoute(data);
        await drawRouteOnMap(data);
    } catch (e) {
        showError('Unable to calculate route. Please check if the server is running.');
    } finally {
        showLoading(false);
    }
}

function extractInterCityStationsFromNote(note, data) {
    interCityStations.originStart = data.StartStationDetails;
    interCityStations.destEnd = data.EndStationDetails;
}

function parseInterCitySegments(note) {
    const lines = note.split('\n');
    const segments = [];
    
    for (let line of lines) {
        if (line.match(/^\d+\./)) {
            segments.push(line);
        }
    }
    
    return segments;
}

function displayRoute(data) {
    const totalKm = (data.TotalKm || 0).toFixed(2);
    const totalMinutes = Math.round((data.TotalSeconds || 0) / 60);

    document.getElementById('total-distance').textContent = `${totalKm} km`;
    document.getElementById('total-time').textContent = `${totalMinutes} min`;

    const detailsContainer = document.getElementById('route-details');
    detailsContainer.innerHTML = '';

    const isInterCity = data.IsInterCity && data.Note && data.Note.includes('Inter-city route from');

    if (isInterCity) {
        const segmentLines = parseInterCitySegments(data.Note);
        
        if (segmentLines.length >= 3) {
            segmentLines.forEach((line, index) => {
                const card = createInterCitySegmentCard(line, index);
                if (card) {
                    detailsContainer.appendChild(card);
                }
            });
        }
    } else {
        if (data.WalkToStartMeters > 0 && !data.IsWalkingOnly) {
            detailsContainer.appendChild(createSegmentCard('walking', 'Walk to bike station', data.WalkToStartMeters, data.StartStation, data.StartStationDetails));
        }

        if (data.BikeMeters > 0) {
            detailsContainer.appendChild(createSegmentCard('biking', 'Bike ride', data.BikeMeters, data.EndStation, data.EndStationDetails));
        }

        if (data.WalkToEndMeters > 0 && !data.IsWalkingOnly) {
            detailsContainer.appendChild(createSegmentCard('walking', 'Walk to destination', data.WalkToEndMeters));
        }

        if (data.IsWalkingOnly) {
            detailsContainer.appendChild(createSegmentCard('walking', 'Walk to destination', data.WalkToStartMeters));
        }
    }

    showResults();
}

function createInterCitySegmentCard(line, index) {
    const match = line.match(/(\d+)\.\s*(Walk|Bike)\s*([\d.]+)\s*km\s*(.+?)\s*\((\d+)\s*min/);
    
    if (!match) return null;
    
    const [_, segNum, type, distance, description, time] = match;
    const isWalking = type.toLowerCase() === 'walk';
    const isBiking = type.toLowerCase() === 'bike';
    
    const card = document.createElement('div');
    card.className = `route-segment ${isWalking ? 'walking' : 'biking'}`;
    
    const icon = isBiking ? 'fa-bicycle' : 'fa-person-walking';
    const title = isBiking ? 'Bike ride' : `Walk ${description.includes('between') ? 'between cities' : ''}`.trim() || 'Walk';
    
    const distanceNum = parseFloat(distance);
    const isLongWalk = distanceNum > 50;
    
    card.innerHTML = `
        <div class="segment-header">
            <div class="segment-icon ${isWalking ? 'walking' : 'biking'}">
                <i class="fas ${icon}"></i>
            </div>
            <div class="segment-info">
                <div class="segment-type">${title}</div>
                <div class="segment-details">
                    <span class="segment-detail">
                        <i class="fas fa-route"></i>
                        ${distance} km ${isLongWalk ? '⚠️' : ''}
                    </span>
                    <span class="segment-detail">
                        <i class="fas fa-clock"></i>
                        ${time} min${parseInt(time) > 60 ? ` (${Math.round(parseInt(time) / 60)} hours)` : ''}
                    </span>
                </div>
            </div>
        </div>
        <div class="station-info-text">
            ${description}
        </div>
    `;
    
    return card;
}

function createSegmentCard(type, title, meters, stationName, stationDetails) {
    const distance = (meters / 1000).toFixed(2);
    const time = Math.round(meters / (type === 'biking' ? 4.4 : 1.4) / 60);
    const icon = type === 'biking' ? 'fa-bicycle' : 'fa-person-walking';

    const card = document.createElement('div');
    card.className = `route-segment ${type}`;

    let stationHtml = '';
    if (stationName && stationDetails) {
        const bikes = stationDetails.AvailableBikes || 0;
        const stands = stationDetails.AvailableStands || 0;
        const bikesClass = bikes > 5 ? 'good' : bikes > 2 ? 'warning' : 'critical';
        const standsClass = stands > 5 ? 'good' : stands > 2 ? 'warning' : 'critical';

        stationHtml = `
            <div class="station-info">
                <div class="station-name">${stationName}</div>
                <div class="station-availability">
                    <div class="availability-item">
                        <i class="fas fa-bicycle"></i>
                        <span class="availability-value ${bikesClass}">${bikes}</span>
                        <span>bikes</span>
                    </div>
                    <div class="availability-item">
                        <i class="fas fa-parking"></i>
                        <span class="availability-value ${standsClass}">${stands}</span>
                        <span>spots</span>
                    </div>
                </div>
            </div>
        `;
    }

    card.innerHTML = `
        <div class="segment-header">
            <div class="segment-icon ${type}">
                <i class="fas ${icon}"></i>
            </div>
            <div class="segment-info">
                <div class="segment-type">${title}</div>
                <div class="segment-details">
                    <span class="segment-detail">
                        <i class="fas fa-route"></i>
                        ${distance} km
                    </span>
                    <span class="segment-detail">
                        <i class="fas fa-clock"></i>
                        ${time} min
                    </span>
                </div>
            </div>
        </div>
        ${stationHtml}
    `;

    return card;
}

async function drawRouteOnMap(data) {
    clearMap();
    const allCoords = [];

    if (data.OriginLat && data.OriginLon) {
        L.marker([data.OriginLat, data.OriginLon], { icon: MapIcons.origin })
            .bindPopup(`<strong>Origin</strong><br>${data.OriginLat.toFixed(5)}, ${data.OriginLon.toFixed(5)}`)
            .addTo(markersLayer);
        allCoords.push([data.OriginLat, data.OriginLon]);
    }

    if (data.DestLat && data.DestLon) {
        L.marker([data.DestLat, data.DestLon], { icon: MapIcons.destination })
            .bindPopup(`<strong>Destination</strong><br>${data.DestLat.toFixed(5)}, ${data.DestLon.toFixed(5)}`)
            .addTo(markersLayer);
        allCoords.push([data.DestLat, data.DestLon]);
    }

    const isInterCity = data.IsInterCity && data.Note && data.Note.includes('Inter-city route from');

    if (data.IsWalkingOnly) {
        const walkRoute = await getWalkingRoute(data.OriginLat, data.OriginLon, data.DestLat, data.DestLon);
        const coords = walkRoute || [[data.OriginLat, data.OriginLon], [data.DestLat, data.DestLon]];
        L.polyline(coords, { color: '#10b981', weight: 5, opacity: 0.8, dashArray: '10, 10' }).addTo(currentRouteLayer);
    } else if (isInterCity) {
        const originStartStation = data.StartStationDetails;
        const destEndStation = data.EndStationDetails;
        
        if (originStartStation && destEndStation) {
            L.marker([originStartStation.Latitude, originStartStation.Longitude], { icon: MapIcons.bikeStation })
                .bindPopup(`<strong>Start Station (Origin City)</strong><br>${originStartStation.Name}<br>🚲 ${originStartStation.AvailableBikes} bikes`)
                .addTo(markersLayer);
            allCoords.push([originStartStation.Latitude, originStartStation.Longitude]);
            
            L.marker([destEndStation.Latitude, destEndStation.Longitude], { icon: MapIcons.bikeStation })
                .bindPopup(`<strong>End Station (Dest City)</strong><br>${destEndStation.Name}<br>🅿️ ${destEndStation.AvailableStands} spots`)
                .addTo(markersLayer);
            allCoords.push([destEndStation.Latitude, destEndStation.Longitude]);
            
            const walk1Route = await getWalkingRoute(
                data.OriginLat, data.OriginLon,
                originStartStation.Latitude, originStartStation.Longitude
            );
            const walk1Coords = walk1Route || [[data.OriginLat, data.OriginLon], [originStartStation.Latitude, originStartStation.Longitude]];
            L.polyline(walk1Coords, { color: '#10b981', weight: 5, opacity: 0.8, dashArray: '10, 10' }).addTo(currentRouteLayer);
            
            const bike1Route = await getCyclingRoute(
                originStartStation.Latitude, originStartStation.Longitude,
                destEndStation.Latitude, destEndStation.Longitude
            );
            if (bike1Route && bike1Route.length > 2) {
                const midPoint = Math.floor(bike1Route.length * 0.1);
                const bike1Segment = bike1Route.slice(0, midPoint);
                L.polyline(bike1Segment, { color: '#f59e0b', weight: 5, opacity: 0.8 }).addTo(currentRouteLayer);
            }
            
            const walkBetweenRoute = await getWalkingRoute(
                originStartStation.Latitude, originStartStation.Longitude,
                destEndStation.Latitude, destEndStation.Longitude
            );
            if (walkBetweenRoute && walkBetweenRoute.length > 2) {
                const start = Math.floor(walkBetweenRoute.length * 0.1);
                const end = Math.floor(walkBetweenRoute.length * 0.9);
                const walkBetweenSegment = walkBetweenRoute.slice(start, end);
                L.polyline(walkBetweenSegment, { color: '#10b981', weight: 5, opacity: 0.8, dashArray: '10, 10' }).addTo(currentRouteLayer);
            } else {
                L.polyline([
                    [originStartStation.Latitude, originStartStation.Longitude],
                    [destEndStation.Latitude, destEndStation.Longitude]
                ], { color: '#10b981', weight: 5, opacity: 0.6, dashArray: '10, 10' }).addTo(currentRouteLayer);
            }
            
            if (bike1Route && bike1Route.length > 2) {
                const startPoint = Math.floor(bike1Route.length * 0.9);
                const bike2Segment = bike1Route.slice(startPoint);
                L.polyline(bike2Segment, { color: '#f59e0b', weight: 5, opacity: 0.8 }).addTo(currentRouteLayer);
            }
            
            const walk2Route = await getWalkingRoute(
                destEndStation.Latitude, destEndStation.Longitude,
                data.DestLat, data.DestLon
            );
            const walk2Coords = walk2Route || [[destEndStation.Latitude, destEndStation.Longitude], [data.DestLat, data.DestLon]];
            L.polyline(walk2Coords, { color: '#10b981', weight: 5, opacity: 0.8, dashArray: '10, 10' }).addTo(currentRouteLayer);
        }
    } else {
        if (data.StartStationDetails) {
            L.marker([data.StartStationDetails.Latitude, data.StartStationDetails.Longitude], { icon: MapIcons.bikeStation })
                .bindPopup(`<strong>Pickup</strong><br>${data.StartStationDetails.Name}<br>🚲 ${data.StartStationDetails.AvailableBikes} bikes`)
                .addTo(markersLayer);
            allCoords.push([data.StartStationDetails.Latitude, data.StartStationDetails.Longitude]);
        }

        if (data.EndStationDetails) {
            L.marker([data.EndStationDetails.Latitude, data.EndStationDetails.Longitude], { icon: MapIcons.bikeStation })
                .bindPopup(`<strong>Dropoff</strong><br>${data.EndStationDetails.Name}<br>🅿️ ${data.EndStationDetails.AvailableStands} spots`)
                .addTo(markersLayer);
            allCoords.push([data.EndStationDetails.Latitude, data.EndStationDetails.Longitude]);
        }

        if (data.StartStationDetails && data.WalkToStartMeters > 0) {
            const route = await getWalkingRoute(data.OriginLat, data.OriginLon, data.StartStationDetails.Latitude, data.StartStationDetails.Longitude);
            const coords = route || [[data.OriginLat, data.OriginLon], [data.StartStationDetails.Latitude, data.StartStationDetails.Longitude]];
            L.polyline(coords, { color: '#10b981', weight: 5, opacity: 0.8, dashArray: '10, 10' }).addTo(currentRouteLayer);
        }

        if (data.StartStationDetails && data.EndStationDetails && data.BikeMeters > 0) {
            const route = await getCyclingRoute(data.StartStationDetails.Latitude, data.StartStationDetails.Longitude, data.EndStationDetails.Latitude, data.EndStationDetails.Longitude);
            const coords = route || [[data.StartStationDetails.Latitude, data.StartStationDetails.Longitude], [data.EndStationDetails.Latitude, data.EndStationDetails.Longitude]];
            L.polyline(coords, { color: '#f59e0b', weight: 5, opacity: 0.8 }).addTo(currentRouteLayer);
        }

        if (data.EndStationDetails && data.WalkToEndMeters > 0) {
            const route = await getWalkingRoute(data.EndStationDetails.Latitude, data.EndStationDetails.Longitude, data.DestLat, data.DestLon);
            const coords = route || [[data.EndStationDetails.Latitude, data.EndStationDetails.Longitude], [data.DestLat, data.DestLon]];
            L.polyline(coords, { color: '#10b981', weight: 5, opacity: 0.8, dashArray: '10, 10' }).addTo(currentRouteLayer);
        }
    }

    if (allCoords.length > 0) {
        map.fitBounds(L.latLngBounds(allCoords), { padding: [50, 50] });
    }
}

function clearMap() {
    currentRouteLayer.clearLayers();
    markersLayer.clearLayers();
}

function showLoading(show) {
    const loading = document.getElementById('loading');
    const searchBtn = document.getElementById('search-btn');
    if (show) {
        loading.classList.add('active');
        searchBtn.disabled = true;
    } else {
        loading.classList.remove('active');
        searchBtn.disabled = false;
    }
}

function showError(message) {
    const errorEl = document.getElementById('error');
    const errorText = document.getElementById('error-text');
    errorText.textContent = message;
    errorEl.classList.add('active');
}

function hideError() {
    document.getElementById('error').classList.remove('active');
}

function showResults() {
    document.getElementById('results').classList.add('active');
}

function hideResults() {
    document.getElementById('results').classList.remove('active');
}

function resetMapView() {
    if (currentRouteLayer.getLayers().length > 0) {
        const bounds = L.latLngBounds();
        currentRouteLayer.eachLayer(layer => {
            if (layer.getBounds) bounds.extend(layer.getBounds());
        });
        markersLayer.eachLayer(layer => {
            if (layer.getLatLng) bounds.extend(layer.getLatLng());
        });
        if (bounds.isValid()) {
            map.fitBounds(bounds, { padding: [50, 50] });
        }
    } else {
        map.setView(API_CONFIG.MAP_CENTER, API_CONFIG.MAP_ZOOM);
    }
}

function toggleFullscreen() {
    const mapContainer = document.querySelector('.map-container');
    if (!document.fullscreenElement) {
        mapContainer.requestFullscreen().catch(() => {});
    } else {
        document.exitFullscreen();
    }
}

// ═══════════════════════════════════════════════════════════
// CHAT PANEL (LLM Assistant)
// ═══════════════════════════════════════════════════════════

function connectChatWs() {
    const statusEl = document.getElementById('chat-ws-status');
    if (statusEl) statusEl.className = 'chat-header-status connecting';

    chatWs = new WebSocket(API_CONFIG.LLM_WS_URL);

    chatWs.onopen = () => {
        if (statusEl) statusEl.className = 'chat-header-status connected';
    };

    chatWs.onclose = () => {
        if (statusEl) statusEl.className = 'chat-header-status disconnected';
        setTimeout(connectChatWs, 5000);
    };

    chatWs.onerror = () => {
        if (statusEl) statusEl.className = 'chat-header-status disconnected';
    };

    chatWs.onmessage = (event) => {
        const msg = JSON.parse(event.data);
        handleChatResponse(msg);
    };
}

function toggleChat() {
    const panel = document.getElementById('chat-panel');
    const fab = document.getElementById('chat-fab');
    chatOpen = !chatOpen;

    if (chatOpen) {
        panel.classList.add('open');
        fab.classList.add('hidden');
        if (!chatWs || chatWs.readyState !== WebSocket.OPEN) {
            connectChatWs();
        }
        document.getElementById('chat-input').focus();
    } else {
        panel.classList.remove('open');
        fab.classList.remove('hidden');
    }
}

function sendChatMessage() {
    const input = document.getElementById('chat-input');
    const message = input.value.trim();
    if (!message) return;

    if (!chatWs || chatWs.readyState !== WebSocket.OPEN) {
        appendChatMessage('assistant', 'Not connected to the assistant. Reconnecting...');
        connectChatWs();
        return;
    }

    appendChatMessage('user', message);
    chatHistory.push({ role: 'user', content: message });
    input.value = '';

    chatWs.send(JSON.stringify({
        message: message,
        history: chatHistory.slice(-10)
    }));

    document.getElementById('chat-send-btn').disabled = true;
}

function handleChatResponse(msg) {
    const messagesEl = document.getElementById('chat-messages');

    switch (msg.type) {
        case 'thinking':
            removeThinkingBubble();
            appendChatBubble('assistant', msg.content, 'thinking');
            break;

        case 'tool_call':
            removeThinkingBubble();
            appendChatBubble('assistant', `<span class="tool-tag"><i class="fas fa-wrench"></i> ${msg.name}</span> Calling...`, 'thinking');
            break;

        case 'tool_result':
            if (msg.name === 'get_itinerary' && msg.data) {
                displayRoute(msg.data);
                drawRouteOnMap(msg.data);
            }
            break;

        case 'done':
            removeThinkingBubble();
            appendChatMessage('assistant', msg.content);
            chatHistory.push({ role: 'assistant', content: msg.content });
            document.getElementById('chat-send-btn').disabled = false;

            if (msg.itinerary) {
                displayRoute(msg.itinerary);
                drawRouteOnMap(msg.itinerary);
            }
            break;

        case 'error':
            removeThinkingBubble();
            appendChatMessage('assistant', msg.content);
            document.getElementById('chat-send-btn').disabled = false;
            break;
    }

    messagesEl.scrollTop = messagesEl.scrollHeight;
}

function appendChatMessage(role, text) {
    appendChatBubble(role, escapeHtml(text));
}

function appendChatBubble(role, html, extraClass) {
    const messagesEl = document.getElementById('chat-messages');
    const wrapper = document.createElement('div');
    wrapper.className = `chat-message ${role}`;
    const bubble = document.createElement('div');
    bubble.className = 'chat-bubble' + (extraClass ? ` ${extraClass}` : '');
    bubble.innerHTML = html;
    wrapper.appendChild(bubble);
    messagesEl.appendChild(wrapper);
    messagesEl.scrollTop = messagesEl.scrollHeight;
}

function removeThinkingBubble() {
    const messagesEl = document.getElementById('chat-messages');
    const thinking = messagesEl.querySelector('.chat-bubble.thinking');
    if (thinking) thinking.closest('.chat-message').remove();
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

document.addEventListener('DOMContentLoaded', () => {
    initializeMap();
    setupAutocomplete();
    connectWebSocket();

    document.getElementById('search-btn').addEventListener('click', calculateRoute);
    document.getElementById('reset-view-btn').addEventListener('click', resetMapView);
    document.getElementById('fullscreen-btn').addEventListener('click', toggleFullscreen);
    document.getElementById('force-unavailable-btn').addEventListener('click', forceStationUnavailable);

    document.getElementById('origin-input').addEventListener('keypress', (e) => {
        if (e.key === 'Enter' && !document.getElementById('origin-suggestions').classList.contains('active')) {
            calculateRoute();
        }
    });

    document.getElementById('destination-input').addEventListener('keypress', (e) => {
        if (e.key === 'Enter' && !document.getElementById('destination-suggestions').classList.contains('active')) {
            calculateRoute();
        }
    });

    document.getElementById('chat-input').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') sendChatMessage();
    });

    window.addEventListener('resize', () => {
        if (map) map.invalidateSize();
    });
});