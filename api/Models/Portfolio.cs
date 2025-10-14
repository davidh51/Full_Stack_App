using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Portfolios")] // Specify the table name
    public class Portfolio
    {
        public string AppUserId { get; set; } = null!;// Foreign Key
        public int StockId { get; set; } // Foreign Key   
        public AppUser AppUser { get; set; } = null!; // Navigation Property
        public Stock Stock { get; set; } = null!; // Navigation Property
    }
}