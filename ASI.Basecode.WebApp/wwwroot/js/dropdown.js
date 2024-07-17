/*let dropdownTriggers = document.querySelectorAll('.dropdown-trigger');

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
});*/

$(document).ready(() => {
    let dropdownTriggers = $('.dropdown-trigger');

    dropdownTriggers.each((index,trigger) => {
        let Id = $(trigger).data('id');
        trigger.addEventListener('click', (event) => {
            event.stopPropagation();

            let dropdown = $('.dropdown-action');

            let offset = $(trigger).offset();
            let dropdownHeight = dropdown.outerHeight();
            let triggerHeight = $(trigger).outerHeight();
            let viewportHeight = $(window).height();
            let spaceBelow = viewportHeight - (offset.top + triggerHeight);
            let spaceAbove = offset.top;
            let leftOffset = offset.left - 25;
        

            if (spaceBelow >= dropdownHeight) {
                dropdown.css({
                    top: offset.top + triggerHeight,
                    left: leftOffset,
                    bottom: 'auto'
                });
            } else if (spaceAbove >= dropdownHeight) {
                dropdown.css({
                    top: 'auto',
                    left: leftOffset,
                    bottom: viewportHeight - offset.top
                });
            } else {
                dropdown.css({
                    top: offset.top + triggerHeight,
                    left: leftOffset,
                    bottom: 'auto'
                });
            }
            
            console.log(Id);
            $(`.dropdown-action input`).val(Id);
            $(`.dropdown-action button`).data('id', Id);
             
            dropdown.show();
        })
    })

    $(document).on('click',() => {
        $('.dropdown-action').hide();
    });

    // Prevent dropdown from hiding when clicking inside
    $('.dropdown-action').on('click',(event) => {
        event.stopPropagation();
    });
})