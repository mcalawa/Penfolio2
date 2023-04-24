/**
 * Scripts for populating the format tags on the _Menu partial view
 * and all other partial views derived from it
 *
 * For the purpose of this code, the following information about the
 * models is relevant:
 * 1. A FormatTag is a tag that identifies the type of writing based on
 *    format rather than genre or subject matter (i.e. novel rather than
 *    romance)
 * 2. Each FormatTag has a primary key int FormatID, a string FormatName,
 *    and a string Explanation of the format
 * 3. A FormatCategory is a connection between two different FormatTags
 *    where one is the parent of another
 * 4. FormatCategory is a separate table with its own individual primary
 *    key int FormatCategoryID, a foreign key int FormatID that represents
 *    the child format, a foreign key int (FormatID) renamed as ParentID
 *    that represents the parent of said child format, and a nullable
 *    foreign key int (FormatID) renamed as SecondaryParentID that represents
 *    a secondary parent that is a required parent, if one exists
 * 5. Using the information from FormatCategory, FormatTag additionally has a
 *    list of FormatCategory where the ParentID is the same as FormatTag's
 *    primary key; this list is called ChildFormats
 *
 * The intended behavior created by this script is as follows:
 * 1. When moused over, a format tag should give an explanation of its meaning.
 * 2. The only format tags to show up initially should be those without parents.
 * 3. When the checkbox for a format tag is checked, it should load that tag's
 *    immediate child formats.
 * 4. If a format tag has two parents, it should only load when the checkboxes 
 *    for both parents are checked.
 * 5. When the checkbox for a format tag is unchecked, it should unload that tag's
 *    immediate children unless one of those said children's checkbox is checked,
 *    in which case, all children without a checked checkbox should be unloaded
 *    and the child(ren) with (a) checked checkbox(es) should remain.
 * 6. Some format tags don't have multiple required parents, but have more than one
 *    potential parent. In the case of children that have potential parents in
 *    addition to the one that matches the checkbox being unchecked, the child
 *    will be unloaded if and only if none of the other potential parents are
 *    checked either.
 * 7. "Loaded" is not being used literally. The content of the format tag container
 *    shouldn't be repopulated every time a checkbox is checked or unchecked, it should
 *    just appear that way to the end user through the use of things like display: hidden,
 *    the collapse class, etc.
 */

/**
 * A function to check whether the format tag's checkbox that has just been
 * changed was just checked or unchecked and then call the proper function to
 * load or unload children
 * 
 * @param {int} id (FormatID for current format tag from partial view)
 * @param {int[]} sdChildren (FormatIDs for children of current format tag that have no secondary parent)
 * @param {int[]} mdChildren (FormatIDs for children of current format tag that do have a secondary parent)
 * @param {int[]} dependencies (SecondaryParentIDs for children of current format tag that have a secondary parent)
 * @param {int[]} mpChildren (FormatIDs for children with more than one possible parent other than id; each FormatID is listed once for each additional parent)
 * @param {int[]} altParents (FormatID for each parent other than id; indexes match mpChildren)
 * @param {int[]} childGenres (GenreIDs for child genres that are either fiction or nonfiction)
 * @param {boolean} isFictionOnly (whether or not the tag is fiction only)
 * @param {boolean} isNonfictionOnly (whether or not the tag is nonfiction only)
 * @param {int[]} fictionOnly (Format tags that are only fiction)
 * @param {int[]} nonfictionOnly (Format tags that are only nonfiction)
 * mp stands for multiple parent; this is different than md because md REQUIRES both parents while mp can have ANY of the parents
 */
function ftChildren(id, sdChildren, mdChildren, dependencies, mpChildren, altParents, childGenres, isFictionOnly, isNonfictionOnly, fictionOnly, nonfictionOnly) {
    //if the checkbox for the format tag with a FormatID of id was checked
    if ($("#formatTagContainer span." + id + " input[type=checkbox]").is(":checked")) {
        //if the current tag is fiction only
        if (isFictionOnly === "true") {
            //change the genre to fiction and uncheck any format tags that are no longer valid
            changeFictionOrNonfiction(1, nonfictionOnly);
        } //if the current tag is fiction only
        else if (isNonfictionOnly === "true") { //if the current tag is nonfiction only
            //change the genre to nonfiction and uncheck any format tags that are no longer valid
            changeFictionOrNonfiction(2, fictionOnly);
        } //if the current tag is nonfiction only
        else { //check to see if there are any child genres and if there are, trigger their change function and let it handle the updates
            for (i = 0; i < childGenres.length; i++)
                $("#genreTagContainer span." + childGenres[i] + " input").change();
        } //check to see if there are any child genres and if there are, trigger their change function and let it handle the updates

        //load all the singular dependency children and load any multi-dependency children with their other dependency checked
        loadChildren(sdChildren, mdChildren, dependencies);
    } //if we're loading children
    else { //if the checkbox was unchecked, you don't need to worry about dependencies
        //for each singular dependency child
        for (i = 0; i < sdChildren.length; i++) {
            //for each singular dependency child where the child has more than one possible parent
            for (var j = 0; j < mpChildren.length; j++) {
                //if the singular dependency child and the multiple possible parent child are the same
                if (sdChildren[i] === mpChildren[j]) {
                    //remove the singular depency child from the array
                    sdChildren.splice(i, 1);
                } //if the singular dependency child and the multiple possible parent child are the same
            } //for each singular dependency child where the child has more than one possible parent
        } //for each singular dependency child

        //add all of the multiple dependency children to the same array as the single dependency children
        for (i = 0; i < mdChildren.length; i++)
            sdChildren.push(mdChildren[i]);

        //call the function to unload all of the singular dependency children with only one possible parent
        //and the multiple dependency children with only one possible combination of parents;
        //remove the children with multiple possible parents only if none of the parents are selected
        unloadChildren(sdChildren, mpChildren, altParents);
    } //if we're unloading children
} //ftChildren(id, sdChildren, mdChildren, dependencies, mpChildren, altParents)

/**
 * A function to load the correct children of a format tag that has been checked.
 * @param {int[]} sdChildren (FormatIDs for children of current format tag that have no secondary parent)
 * @param {int[]} mdChildren (FormatIDs for children of current format tag that do have a secondary parent)
 * @param {int[]} dependencies (SecondaryParentIDs for children of current format tag that have a secondary parent)
 * sd stands for singular dependency and md stands for multiple dependency
 */
function loadChildren(sdChildren, mdChildren, dependencies) {
    //for each singular dependency child
    for (var i = 0; i < sdChildren.length; i++) {
        //if the singular dependency child is collapsed, uncollapse it
        if ($("#formatTagContainer span." + sdChildren[i]).hasClass("collapse"))
            $("#formatTagContainer span." + sdChildren[i]).removeClass("collapse");
    } //for each singular dependency child

    //for each multiple dependency child
    for (i = 0; i < mdChildren.length; i++) {
        //if the other tag it's dependent on is already selected
        if ($("#formatTagContainer span." + dependencies[i] + " input[type=checkbox]").is(":checked")) {
            //if the multiple dependency child is currently collapsed, uncollapse it
            if ($("#formatTagContainer span." + mdChildren[i]).hasClass("collapse"))
                $("#formatTagContainer span." + mdChildren[i]).removeClass("collapse");
        } //if the other tag it's dependent on is already selected
    } //for each multiple dependency child
} //loadChildren

/**
 * A function for depopulating the format tags that no longer have a parent tag (or in the case of tags with multiple dependencies, have both
 * the required tags) selected
 * @param {int[]} sdChildren (Children who are definitely going to be collapsed)
 * @param {int[]} mpChildren (Children with more than one potential parent; will be collapsed only if one of their other potential parents is not already selected)
 * @param {int[]} altParents (Each index corresponds to an index in mpChildren and is a potential parent that is not the format tag that was unchecked)
 * sd stands for singular dependency; in this case it also includes md (or multiple dependency) children
 * mp stands for multiple parent; this is different than md because md REQUIRES both parents while mp can have ANY of the parents
 */
function unloadChildren(sdChildren, mpChildren, altParents) {
    //for all of the children that we KNOW can be depopulated
    for (var i = 0; i < sdChildren.length; i++) {
        //if the checkbox for this child is not checked
        if ($("#formatTagContainer span." + sdChildren[i] + " input[type=checkbox]").is(':checked') !== true) {
            //and this child isn't already collapsed, collapse it
            if ($("#formatTagContainer span." + sdChildren[i]).hasClass("collapse") !== true)
                $("#formatTagContainer span." + sdChildren[i]).addClass("collapse");
        } //if the checkbox for this child is not checked
    } //for all of the children that we KNOW can be depopulated

    //next, it's necessary to check the tags that have alternate parents to see if those parents are 
    //checked or they can be depopulated

    //set the value of the variable current to the first item in the list of children we might unload
    var current = mpChildren[0];
    //set a variable to keep track of whether or not a parent is checked to false
    var parentChecked = false;
    //for each child in mpChildren
    for (i = 0; i < mpChildren.length; i++) {
        //because selected format tags are not depopulated, we only need to check if a tag needs to be depopulated
        //if we know it isn't already checked

        //if this child doesn't have its checkbox checked
        if ($("#formatTagContainer span." + mpChildren[i] + " input[type=checkbox]").is(":checked") !== true) {
            //since some children appear in mpChildren more than once due to having more than 
            //one possible alternate parent, we need to check if the for loop will continue to 
            //run after this and, if so, if the value of mpChildren will be the same at the next
            //index; the reason for this is because we don't want to depopulate a child tag when
            //parentChecked is false unless it is still false after checking the last potential parent

            //if the next item in mpChildren is the same as the current item
            if (i + 1 < mpChildren.length && mpChildren[i + 1] === current) {
                //if parentChecked is not already true and the current potential parent needs to be checked
                if (parentChecked !== true) {
                    //if this potential parent is checked, mark parentChecked true so that a check won't be made
                    //next iteration that might result in a false negative
                    if ($("#formatTagContainer span." + altParents[i] + " input[type=checkbox]").is(":checked"))
                        parentChecked = true;
                } //if parentChecked is not already true and the current potential parent needs to be checked
            } //if the next item in mpChildren is the same as the current item
            else if (mpChildren[i] === current) { //if mpCurrent is the same as last time but will not be next time
                //basically this means this is the last chance to depopulate this child

                //if one of the possible parents for this child wasn't already checked on a previous iteration
                if (parentChecked !== true) {
                    //if the possible parent this time around isn't checked either
                    if ($("#formatTagContainer span." + altParents[i] + " input[type=checkbox]").is(":checked") !== true) {
                        //collapse the child if it isn't arleady collapsed
                        if ($("#formatTagContainer span." + mpChildren[i]).hasClass("collapse") !== true)
                            $("#formatTagContainer span." + mpChildren[i]).addClass("collapse");
                    } //if the possible parent this time around isn't checked either
                } //if one of the possible parents for this child wasn't already checked on a previous iteration
            } //if mpCurrent is the same as last time but will not be next time
            else { //if this child isn't the same child as was being checked lasted iteration
                //reset the values of our variables to their default
                parentChecked = false;
                current = mpChildren[i];

                //if this child has its matching parent checked, mark parentChecked as true
                if ($("#formatTagContainer span." + altParents[i] + " input[type=checkbox]").is(":checked")) {
                    parentChecked = true;
                } //if this potential parent isn't checked and mpChildren will not be the same child next iteration
                else if (i + 1 === mpChildren.length || mpChildren[i + 1] !== current) {
                    //collapse this child if it isn't already collapsed
                    if ($("#formatTagContainer span." + mpChildren[i]).hasClass("collapse") !== true)
                        $("#formatTagContainer span." + mpChildren[i]).addClass("collapse");
                } //if this potential parent isn't checked and mpChildren will not be the same child next iteration
            } //if this child isn't the same child as was being checked lasted iteration
        } //if this child doesn't have its checkbox checked
    } //for each child in mpChildren
} //unloadChildren(sdChildren, mpChildren, altParents)

function changeFictionOrNonfiction(id, ftToChange) {
    //for each format tag that needs to be changed
    for (var i = 0; i < ftToChange.length; i++) {
        //if the format tag is currently checked, uncheck it and indicate it has changed so the correct functions are run
        if ($("#formatTagContainer span." + ftToChange[i] + " input").is(":checked")) {
            $("#formatTagContainer span." + ftToChange[i] + " input").prop("checked", false);
            $("#formatTagContainer span." + ftToChange[i] + " input").change();
        } //if the format tag is currently checked, uncheck it and indicate it has changed so the correct functions are run
    } //for each format tag that needs to be changed

    //and update whether the genre is marked as fiction or nonfiction and call the correct function for that change
    $("#genreTagContainer span." + id + " input").prop("checked", true).trigger("click");
    $("#genreTagContainer span." + id + " input").change();
} //changeFictionOrNonfiction