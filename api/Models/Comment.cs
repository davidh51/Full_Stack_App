using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Comments")] // Specify the table name
    public class Comment
    {
        public int Id { get; set; } // Primary Key
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int? StockId { get; set; } // Foreign Key
        public Stock? Stock { get; set; } // Navigation Property
        public string? AppUserId { get; set; } // Foreign Key
        public AppUser? AppUser { get; set; } // Navigation Property
    }
}