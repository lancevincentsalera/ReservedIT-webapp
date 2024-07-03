
const displayModal = () => {
    $('#createUserModal').modal('show');
}
const displayConfirmationModal = (hideId, showId) => {
    $(hideId).modal('hide');
    $(showId).modal('show');
}
const hideConfirmationModal = (hideId, showId) => {
    $(showId).modal('show');
    $(hideId).modal('hide');
}

const submitForm = (formId, action, controller) => {
    $.ajax({
        url: `/${controller}/${action}`,
        type: 'POST',
        data: $(formId).serialize(),
        success: (response) => {
            console.log('Form submitted successfully');
        },
        error: function (xhr, status, error) {
            console.error('Error submitting form:', error);
        }
    })
}