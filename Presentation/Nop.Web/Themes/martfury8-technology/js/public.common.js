/*
** nopCommerce custom js functions
*/



function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function setLocation(url) {
    window.location.href = url;
}

function displayAjaxLoading(display) {
    if (display) {
        $('.ajax-loading-block-window').show();
    }
    else {
        $('.ajax-loading-block-window').hide('slow');
    }
}

function displayPopupNotification(message, messagetype, modal) {
    //types: success, error, warning
    var title = $('#dialog-notifications-success').text();
    if (messagetype == 'success') {
        //success
        title = title;
    }
    else if (messagetype == 'error') {
        //error
        title = $('#dialog-notifications-error').text();
    }
    else if (messagetype == 'warning') {
        //warning
        title = $('#dialog-notifications-warning').text();
    }
    else {
        //other
        title = title;
    }
    $('#PopupNotification .modal-title').text(title);
    //we do not encode displayed message
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p>' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p>' + message[i] + '</p>';
        }
    }
    $('#PopupNotification .modal-body').html(htmlcode);
    $('#PopupNotification').modal({
        keyboard: true
    });
}
function displayPopupContentFromUrl(url, title, modal, width) {
    $('#modalLoadContent .modal-title').text('');
    $('#modalLoadContent .modal-body').text('');
    if (width) {
        $('#modalLoadContent .modal-dialog').addClass(width);
    }
    $('#modalLoadContent .modal-body').load(url, function() {
        $('#modalLoadContent .modal-title').text(title);
        $('#modalLoadContent').modal();
    })
        
}

var barNotificationTimeout;
function displayBarNotification(message, messagetype, timeout) {
    clearTimeout(barNotificationTimeout);

    //types: success, error, warning
    var cssclass = 'success';
    if (messagetype == 'success') {
        cssclass = 'success';
    }
    else if (messagetype == 'error') {
        cssclass = 'error';
    }
    else if (messagetype == 'warning') {
        cssclass = 'warning';
    }
    //remove previous CSS classes and notifications
    $('#bar-notification')
        .removeClass('success')
        .removeClass('error')
        .removeClass('warning');
    $('#bar-notification .content').remove();

    //we do not encode displayed message

    //add new notifications
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p class="content">' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p class="content">' + message[i] + '</p>';
        }
    }
    $('#bar-notification').append(htmlcode)
        .addClass(cssclass)
        .fadeIn('slow')
        .on('mouseenter', function ()
            {
                clearTimeout(barNotificationTimeout);
            });

    $('#bar-notification .close').off('click').on('click', function () {
        $('#bar-notification').fadeOut('slow');
    });

    //timeout (if set)
    if (timeout > 0) {
        barNotificationTimeout = setTimeout(function () {
            $('#bar-notification').fadeOut('slow');
        }, timeout);
    }
}
function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}


// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};