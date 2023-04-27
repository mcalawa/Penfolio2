/**
 * Scripts for populating the genre tags for the genre tag selection in the Create view
 * of the Writing controller.
 */

/**
 * This function is for dealing with the top level genres, fiction and
 * nonfiction. It loads the children of whichever one is selected and then
 * unchecks and unloads any tags that require the writing to be of the opposite
 * top level genre
 * @param {string} name (name of the tag that has just been selected, either Fiction or Nonfiction)
 * @param {int[]} fictionOrNonfictionOnly (GenreIds of genres that can only be the opposite of the tag that was just selected)
 * @param {int[]} children (GenreIds of the children of the one that was just selected)
 * @param {int[]} parentFormats (FormatIds that aren't valid based on the item that was just selected)
 */
function fictionOrNonfiction(name, fictionOrNonfictionOnly, children, parentFormats) {
    //for each of this genre's children
    for (var i = 0; i < children.length; i++) {
        //if the child is collapsed, uncollapse it
        if ($("#genreTagContainer span." + children[i]).hasClass("collapse")) {
            $("#genreTagContainer span." + children[i]).removeClass("collapse");
        }
    }

    //for each genre tag that can only be the opposite of the current tag
    for (i = 0; i < fictionOrNonfictionOnly.length; i++) {
        //because collapsed tags are always unchecked, 
        //we only need to do anything to this tag if it isn't collapsed
        //if it isn't collapsed
        if ($("#genreTagContainer span." + fictionOrNonfictionOnly[i]).hasClass("collapse") !== true) {
            //if it is currently checked, uncheck it and trigger onchange so that the functions run
            if ($("#genreTagContainer span." + fictionOrNonfictionOnly[i] + " input").is(":checked")) {
                $("#genreTagContainer span." + fictionOrNonfictionOnly[i] + " input").prop("checked", false);
                $("#genreTagContainer span." + fictionOrNonfictionOnly[i] + " input").change();
            }

            //collapse it
            $("#genreTagContainer span." + fictionOrNonfictionOnly[i]).addClass("collapse");
        }
    }

    //for each format in parentFormats
    for (i = 0; i < parentFormats.length; i++) {
        //if the current format tag is checked, uncheck it and then trigger onchange so the functions will run
        if ($("#formatTagContainer span." + parentFormats[i] + " input").is(":checked")) {
            $("#formatTagContainer span." + parentFormats[i] + " input").prop("checked", false);
            $("#formatTagContainer span." + parentFormats[i] + " input").change();
        }
    } //for each format in parentFormats
}

/**
 * 
 * @param {int} id (GenreId of item that was just checked or unchecked)
 * @param {int[]} ndChildren (array of children without any dependencies)
 * @param {int[]} sdChildren (array of children with a single dependency)
 * @param {int[]} sDependencies (array of dependencies for sdChildren)
 * @param {int[]} mdChildren (array of children with multiple dependencies)
 * @param {int[]} mdfDependencies (array of the first dependencies for mdChildren)
 * @param {int[]} mdsDependencies (array of the second dependencies for mdChildren)
 * @param {int[]} mpndChildren (array of children with alternate parents and no dependencies)
 * @param {int[]} mpndParents (array of alternate parents for mpndChildren)
 * @param {int[]} mpsdChildren (array of children with alternate parents and a single dependency)
 * @param {int[]} mpsdParents (array of alternate parents for mpsdChildren)
 * @param {int[]} mpsDependencies (array of dependencies for mpsdChildren)
 * @param {boolean} isFictionOnly (bool that is true if it's fiction only)
 * @param {boolean} isNonfictionOnly (bool that is true if it's nonfiction only)
 * @param {int[]} fictionOnly (array of genres that are fiction only)
 * @param {int[]} nonfictionOnly (array of genres that are nonfiction only)
 * @param {int[]} parentFormats (array of formats that are parents of the current tag)
 */
function gtChildren(id, ndChildren, sdChildren, sDependencies, mdChildren, mdfDependencies, mdsDependencies, mpndChildren, mpndParents, mpsdChildren, mpsdParents, mpsDependencies, isFictionOnly, isNonfictionOnly, fictionOnly, nonfictionOnly, parentFormats) {
    //if the checkbox for the genre tag with a GenreId of id was checked
    if ($("#genreTagContainer span." + id + " input").is(":checked")) {
        //if this is a fiction only genre tag
        if (isFictionOnly === "true") {
            //if fiction isn't already selected, select it and call the onchange function for fiction
            if ($("#genreTagContainer span.1 input").is(":checked") === false) {
                $("#genreTagContainer span.1 input").prop("checked", true).trigger("click");
                $("#genreTagContainer span.1 input").change();
            }
        } //if this is a nonfiction only genre tag
        else if (isNonfictionOnly === "true") {
            //if nonfiction isn't already selected, select it and call the onchange function for nonfiction
            if ($("#genreTagContainer span.2 input").is(":checked") === false) {
                $("#genreTagContainer span.2 input").prop("checked", true).trigger("click");
                $("#genreTagContainer span.2 input").change();
            }
        }

        //if the current genre tag has required parent formats, you need to check to make sure that one of them is checked before loading children
        if (parentFormats.length > 0) {
            var parentChecked = false;
            //for each parent format
            for (var i = 0; i < parentFormats.length; i++) {
                //if the current parentFormat is checked, mark it as true and exit so that it doesn't continue to run
                if ($("#formatTagContainer span." + parentFormats[i] + " input[type=checkbox]").is(":checked")) {
                    parentChecked = true;
                    break;
                }
            }

            //if parentChecked is still false, check the first parent format before loading the child tags
            if (parentChecked === false) {
                $("#formatTagContainer span." + parentFormats[0] + " input").prop("checked", true);
                $("#formatTagContainer span." + parentFormats[0] + " input").change();
            }
        } //if the current genre tag has required parent formats

        //load all no dependency children and load any single dependency children with their other dependency checked and 
        //any multi-dependency children with their other two dependencies checked
        loadGenres(ndChildren, sdChildren, sDependencies, mdChildren, mdfDependencies, mdsDependencies);
    } //if we are loading children
    else { //if we are unloading children, we only care about dependencies of mpsdChildren
        //if this genre tag is fiction only 
        if (isFictionOnly === "true") {
            //for each tag that is nonfiction only
            for (i = 0; i < nonfictionOnly.length; i++) {
                //check to make sure it isn't one of the parents in mpndParents
                for (j = 0; j < mpndParents.length; j++) {
                    //if there's a match for the parent or connected child, remove them
                    if (nonfictionOnly[i] === mpndParents[j] || nonfictionOnly[i] === mpndChildren[j]) {
                        mpndParents.splice(j, 1);

                        //for each child in ndChildren
                        for (var k = 0; k < ndChildren.length; k++) {
                            //if it is already in ndChildren, remove it from ndChildren before adding it
                            if (mpndChildren[j] === ndChildren[k]) {
                                ndChildren.splice(k, 1);
                            } //if it is already in ndChildren, remove it from ndChildren before adding it
                        } //for each child in ndChildren

                        //add mpndChildren to ndChildren and remove it from mpndChildren
                        ndChildren.push(mpndChildren[j]);
                        mpndChildren.splice(j, 1);
                    } //if there's a match for the parent or connected child, remove them
                } //for each parent in mpndParents

                //check to make sure it isn't one of the parents in mpsdParents
                for (j = 0; j < mpsdParents.length; j++) {
                    //if there is a match between either the parent, the child, or the matching dependency, remove them
                    if (nonfictionOnly[i] === mpsdParents[j] || nonfictionOnly[i] === mpsdChildren[j] || nonfictionOnly[i] === mpsDependencies[j]) {
                        mpsdParents.splice(j, 1);
                        mpsDependencies.splice(j, 1);

                        //for each child in sdChildren, check if there's a match to the current child in mpsdChildren
                        for (k = 0; k < sdChildren.length; k++) {
                            //if there's a match, remove it from sdChildren before adding it
                            if (mpsdChildren[j] === sdChildren[k]) {
                                sdChildren.splice(k, 1);
                            } //if there's a match, remove it from sdChildren before adding it
                        } //for each child in sdChildren, check if there's a match to the current child in mpsdChildren

                        //add the matching child to sdChildren and remove it from mpsdChildren
                        sdChildren.push(mpsdChildren[j]);
                        mpsdChildren.splice(j, 1);
                    } //if there is a match between either the parent, the child, or the matching dependency, remove them
                } //check to make sure it isn't one of the parents in mpsdParents
            } //for each genre tag that is nonfiction only
        } //if this genre tag is fiction only
        else if (isNonfictionOnly === "true") { //if this genre tag is nonfiction only
            //for each tag that is fiction only
            for (i = 0; i < fictionOnly.length; i++) {
                //check to make sure it isn't one of the parents in mpndParents
                for (j = 0; j < mpndParents.length; j++) {
                    //if there's a match for the parent or connected child, remove them
                    if (fictionOnly[i] === mpndParents[j] || fictionOnly[i] === mpndChildren[j]) {
                        mpndParents.splice(j, 1);

                        //for each child in ndChildren
                        for (k = 0; k < ndChildren.length; k++) {
                            //if it is already in ndChildren, remove it from ndChildren before adding it
                            if (mpndChildren[j] === ndChildren[k]) {
                                ndChildren.splice(k, 1);
                            } //if it is already in ndChildren, remove it from ndChildren before adding it
                        } //for each child in ndChildren

                        //add mpndChildren to ndChildren and remove it from mpndChildren
                        ndChildren.push(mpndChildren[j]);
                        mpndChildren.splice(j, 1);
                    } //if there's a match for the parent or connected child, remove them
                } //for each parent in mpndParents

                //check to make sure it isn't one of the parents in mpsdParents
                for (j = 0; j < mpsdParents.length; j++) {
                    //if there is a match between either the parent, the child, or the matching dependency, remove them
                    if (fictionOnly[i] === mpsdParents[j] || fictionOnly[i] === mpsdChildren[j] || fictionOnly[i] === mpsDependencies[j]) {
                        mpsdParents.splice(j, 1);
                        mpsDependencies.splice(j, 1);

                        //for each child in sdChildren, check if there's a match to the current child in mpsdChildren
                        for (k = 0; k < sdChildren.length; k++) {
                            //if there's a match, remove it from sdChildren before adding it
                            if (mpsdChildren[j] === sdChildren[k]) {
                                sdChildren.splice(k, 1);
                            } //if there's a match, remove it from sdChildren before adding it
                        } //for each child in sdChildren, check if there's a match to the current child in mpsdChildren

                        //add the matching child to sdChildren and remove it from mpsdChildren
                        sdChildren.push(mpsdChildren[j]);
                        mpsdChildren.splice(j, 1);
                    } //if there is a match between either the parent, the child, or the matching dependency, remove them
                } //check to make sure it isn't one of the parents in mpsdParents
            } //for each genre tag that is fiction only
        } //if this genre tag is nonfiction only

        //for each no dependency child
        for (i = 0; i < ndChildren.length; i++) {
            //for each no dependency child where the child has more than one possible parent
            for (var j = 0; j < mpndChildren.length; j++) {
                //if the no dependency child and the child with more than one possible parent are the same
                if (ndChildren[i] === mpndChildren[j]) {
                    //remove the no dependency child from the array
                    ndChildren.splice(i, 1);
                } //if the no dependency child and the child with more than one possible parent are the same
            } //for each no dependency child where the child has more than one possible parent
        } //for each no dependency child

        //for each single dependency child
        for (i = 0; i < sdChildren.length; i++) {
            //for each single dependency child where the child has more than one possible parent
            for (j = 0; j < mpsdChildren.length; j++) {
                //if the single dependency child and the child with more than one possible parent are the same
                if (sdChildren[i] === mpsdChildren[j]) {
                    //remove the single dependency child from the array
                    sdChildren.splice(i, 1);
                } //if the single dependency child and the child with more than one possible parent are the same
            } //for each single dependency child where the child has more than one possible parent
        } //for each single dependency child

        //add all the single dependency children to the same array as the no dependency children
        for (i = 0; i < sdChildren.length; i++)
            ndChildren.push(sdChildren[i]);

        //add all the multi-dependency children to the same array as the no dependency children
        for (i = 0; i < mdChildren.length; i++)
            ndChildren.push(mdChildren[i]);

        //call the function to unload all of the no dependency children and single dependency
        //and multi-dependency children with only one possible combination of parents; remove
        //the children with multiple possible parents or combination of parents only if none of
        //the parents or combination of parents are selected
        if (isFictionOnly === "true")
            unloadGenres(ndChildren, mpndChildren, mpndParents, mpsdChildren, mpsdParents, mpsDependencies, nonfictionOnly);
        else if (isNonfictionOnly === "true")
            unloadGenres(ndChildren, mpndChildren, mpndParents, mpsdChildren, mpsdParents, mpsDependencies, fictionOnly);
        else
            unloadGenres(ndChildren, mpndChildren, mpndParents, mpsdChildren, mpsdParents, mpsDependencies, []);
    } //if we are unloading children
} //gtChildren

function loadGenres(ndChildren, sdChildren, sDependencies, mdChildren, mdfDependencies, mdsDependencies) {
    //for each child without any dependencies
    for (var i = 0; i < ndChildren.length; i++) {
        //if the child is currently collapsed, uncollapse it
        if ($("#genreTagContainer span." + ndChildren[i]).hasClass("collapse"))
            $("#genreTagContainer span." + ndChildren[i]).removeClass("collapse");
    } //for each child without any dependencies

    //for each child with only one dependency
    for (i = 0; i < sdChildren.length; i++) {
        //if the dependency is one that involves fiction vs nonfiction and neither is checked or if the dependency for this child is checked
        if ((sDependencies[i] === 1 && $("#genreTagContainer span.2 input").is(":checked") === false) || (sDependencies[i] === 2 && $("#genreTagContainer span.1 input").is(":checked") === false) || $("#genreTagContainer span." + sDependencies[i] + " input").is(":checked")) {
            //if the child is currently collapsed, uncollapse it
            if ($("#genreTagContainer span." + sdChildren[i]).hasClass("collapse"))
                $("#genreTagContainer span." + sdChildren[i]).removeClass("collapse");
        } //if the dependency is one that involves fiction vs nonfiction and neither is checked or if the dependency for this child is checked
    } //for each child with only one dependency

    //for each child with two dependencies
    for (i = 0; i < mdChildren.length; i++) {
        //if the first dependency for this child is checked 
        if ($("#genreTagContainer span." + mdfDependencies[i] + " input").is(":checked")) {
            //if the second dependency is one that involves fiction vs nonfiction and neither is checked or if the dependency for this child is checked
            if ((mdsDependencies[i] === 1 && $("#genreTagContainer span.2 input").is(":checked") === false) || (mdsDependencies[i] === 2 && $("#genreTagContainer span.1 input").is(":checked") === false) || $("#genreTagContainer span." + mdsDependencies[i] + " input").is(":checked")) {
                //if the child is currently collapsed, uncollapse it
                if ($("#genreTagContainer span." + mdChildren[i]).hasClass("collapse"))
                    $("#genreTagContainer span." + mdChildren[i]).removeClass("collapse");
            } //if the second dependency is one that involves fiction vs nonfiction and neither is checked or if the dependency for this child is checked
        } //if the first dependency for this child is checked 
    } //for each child with two dependencies
} //loadGenres

function unloadGenres(rChildren, mpndChildren, mpndParents, mpsdChildren, mpsdParents, mpsDependencies, fictionOrNonfictionOnly) {
    //for all of the children that we KNOW can be depopulated
    for (var i = 0; i < rChildren.length; i++) {
        //if the checkbox for this child is checked, we need to make sure it isn't under fiction only when 
        //nonfiction is currently selected and vice versa; the reason we check this is because we only uncheck
        //and collapse children that are no longer valid
        if ($("#genreTagContainer span." + rChildren[i] + " input").is(":checked")) {
            //for each item in fictionOrNonfictionOnly
            for (var j = 0; j < fictionOrNonfictionOnly.length; j++) {
                //if there's a match, uncheck and collapse it
                if (rChildren[i] === fictionOrNonfictionOnly[j]) {
                    $("#genreTagContainer span." + rChildren[i] + " input").prop("checked", false);

                    //if it isn't already collapsed, collapse it
                    if ($("#genreTagContainer span." + rChildren[i]).hasClass("collapse") !== true)
                        $("#genreTagContainer span." + rChildren[i]).removeClass("collapse");

                    $("#genreTagContainer span." + rChildren[i] + " input").change();
                } //if there's a match, uncheck and collapse it
            } //for each item in fictionOrNonfictionOnly
        } //if the checkbox for this child is checked
        else {//if the checkbox for this child is not checked
            //if it isn't already collapsed, collapse it
            if ($("#genreTagContainer span." + rChildren[i]).hasClass("collapse") !== true)
                $("#genreTagContainer span." + rChildren[i]).addClass("collapse");
        } //if the checkbox for this child is not checked
    } //for all of the children that we KNOW can be depopulated

    //next, it's necessary to check the tags that have alternate parents to see if those parents are 
    //checked or they can be depopulated

    //set the value of the variable current to the first item in the list of children with a single dependency we might unload
    var current = mpndChildren[0];
    //set a variable to keep track of whether or not a parent is checked to false
    var parentChecked = false;
    //for each child in mpndChildren
    for (i = 0; i < mpndChildren.length; i++) {
        //because we already removed children that are invalid from mpndChildren, 
        //we don't have to check for that if the checkbox is checked

        //if this child has its checkbox checked
        if ($("#genreTagContainer span." + mpndChildren[i] + " input").is(":checked") !== true) {
            //since some children appear in mpndChildren more than once due to having more than 
            //one possible alternate parent, we need to check if the for loop will continue to 
            //run after this and, if so, if the value of mpndChildren will be the same at the next
            //index; the reason for this is because we don't want to depopulate a child tag when
            //parentChecked is false unless it is still false after checking the last potential parent

            //if the next item in mpndChildren is the same as the current item
            if (i + 1 < mpndChildren.length && mpndChildren[i + 1] === current) {
                //if parentChecked is not already true and the current potential parent needs to be checked
                if (parentChecked !== true) {
                    //if this potential parent is checked, mark parentChecked true so that a check won't be made
                    //next iteration that might result in a false negative
                    if ($("#genreTagContainer span." + mpndParents[i] + " input").is(":checked"))
                        parentChecked = true;
                } //if parentChecked is not already true and the current potential parent needs to be checked
            } //if the next item in mpndChildren is the same as the current item
            else if (mpndChildren[i] === current) { //if mpndCurrent is the same as last time but will not be next time
                //basically this means this is the last chance to depopulate this child

                //if one of the possible parents for this child wasn't already checked on a previous iteration
                if (parentChecked !== true) {
                    //if the possible parent this time around isn't checked either
                    if ($("#genreTagContainer span." + mpndParents[i] + " input").is(":checked") !== true) {
                        //if the child isn't already collapsed, collapse it
                        if ($("#genreTagContainer span." + mpndChildren[i]).hasClass("collapse") !== true)
                            $("#genreTagContainer span." + mpndChildren[i]).addClass("collapse");
                    }
                }
            } //if mpndChildren is the same as last time but will not be next time
            else { //if this child isn't the same child as was being checked last iteration
                //reset the values of our variable to their default
                parentChecked = false;
                current = mpndChildren[i];

                //if this child has its matching parent checked, mark parentChecked as true
                if ($("#genreTagContainer span." + mpndParents[i] + " input").is(":checked")) {
                    parentChecked = true;
                } //if this potential parent isn't checked and mpndChildren will not be the same child next iteration
                else if (i + 1 === mpndChildren.length || mpndChildren[i + 1] !== current) {
                    //collapse this child if it isn't already collapsed
                    if ($("#genreTagContainer span." + mpndChildren[i]).hasClass("collapse") !== true)
                        $("#genreTagContainer span." + mpndChildren[i]).addClass("collapse");
                } //if this potential parent isn't checked and mpndChildren will not be the same child next iteration
            } //if this child isn't the same child as was being checked last iteration
        } //if this child doesn't have its checkbox checked
    } //for each child in mpndChildren

    //now we need to check the children with a single dependency and alternate possible parents, so reset the variables
    current = mpsdChildren[0];
    parentChecked = false;
    //for each child in mpsdChildren
    for (i = 0; i < mpsdChildren.length; i++) {
        //because we already removed children that are invalid from mpsdChildren, 
        //we don't have to check for that if the checkbox is checked

        //if this child doesn't have its checkbox checked
        if ($("#genreTagContainer span." + mpsdChildren[i] + " input").is(":checked") !== true) {
            //since some children appear in mpsdChildren more than once due to having more than 
            //one possible alternate parent, we need to check if the for loop will continue to 
            //run after this and, if so, if the value of mpsdChildren will be the same at the next
            //index; the reason for this is because we don't want to depopulate a child tag when
            //parentChecked is false unless it is still false after checking the last potential parent

            //if the next item in mpsdChildren is the same as the current item
            if (i + 1 < mpsdChildren.length && mpsdChildren[i + 1] === current) {
                //if parentChecked is not already true and the current potential parent needs to be checked
                if (parentChecked !== true) {
                    //if this potential parent is checked, mark parentChecked true so that a check won't be made
                    //next iteration that might result in a false negative
                    if ($("#genreTagContainer span." + mpsdParents[i] + " input").is(":checked") && $("#genreTagContainer span." + mpsDependencies[i] + " input").is(":checked"))
                        parentChecked = true;
                } //if parentChecked is not already true and the current potential parent needs to be checked
            } //if the next item in mpsdChildren is the same as the current item
            else if (mpsdChildren[i] === current) { //if mpsdCurrent is the same as last time but will not be next time
                //basically this means this is the last chance to depopulate this child

                //if one of the possible parents for this child wasn't already checked on a previous iteration
                if (parentChecked !== true) {
                    //if the possible parent this time around isn't checked either
                    if ($("#genreTagContainer span." + mpsdParents[i] + " input").is(":checked") !== true ||
                        $("#genreTagContainer span." + mpsDependencies[i] + " input").is(":checked") !== true) {
                        //if the child isn't already collapsed, collapse it
                        if ($("#genreTagContainer span." + mpndChildren[i]).hasClass("collapse") !== true)
                            $("#genreTagContainer span." + mpndChildren[i]).addClass("collapse");
                    } //if the possible parent this time around isn't checked either
                } //if one of the possible parents for this child wasn't already checked on a previous iteration
            } //if mpsdChildren is the same as last time but will not be next time
            else { //if this child isn't the same child as was being checked last iteration
                //reset the values of our variable to their default
                parentChecked = false;
                current = mpsdChildren[i];

                //if this child has its matching parent checked, mark parentChecked as true
                if ($("#genreTagContainer span." + mpsdParents[i] + " input").is(":checked") && $("#genreTagContainer span." + mpsDependencies[i] + " input").is(":checked")) {
                    parentChecked = true;
                } //if this potential parent isn't checked and mpsdChildren will not be the same child next iteration
                else if (i + 1 === mpsdChildren.length || mpsdChildren[i + 1] !== current) {
                    //collapse this child if it isn't already collapsed
                    if ($("#genreTagContainer span." + mpsdChildren[i]).hasClass("collapse") !== true)
                        $("#genreTagContainer span." + mpsdChildren[i]).addClass("collapse");
                } //if this potential parent isn't checked and mpsdChildren will not be the same child next iteration
            } //if this child isn't the same child as was being checked last iteration
        } //if this child doesn't have its checkbox checked
    } //for each child in mpsdChildren
} //unloadChildren