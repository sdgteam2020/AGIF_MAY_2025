using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataTransferObject.Model
{
    public class MDist
    {
        [Key]
        public int DistrictId { get; set; }

        [Required]
        public string DistrictName { get; set; }

        [Required]
        public int StateId { get; set; }

        [ForeignKey(nameof(StateId))]
        public MState? State { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
