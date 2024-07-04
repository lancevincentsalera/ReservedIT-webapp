
const displayModal = (modalId) => {
    console.log("here");

    $(modalId).modal('show');
}

const getUserDetails = (btnId, modalId, action, controller) => {
    let id = $(btnId).data('user-id');

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

const displayConfirmationModal = (hideId, showId) => {
    event.preventDefault();
    $(hideId).modal('hide');
    $(showId).modal('show');
}
const hideConfirmationModal = ( hideId, showId) => {
    $(showId).modal('show');
    $(hideId).modal('hide');
}

const submitForm = (modalId, formId, action, controller) => {
    try {
        let form = document.querySelector(formId);
        let formData = new FormData(form);

        $.ajax({
            url: `/${controller}/${action}`,
            type: 'POST',
            contentType: false,
            processData: false,
            data: formData,
            success: (response) => { 
                $(modalId).modal('hide');
                toastr.success('Successful. Redirecting back to dashboard');
                setTimeout(() => {
                    window.location.href = `/${controller}/Index`
                }, toastr.options.timeOut)
            },
            error: function (xhr, status, error) {
                $(modalId).modal('hide');
                toastr.error('Error. Please try again.');
            }
        });
    } catch (err) {
        console.error('Error in submitForm:', err);
        toastr.error('Error submitting form. Please try again.');
        $(modalId).modal('hide');
    }
};