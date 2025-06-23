
using Agif_V2.Helpers;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Helpers;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Mvc;

namespace Agif_V2.Controllers
{
    public class ClaimController : Controller
    {
        private readonly IOnlineApplication _IonlineApplication1;
        private readonly IMasterOnlyTable _IMasterOnlyTable;
        private readonly PdfGenerator _pdfGenerator;
        private readonly MergePdf _mergePdf;
        private readonly IWebHostEnvironment _env;
        private readonly ICar _car;
        private readonly IHba _Hba;
        private readonly IPca _Pca;

        public ClaimController(IOnlineApplication OnlineApplication, IMasterOnlyTable MasterOnlyTable, ICar _car, IHba _Hba, IPca _Pca, PdfGenerator pdfGenerator, IWebHostEnvironment env, MergePdf mergePdf)
        {
            _IonlineApplication1 = OnlineApplication;
            _IMasterOnlyTable = MasterOnlyTable;
            this._car = _car;
            this._Hba = _Hba;
            this._Pca = _Pca;
            _pdfGenerator = pdfGenerator;
            _env = env;
            _mergePdf = mergePdf;
        }

        public IActionResult MaturityLoanType()
        {
            return View();
        }

        public async Task<IActionResult> OnlineApplication()
        {
            var Category = TempData["Category"] as string;
            var WithdrwalPurpose = TempData["WithdrwalPurpose"] as string;


            TempData["CategoryNew"] = EncryptDecrypt.DecryptionData(Category);

            TempData["WithdrwalPurposeNew"] = EncryptDecrypt.DecryptionData(WithdrwalPurpose);


            TempData.Keep("Category");
            TempData.Keep("WithdrwalPurpose");

            DTOOnlineApplication DTOOnlineapplication = new DTOOnlineApplication();
            return View(DTOOnlineapplication);
        }

        public IActionResult Redirection(string applicantCategory,string PurposeOfWithdrwal)
        {
            // Handle the received loanType and applicantCategory
            // You can now access the form data here
            // For example, you can pass these values to a view or use them in processing logic
            string Category = EncryptDecrypt.EncryptionData(applicantCategory);
            string WithdrwalPurpose = EncryptDecrypt.EncryptionData(PurposeOfWithdrwal);

            TempData["Category"] = Category;
            TempData["WithdrwalPurpose"] = WithdrwalPurpose;
            return RedirectToAction("OnlineApplication");
        }



    }
}
