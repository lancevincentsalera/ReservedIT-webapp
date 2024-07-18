
const passId = (btnId, confirmBtn) => {
    let Id = $(btnId).data('id');
    console.log("hey",btnId)
    $(confirmBtn).data('id', Id);
}

const displayModal = (modalId) => {
    $(modalId).modal('show');
}

const getUserDetails = (btnId, modalId, action, controller) => {
    let id = $(btnId).data('id');

    console.log(id);
    console.log(`/${controller}/${action}`)

    $.ajax({
        url: `/${controller}/${action}`,
        type: 'GET',
        data: { userId : id },
        success: (response) => {
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
                    $(`${modalId} input[name="${key}"]`).val(response[key]);
                }
                
            }
        },
        error: (xhr, status, error) => {
            console.error('Error fetching user details:', error);
        }
    })
}

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


const getBookingDetails = (btnId, modalId, action, controller) => {
    let id = $(btnId).data('id');

    console.log(id);
    console.log(`/${controller}/${action}`)

    $.ajax({
        url: `/${controller}/${action}`,
        type: 'GET',
        data: { bookingId: id },
        success: (response) => {
            console.log(response)
            for (let key in response) {
                if (response[key] != null && key !== 'DayOfTheWeekIds' && key !== "TimeFrom" && key !== "TimeTo") {
                    if (key === "StartDate" || key === "EndDate") {
                        let date = new Date(response[key]);
                        let timeKey = key === "StartDate" ? "TimeFrom" : "TimeTo";
                        let timeString = response[timeKey];

                        if (timeString && typeof timeString === 'string') {
                            let [hours, minutes] = timeString.split(':').map(Number);

                            if (!isNaN(hours) && !isNaN(minutes)) {
                                date.setHours(hours);
                                date.setMinutes(minutes);

                                let formattedDate = date.getFullYear() + '-' +
                                    String(date.getMonth() + 1).padStart(2, '0') + '-' +
                                    String(date.getDate()).padStart(2, '0') + 'T' +
                                    String(date.getHours()).padStart(2, '0') + ':' +
                                    String(date.getMinutes()).padStart(2, '0');


                                let currentTime = new Date();
                                currentTime.setHours(currentTime.getHours() + 2);
                                let minTime = String(currentTime.getHours()).padStart(2, '0') + ':' +
                                    String(currentTime.getMinutes()).padStart(2, '0');

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
                                        const selectedDate = selectedDates[0];
                                        if (selectedDate && selectedDate.toDateString() === new Date().toDateString()) {
                                            instance.set('minTime', minTime);
                                        } else {
                                            instance.set('minTime', '00:00');
                                        }
                                    }
                                });
                                flatpickrInstance.setDate(formattedDate, true);
                            } else {
                                console.error("Invalid time value");
                            }
                        } else {
                            console.error("Invalid time string");
                        }
                    } else if (key === "RoomId") {
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
                        $(`${modalId} input[name="${key}"]`).val(response[key]);
                    }
                    console.log(key, $(`${modalId} input[name="${key}"]`).val() )
                }
            }

            $(`${modalId} .checkbox-recur input[type="checkbox"]`).prop('checked', false).trigger('change');
            if (response.DayOfTheWeekIds && response.DayOfTheWeekIds.$values && response.DayOfTheWeekIds.$values.length > 0) {
                response.DayOfTheWeekIds.$values.forEach(dayId => {
                    const checkboxSelector = `${modalId} .checkbox-recur input[type="checkbox"][value="${dayId}"]`;
                    const checkbox = $(checkboxSelector);

                    if (checkbox.length > 0) {
                        checkbox.prop('checked', true).trigger('change');
                    } 
                });
                

                $('.custom-recur').show();
                $('#customButton').addClass('active');
                $('#scheduleOnceButton').removeClass('active');
            } else {
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
            response.RoomGallery.forEach((img, index) => {
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


let count = 0;

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

const hideConfirmationModal = ( hideId, showId) => {
    $(showId).modal('show');
    $(hideId).modal('hide');
}

const submitForm = (modalId, formId, action, controller) => {
    try {
        let form = document.querySelector(formId);
        let formData = new FormData(form);
        setTimeout(()=>console.log(formData), 5000)

        for (var pair of formData.entries()) {
            console.log(pair[0] + ', ' + pair[1]);
        }

        $.ajax({
            url: `/${controller}/${action}`,
            type: 'POST',
            contentType: false,
            processData: false,
            data: formData,
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

  