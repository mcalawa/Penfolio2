/**
 * Scripts for the profile and writing create pages to give information that 
 * explains all of the various parts of the create pages.
 * */

/**
 * A function for emptying a field and appending a description
 * from another element to it.
 * @param {string} id (id of the container you want to empty/write to as a string)
 * @param {string} descriptionId (the id of the element that we are taking the description from)
 * */
function showDescription(id, descriptionId) {
    $("#" + id).empty();
    var description = $("div#" + descriptionId).html();
    $("#" + id).append(description);
}

/**
 * A function for emptying a field and appending an explanation to it.
 * Unlike showDefinition, this function doesn't take a definition from
 * the model but passes one to the function as a string.
 * @param {string} id (id of the container you want to empty/write to as a string)
 * @param {string} explantion (the string with the explanation that is being written to the container)
 * */
function showExplanation(id, explanation) {
    $("#" + id).empty();
    $("#" + id).append(explanation);
}

/**
 * A function to make sure that publisher and friend access are always 
 * selected if public access is and that public access isn't checked if 
 * both publisher and friend access aren't.
 * @param {string} id (the id name of the container for the checkbox you are changing)
 * */
function changeAccess(id) {
    //if public access is checked, check friend access and publisher access
    if (id === "public-access" && $("div#" + id + " input[type=checkbox]").is(':checked')) {
        $("div#friend-access input[type=checkbox]").prop('checked', true);
        $("div#publisher-access input[type=checkbox]").prop('checked', true);
        $("div#my-agent-access input[type=checkbox]").prop('checked', true);
    } //if friend access or publisher access were unchecked, uncheck public access as well
    else if ((id === "friend-access" || id === "publisher-access") && $("div#" + id + " input[type=checkbox]").is(':checked') !== true) {
        $("div#public-access input[type=checkbox]").prop('checked', false);
    }
    else if (id === "my-agent-access" && $("div#" + id + " input[type=checkbox]").is(':checked') !== true) {
        $("div#public-access input[type=checkbox]").prop('checked', false);
        $("div#publisher-access input[type=checkbox]").prop('checked', false);
    }
}
