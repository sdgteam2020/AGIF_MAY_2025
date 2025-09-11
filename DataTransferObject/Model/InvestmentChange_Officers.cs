using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class InvestmentChange_Officers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ChangeDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InvestmentAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        [Column(TypeName = "decimal(7,2)")]
        public decimal PrAmount { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
