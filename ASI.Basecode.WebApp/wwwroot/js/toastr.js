document.addEventListener('DOMContentLoaded', () => {
    if (errorMessage) {
        toastr.error(errorMessage);
    }

    if (successMessage) {
        toastr.success(successMessage);
    }
});
