using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class PropertyRenovationModel : Common
    {
        [Key]
        public int PrId { get; set; }

        [Required(ErrorMessage = "Application ID is required.")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonModel { get; set; }
        [Required]
        public string AddressOfProperty { get; set; }

        // Name of property holder(s)
        [Required]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Property Holder Name must only contain alphabets and spaces.")]
        public string PropertyHolderName { get; set; }

        // Estimated cost of expenditure
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid estimated cost.")]
        public double EstimatedCost { get; set; }

        // Total expenditure file upload
        [NotMapped]
        [Required]
        public IFormFile TotalExpenditureFile { get; set; }

        public string? TotalExpenditureFilePdf { get; set; }

        public bool IsTotalExpenditureFilePdf { get; set; }
    }
}
