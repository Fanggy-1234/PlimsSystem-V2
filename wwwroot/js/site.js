// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('form').on('submit', function () {
        if (this.checkValidity()) {
            $(this).find('button.disable-on-submit').each(function() {
                var button = $(this);
                button.prop('disabled', true);
                button.data('original-text', button.html());
                button.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
            });
        }
    });
});