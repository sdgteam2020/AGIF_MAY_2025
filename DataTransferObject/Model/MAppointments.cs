using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MAppointment:Common
    {
        [Key]
        public int ApptId { get; set; }
        [Required(ErrorMessage ="Appointment Name is Required")]
        public string AppointmentName { get; set; }=string.Empty;

    }
}
