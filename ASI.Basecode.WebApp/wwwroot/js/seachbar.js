function filterTable() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("searchInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("table");
    tr = table.getElementsByTagName("tr");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 1; i < tr.length; i++) {
        var shouldShow = false;
        td = tr[i].getElementsByTagName("td");  // Replace with the tag used for each cell in your table

        // Check each cell in the current row
        for (var j = 0; j < td.length; j++) {
            txtValue = td[j].textContent || td[j].innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                shouldShow = true;
                break;
            }
        }

        // Toggle row visibility based on search match
        tr[i].style.display = shouldShow ? "" : "none";
    }
}

function filterRoom() {
    console.log("filter room")
}