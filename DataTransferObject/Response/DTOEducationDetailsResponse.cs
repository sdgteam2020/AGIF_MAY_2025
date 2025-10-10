using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOEducationDetailsResponse
    {
        public string ChildName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DOPartIINo { get; set; }
        public DateTime? DoPartIIDate { get; set; }
        public string CourseForWithdrawal { get; set; }
        public string CollegeInstitution { get; set; }
        public double TotalExpenditure { get; set; }
        public string Gender { get; set; }
    }
}
