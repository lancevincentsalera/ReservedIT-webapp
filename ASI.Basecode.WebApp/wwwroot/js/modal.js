
const displayModal = () => {
    $('#createUserModal').modal('show');
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

const submitForm = (btnId, formId, action, controller) => {
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
                $(btnId).modal('hide');
                toastr.success('User created successfully. Redirecting back to dashboard');
                setTimeout(() => {
                    window.location.href = `/${controller}/Index`
                }, toastr.options.timeOut)
            },
            error: function (xhr, status, error) {
                $(btnId).modal('hide');
                toastr.error('Error creating user. Please try again.');
            }
        });
    } catch (err) {
        console.error('Error in submitForm:', err);
        toastr.error('Error submitting form. Please try again.');
        $(btnId).modal('hide');
    }
};