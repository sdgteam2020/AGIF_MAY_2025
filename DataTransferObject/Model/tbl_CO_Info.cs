using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class tbl_CO_Info
    {
        [Key]
        public int Id { get; set; }
        public string armyNo { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public int unit { get; set; }
        public int UnitId { get; set; }
        public int rank { get; set; }
        public string userId { get; set; }
        public bool isActive { get; set; }
        public DateTime CoRegisteredDate { get; set; }
        public int ApptId { get; set; }
    }
}
