
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
                if (key !== 'Recurrence') {
                    $(`${modalId} input[name="${key}"]`).val(response[key]);
                    console.log(`${key}:`, response[key]);
                }
            }
            if (response.Recurrence.length == 0) {
                $('.custom-recur').hide();
                $('#scheduleOnceButton').addClass('active');
                $('#customButton').removeClass('active');
            } else if (response.Recurrence.length > 0) {
                console.log('Processing recurrences...');

                response.Recurrence.forEach(rec => {
                    const dayId = rec.DayOfWeekId;
                    const checkboxSelector = `${modalId} .checkbox-recur input[name="DayOfTheWeekIds"][value="${dayId}"]`;
                    const checkbox = $(checkboxSelector);

                    console.log(`Checking checkbox with value: ${dayId}`);
                    console.log("Checkbox selector: ", checkboxSelector);

                    if (checkbox.length > 0) {
                        checkbox.prop('checked', true).trigger('change');
                        console.log(`Checkbox with value ${dayId} is checked.`, checkbox.is(":checked"));
                    } else {
                        checkbox.prop('checked', false).trigger('change');
                        console.log(`Checkbox with value ${dayId} not found.`);
                    }
                });
                
                $('.custom-recur').show();
                $('#customButton').addClass('active');
                $('#scheduleOnceButton').removeClass('active');
            }
            // Update other input fields with fetched data
            
        },
        error: (xhr, status, error) => {
            console.error('Error fetching booking details:', xhr.responseText, error);
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

  