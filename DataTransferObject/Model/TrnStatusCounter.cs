using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class TrnStatusCounter
    {
        [Key]
        public int Id { get; set; }

        public int StatusId { get; set; }

        public int ApplicationId { get; set; }  

        public DateTime? ActionOn { get; set; }
    }
}
