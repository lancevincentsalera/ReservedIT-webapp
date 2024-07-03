let dropdownTriggers = document.querySelectorAll('.dropdown-trigger');

dropdownTriggers.forEach((trigger) => {
    let dropdown = trigger.querySelector('.dropdown-action');
    let editUserBtn = trigger.querySelector('.create-user-btn');

    editUserBtn.addEventListener('click', (event) => {
        event.stopPropagation();
        $('#editUserModal').modal('show');
    });

    trigger.addEventListener('click', (event) => {
        event.stopPropagation();

        dropdownTriggers.forEach((otherTrigger) => {
            if (otherTrigger !== trigger) {
                let otherDropdown = otherTrigger.querySelector('.dropdown-action');
                otherDropdown.classList.remove("show-dropdown");
            }
        });

        dropdown.classList.toggle('show-dropdown');
    });
});

document.addEventListener('click', (event) => {
    dropdownTriggers.forEach((trigger) => {
        let dropdown = trigger.querySelector('.dropdown-action');

        if (!trigger.contains(event.target)) {
            dropdown.classList.remove('show-dropdown');
        }
    });
});