using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MAgeMapping
    {
        [Key]
        public int AgeMappingId { get; set; }
        public int RegtId { get; set; }
        [ForeignKey("RegtId")]
        public MRegtCorps? MRegtCorps { get; set; }
        public int RankId { get; set; }
        [ForeignKey("RankId")]
        public MRank? MRank { get; set; }
        public int RetirementAge { get; set; }
    }
}
