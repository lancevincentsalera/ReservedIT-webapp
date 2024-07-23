
// this is used to style the status cell based on the status of the booking/user
document.addEventListener('DOMContentLoaded',function () {
    let statusCells = document.querySelectorAll('.user-cell[data-label="STATUS"] span');

    statusCells.forEach((cell) => {
        let statusText = cell.textContent.trim().toLowerCase();

        switch (statusText) {
            case 'approved':
                cell.classList.add('status-active');
                break;
            case 'rejected':
                cell.classList.add('status-restricted');
                break;
            case 'pending':
                cell.classList.add('status-pending');
                break;
            case 'cancelled':
                cell.classList.add('status-cancelled');
                break;
            case 'completed':
                cell.classList.add('status-completed');
                break;
            default:
                // Add a default style or leave it as is
                break;
        }
    });



});
document.getElementById("searchInput").addEventListener("keyup", filterList);