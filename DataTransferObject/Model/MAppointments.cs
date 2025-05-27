using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MAppointment
    {
        [Key]
        public short ApptId { get; set; }
        public string AppointmentName { get; set; }
        public bool IsActive { get; set; }
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string AppointmentAbbreviation { get; set; }

    }
}
