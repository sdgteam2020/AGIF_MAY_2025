using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Interfaces
{
    public interface IModelValidationService
    {
        DateTime CalculateRetirementDate(
        int userTypeId,
        int rankId,
        string prefix,
        int regtId,
        DateTime dob,
        DateTime doc,
        DateTime? promotionDate,
        bool extensionOfService,
        int retirementAge);

        decimal CalculateTotalService(DateTime doc);

        decimal CalculateResidualService(DateTime retirementDate);
        Task<DTORetirementInforesponse?> GetRetirementInfo(int rankId, int prefix, int regtId);

        Task ValidateHBADetails(DTOOnlineApplication model, ModelStateDictionary modelState);

        Task ValidateCADetails(DTOOnlineApplication model, ModelStateDictionary modelState);

        Task ValidatePCADetails(DTOOnlineApplication model, ModelStateDictionary modelState);
        Task ValidateClaimRetirementDetails(DTOClaimApplication model, ModelStateDictionary modelState);

    }

}
