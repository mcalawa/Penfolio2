﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Penfolio2.Validation;

namespace Penfolio2.Models
{
    public class AccessPermissionViewModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

        }
    }

    public class CreateProfileViewModel
    {
        [Required]
        [Display(Name = "Display Name")]
        [ProfileValidator.DisplayNameForCreate]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Profile Type")]
        public int RoleType { get; set; }

        [Required]
        [Display(Name = "Set as Main Profile?")]
        public bool IsMainProfile { get; set; }

        [Display(Name = "Profile Description (Optional)")]
        public string? ProfileDescription { get; set; }

        [Display(Name = "Custom URL (Optional)")]
        [ProfileValidator.UrlStringForCreate]
        public string? UrlString { get; set; }

        [Display(Name = "Profile Image (Optional)")]
        public IFormFile? ProfileImage { get; set; }

        [Required]
        [Display(Name = "Allow Public Access")]
        public bool PublicAccess { get; set; }

        [Required]
        [Display(Name = "Allow Friend Access")]
        public bool FriendAccess { get; set; }

        [Required]
        [Display(Name = "Allow Publisher and Literary Agent Access")]
        public bool PublisherAccess { get; set; }

        [Required]
        [Display(Name = "Allow Minor Access")]
        public bool MinorAccess { get; set; }

        [Required]
        [Display(Name = "Profile Shows Up in Search")]
        public bool ShowsUpInSearch { get; set; }
    }

    public class EditProfileViewModel
    {
        [Required]
        public int ProfileId { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        [ProfileValidator.DisplayNameForEdit]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Set as Main Profile?")]
        public bool IsMainProfile { get; set; }

        [Display(Name = "Profile Description (Optional)")]
        public string? ProfileDescription { get; set; }

        [Display(Name = "Custom URL (Optional)")]
        [ProfileValidator.UrlStringForEdit]
        public string? UrlString { get; set; }

        [Display(Name = "Profile Image (Optional)")]
        public IFormFile? ProfileImage { get; set; }

        [Required]
        [Display(Name = "Allow Public Access")]
        public bool PublicAccess { get; set; }

        [Required]
        [Display(Name = "Allow Friend Access")]
        public bool FriendAccess { get; set; }

        [Required]
        [Display(Name = "Allow Publisher and Literary Agent Access")]
        public bool PublisherAccess { get; set; }

        [Required]
        [Display(Name = "Allow Minor Access")]
        public bool MinorAccess { get; set; }

        [Required]
        [Display(Name = "Profile Shows Up in Search")]
        public bool ShowsUpInSearch { get; set; }
    }

    public class DeleteProfileViewModel
    {
        public DeleteProfileViewModel()
        {
            OtherProfiles = new HashSet<DeleteProfileViewModel>();
        }

        [Required]
        public int ProfileId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string RoleName { get; set; }

        [Required]
        public bool IsMainProfile { get; set; }

        [Display(Name = "Select New Main Profile")]
        public int? NewMainProfile { get; set; }

        public virtual ICollection<DeleteProfileViewModel> OtherProfiles { get; set; }
    }

    public class WritingViewModel
    {
        public WritingViewModel() 
        {
            WritingProfiles = new HashSet<PenProfile>();
            FormatTags = new HashSet<FormatTag>();
            GenreTags = new HashSet<GenreTag>();
            FormatCategories = new HashSet<FormatCategory>();
            GenreCategories = new HashSet<GenreCategory>();
            GenreFormats = new HashSet<GenreFormat>();
            SelectedProfileIds = new HashSet<int>();
            SelectedFormatIds = new HashSet<int>();
            SelectedGenreIds = new HashSet<int>();
        }

        [Required, NotNull]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [AllowNull]
        [Display(Name = "Description")]
        public string? Description { get; set; } = null;

        [Required, NotNull]
        public string EditorContent { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Allow Public Access")]
        public bool PublicAccess { get; set; } = false;

        [Required]
        [Display(Name = "Allow Friend Access")]
        public bool FriendAccess { get; set; } = false;

        [Required]
        [Display(Name = "Allow Publisher and Literary Agent Access")]
        public bool PublisherAccess { get; set; } = false;

        [Required]
        [Display(Name = "Allow Minor Access")]
        public bool MinorAccess { get; set; } = false;

        [Required]
        [Display(Name = "Writing Shows Up in Search")]
        public bool ShowsUpInSearch { get; set; } = false;

        [Required, NotNull]
        public string SelectedProfiles { get; set; } = string.Empty;

        [Required, NotNull]
        public string SelectedFormats { get; set; } = string.Empty;

        [Required, NotNull]
        public string SelectedGenres { get; set; } = string.Empty;

        public virtual ICollection<PenProfile> WritingProfiles { get; set; }

        public virtual ICollection<FormatTag> FormatTags { get; set; }

        public virtual ICollection<GenreTag> GenreTags { get; set; }

        public virtual ICollection<FormatCategory> FormatCategories { get; set; }

        public virtual ICollection<GenreCategory> GenreCategories { get; set; }

        public virtual ICollection<GenreFormat> GenreFormats { get; set; }

        public virtual ICollection<int> SelectedProfileIds { get; set; }

        public virtual ICollection<int> SelectedFormatIds { get; set; }

        public virtual ICollection<int> SelectedGenreIds { get; set; }
    }

    public class AuthorViewModel
    {
        [Required]
        public string DisplayName { get; set; }

        [AllowNull]
        public string? UrlString { get; set; }

        [Required]
        public bool IsAnonymous { get; set; }
    }

    public class NotificationViewModel
    {

    }
}
