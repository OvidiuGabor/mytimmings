function ConvertToUTC(date) {
        
    var utcDate = new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
    return utcDate;
  
}

function convertUTCDateToLocalDate(date) {
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds()));
}


//Function used for generating the status bar
// take as arguments, 
function generateStatusBar(item, counter, maxworkingtime) {
    var elem = document.getElementById("status-bar").getElementsByClassName("status-bar-tracker")[0];
    var currentItem = item
    var createId = "my-bar-" + counter;
    var createElement = document.createElement('div');
    createElement.classList.add("w3-container")
    createElement.classList.add("w3-center")
    createElement.classList.add("status-bar-interval")

    if (currentItem.status == "Break") {
        createElement.classList.add("w3-red")
    } else if (currentItem.status == "On Duty") {
        createElement.classList.add("w3-blue")
    } else if (currentItem.status == "Meeting") {
        createElement.classList.add("w3-black")
    } else if (currentItem.status == "Extra Time") {
        createElement.classList.add("w3-green")
    } else {
        createElement.classList.add("w3-purple")
    }

    createElement.id = createId;
    createElement.innerHTML = secondsToHms(currentItem.totalTime);
    createElement.style.width = (((currentItem.totalTime * 100) / maxworkingtime)) + "%"
    elem.appendChild(createElement);

    //if there is Extra time, then we need to updated all other elements and substract the with from all of them, with the same ammount that the extra time grows

    if (currentItem.status == "Extra Time") {

        var extratime = currentItem.totalTime;
        var statusElements = document.getElementsByClassName("status-bar-interval");
        var elementslength = statusElements.length - 1;
        var widthtoSubstract = (((extratime / maxworkingtime) * 100) / (elementslength - 1));
        for (var i = 0; i < elementslength; i++) {
            var elemwidth = statusElements[i].style.width.slice(0, -1);
            console.log(elemwidth)
            elemwidth = elemwidth - widthtoSubstract.toFixed(5);
            console.log(elemwidth)
            statusElements[i].style.width = elemwidth + "%";
        }



    }

}



//converts seconds to HH MM SS format
function secondsToHms(d) {
    // d = Number(d);
    var h = Math.floor(d / 3600);
    var m = Math.floor(d % 3600 / 60);
    var s = Math.floor(d % 3600 % 60);

    var hDisplay = h > 0 ? h + (h == 1 ? " h " : " h ") : "";
    var mDisplay = m > 0 ? m + (m == 1 ? " m " : " m ") : "0 m ";
    var sDisplay = s > 0 ? s + (s == 1 ? " s" : " s") : "0 s";
    //console.log(hDisplay + mDisplay + sDisplay)
    return hDisplay + mDisplay + sDisplay;
}



function showAllertMessage(result) {

    if (result.isRedirect) {
        window.location.href = result.redirectUrl
    }

    if (result.result == "Error") {
        //var buttonforallert = '<button type="button" class="close" data-dismiss="alert" aria-label="Close">'
        //buttonforallert +=          ' <span aria-hidden="true">&times;</span>'
        //buttonforallert +=             '</button>'
        $('.alert').removeClass("alert-danger")
        $('.alert').addClass("alert-danger")
        $('.alert').addClass("show")

        //based on the error that we receive, need to change the message

        if (result.message != null) {
            $(".alert").html("<strong>ERROR: </strong>" + result.message)
        } else {
            $(".alert").html("<strong>ERROR: </strong>" + result.projectError + " " + result.statusError)
        }

        $('.alert').alert()

        setTimeout(function () {
            $('.alert').removeClass("show")
            $('.alert').addClass("hide")

        }, 5000)

    }
    else {
        if (result.result == "Success") {
            $('.alert').removeClass("alert-danger")
            $('.alert').addClass("alert-success")
            $('.alert').addClass("show")
            if (result.message != null) {
                $(".alert").html(result.message)
            }
            $('.alert').alert()


            setTimeout(function () {
                $('.alert').removeClass("show")
                $('.alert').addClass("hide")

            }, 5000)
        }
        location.reload();
    }
}