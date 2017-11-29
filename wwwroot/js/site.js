$(document).ready(function () {
    $('.datepicker').datepicker();
    $('#add-item-button').on('click', addItem);
});

function addItem() {
    $('#add-item-error').hide();
    var newTitle = $('#add-item-title').val();
    var dueAt = $('#add-item-dueAt').val();

    $.post('/Todo/AddItem', { title: newTitle, dueAt : dueAt }, function () {
        window.location = '/Todo';
    })
        .fail(function (data) {
            if (data && data.responseJSON) {
                var firstError = data.responseJSON[Object.keys(data.responseJSON)[0]];
                $('#add-item-error').text(firstError);
                $('#add-item-error').show();
            }
        })
}
