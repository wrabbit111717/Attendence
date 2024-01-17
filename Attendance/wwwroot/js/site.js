// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

    $(document).ready(function() {
        $("#createBtn").click(function (e) {
            e.preventDefault();
            if ($('#strUsername').val().length == 0)
                alert('Please enter username');
            return false;
        });
});
