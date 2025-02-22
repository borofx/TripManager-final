document.addEventListener("DOMContentLoaded", function () {
    var map = L.map('map').setView([42.6977, 23.3242], 6); // Default center: Sofia, Bulgaria

    // Load OpenStreetMap tiles
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    // Store markers and tour lines
    var markers = {};
    var tourLines = {};

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

    function loadTours() {
        fetch('/api/tours')
            .then(response => response.json())
            .then(tours => {
                // Clear existing tour lines
                Object.values(tourLines).forEach(line => map.removeLayer(line));
                tourLines = {};

                tours.forEach((tour, index) => {
                    if (tour.landmarks && tour.landmarks.length >= 2) {
                        // Create array of landmark coordinates
                        const tourPoints = tour.landmarks.map(landmark =>
                            [landmark.latitude, landmark.longitude]
                        );

                        // Draw line connecting landmarks
                        const tourLine = L.polyline(tourPoints, {
                            color: '#FF0000',  // You can use different colors for different tours
                            weight: 3
                        }).addTo(map);

                        // Add tour information popup
                        tourLine.bindPopup(`<b>${tour.name}</b><br>${tour.description}`);

                        tourLines[tour.id] = tourLine;
                    }
                });
            })
            .catch(error => console.error("Error loading tours:", error));
    }

    // Load both landmarks and tours
    loadLandmarks();
    loadTours();

    // Refresh data periodically
    setInterval(() => {
        loadLandmarks();
        loadTours();
    }, 10000);
});