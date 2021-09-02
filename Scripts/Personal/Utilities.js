function ConvertJsonDate(date) {

    if (typeof date !== 'undefined' && date.length > 0) {
        var num = date.match(/\d+/g); //regex to extract numbers 
        var date = new Date(parseFloat(num)); //converting to date
        console.log(date)
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