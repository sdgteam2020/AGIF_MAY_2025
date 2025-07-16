using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class DTODataTableRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int Draw { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int Start { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int Length { get; set; }

        // Only Alphabets and Numbers allowed for search value
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string? searchValue { get; set; }

        // Valid column names: ensure the sortColumn is in the allowed columns list
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Only alphabets, numbers, and underscores allowed.")]
        public string sortColumn { get; set; } = string.Empty;

        // Only "asc" or "desc" for sort direction
        [RegularExpression("^(asc|desc)$", ErrorMessage = "Only 'asc' or 'desc' are allowed.")]
        public string sortDirection { get; set; } = string.Empty;
    }
}
