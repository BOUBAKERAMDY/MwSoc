# LetsGoBiking System Knowledge

## What is LetsGoBiking?
LetsGoBiking is a bike-sharing route planner that helps users plan trips using JCDecaux bike-sharing stations across French and European cities. It calculates routes combining walking and biking segments.

## How routing works
- The system finds the nearest bike station with available bikes to the origin
- It then finds the nearest station with available stands near the destination
- The route is: Walk to pickup station -> Bike to dropoff station -> Walk to final destination
- For inter-city routes, bikes are used within each city with walking between cities

## Station data
- Station data comes from the JCDecaux API in real-time
- Each station has: name, address, GPS coordinates, available bikes count, available stands count, status (OPEN/CLOSED)
- Data is cached for 5 minutes on the server

## Route types
- Intra-city: Both origin and destination in the same supported city
- Inter-city: Origin and destination in different supported cities (involves walking gap between cities)
- Walking only: When no bike stations are available or cities are not supported

## Tips for users
- Bike-sharing is best for short to medium urban trips (1-10 km)
- For inter-city travel, the walking gap between cities is not practical - consider trains or buses
- Station availability changes in real-time, especially during rush hours
- Most stations are concentrated in city centers
