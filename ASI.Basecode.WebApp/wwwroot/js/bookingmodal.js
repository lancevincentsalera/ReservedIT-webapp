
// show recurrence list when custom button is clicked
// hide recurrence list when schedule once button is clicked
// check booking conflict when custom button is clicked
$(document).ready(function () {
    $('#customButton').on('click', function () {
        $('.custom-recur').toggle();
        $('#customButton').addClass('active');
        $('#scheduleOnceButton').removeClass('active');
        checkBookingConflict();
    });

    $('#scheduleOnceButton').on('click', function () {
        $(`.checkbox-recur input[type="checkbox"]`).prop('checked', false).trigger('change');
        $('.custom-recur').hide();
        $('#scheduleOnceButton').addClass('active');
        $('#customButton').removeClass('active');
        checkBookingConflict();
    });
})


// check booking conflict when date is changed
function checkBookingConflict() {
    $.ajax({
        url: '/Rooms/CheckBookingConflict', // url to controller action
        type: 'POST',
        data: {
            startDate: $('#startDate').val(),
            endDate: $('#endDate').val(),
            roomName: $('#roomName').val(),
            dayOfTheWeekIds: $('input[name="DayOfTheWeekIds"]:checked').map(function () {
                return this.value;
            }).get()
        },
        success: function (response) {

            // if there is a conflict, add is-invalid class to date fields and show error message
            // if there is no conflict, remove is-invalid class and clear error message
            if (response.isConflict) {
                $('#startDate').addClass('is-invalid');
                $('#endDate').addClass('is-invalid');
                $('#errorMessage').text(response.errorMessage);
            } else {
                $('#startDate').removeClass('is-invalid');
                $('#endDate').removeClass('is-invalid');
                $('#errorMessage').text('');
            }
        }
    });
}