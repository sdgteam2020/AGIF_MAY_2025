using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MRank : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Rank Id is number.")]
        public int RankId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50, ErrorMessage = "Maximum length of Rank Name is fifty character.")]
        public string RankName { get; set; } = string.Empty;


        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public short Orderby { get; set; }

        [Required(ErrorMessage = "required!")]
        [ForeignKey(nameof(MApplicantType))]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApplyForId is number.")]
        public int ApplyForId { get; set; }

        public MApplicantType? MApplicantType { get; set; }


    }
}
