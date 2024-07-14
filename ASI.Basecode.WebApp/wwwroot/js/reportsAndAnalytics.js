document.addEventListener('DOMContentLoaded', function () {
    const ctx = document.getElementById('barChart').getContext('2d');
    const ctx2 = document.getElementById('lineChart').getContext('2d'); // For the line chart

    const barchart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            datasets: [{
                label: '', // Set to an empty string to remove the label
                data: [65, 59, 80, 81, 56, 55],
                borderColor: 'rgb(229, 158, 176)',
                backgroundColor: 'rgb(229, 158, 176)',
                borderWidth: 2,
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false, // Allow resizing
            plugins: {
                legend: {
                    display: false // Completely hide the legend
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Month'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'No. of Bookings'
                    },
                    beginAtZero: true
                }
            }
        }
    });

    // Line chart configuration
    const lineChart = new Chart(ctx2, {
        type: 'line',
        data: {
            labels: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10'], // Sample numeric x-axis values
            datasets: [{
                label: 'Daily Bookings', // Set the label for the line graph
                data: [10, 15, 20, 25, 30, 25, 20, 30, 35, 40], // Sample data for the line chart
                borderColor: 'rgb(229, 158, 176)',
                backgroundColor: 'rgb(229, 158, 176)',
                borderWidth: 2,
                fill: false,
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false // Hide the legend if not needed
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Days'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Bookings'
                    },
                    beginAtZero: true
                }
            }
        }
    });
});
