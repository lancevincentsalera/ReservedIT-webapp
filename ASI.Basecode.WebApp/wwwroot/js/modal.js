

// pass the id of the button that was clicked to the id of the confirm button
// usually used by the delete modal and cancel modal
const passId = (btnId, confirmBtn) => {
    let Id = $(btnId).data('id');
    console.log("hey",btnId)
    $(confirmBtn).data('id', Id);
}




// display the modal on click
const displayModal = (modalId) => {
    $(modalId).modal('show');
}



// get the details of the user and display it in the modal
// btnId: the id of the button that was clicked
// modalId: the id of the modal to display
// action: the action to be performed
// controller: the controller to be used
// used in the EDIT user modal to populate the fields with the user's current details
const getUserDetails = (btnId, modalId, action, controller) => {

    // get the id of the user
    // the id is stored in the data-id attribute of the button
    // that was passed through the dropdown.js script
    let id = $(btnId).data('id');

    console.log(id);
    console.log(`/${controller}/${action}`)

    $.ajax({
        url: `/${controller}/${action}`,
        type: 'GET',
        data: { userId : id }, // pass the id to the controller, should be the same variable name or key in the controller parameter
        success: (response) => {
            for (let key in response) {

                // if the key is RoleId, then get the select element with the name RoleId
                // then loop through the options and check if the value of the option is equal to the value of the response
                // if it is, then set the option to selected
                // else, set it to false
                // this is to populate the RoleId select element with the user's current role
                if (key === 'RoleId') {
                    $(`${modalId} select[name="RoleId"] option`).each((index, element) => {
                        let $option = $(element);
                        if ($option.val() === ('' + response[key])) {
                            console.log("val", typeof $option.val(), "vs", typeof ('' + response[key]));
                            $option.prop('selected', true);
                        } else {
                            $option.prop('selected', false);
                        }
                    })
                } else {

                    // if the key is not RoleId, then get the input element with the name of the key
                    $(`${modalId} input[name="${key}"]`).val(response[key]);
                }
                
            }
        },
        error: (xhr, status, error) => {
            console.error('Error fetching user details:', error);
        }
    })
}



// get the details of the room and display it in the modal
// similar to getUserDetails
// used in the EDIT room modal to populate the fields with the room's current details
const getRoomDetails = (btnId, modalId, action, controller) => {
    let id = $(btnId).data('id');

    console.log(id);
    console.log(`/${controller}/${action}`)

    $.ajax({
        url: `/${controller}/${action}`,
        type: 'GET',
        data: { userId: id },
        success: (response) => {
            console.log(response);
            for (let key in response) {
                if (key === 'RoleId') {
                    $(`${modalId} select[name="RoleId"] option`).each((index, element) => {
                        let $option = $(element);
                        if ($option.val() === ('' + response[key])) {
                            console.log("val", typeof $option.val(), "vs", typeof ('' + response[key]));
                            $option.prop('selected', true);
                        } else {
                            $option.prop('selected', false);
                        }
                    })
                } else {
                    $(`${modalId} input[name="${key}"], ${modalId} textarea[name="${key}"]`).val(response[key]);
                }

            }
        },
        error: (xhr, status, error) => {
            console.error('Error fetching room details:', error);
        }
    })
}



// get the details of the booking and display it in the modal
// used in the EDIT booking modal to populate the fields with the booking's current details
const getBookingDetails = (btnId, modalId, action, controller) => {
    let id = $(btnId).data('id'); // similar to getUserDetails

    console.log(id);
    console.log(`/${controller}/${action}`)

    $.ajax({
        url: `/${controller}/${action}`,
        type: 'GET',
        data: { bookingId: id },
        success: (response) => {
            console.log(response)

            // loop through the response object
            for (let key in response) {

                // if the key is not DayOfTheWeekIds, TimeFrom, or TimeTo
                if (response[key] != null && key !== 'DayOfTheWeekIds' && key !== "TimeFrom" && key !== "TimeTo") {

                    /* get the input element with the name of the key
                        then set the value of the input element to the value of the response
                   */


                    if (key === "StartDate" || key === "EndDate") {

                        // for populating the FLATPICKR in the view
                        // logic adjusted for JsonSerialization Reference.Preserve
                        // to get the date and time from the response
                        let date = new Date(response[key]);
                        let timeKey = key === "StartDate" ? "TimeFrom" : "TimeTo";
                        let timeString = response[timeKey];
                        
                        if (timeString && typeof timeString === 'string') {
                            let [hours, minutes] = timeString.split(':').map(Number);

                            // check if the hours and minutes are numbers
                            if (!isNaN(hours) && !isNaN(minutes)) {
                                date.setHours(hours);
                                date.setMinutes(minutes);


                                // manually format the date to the format that flatpickr accepts
                                let formattedDate = date.getFullYear() + '-' +
                                    String(date.getMonth() + 1).padStart(2, '0') + '-' +
                                    String(date.getDate()).padStart(2, '0') + 'T' +
                                    String(date.getHours()).padStart(2, '0') + ':' +
                                    String(date.getMinutes()).padStart(2, '0');


                                // get the current time and add 2 hours to it
                                // then get the current minute and get the remainder when divided by 15
                                // if the remainder is not 0, add the difference to the current minute
                                // then set the minute to the nearest 15 minutes
                                let currentTime = new Date();
                                currentTime.setHours(currentTime.getHours() + 2);
                                let currentMinute = currentTime.getMinutes();
                                let remainder = currentMinute % 15;
                                if (remainder !== 0) {
                                    currentTime.setMinutes(currentMinute + (15 - remainder));
                                }

                                // manually format the current time to the format that flatpickr accepts
                                let minTime = `${String(currentTime.getHours()).padStart(2, '0')}:${String(currentTime.getMinutes()).padStart(2, '0')}`;


                                // initialize the flatpickr instance
                                // set the altInput to true to display the date in the input field
                                // set the altFormat to the format that the date will be displayed in the input field
                                // set the dateFormat to the format that the date will be sent to the server
                                // set enableTime to true to enable the time picker
                                // set time_24hr to false to use the 12-hour format
                                // set minuteIncrement to 15 to increment the minutes by 15
                                const flatpickrInstance = $(`${modalId} input[name="${key}"]`).flatpickr({
                                    altInput: true,
                                    altFormat: "F j, Y (h:i K)",
                                    dateFormat: "Y-m-d H:i",
                                    enableTime: true,
                                    time_24hr: false,
                                    minuteIncrement: 15,
                                    minDate: "today",
                                    minTime: new Date(),
                                    onChange: function (selectedDates, dateStr, instance) {

                                        // get the selected date
                                        // if the selected date is today, set the minTime to the current time
                                        // else, set the minTime to 00:00
                                        const selectedDate = selectedDates[0];
                                        if (selectedDate && selectedDate.toDateString() === new Date().toDateString()) {
                                            instance.set('minTime', minTime);
                                        } else {
                                            instance.set('minTime', '00:00');
                                        }

                                        // check for booking conflicts on date and time change
                                        checkBookingConflict();
                                    }
                                });

                                // set the date of the flatpickr instance to the formatted date
                                flatpickrInstance.setDate(formattedDate, true);
                            } else {
                                console.error("Invalid time value");
                            }
                        } else {
                            console.error("Invalid time string");
                        }
                    } else if (key === "RoomId") {
                        // for populating the RoomId select element
                        // loop through the options of the select element, a DROPDOWN OF ROOMS
                        // then check if the value of the option is equal to the value of the response
                        // if it is, set the option to selected
                        // else, set it to false
                        $(`${modalId} select[name="RoomId"] option`).each((index, element) => {
                            let $option = $(element);
                            if ($option.val() === ('' + response[key])) {
                                console.log("val", typeof $option.val(), "vs", typeof ('' + response[key]));
                                $option.prop('selected', true);
                            } else {
                                $option.prop('selected', false);
                            }
                        })
                    }
                    else {

                        // for other input elements
                        $(`${modalId} input[name="${key}"]`).val(response[key]);
                    }
                    console.log(key, $(`${modalId} input[name="${key}"]`).val() )
                }
            }

            // uncheck all checkboxes first to avoid conflicts of checked checkboxes
            $(`${modalId} .checkbox-recur input[type="checkbox"]`).prop('checked', false).trigger('change');


            // if the response has DayOfTheWeekIds
            if (response.DayOfTheWeekIds && response.DayOfTheWeekIds.$values && response.DayOfTheWeekIds.$values.length > 0) {

                // loop through the DayOfTheWeekIds
                response.DayOfTheWeekIds.$values.forEach(dayId => {

                    // get the checkbox with a value equal to dayId
                    // then set the checkbox to checked
                    // if more than one checkbox element is found, set all to checked
                    const checkboxSelector = `${modalId} .checkbox-recur input[type="checkbox"][value="${dayId}"]`;
                    const checkbox = $(checkboxSelector);

                    if (checkbox.length > 0) {
                        checkbox.prop('checked', true).trigger('change');
                    } 
                });
                

                // if the response has DayOfTheWeekIds
                // then set the custom button to active
                $('.custom-recur').show();
                $('#customButton').addClass('active');
                $('#scheduleOnceButton').removeClass('active');
            } else {

                // if the response does not have DayOfTheWeekIds
                // then set the schedule once button to active
                $('.custom-recur').hide();
                $('#scheduleOnceButton').addClass('active');
                $('#customButton').removeClass('active');
            }
        },
        error: (xhr, status, error) => {
            console.error('Error fetching booking details:', status, error);
            if (xhr.status === 500) {
                alert('Internal Server Error. Please try again later.');
            } else {
                alert('An error occurred while fetching booking details.');
            }
        }
    })
}



// get the details of the room and display it in the modal
// this is the one where the room gallery is displayed
// on click of a room in the room list
const getRoomModalDetails = (btnId, modalId, action, controller) => {
    let id = $(btnId).data('id');

    $.ajax({
        url: `/${controller}/${action}`,
        type: 'GET',
        data: { roomId: id },
        success: (response) => {
            for (let key in response) {
                if (response.hasOwnProperty(key) && key !== "RoomGallery") {
                    $(`${modalId} [data-name="${key}"]`).text(response[key]);
                }
            }

            let carouselInner = $(`${modalId} .carousel-inner`);
            carouselInner.empty();

            console.log(response.RoomGallery)
            response.RoomGallery.$values.forEach((img, index) => {
                let activeClass = index === 0 ? "active" : "";
                carouselInner.append(`
                    <div class="carousel-item ${activeClass}">
                        <img src="${img.GalleryUrl}" class="d-block w-100 modal-image" alt="${img.GalleryName}">
                    </div>
                `);
            })
        },
        error: (xhr, status, error) => {
            console.error('Error fetching room details:', error);
        }
    })
}



// to display a confirmation modal after another mobile
// used after create user and edit user modal
const displayConfirmationModal = (formId, hideId, showId) => {
    $(formId).validate();
    let formIsValid = false;

    if ($(formId).valid()) {
        formIsValid = true;
    }
    
    console.log(formIsValid);

    if (formIsValid) {
        event.preventDefault();
        $(hideId).modal('hide');
        $(showId).modal('show');
    }
}


// to hide the confirmation modal and show the previous modal
const hideConfirmationModal = ( hideId, showId) => {
    $(showId).modal('show');
    $(hideId).modal('hide');
}



// to submit details and do action using AJAX
// used in the create user, edit user
const submitForm = (modalId, formId, action, controller) => {
    try {
        let form = document.querySelector(formId);
        let formData = new FormData(form); // model binding in the controller
        
        $.ajax({
            url: `/${controller}/${action}`,
            type: 'POST',
            contentType: false,
            processData: false,
            data: formData, 
            success: (response) => { 

                // if the response is successful, hide the modal and show a success message
                // then redirect to the index page of the controller
                // else, hide the modal and show an error message
                if (response.success) {
                    $(modalId).modal('hide');
                    toastr.success(response.message);
                    setTimeout(() => {
                        window.location.href = `/${controller}/Index`
                    }, toastr.options.timeOut);
                } else {
                    $(modalId).modal('hide');
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                $(modalId).modal('hide');
                toastr.error('Error. Please try again.');
            }
        });
    } catch (err) {
        $(modalId).modal('hide');
        console.error('Error in submitForm:', err);
        toastr.error('Error submitting form. Please try again.');
    }
};



// for delete actions
// used in the delete user
const deleteModal = (btnId, modalId, action, controller) => {

    try {
        let id = $(btnId).data('id');

        console.log(id);

        $.ajax({
            url: `/${controller}/${action}`,
            type: 'POST',
            data: { Id: id },
            success: (response) => {
                if (response.success) {
                    $(modalId).modal('hide');
                    toastr.success(response.message);
                    setTimeout(() => {
                        window.location.href = `/${controller}/Index`
                    }, toastr.options.timeOut);
                } else {
                    $(modalId).modal('hide');
                    toastr.error(response.message);
                }

            },
            error: (xhr, status, error) => {
                console.error('Error deleting user', error);
            }
        });
    } catch (err) {
        $(modalId).modal('hide');
        console.error('Error in submitForm:', err);
        toastr.error('Error submitting form. Please try again.');
    }
}

//for cancel bookings
const cancelModal = (btnId, modalId, action, controller) => {
    try {
        let id = $(btnId).data('id');

        console.log(id);

        $.ajax({
            url: `/${controller}/${action}`,
            type: 'POST',
            data: { Id: id },
            success: (response) => {
                if (response.success) {
                    $(modalId).modal('hide');
                    toastr.success(response.message);
                    setTimeout(() => {
                        window.location.href = `/${controller}/Index`
                    }, toastr.options.timeOut);
                } else {
                    $(modalId).modal('hide');
                    toastr.error(response.message);
                }
            },
            error: (xhr, status, error) => {
                console.error('Error canceling booking', error);
            }
        });
    } catch (err) {
        $(modalId).modal('hide');
        console.error('Error in submitForm:', err);
        toastr.error('Error submitting form. Please try again.');
    }
}

  