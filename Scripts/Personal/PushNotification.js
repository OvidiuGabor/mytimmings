function showNotificationsModal() {
    var $modal = $("#pushNotificationsModal");
    $modal.toggle();
}

//close a modal when click otside of it
$(document).on("click", function (e) {
    var $modal = $("#pushNotificationsModal");
    if (!$modal.is(e.target) && $modal.has(e.target).length === 0 && !e.target.classList.contains("notificationCheck") && !e.target.classList.contains("fa-bell")) {
        $modal.css("display", "none")
    }
})


$(function () {

    var notifications = $.connection.notificationHub;
    notifications.client.receiveNotification = function () {
        console.log("aa")
        getAllNotif()
    };

    console.log(notifications)
    $.connection.hub.start().done(function () {
        console.log("aaa");
        getAllNotif();
    })
        .fail(function (e) { console.log(e) })
})



function getAllNotif() {

    $.ajax({
        url: '../Portal/GeLiveNotification',
        contentType: 'application/html ; charset:utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            const obj = JSON.parse(result)
            UpdateNotifNumber(obj)
            AddNotificationToView(obj)
            console.log(obj);
        },
        error: function (result) {
            alert(result)
        }

    })
}


function UpdateNotifNumber(result) {

    var span = document.getElementById("notif-placeholder");
    var notifbell = document.getElementById("notif-bell");
    if (result.length > 0) {

        var notifCount = result.length;
        span.innerHTML = notifCount;
        notifbell.style.color = "#f93939"
    } else {

        span.innerHTML = "";
        notifbell.style.color = "black"
    }
}


function AddNotificationToView(result) {

    var $container = document.getElementById("notification-main-container");
    console.log($container)

    var linebreak = "<hr />"

    if (result.length > 0) {
        $container.innerHTML = null;
        for (var i = 0; i < result.length; i++) {
            var current = result[i];

            var html = '<div class="notif">';
                html += '<div class="notification-body"> ';
                    html += '<div class="notification-type">';
            switch (current.Type) {
                case "Info": html += '<p class="notif-type-text notif-blue">' + current.Type + '</p>'
                    break;
                case "Approve": html += '<p class="notif-type-text notif-purple">' + current.Type + '</p>'
                    break;
                case "Success": html += '<p class="notif-type-text notif-green">' + current.Type + '</p>'
                    break;
                case "Warning": html += '<p class="notif-type-text notif-red">' + current.Type + '</p>'
                    break;

                default: html += '<p class="notif-type-text notif-orange">' + current.Type + '</p>'
            }
                        ;
                    html += '</div';
                html += ' <div class="notification-title">';
                    html += ' <p class="notif-title-text">' + current.Title + '</p>';
                html += ' </div>'

                 //Message part
                html += '<div class="notification-message">'
                    html += '<p class="notif-message-body-text">'
                    //check for the length of the message
                    //if message length > 50 characters then add the click here part for opening the notification widow page, in order to see more detail about the message
                    if (current.Message.length > 70) {
                        html += current.Message.substring(0, 70) + '...  <a href = "#" style="color: red">read</a>'
                    } else {
                        html += current.Message
                    }

                    html += '</p>'
                    if (current.Type == "Approve") {
                        html += "<a href = '#' class = 'notif-small-link-btn' style='margin-right: 30px'>Approve</a>"
                        html += "<a href = '#' class = 'notif-small-link-btn'>Decline</a>"
                    }

                 html += '</div>'

            //add the signature
                    html +='<div class="notification-signature">'
                        html += '<p class="notif-signature-text">' + current.Sender + '</p>'
                    html += '</div>'
            //add the time

            html += '<div class="notification-time">'
            html += '<p class="notif-time-text">' + formatDate(ConvertJsonDate(current.DateReceived)) + '</p>'
            html += '</div></div>'
            html += linebreak
            
            $container.innerHTML += html
        }

      
    } else {

    }
}


//Format dthe date to: 16 Nov 2021 at 9:30 AM
function formatDate(date) {

    //check if the el is an actually date
    if (typeof date !== 'undefined' && typeof date.getMonth === "function") {

        var time = date.toLocaleTimeString();

        var formatedDate = date.getDate() + " " + mothsList(date.getMonth()) + " " + date.getFullYear() + " at " + time

        return formatedDate
    }
    else
    {
        return "N/A"
    }

}




//<div class="notif">
//    <div class="notification-body">
//        <div class="notification-type">
//            <p class="notif-type-text">Information</p>
//        </div>
//        <div class="notification-title">
//            <p class="notif-title-text">Update Yesterday Clock Out</p>
//        </div>
//        <div class="notification-message">
//            <p class="notif-message-body-text">You have an update pending. Click <span style="color: red; cursor: pointer">here</span></p>
//        </div>
//        <div class="notification-signature">
//            <p class="notif-signature-text">System</p>
//        </div>
//    </div>
//    <div class="notification-time">

//        <p class="notif-time-text"> 16 Nov 2021 at 9:30 AM</p>
//    </div>
//</div>