using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Stock
{
    public class UpdateStockRequestDto
    {
        [Required]  //Data Validation
        [MaxLength(10, ErrorMessage = "Symbol cannot exceed 10 characters")]
        public string Symbol { get; set; } = string.Empty;
        [Required]  //Data Validation
        [MaxLength(30, ErrorMessage = "Company cannot exceed 30 characters")]
        public string CompanyName { get; set; } = string.Empty;
        [Required]  //Data Validation
        [Range(1, 10000)]
        public decimal Purchase { get; set; }
        [Required]  //Data Validation
        [Range(0.01, 100)]
        public decimal LastDiv { get; set; }
        [Required]  //Data Validation
        [MaxLength(30, ErrorMessage = "Industry cannot exceed 30 characters")]
        public string Industry { get; set; } = string.Empty;
        [Required]  //Data Validation
        [Range(1, 500000000)]
        public long MarketCap { get; set; }
    }
}