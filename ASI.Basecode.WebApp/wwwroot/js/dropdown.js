
// for dropdowns, usually with an ellipses like button that triggers the dropdown
// the dropdown is a div that is hidden by default and shown when the button is clicked
$(document).ready(() => {
    let dropdownTriggers = $('.dropdown-trigger'); // get all dropdown triggers


    dropdownTriggers.each((index, trigger) => {

        // get the id of the trigger element
        let Id = $(trigger).data('id');
        trigger.addEventListener('click', (event) => {

            event.stopPropagation(); // prevent the click event from bubbling up to the document

            // get the hidden dropdown element
            let dropdown = $('.dropdown-action');

            let offset = $(trigger).offset(); // get the offset of the trigger element, this is used to position the dropdown
            let dropdownHeight = dropdown.outerHeight(); // get the height of the dropdown, this is used to determine if the dropdown should be shown above or below the trigger
            let triggerHeight = $(trigger).outerHeight(); // get the height of the trigger element, this is used to position the dropdown
            let viewportHeight = $(window).height(); // get the height of the viewport, this is used to determine if the dropdown should be shown above or below the trigger
            let spaceBelow = viewportHeight - (offset.top + triggerHeight); // get the space below the trigger element
            let spaceAbove = offset.top;  // get the space above the trigger element, 
            let leftOffset = offset.left - 25; // get the left offset of the dropdown, this is used to position the dropdown


            // if there is enough space below the trigger element, show the dropdown below the trigger element
            // if there is enough space above the trigger element, show the dropdown above the trigger element
            // if there is not enough space above or below the trigger element, show the dropdown below the trigger element
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

            // set the value of the input element inside the dropdown to the id of the trigger element
            // set the data-id attribute of the button inside the dropdown to the id of the trigger element
            $(`.dropdown-action input`).val(Id);
            $(`.dropdown-action button`).data('id', Id);
             
            dropdown.show(); // show the dropdown
        })
    })

    // Hide dropdown when clicking outside
    $(document).on('click',() => {
        $('.dropdown-action').hide();
    });

    // Prevent dropdown from hiding when clicking inside
    $('.dropdown-action').on('click',(event) => {
        event.stopPropagation();
    });
})