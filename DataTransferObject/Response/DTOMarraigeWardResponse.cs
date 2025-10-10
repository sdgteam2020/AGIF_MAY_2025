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
    public class DTOMarraigeWardResponse
    {
        public string NameOfChild { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DOPartIINo { get; set; }
        public DateTime? DoPartIIDate { get; set; }
        public int AgeOfWard { get; set; }
        public DateTime? DateofMarriage { get; set; }
        public string Gender { get; set; }
    }
}
