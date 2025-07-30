using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class ClaimAddressDetailsModel:Common
    {
        [Key]
        public int AddressId { get; set; }
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonDataModels { get; set; }

        [Required(ErrorMessage = "Village/Town is required.")]
        [StringLength(100, ErrorMessage = "Village/Town can't be longer than 100 characters.")]
        public string? Vill_Town { get; set; }

        [Required(ErrorMessage = "Post Office is required.")]
        [StringLength(100, ErrorMessage = "Post Office can't be longer than 100 characters.")]
        public string? PostOffice { get; set; }

        [Required(ErrorMessage = "District is required.")]
        [StringLength(100, ErrorMessage = "District can't be longer than 100 characters.")]
        public string? Distt { get; set; }


        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State can't be longer than 100 characters.")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Zip Code is required.")]
        [StringLength(6, ErrorMessage = "Zip Code can't be longer than 6 characters.")]
        public string? Code { get; set; }

    }
}
