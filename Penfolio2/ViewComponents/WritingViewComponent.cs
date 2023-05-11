using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;
using Penfolio2.Models;
using System.Text;

namespace Penfolio2.ViewComponents
{
    public class WritingViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public WritingViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id = null, string? viewName = null)
        {
            Writing writing;
            List<WritingProfile> owners = new List<WritingProfile>();
            string? userId = GetUserId();

            if (userId == null)
            {
                return View();
            }

            PenUser user = _db.PenUsers.Where(i => i.Id == userId).FirstOrDefault();

            if(user == null)
            {
                return View();
            }

            if (!string.IsNullOrEmpty(viewName))
            {
                if(viewName.CompareTo("SelectAuthors") == 0)
                {
                    List<int> selectedProfileIds = new List<int>();
                    var writingProfiles = GetWriterProfiles(userId);
                    ViewBag.IsCreator = true;

                    if (id != null)
                    {
                        writing = _db.Writings.Where(i => i.WritingId == id.Value).FirstOrDefault();

                        if(writing != null && writing.UserId != userId)
                        {
                            ViewBag.IsCreator = false;
                        }

                        var wp = _db.WritingProfiles.Where(i => i.WritingId == id.Value).ToList();

                        foreach (var profile in wp)
                        {
                            if (!writingProfiles.Any(i => i.ProfileId == profile.ProfileId))
                            {
                                if (profile.PenProfile == null)
                                {
                                    profile.PenProfile = _db.PenProfiles.Where(i => i.ProfileId == profile.ProfileId).FirstOrDefault();
                                }

                                if (profile.PenProfile != null)
                                {
                                    profile.PenProfile = PopulatePenProfile(profile.PenProfile);
                                    writingProfiles.Add(profile.PenProfile);
                                }
                            }

                            selectedProfileIds.Add(profile.ProfileId);
                        }
                    }

                    ViewBag.User = userId;

                    var model = new WritingViewModel
                    {
                        WritingProfiles = writingProfiles,
                        SelectedProfileIds = selectedProfileIds
                    };

                    return View(viewName, model);
                } //SelectAuthor
                else if(viewName.CompareTo("SelectFormat")  == 0)
                {
                    var formatTags = _db.FormatTags.ToList();
                    var formatCategories = _db.FormatCategories.ToList();
                    List<int> selectedFormatIds = new List<int>();

                    if(id != null)
                    {
                        writing = _db.Writings.Where(i => i.WritingId == id.Value).FirstOrDefault();

                        if(writing != null)
                        {
                            if(writing.WritingFormats.Count == 0)
                            {
                                writing.WritingFormats = _db.WritingFormats.Where(i => i.WritingId == id.Value).ToList();
                            }

                            foreach(var format in writing.WritingFormats)
                            {
                                selectedFormatIds.Add(format.FormatId);
                            }
                        }
                    }

                    var model = new WritingViewModel
                    {
                        FormatTags = formatTags,
                        FormatCategories = formatCategories,
                        SelectedFormatIds = selectedFormatIds
                    };

                    return View(viewName, model);
                } //SelectFormat
                else if(viewName.CompareTo("SelectGenre") == 0)
                {
                    var genreTags = _db.GenreTags.ToList();
                    var genreCategories = _db.GenreCategories.ToList();
                    var genreFormats = _db.GenreFormats.ToList();
                    var formatTags = _db.FormatTags.ToList();
                    List<int> selectedGenreIds = new List<int>();
                    List<int> selectedFormatIds = new List<int>();

                    if (id != null)
                    {
                        writing = _db.Writings.Where(i => i.WritingId == id.Value).FirstOrDefault();

                        if (writing != null)
                        {
                            if (writing.WritingGenres.Count == 0)
                            {
                                writing.WritingGenres = _db.WritingGenres.Where(i => i.WritingId == id.Value).ToList();
                            }

                            foreach (var genre in writing.WritingGenres)
                            {
                                selectedGenreIds.Add(genre.GenreId);
                            }

                            if(writing.WritingFormats.Count == 0)
                            {
                                writing.WritingFormats = _db.WritingFormats.Where(i => i.WritingId == id.Value).ToList();
                            }

                            foreach(var format in writing.WritingFormats)
                            {
                                selectedFormatIds.Add(format.FormatId);
                            }
                        }
                    }

                    var model = new WritingViewModel
                    {
                        GenreTags = genreTags,
                        GenreCategories = genreCategories,
                        GenreFormats = genreFormats,
                        FormatTags = formatTags,
                        SelectedGenreIds = selectedGenreIds,
                        SelectedFormatIds = selectedFormatIds
                    };

                    return View(viewName, model);
                }  //SelectGenre
                else if(viewName.CompareTo("SelectPermissions") == 0)
                {
                    bool publicAccess = false;
                    bool friendAccess = false;
                    bool publisherAccess = false;
                    bool minorAccess = false;
                    bool showsUpInSearch = false;

                    if(id != null)
                    {
                        writing = _db.Writings.Where(i => i.WritingId == id.Value).FirstOrDefault();
                        AccessPermission accessPermission = null;

                        if(writing != null)
                        {
                            accessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == writing.AccessPermissionId).FirstOrDefault();
                        }
                        
                        if(accessPermission != null)
                        {
                            publicAccess = accessPermission.PublicAccess;
                            friendAccess = accessPermission.FriendAccess;
                            publisherAccess = accessPermission.PublisherAccess;
                            minorAccess = accessPermission.MinorAccess;
                            showsUpInSearch = accessPermission.ShowsUpInSearch;
                        }
                    }

                    var model = new WritingViewModel
                    {
                        PublicAccess = publicAccess,
                        FriendAccess = friendAccess,
                        PublisherAccess = publisherAccess,
                        MinorAccess = minorAccess,
                        ShowsUpInSearch = showsUpInSearch
                    };

                    return View(viewName, model);
                } //SelectPermissions
                else if(viewName.CompareTo("Author") == 0)
                {
                    if(id == null)
                    {
                        return View();
                    }

                    writing = _db.Writings.Where(i => i.WritingId == id.Value).FirstOrDefault();

                    if(writing == null)
                    {
                        return View();
                    }

                    List<WritingProfile> writingProfiles = _db.WritingProfiles.Where(i => i.WritingId == id.Value).ToList();
                    List<PenProfile> penProfiles = new List<PenProfile>();
                    List<AuthorViewModel> model = new List<AuthorViewModel>();

                    //add all of the writingProfiles to penProfiles
                    foreach (var profile in writingProfiles)
                    {
                        if(profile.PenProfile == null)
                        {
                            profile.PenProfile = _db.PenProfiles.Where(i => i.ProfileId == profile.ProfileId).FirstOrDefault();
                        }

                        if(profile.PenProfile != null)
                        {
                            penProfiles.Add(PopulatePenProfile(profile.PenProfile));
                        }
                    }

                    //add all of the penProfiles to authors 
                    foreach(var profile in penProfiles)
                    {
                        List<IdentityError> errors = new List<IdentityError>();
                        bool isAnonymous = true;

                        if(IsAccessableByUser(profile.AccessPermissionId, ref errors, "search"))
                        {
                            isAnonymous = false;
                        }

                        var author = new AuthorViewModel
                        {
                            IsAnonymous = isAnonymous,
                            DisplayName = isAnonymous ? "Anonymous" : profile.DisplayName,
                            UrlString = isAnonymous ? null : profile.UrlString
                        };

                        model.Add(author);
                    }

                    return View(viewName, model);
                } //Author
                else if(viewName.CompareTo("Delete") == 0)
                {
                    if (id == null)
                    {
                        return View();
                    }

                    writing = _db.Writings.Where(i => i.WritingId == id.Value).FirstOrDefault();

                    if (writing == null)
                    {
                        return View();
                    }

                    ViewBag.IsCreator = false;
                    ViewBag.IsCollaborator = false;
                    ViewBag.Delete = true;

                    if (userId == writing.UserId)
                    {
                        ViewBag.IsCreator = true;
                    }

                    owners = _db.WritingProfiles.Where(i => i.WritingId == id).ToList();

                    foreach (var owner in owners)
                    {
                        if (owner.PenProfile == null)
                        {
                            owner.PenProfile = _db.PenProfiles.Where(i => i.ProfileId == owner.ProfileId).First();
                        }

                        if (owner.PenProfile.UserId == userId)
                        {
                            ViewBag.IsCollaborator = true;
                        }
                    }

                    if (writing.WritingFormats == null || writing.WritingFormats.Count == 0)
                    {
                        writing.WritingFormats = _db.WritingFormats.Where(i => i.WritingId == writing.WritingId).ToList();

                        foreach (var format in writing.WritingFormats)
                        {
                            if (format.FormatTag == null)
                            {
                                format.FormatTag = _db.FormatTags.Where(i => i.FormatId == format.FormatId).FirstOrDefault();

                                if (format.FormatTag.AltFormatNames == null || format.FormatTag.AltFormatNames.Count == 0)
                                {
                                    format.FormatTag.AltFormatNames = _db.AltFormatNames.Where(i => i.FormatId == format.FormatId).ToList();
                                }
                            }
                        }
                    }

                    if (writing.WritingGenres == null || writing.WritingGenres.Count == 0)
                    {
                        writing.WritingGenres = _db.WritingGenres.Where(i => i.WritingId == writing.WritingId).ToList();

                        foreach (var genre in writing.WritingGenres)
                        {
                            if (genre.GenreTag == null)
                            {
                                genre.GenreTag = _db.GenreTags.Where(i => i.GenreId == genre.GenreId).FirstOrDefault();

                                if (genre.GenreTag.AltGenreNames == null || genre.GenreTag.AltGenreNames.Count == 0)
                                {
                                    genre.GenreTag.AltGenreNames = _db.AltGenreNames.Where(i => i.GenreId == genre.GenreId).ToList();
                                }
                            }
                        }
                    }

                    string document = HTMLByteArrayToString(writing.Document);

                    ViewBag.Document = document;

                    return View(viewName, writing);
                } //Delete
            }

            if(id == null)
            {
                return View();
            }

            writing = _db.Writings.Where(i => i.WritingId == id.Value).FirstOrDefault();

            if (writing == null)
            {
                return View();
            }

            ViewBag.IsCreator = false;
            ViewBag.IsCollaborator = false;
            ViewBag.Delete = false;

            if (userId == writing.UserId)
            {
                ViewBag.IsCreator = true;
            }

            owners = _db.WritingProfiles.Where(i => i.WritingId == id).ToList();

            foreach (var owner in owners)
            {
                if (owner.PenProfile == null)
                {
                    owner.PenProfile = _db.PenProfiles.Where(i => i.ProfileId == owner.ProfileId).First();
                }

                if (owner.PenProfile.UserId == userId)
                {
                    ViewBag.IsCollaborator = true;
                }
            }

            if (writing.WritingFormats == null || writing.WritingFormats.Count == 0)
            {
                writing.WritingFormats = _db.WritingFormats.Where(i => i.WritingId == writing.WritingId).ToList();

                foreach (var format in writing.WritingFormats)
                {
                    if (format.FormatTag == null)
                    {
                        format.FormatTag = _db.FormatTags.Where(i => i.FormatId == format.FormatId).FirstOrDefault();

                        if (format.FormatTag.AltFormatNames == null || format.FormatTag.AltFormatNames.Count == 0)
                        {
                            format.FormatTag.AltFormatNames = _db.AltFormatNames.Where(i => i.FormatId == format.FormatId).ToList();
                        }
                    }
                }
            }

            if (writing.WritingGenres == null || writing.WritingGenres.Count == 0)
            {
                writing.WritingGenres = _db.WritingGenres.Where(i => i.WritingId == writing.WritingId).ToList();

                foreach (var genre in writing.WritingGenres)
                {
                    if (genre.GenreTag == null)
                    {
                        genre.GenreTag = _db.GenreTags.Where(i => i.GenreId == genre.GenreId).FirstOrDefault();

                        if (genre.GenreTag.AltGenreNames == null || genre.GenreTag.AltGenreNames.Count == 0)
                        {
                            genre.GenreTag.AltGenreNames = _db.AltGenreNames.Where(i => i.GenreId == genre.GenreId).ToList();
                        }
                    }
                }
            }

            ViewBag.Document = "";

            return View(writing);
        }

        protected bool UserIsMinor(PenUser user)
        {
            DateTime now = DateTime.Today;

            //if they have entered a birthday
            if (user.Birthdate != null)
            {
                //get their birthday
                DateTime birthdate = user.Birthdate.Value;

                //if they are 18 or older, they are not a minor
                if (now.Year - birthdate.Year > 18 || (now.Year - birthdate.Year == 18 && (now.Month > birthdate.Month || (now.Month == birthdate.Month && now.Day >= birthdate.Day))))
                {
                    return false;
                }
            }

            return true;
        }

        protected bool UserHasEnteredBirthdate(PenUser user)
        {
            if (user.Birthdate == null)
            {
                return false;
            }

            return true;
        }

        protected ICollection<PenProfile> GetVerifiedPublisherProfiles(string userId)
        {
            List<PenProfile> penProfiles = _db.PenProfiles.Where(i => i.UserId == userId && i.RoleId == 2 && i.Verified).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected PenProfile PopulatePenProfile(PenProfile penProfile)
        {
            //if PenUser isn't populated, populate it
            if (penProfile.PenUser == null)
            {
                penProfile.PenUser = _db.PenUsers.Where(i => i.Id == penProfile.UserId).FirstOrDefault();
            }

            //if AccessPermission isn't populated, populate it
            if (penProfile.AccessPermission == null)
            {
                penProfile.AccessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == penProfile.AccessPermissionId).FirstOrDefault();
            }

            //if PenRole isn't populated, populate it
            if (penProfile.PenRole == null)
            {
                penProfile.PenRole = _db.ProfileRoles.Where(i => i.RoleId == penProfile.RoleId).FirstOrDefault();
            }

            return penProfile;
        }

        public string? GetUserName()
        {
            return User?.Identity?.Name;
        }

        protected string? GetUserId()
        {
            var username = GetUserName();

            if (username == null)
            {
                return null;
            }

            return _db.PenUsers.Where(i => i.NormalizedUserName == username.ToUpper()).FirstOrDefault()?.Id;
        }

        protected bool UserHasPublisherProfile()
        {
            return _db.PenProfiles.Any(i => i.UserId == GetUserId() && i.RoleId == 2);
        }

        protected bool UserHasVerifiedPublisherProfile()
        {
            return _db.PenProfiles.Any(i => i.UserId == GetUserId() && i.RoleId == 2 && i.Verified);
        }

        protected ICollection<PenProfile> GetWriterProfiles(string userId)
        {
            List<PenProfile> penProfiles = _db.PenProfiles.Where(i => i.UserId == userId && i.RoleId == 1).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected bool IsAccessableByUser(int accessPermissionId, ref List<IdentityError> errors)
        {
            return IsAccessableByUser(accessPermissionId, ref errors, null);
        }

        protected bool IsAccessableByUser(int accessPermissionId, ref List<IdentityError> errors, string? action)
        {
            if (action == null)
            {
                action = "";
            }

            action = action.ToLower().Trim();

            //if the error list doesn't exist, create it
            if (errors == null)
            {
                errors = new List<IdentityError>();
            }

            string? userId = GetUserId();

            if (userId == null)
            {
                errors.Add(new IdentityError
                {
                    Description = "User not found."
                });

                return false;
            }

            PenUser user = _db.PenUsers.Where(i => i.Id == userId).First();

            if(user.PenProfiles.Count == 0)
            {
                user.PenProfiles = _db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            //get a list of all of this user's profiles
            List<int> userProfileIds = user.PenProfiles.ToList().Select(i => i.ProfileId).ToList();
            //get the access permission from the database by id
            AccessPermission accessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

            //if the access permission doesn't exist, return false
            if (accessPermission == null)
            {
                errors.Add(new IdentityError
                {
                    Description = "Request not found."
                });

                return false;
            }

            //a string to mark whether this access permission is for a profile, a folder, a series, or a piece of writing
            string accessType = accessPermission.ProfileId != null ? "profile" : accessPermission.FolderId != null ? "folder" : accessPermission.SeriesId != null ? "series" : accessPermission.WritingId != null ? "writing" : "";

            //if this AccessPermission was never assigned an id it is relating to, return false
            if (accessPermission.ProfileId is null && accessPermission.WritingId is null && accessPermission.FolderId is null && accessPermission.SeriesId is null)
            {
                errors.Add(new IdentityError
                {
                    Description = "Request not found."
                });

                return false;
            }

            //a user can always access their own content
            //if the AccessPermission is for a PenProfile owned by the user, return true
            if (_db.PenProfiles.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == userId))
            {
                return true;
            } //if it's a profile, they aren't allowed to edit it unless it's their own
            else if (accessType == "profile" && (action == "edit" || action == "delete"))
            {
                errors.Add(new IdentityError
                {
                    Description = "You are not allowed to " + action + " a profile you are not the owner of."
                });

                return false;
            } //if the AccessPermission is for a piece of writing
            else if (_db.Writings.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the writing
                Writing writing = _db.Writings.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if there's no writing with that AccessPermissionId, return false
                if (writing == null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Writing not found."
                    });

                    return false;
                }

                //if the Writing was created by the user, return true
                if (writing.UserId == userId)
                {
                    return true;
                } //if the writing was not created by the user and the action is delete
                else if (action == "delete")
                {
                    //writing may be edited by collaborators, but it can only be deleted by its owners
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a piece of writing that you do not own."
                    });

                    return false;
                } //if the writing was not created by the user
                else
                {
                    //get a list of all profiles that are connected to this piece of writing
                    List<WritingProfile> writingProfiles = _db.WritingProfiles.Where(i => i.WritingId == writing.WritingId).ToList();

                    foreach (var writingProfile in writingProfiles)
                    {
                        //if the user has a profile that is a collaborator on this piece of writing
                        if (userProfileIds.Contains(writingProfile.ProfileId))
                        {
                            return true;
                        }
                    }
                } //if the writing was not created by the user

                //if they have reached this point, they are not an owner or collaborator, so create an error message and return false
                if (action == "edit")
                {
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a piece of writing that you do not own or which you are not a collaborator on."
                    });

                    return false;
                }
            } //if the AccessPermission is for a Folder
            else if (_db.Folders.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the folder
                Folder folder = _db.Folders.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if there's no folder with that AccessPermissionId, return false
                if (folder == null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Folder not found."
                    });

                    return false;
                }

                //if the user created the folder, return true
                if (folder.CreatorId == userId)
                {
                    return true;
                } //if the folder was not created by the user and the action is delete
                else if (action == "delete")
                {
                    //folders may be edited by collaborators, but they can only be deleted by their owners
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a folder that you do not own."
                    });

                    return false;
                } //if the folder was not created by the user
                else
                {
                    //get a list of all the profiles connected to the folder
                    List<FolderOwner> folderOwners = _db.FolderOwners.Where(i => i.FolderId == folder.FolderId).ToList();

                    foreach (var folderOwner in folderOwners)
                    {
                        //if the user has a profile that is a contributor to this folder, return true
                        if (userProfileIds.Contains(folderOwner.OwnerId))
                        {
                            return true;
                        }
                    }
                } //if the folder was not created by the user

                //if they have reached this point, they are not an owner or a contributor, so create an error message and return false
                if (action == "edit")
                {
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a folder that you do not own or which you are not a contributor on."
                    });

                    return false;
                }
            } //if the AccessPermission matches to a Series
            else if (_db.Series.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the series
                Series series = _db.Series.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if there's no series with that AccessPermissionId, return false
                if (series == null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Series not found."
                    });

                    return false;
                }

                //if the user is the creator of this series, return true
                if (series.CreatorId == userId)
                {
                    return true;
                } //if the series was not created by the user and the action is delete
                else if (action == "delete")
                {
                    //series may be edited by collaborators, but they can only be deleted by their owners
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a series that you do not own."
                    });

                    return false;
                } //if the user is not the creator of the series
                else
                {
                    //get a list of profiles connected to the series
                    List<SeriesOwner> seriesOwners = _db.SeriesOwners.Where(i => i.SeriesId == series.SeriesId).ToList();

                    foreach (var seriesOwner in seriesOwners)
                    {
                        //if one of the profiles owned by the user is a contributor to this series, return true
                        if (userProfileIds.Contains(seriesOwner.OwnerId))
                        {
                            return true;
                        }
                    }
                } // if the user is not the creator of the series

                //if they have reached this point, they are not an owner or contributor, so create an error message and return false
                if (action == "edit")
                {
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a series that you do not own or which you are not a contributor on."
                    });

                    return false;
                }
            } //if the AccessPermission is for a Series

            //if the user is a minor and minor access has not been granted, we do not allow permission even in the case of an individual access grant having been given
            if (accessPermission.MinorAccess == false && UserIsMinor(user))
            {
                if (UserHasEnteredBirthdate(user))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "This " + accessType + " is not accessable by minors."
                    });
                }
                else
                {
                    errors.Add(new IdentityError
                    {
                        Description = "This " + accessType + " is not accessable by minors or those who have not entered their date of birth for their account. If you are not a minor, you may be able to access this " + accessType + " by editing your user account to include your birthday."
                    });
                }
                return false;
            }

            //get a list of user blocks for this particular user that are still active
            List<UserBlock> userBlocks = _db.UserBlocks.Where(i => i.BlockedUserId == userId && i.Active).ToList();

            //return false if the creator of the item connected to the access permission blocked the current user
            foreach (var block in userBlocks)
            {
                if (_db.PenProfiles.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == block.BlockingUserId)
                    || _db.Writings.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == block.BlockingUserId)
                    || _db.Folders.Any(i => i.AccessPermissionId == accessPermissionId && i.CreatorId == block.BlockingUserId)
                    || _db.Series.Any(i => i.AccessPermissionId == accessPermissionId && i.CreatorId == block.BlockingUserId))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Your account has been blocked from accessing this " + accessType + " by its owner."
                    });

                    return false;
                }
            }

            //past this point, we know that a user has not been turned away because of being a minor or because they were blocked,
            //so now we check to see if the action being performed is a search
            if (action == "search" && accessPermission.ShowsUpInSearch)
            {
                return true;
            }

            //get a list of individual access grants for this AccessPermissionId where the grant is still active
            List<IndividualAccessGrant> individualAccessGrants = _db.IndividualAccessGrants.Where(i => i.AccessPermissionId == accessPermissionId && i.Active).ToList();
            //get a list of individual access revokes for this AccessPermissionId where the revoke is still active
            List<IndividualAccessRevoke> individualAccessRevokes = _db.IndividualAccessRevokes.Where(i => i.AccessPermissionId == accessPermissionId && i.Active).ToList();
            //get a list of all of this user's profiles so we can remove any ids that don't belong to them from the list of individual access grants and revokes
            List<int> overlappingProfileIds = new List<int>();

            //remove anything from the list of individual access grants that doesn't apply to us
            foreach (var item in individualAccessGrants)
            {
                //if the GranteeId isn't one of the user's profiles
                if (!userProfileIds.Contains(item.GranteeId))
                {
                    //remove the unrelated grant
                    individualAccessGrants.Remove(item);
                } //if the GranteeId is one of the user's profiles and there's also a revoke with a matching RevokeeId in the list of individual access revokes, add the ProfileId to overlappingProfileIds
                else if (individualAccessRevokes.Any(i => i.RevokeeId == item.GranteeId))
                {
                    overlappingProfileIds.Add(item.GranteeId);
                }
            }

            //remove anything from the list of individual access revokes that doesn't apply to us
            foreach (var item in individualAccessRevokes)
            {
                //if the RevokeeId isn't one of the user's profiles
                if (!userProfileIds.Contains(item.RevokeeId))
                {
                    //remove the unrelated revoke
                    individualAccessRevokes.Remove(item);
                }
            }

            IndividualAccessGrant? mostRecentGrant;
            IndividualAccessRevoke? mostRecentRevoke;

            //if there are any overlapping access grants and revokes, we need to settle them
            foreach (var item in overlappingProfileIds)
            {
                //get all the individual access grants for this ProfileId where the grant is active
                List<IndividualAccessGrant> grants = individualAccessGrants.Where(i => i.GranteeId == item && i.Active).ToList();
                //get all the individual access revokes for this ProfileId where the revoke is active
                List<IndividualAccessRevoke> revokes = individualAccessRevokes.Where(i => i.RevokeeId == item && i.Active).ToList();
                //get the most recent grant
                mostRecentGrant = grants.OrderByDescending(i => i.GrantDate).FirstOrDefault();
                //get the most recent revoke
                mostRecentRevoke = revokes.OrderByDescending(i => i.RevokeDate).FirstOrDefault();

                //if there is more than one active grant in the list, our first objective is to find the most recent grant and make all of the older grants inactive
                if (grants.Count > 1)
                {
                    //go through all of the grants
                    foreach (var grant in grants)
                    {
                        //if it's not the most recent grant, set active to false, update it in the database, and then remove it from the list
                        if (!grant.Equals(mostRecentGrant))
                        {
                            grant.Active = false;
                            _db.Entry(grant).State = EntityState.Modified;
                            _db.SaveChanges();
                            grants.Remove(grant);
                        }
                    }
                } //if there's more than one grant

                //if there is more than one active revoke in the list, our first objective is to find the most recent revoke and make all of the older revokes inactive
                if (revokes.Count > 1)
                {
                    //go through all the revokes
                    foreach (var revoke in revokes)
                    {
                        //if it's not the most recent revoke, set active to false, update it in the database, and then remove it from the list
                        if (!revoke.Equals(mostRecentRevoke))
                        {
                            revoke.Active = false;
                            _db.Entry(revoke).State = EntityState.Modified;
                            _db.SaveChanges();
                            revokes.Remove(revoke);
                        }
                    }
                } //if there's more than one revoke

                //there should now only be one revoke and one grant their respective lists
                //compare the dates and set the older one to no longer be active and then remove it from its list
                if (mostRecentGrant.GrantDate > mostRecentRevoke.RevokeDate)
                {
                    mostRecentRevoke.Active = false;
                    _db.Entry(mostRecentRevoke).State = EntityState.Modified;
                    _db.SaveChanges();
                    individualAccessRevokes.Remove(mostRecentRevoke);
                }
                else
                {
                    mostRecentGrant.Active = false;
                    _db.Entry(mostRecentGrant).State = EntityState.Modified;
                    _db.SaveChanges();
                    individualAccessGrants.Remove(mostRecentGrant);
                }

                overlappingProfileIds.Remove(item);
            } //foreach overlappingProfileId

            //there should no longer be any overlapping profile IDs, and the individualAccessGrants and individualAccessRevokes should only contain relevant information now
            //let's update the mostRecentGrant and mostRecentRevoke
            mostRecentGrant = individualAccessGrants.OrderByDescending(i => i.GrantDate).FirstOrDefault();
            mostRecentRevoke = individualAccessRevokes.OrderByDescending(i => i.RevokeDate).FirstOrDefault();

            //if they have individual access revokes but no grants, return false
            if (individualAccessRevokes.Count > 0 && individualAccessGrants.Count == 0)
            {
                errors.Add(new IdentityError
                {
                    Description = "You have had your access revoked by the owner of this " + accessType + "."
                });

                return false;
            } //if they have individual access grants and no revokes, return true
            else if (individualAccessGrants.Count > 0 && individualAccessRevokes.Count == 0)
            {
                return true;
            } //if there are both individual access grants and individual access revokes
            else if (individualAccessGrants.Count > 0 && individualAccessRevokes.Count > 0)
            {
                if (mostRecentGrant.GrantDate > mostRecentRevoke.RevokeDate)
                {
                    return true;
                }
                else
                {
                    errors.Add(new IdentityError
                    {
                        Description = "You have had your access revoked by the owner of this " + accessType + "."
                    });

                    return false;
                }
            }

            //if you made it this far, you no longer need to worry about user blocks, individual access revokes and grants, or minor access
            //if there is public access, return true
            if (accessPermission.PublicAccess)
            {
                return true;
            } //if there's not public access
            else
            {
                //if there's publisher access and the user has at least one verified publisher account, return true
                if (accessPermission.PublisherAccess && GetVerifiedPublisherProfiles(userId).Count > 0)
                {
                    return true;
                }

                //if there's friend access, let's check if we are friends
                if (accessPermission.FriendAccess)
                {
                    List<Friendship> friendships = new List<Friendship>();

                    foreach (var userProfileId in userProfileIds)
                    {
                        //get all the friendships for this profile id
                        List<Friendship> friends = _db.Friendships.Where(i => i.FirstFriendId == userProfileId && i.Active).ToList();

                        foreach (var friend in friends)
                        {
                            friendships.Add(friend);
                        }
                    }

                    //if this is the access permission for a profile and the user has a profile that has a friendship with this profile, return true
                    if (accessPermission.ProfileId != null && friendships.Any(i => i.SecondFriendId == accessPermission.ProfileId))
                    {
                        return true;
                    } //if the AccessPermission is for a piece of Writing
                    else if (accessPermission.WritingId != null)
                    {
                        List<PenProfile> writerProfiles = _db.Writings.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().PenUser.PenProfiles.ToList();

                        foreach (var friendship in friendships)
                        {
                            if (writerProfiles.Any(i => i.ProfileId == friendship.SecondFriendId))
                            {
                                return true;
                            }
                        }
                    } //if the AccessPermission is for a Folder
                    else if (accessPermission.FolderId != null)
                    {
                        List<FolderOwner> folderOwners = _db.Folders.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().Owners.ToList();

                        foreach (var friendship in friendships)
                        {
                            if (folderOwners.Any(i => i.OwnerId == friendship.SecondFriendId))
                            {
                                return true;
                            }
                        }
                    } //if the AccessPermission is for a Series
                    else if (accessPermission.SeriesId != null)
                    {
                        List<SeriesOwner> seriesOwners = _db.Series.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().Owners.ToList();

                        foreach (var friendship in friendships)
                        {
                            if (seriesOwners.Any(i => i.OwnerId == friendship.SecondFriendId))
                            {
                                return true;
                            }
                        }
                    } //if it's a series
                } //if there's friend access

                if (accessPermission.PublisherAccess && accessPermission.FriendAccess)
                {
                    if (UserHasPublisherProfile() && !UserHasVerifiedPublisherProfile())
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This " + accessType + " is only accessable by verified publishers and friends. To gain access, have your publisher account verified, send a friend request, or request individual access."
                        });
                    }
                    else
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This " + accessType + " is only accessable by verified publishers and friends. To gain access, send a friend request or request individual access."
                        });
                    }
                }
                else if (accessPermission.PublisherAccess)
                {
                    if (UserHasPublisherProfile() && !UserHasVerifiedPublisherProfile())
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This " + accessType + " is only accessable by verified publishers. To gain access, have your publisher account verified or request individual access."
                        });
                    }
                    else
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This " + accessType + " is only accessable by verified publishers. To gain access, request individual access."
                        });
                    }
                }
                else if (accessPermission.FriendAccess)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "This " + accessType + " is only accessable by friends. To gain access, send a friend request or request individual access."
                    });
                }
                else
                {
                    errors.Add(new IdentityError
                    {
                        Description = "This " + accessType + " is only accessable by owner permission. To gain access, request individual access."
                    });
                }
            } //if there's not public access

            //if you somehow manage to make it this far, return false
            return false;
        }

        public string HTMLByteArrayToString(byte[] input)
        {
            if (input == null)
            {
                return null;
            }

            string output = Encoding.Unicode.GetString(input);
            output = output.Replace("&lt;", "<").Replace("&gt;", ">").Replace("'", "&#39;");

            return output;
        }
    }
}

