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

function filterList() {
    var input, filter, list, li, divs, i, txtValue;
    input = document.getElementById("searchInput");
    filter = input.value.toUpperCase();
    list = document.querySelector(".user-list");
    li = list.getElementsByTagName("li");

    // Loop through all list items, and hide those who don't match the search query
    for (i = 0; i < li.length; i++) {
        var shouldShow = false;
        divs = li[i].getElementsByClassName("user-cell");  // Adjust to your specific cell class if different

        // Check each cell in the current list item
        for (var j = 0; j < divs.length; j++) {
            txtValue = divs[j].textContent || divs[j].innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                shouldShow = true;
                break;
            }
        }

        // Toggle list item visibility based on search match
        li[i].style.display = shouldShow ? "" : "none";
    }
}