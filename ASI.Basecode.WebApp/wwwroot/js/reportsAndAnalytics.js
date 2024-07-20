/*document.addEventListener('DOMContentLoaded', function () {
    const ctx = document.getElementById('barChart').getContext('2d');
    const ctx2 = document.getElementById('lineChart').getContext('2d'); // For the line chart
    const daysOfMonth = generateDaysOfMonth();

    const barchart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            datasets: [{
                label: 'No. of Bookings', // Set to an empty string to remove the label
                data: @Html.Raw(Json.Serialize(barChartData)), // Use the data passed from the controller
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
                    display: true // Completely hide the legend
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
            labels: daysOfMonth, // Sample numeric x-axis values
            datasets: [{
                label: 'No. of Daily Booking', // Set the label for the line graph
                data: @Html.Raw(Json.Serialize(lineChartData)), // Use the data passed from the controller
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
    setInterval(updateCharts, 60000); // 60000 ms = 1 minute
});

function generateDaysOfMonth() {
    const days = [];
    const currentDate = new Date();
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth(); // getMonth() returns 0-11

    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    const monthName = monthNames[month];

    // Get the number of days in the current month
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    for (let day = 1; day <= daysInMonth; day++) {
        // Format day and month as MM-DD
        const formattedDay = `${monthName}-${day.toString().padStart(2, '0')}`;
        days.push(formattedDay);
    }

    return days;
}

function updateCharts() {
    const daysOfMonth = generateDaysOfMonth();

    // Update Bar Chart
    barChart.data.labels = daysOfMonth;
    barChart.update();

    // Update Line Chart
    lineChart.data.labels = daysOfMonth;
    lineChart.update();
}

function toggleDetail(row) {
    const detailTable = row.nextElementSibling;
    if (detailTable.style.display === "none" || detailTable.style.display === "") {
        detailTable.style.display = "block";
    } else {
        detailTable.style.display = "none";
    }
}*/