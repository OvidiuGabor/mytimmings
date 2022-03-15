//function for adding "#" on the begining of the input field
(function addhashtag() {
    var tagsinput = document.getElementById("tags");
    var inputValue = "";

    //check if the element exists
    if (tagsinput) {
        inputValue = tagsinput.value;
        if (inputValue.indexOf("#") < 0) {
            tagsinput.value = "#"
        }
    }

}())


//function for checking the input on the textbox
//last character is mandatory to be #
//check for # at the beging of the word, and if there is, all character are colored with the same color and with italic so that the user know is typing a hashtag

$("#tags").on('keydown change', function (e) {
    var inputValue = $(this).val()
    if (inputValue == "") {
        $(this).val("#")
    }
    //if the user type a # that is not at the begining of the word, we then remove it
    if (e.key == '#') {
        //console.log("you have pressed #")
        if (inputValue.charAt(0) == "#") {
            e.preventDefault();
            $(this).val(inputValue);
        } else {
            e.preventDefault();
            $(this).val("#" + inputValue)
        }
    }

    if (e.keyCode == 13) {
        AddTagToList(inputValue)
        $(this).val("#")
    }

})

var previousNumber = 0;
function AddTagToList(tag) {
    //set a copy of the global colors
    //if there are no colors in the list, we then use the constant default list

    let localColors = colors
    if (localColors.length == 0)
        localColors = defaultColors;

    var container = document.getElementById("tags-container")
    let randomNumber = Math.floor(Math.random() * colors.length)

    while (previousNumber == randomNumber) {

        randomNumber = Math.floor(Math.random() * colors.length);

    }
    previousNumber = randomNumber;
    var pickedColor = colors[randomNumber]

    var backgroundColor = "rbga(167, 58, 232, 0.1)";
    var color = "rbga(167, 58, 232, 1)";

    if (pickedColor != undefined || pickedColor != "") {
        backgroundColor = pickedColor.replace(",1)", ",0.1)");
        color = pickedColor;
    }


    //container.innerHTML += "<p style= 'padding: 7px; margin: 12px; border-radius: 50px; font-size: 12px; margin-left: 0px; color: red; background-color: blue'>" + tag + "</p>"
    var element = "<p style='padding: 7px; cursor: pointer; margin: 12px; border-radius: 50px; font-size: 12px; margin-left: 0px; color: " + color + "; background-color: " + backgroundColor + "' class='tag-item' onclick='removeSelf(this)'>" + tag + "</p>"
    container.innerHTML += element

}

function removeSelf(self) {
    self.remove()
}

function submitData() {
    var notif = new Notifier();
    let formData = new FormData();
    let tags = [];
    let tagsColor = [];
    //set values of the form
    formData.append("currentDate", $("#dateInput").val());
    formData.append("startDate", $("#startTime").val());
    formData.append("endDate", $("#endTime").val());
    formData.append("status", $("#status").val());
    formData.append("projectId", $("#project").val());
    formData.append("description", $("#details").val());
    formData.append("title", $("#title").val());

    //get tags
    var tagshtml = document.getElementsByClassName("tag-item");
    for (var i = 0; i < tagshtml.length; i++) {
        var value = tagshtml[i].innerHTML;
        var color = tagshtml[i].style.backgroundColor;

        tags.push(value);
        tagsColor.push(color)
    }
    formData.append("tags", tags);
    formData.append("tagsColors", tagsColor);
    if (validateForm()) {
        $.ajax({
            type: "post",
            url: '@Url.Action("AddTask", "Activity")',
            contentType: false,
            processData: false,
            data: formData,
            success: function (data) {
                console.log(data)
                if (data.status == "success") {
                    $('#modal-right').modal('hide')
                    notif.success("New Task Created successfully", "Success");
                    //console.log(data.recordAdded)
                    createWorklogElement(data.recordAdded, data.position);
                    //reset the form
                    resetForm();

                } else if (data.status == "failed") {
                    notif.error(parseErrorMessage(data.errors), "Error");
                }


            },
            error: function (data) {
                notif.error("Something happen! Please try again later!", "Error");
            }
        })

    } else {

    }

    //console.log(formData)
}

function parseErrorMessage(errors) {
    var errorMessage = ""

    for (const [key, value] of Object.entries(errors)) {
        temp = value + "</br>";
        errorMessage += temp;
    }
    return errorMessage
}

function validateForm() {

    var formValid = true;

    var forms = document.getElementsByClassName('needs-validation');
    var validation = Array.prototype.filter.call(forms, function (form) {
        form.classList.add('was-validated');
        if (form.checkValidity() === false) {
            formValid = false;
        }
    });


    return formValid
}

function resetForm() {
    //Reset the input
    //since we have validation, when reseting the input, the fields that are requried will be with red.
    //Need to investigate this further
    $('form').get(0).reset();


    //remove all tags
    var tags = document.getElementsByClassName("tag-item");
    if (tags != null) {
        for (var i = 0; i < tags.length; i++) {
            tags[i].remove();
        }

    }


}