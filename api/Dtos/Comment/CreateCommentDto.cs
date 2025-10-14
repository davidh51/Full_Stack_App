using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CreateCommentDto
    {
        [Required]  //Data Validation
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters long")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Tittle { get; set; } = string.Empty;

        [Required]  //Data Validation
        [MinLength(5, ErrorMessage = "Content must be at least 5 characters long")]
        [MaxLength(100, ErrorMessage = "Content cannot exceed 100 characters")]
        public string Content { get; set; } = string.Empty;
    }
}