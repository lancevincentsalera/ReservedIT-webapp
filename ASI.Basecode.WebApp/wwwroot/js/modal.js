
const passId = (btnId, confirmBtn) => {
    let Id = $(btnId).data('id');
    console.log(Id)
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
            console.error('Error fetching user details:', error);
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


  