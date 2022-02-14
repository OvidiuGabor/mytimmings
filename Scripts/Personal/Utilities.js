function ConvertJsonDate(date) {

    if (typeof date !== 'undefined' && date.length > 0) {
        var num = date.match(/\d+/g); //regex to extract numbers 
        var date = new Date(parseFloat(num)); //converting to date
        //console.log(date)
        return ConvertFromUTC(date);
    }
}


function mothsList(index) {
    if (index > 0 && index < 12) {
        var month = new Array();
        month[0] = "Jan";
        month[1] = "Feb";
        month[2] = "Mar";
        month[3] = "Apr";
        month[4] = "May";
        month[5] = "Jun";
        month[6] = "Jul";
        month[7] = "Aug";
        month[8] = "Sep";
        month[9] = "Oct";
        month[10] = "Nov";
        month[11] = "Dec";
    }
   

    return month[index];
}

function ConvertFromUTC(date) {

    var newDate = new Date(date.getTime() + +date.getTimezoneOffset() * 60 * 1000)

    var offset = date.getTimezoneOffset() / 60;
    var hours = date.getHours();

    newDate.setHours(hours - offset);

    return newDate;   
}

//Adds a new function toIndianFormat() to all instances of Date.
//can be used as an extension method for date
// new Date().toIndianFormat()
Date.prototype.toIndianFormat = function () {

    let monthNames = ["Jan", "Feb", "Mar", "Apr",
        "May", "Jun", "Jul", "Aug",
        "Sep", "Oct", "Nov", "Dec"];

    let day = this.getDate();

    let monthIndex = this.getMonth();
    let monthName = monthNames[monthIndex];

    let year = this.getFullYear();

    return `${day} ${monthName} ${year}`;
}


function formatTimeInAMPM(date) {

    if (date != null) {
        let hours = date.getHours();
        let minutes = date.getMinutes();

        var newFormat = hours >= 12 ? "PM" : "AM";
        hours = hours % 12;
        hours = hours ? hours : 12;
        minutes = minutes < 10 ? "0" + minutes : minutes;
        hours = hours < 10 ? "0" + hours : hours;


        return hours + ":" + minutes + " " + newFormat;
    } else {
        return null;
    }

}
