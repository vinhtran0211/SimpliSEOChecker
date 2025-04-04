using System;
using System.ComponentModel.DataAnnotations;

namespace SimpliSEOChecker.Models
{
    public class HomeModel
    {
        [Required(ErrorMessage = "Keyword is required")]
        [StringLength(80, ErrorMessage = "Keyword cannot exceed 80 characters")]
        public string Keyword { get; set; }

        [Required(ErrorMessage = "UrlToFind is required")]
        [StringLength(80, ErrorMessage = "UrlToFind cannot exceed 80 characters")]
        public string UrlToFind { get; set; }
        
    }
}
