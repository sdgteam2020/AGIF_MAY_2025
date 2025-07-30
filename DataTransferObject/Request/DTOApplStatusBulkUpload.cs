using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class DTOApplStatusBulkUpload
    {
        public int ApplId { get; set; }
        public string ArmyNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ApplicationType { get; set; } = string.Empty;
        public int Status_Code { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class DTOApplStatusBulkUploadlst
    {
        public List<DTOApplStatusBulkUpload> DTOApplStatusBulkUploadOK { get; set; } = new List<DTOApplStatusBulkUpload>();
        public List<DTOApplStatusBulkUpload> DTOApplStatusBulkUploadNotOk { get; set; } = new List<DTOApplStatusBulkUpload>();
    }
}