using System.ComponentModel.DataAnnotations;
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
}
