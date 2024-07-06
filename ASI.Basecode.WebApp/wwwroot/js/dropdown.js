let dropdownTriggers = document.querySelectorAll('.dropdown-trigger');

dropdownTriggers.forEach((trigger) => {
    let dropdown = trigger.querySelector('.dropdown-action');

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