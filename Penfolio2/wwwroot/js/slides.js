/**
 * Scripts for dealing with the "slides" on the Create view in
 * the Writing controller
 * Also populates the confirmation fields and some of the hidden
 * form fields that allow the controller to access the information
 * from the submitted form
 *
 * Finally, features a small function to check if an element is
 * empty using jQuery
 */

/**
 * Interacts with the Writing controller Create view to switch
 * between the different "pages" that are used to enter
 * information when creating or editing a writing.
 * Works by passing the class of the current (noncollapsed)
 * slide and the class of the next slide (the collapsed slide
 * we want to go to) and adding the collapse class to the current
 * slide and removing the collapse class from the next slide.
 * Can be used to go "back" by inserting the class name of the
 * previous slide into the parameter next.
 * @param {string} current (class name)
 * @param {string} next (class name)
 */
function loadSlide(current, next) {
    //collapse the current slide and uncollapse the next slide
    $("div." + current).addClass("collapse");
    $("div." + next).removeClass("collapse");
}

/**
 * Works the same way as the previous function to switch between slides,
 * but also populates the various spans and divs on the confirmation "page"
 * with the current values so that the user can decide whether the values
 * they are using are correct before saving. Uses a different function name
 * rather than a function overload because apparently trying to overload a
 * function in JavaScript makes your code explode.
 * @param {string} current (class name)
 * @param {string} next (class name)
 * @param {int[]} profiles (an array of all the ProfileIds; from the Model)
 * @param {int[]} formatTags (an array of all the FormatIDs; from the view's Controller)
 * @param {int[]} genreTags (an array of all the GenreIDs; from the view's Controller)
 */
function loadSlideAndConfirm(current, next, profiles, formatTags, genreTags) {
    //collapse the current slide and uncollapse the next slide as usual
    $("div." + current).addClass("collapse");
    $("div." + next).removeClass("collapse");

    //empty the field for title confirmation, then append the value of the title input
    $("#confirmTitle").empty();
    $("#confirmTitle").append($("#title").val());

    //empty the field for the description confirmation, then append the value of the description input
    $("#confirmDescription").empty();
    $("#confirmDescription").append($("#description").val());

    //empty the field for the profile confirmation
    $("#confirmProfiles").empty();
    //empty the field for recording selected profiles
    $("#selectedProfiles").empty();
    //for each writing profile this user has
    for (var i = 0; i < profiles.length; i++) {
        //if the checkbox for the current profile is checked
        if ($("#profileContainer input." + profiles[i]).is(':checked')) {
            //append the value of profile name that matches this checkbox to the confirmation field
            $("#confirmProfiles").append($("#profileContainer span." + profiles[i]).html());
            //append a break so everything is still legible 
            $("#confirmProfiles").append("<br />");

            if (isEmpty($("#selectedProfiles"))) {
                $("#selectedProfiles").append(profiles[i]);
            }
            else {
                $("#selectedProfiles").append(",");
                $("#selectedProfiles").append(profiles[i]);
            }
        }
    }

    //empty the field for format tag confirmation
    $("#confirmFormats").empty();
    //empty the field for recording selected formats
    $("#selectedFormats").empty();
    //for each format tag in the database
    for (i = 0; i < formatTags.length; i++) {
        //if the current format tag has been checked
        if ($("#formatTagContainer input." + formatTags[i]).is(':checked')) {
            //append the value of the format tag name that matches this checkbox to the confirmation field
            $("#confirmFormats").append($("#formatTagContainer span." + formatTags[i] + " span").html());
            //append a break so everything is still legible 
            $("#confirmFormats").append("<br />");

            if (isEmpty($("#selectedFormats"))) {
                $("#selectedFormats").append(formatTags[i]);
            }
            else {
                $("#selectedFormats").append(",");
                $("#selectedFormats").append(formatTags[i]);
            }
        }
    }

    ////empty the field for genre tag confirmation
    //$("#confirmGenres").empty();
    ////for each genre tag in the database
    //for (i = 0; i < genreTags.length; i++) {
    //    //if the current genre tag has been checked
    //    if ($("#genreTagContainer span." + genreTags[i] + " input").is(':checked')) {
    //        //append the value of the genre tag name that matches this checkbox to the confirmation field
    //        $("#confirmGenres").append($("#genreTagContainer span." + genreTags[i] + " span").html());
    //        //append a break so everything is still legible
    //        $("#confirmGenres").append("<br />");
    //    }
    //}

    //empty the field for public access confirmation
    $("#confirmPublicAccess").empty();
    //if the checkbox for allowing public access is checked, append true
    if ($("#public-access input").is(':checked'))
        $("#confirmPublicAccess").append("True");
    else //otherwise, append false
        $("#confirmPublicAccess").append("False");

    //empty the field for friend access confirmation
    $("#confirmFriendAccess").empty();
    //if the checkbox for allowing friend access is checked, append true
    if ($("#friend-access input").is(':checked'))
        $("#confirmFriendAccess").append("True");
    else //otherwise, append false
        $("#confirmFriendAccess").append("False");

    //empty the field for publisher access confirmation
    $("#confirmPublisherAccess").empty();
    //if the checkbox for allowing publisher access is checked, append true
    if ($("#publisher-access input").is(':checked'))
        $("#confirmPublisherAccess").append("True");
    else //otherwise, append false
        $("#confirmPublisherAccess").append("False");

    //empty the field for minor access confirmation
    $("#confirmMinorAccess").empty();
    //if the checkbox for allowing minor access is checked, append true
    if ($("#minor-access input").is(':checked'))
        $("#confirmMinorAccess").append("True");
    else //otherwise, append false
        $("#confirmMinorAccess").append("False");

    //empty the field for minor access confirmation
    $("#confirmShowsUpInSearch").empty();
    //if the checkbox for allowing minor access is checked, append true
    if ($("#shows-up-in-search input").is(':checked'))
        $("#confirmShowsUpInSearch").append("True");
    else //otherwise, append false
        $("#confirmShowsUpInSearch").append("False");

    ////empty the field for like confirmation
    //$("#confirmLikes").empty();
    ////if the checkbox for allowing likes is checked, append true
    //if ($("span.likes input").is(':checked'))
    //    $("#confirmLikes").append("True");
    //else //otherwise, append false
    //    $("#confirmLikes").append("False");

    ////empty the field for comment confirmation
    //$("#confirmComments").empty();
    ////if the checkbox for allowing comments is checked, append true
    //if ($("span.comments input").is(':checked'))
    //    $("#confirmComments").append("True");
    //else //otherwise, append false
    //    $("#confirmComments").append("False");

    ////empty the field for critique confirmation
    //$("#confirmCritique").empty();
    ////if the checkbox for allowing critique is checked, append true
    //if ($("span.critique input").is(':checked'))
    //    $("#confirmCritique").append("True");
    //else //otherwise, append false
    //    $("#confirmCritique").append("False");

    //this field is for use on the normal create and edit pages where you are working with the editor
    //empty the field for text content confirmation
    $("#confirmText").empty();
    //take the formatted text from the editor and put it the text confirmation field
    $("#confirmText").append($(".ql-editor").html());

    ////this field is for use on the upload and upload edit pages where there is no editor
    ////empty the field for file to upload confirmation
    //$("confirmFile").empty();
    ////take the name of the file to upload and put it in the file to upload confirmation field
    //$("confirmFile").append($("input#File").val());

    //this field is for use on the normal create and edit pages where you are working with the editor
    //this is where the information from the editor is put into a format that can be read as part of the submitted form collection
    //#editorContent is a hidden input; because inputs cannot have HTML in them, HTML characters must be escaped
    //the main characters that count as HTML characters are the brackets used for tags
    //therefore, < must be replaced with &lt; and > must be replaced with &gt; before adding the string to #editorContent
    $("#editorContent").val($(".ql-editor").html().replace(/</g, "&lt;").replace(/>/g, "&gt;"));

    ////this field is for use on the normal create and edit pages where you are working with the editor
    ////this is where the file name (everything before the file extention) is decided for created (not uploaded) writings
    ////this file name will be used when the user downloads a piece of writing and is taken from the title
    ////because there are certain characters that can't be used in file names, these characters are replaced with ""
    ////any kind of white space is replaced with an underscore
    //var fileTitle = $("#title").val().replace(/\s/g, "_").replace(/[\~\#\%\&\*\{\}\\\:\<\>\?\/\+\|!=.]/g, "");
    ////now it is time to assign the file name to the hidden input so that it can be read as part of the submitted form collection
    ////if the fileTitle after getting rid of all the white space and invalid characters is an empty string
    //if (fileTitle === "") {
    //    //put a generic file name like "document" in the #fileName input
    //    $("#fileName").val("document");
    //}
    //else //otherwise, if the file name is already a valid one, assign its value to the #fileName input
    //    $("#fileName").val(fileTitle);

    //empty the validation message field; this is used to put error messages if there is a reason the form can't be submitted
    $("#validationMessage").empty();

    //this field is for use on the normal create and edit pages where you are working with the editor
    //the upload pages do not use a submit button with this ID, so they will not be effected by this
    //if the writing doesn't have a title or an author or text, inform the user and disable the submit button
    if (isEmpty($("#confirmTitle")) || isEmpty($("#confirmText")) || isEmpty($("#confirmProfiles"))) {
        $("#validationMessage").append("Writing cannot be saved without a title, author, and body of text.");
        $("#createWriting").prop("disabled", true);
    }
    else { //otherwise, if the writing is valid, remove the disabled property from the submit button
        $("#createWriting").prop("disabled", false);
    }
}

/*
 * Checks if the value of an HTML element is the same as an empty string
 * by first trimming any white space; returns true if the trimmed string
 * is empty and returns false if the trimmed string is not equal to ""
 * Code from Emil's answer on StackOverflow:
 * https://stackoverflow.com/questions/6813227/how-do-i-check-if-an-html-element-is-empty-using-jquery
 * @param {HTML element} el
 */
function isEmpty(el) {
    return !$.trim(el.html());
}