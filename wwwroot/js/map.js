document.addEventListener("DOMContentLoaded", function () {
    var map = L.map('map').setView([42.6977, 23.3242], 6);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    var markers = {};
    var activeRoutes = [];

    function loadLandmarks() {
        fetch('/api/landmarks')
            .then(response => response.json())
            .then(landmarks => {
                landmarks.forEach(landmark => {
                    L.marker([landmark.latitude, landmark.longitude])
                        .addTo(map)
                        .bindPopup(`<b>${landmark.name}</b>`);
                });
            })
            .catch(error => console.error("Error loading landmarks:", error));
    }

    async function calculateRoute(points) {
        try {
            if (points.length < 2) {
                console.error("Not enough points for routing.");
                return points.map(point => [point.lat, point.lng]); // Return a straight line as a fallback
            }

            const waypoints = points.map(point => `point=${point.lat},${point.lng}`).join('&');

            console.log("GraphHopper Request URL:", `https://graphhopper.com/api/1/route?${waypoints}&vehicle=car&locale=bg&points_encoded=false&key=7188b451-4bf1-4cfe-8e47-a05e5107fa64`);

            const response = await fetch(
                `https://graphhopper.com/api/1/route?${waypoints}&vehicle=car&locale=bg&points_encoded=false&key=7188b451-4bf1-4cfe-8e47-a05e5107fa64`
            );

            if (!response.ok) {
                throw new Error("Failed to calculate route");
            }

            const data = await response.json();
            console.log("GraphHopper Response:", data);

            if (!data.paths || data.paths.length === 0) {
                throw new Error("No route found");
            }

            // Convert GraphHopper route format to Leaflet format
            return data.paths[0].points.coordinates.map(coord => [coord[1], coord[0]]);
        } catch (error) {
            console.error("Route calculation error:", error);
            return points.map(point => [point.lat, point.lng]); // Fallback to straight line
        }
    }

    function loadTours() {
        // Clear existing routes
        activeRoutes.forEach(route => map.removeLayer(route));
        activeRoutes = [];

        fetch('/api/tours')
            .then(response => response.json())
            .then(async tours => {
                for (const tour of tours) {
                    if (tour.landmarks && tour.landmarks.length >= 2) {
                        try {
                            const waypoints = tour.landmarks.map(landmark =>
                                ({ lat: landmark.latitude, lng: landmark.longitude })
                            );

                            // Calculate route using GraphHopper
                            const routeCoordinates = await calculateRoute(waypoints);

                            //polyline with the route
                            const routeLine = L.polyline(routeCoordinates, {
                                color: getRandomColor(),
                                weight: 4,
                                opacity: 0.7
                            }).addTo(map);

                            //markers for each landmark in the tour
                            tour.landmarks.forEach(landmark => {
                                const marker = L.marker([landmark.latitude, landmark.longitude])
                                    .addTo(map)
                                    .bindPopup(`<b>${landmark.name}</b>`);
                            });

                            // tour information
                            const tourInfo = `
                                <b>${tour.name}</b><br>
                                Landmarks: ${tour.landmarks.map(l => l.name).join(' > ')}
                            `;
                            routeLine.bindPopup(tourInfo);

                            activeRoutes.push(routeLine);
                        } catch (error) {
                            console.error(`Error calculating route for tour ${tour.name}:`, error);

                            // Fallback to straight line if routing fails
                            const coordinates = tour.landmarks.map(landmark =>
                                [landmark.latitude, landmark.longitude]
                            );

                            const fallbackLine = L.polyline(coordinates, {
                                color: green,
                                weight: 4,
                                opacity: 0.7,
                                dashArray: '10, 10' // to know there is fallback
                            }).addTo(map);

                            activeRoutes.push(fallbackLine);
                        }
                    }
                }
            })
            .catch(error => console.error("Error loading tours:", error));
    }

    function getRandomColor() {
        var letters = '0123456789ABCDEF';
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }

    loadLandmarks();
    loadTours();

    // Refresh less frequently
    setInterval(() => {
        loadLandmarks();
        loadTours();
    }, 1000);
});