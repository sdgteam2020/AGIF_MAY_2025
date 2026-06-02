using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataTransferObject.Model
{
    public class MBank
    {
        [Key]
        public int BankId { get; set; }

        public string BankName { get; set; }

        public string BankAbbreviation { get; set; }

        public bool IsActive { get; set; }
    }
}
