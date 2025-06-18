using DataAccessLayer.Interfaces;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Path = System.IO.Path;

namespace Agif_V2.Helpers
{
    public class PdfGenerator
    {
        private readonly IOnlineApplication _usersApplications;
        public PdfGenerator(IOnlineApplication usersApplications)
        {
            _usersApplications = usersApplications;
        }
        public async Task<int> CreatePdfForOnlineApplication(int applicationId, string generatedPdfPath)
        {
            var data = await _usersApplications.GetApplicationDetails(applicationId);
            var directory = Path.GetDirectoryName(generatedPdfPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var writer = new PdfWriter(generatedPdfPath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var pageSize = PageSize.A4; // Use PageSize directly
                    var document = new Document(pdf, pageSize); // Use PageSize instead of Rectangle
                    document.SetMargins(30f, 30f, 30f, 30f);

                    //Define Fonts
                    PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    PdfFont subtitleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    
                    Table headerTable = new Table(2).UseAllAvailableWidth();
                    headerTable.AddCell(new Cell().Add(new Paragraph(DateTime.Now.ToString("dd-MM-yyyy hh:mm tt")).SetFont(normalFont))
                                                 .SetTextAlignment(TextAlignment.LEFT).SetBorder(Border.NO_BORDER));
                    headerTable.AddCell(new Cell().Add(new Paragraph($"{data.OnlineApplicationResponse.Number}_AGIF_Loan_ApplForm").SetFont(normalFont))
                                                 .SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER));
                    document.Add(headerTable);

                    // Add a title
                    document.Add(new Paragraph("Applicant Details")
                        .SetFont(boldFont)
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(20)
                        .SetUnderline());

                    // 4-column table
                    //Table table = new Table(new float[] { 8f, 7f, 8f, 7f });
                    //table.SetWidth(UnitValue.CreatePercentValue(100));

                    Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 5, 3, 5 })).UseAllAvailableWidth();
                    // Helper method
                    void AddRow(string label1, string value1, string label2, string value2)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(label1).SetFont(boldFont)).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(value1 ?? "").SetFont(normalFont)).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(label2).SetFont(boldFont)).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(value2 ?? "").SetFont(normalFont)).SetBorder(Border.NO_BORDER));
                    }

                    var common = data.OnlineApplicationResponse;

                    // Populate rows
                    AddRow("1. Applicant Name", common.ApplicantName, "2. Army Number", common.Number);
                    AddRow("3. Rank", common.DdlRank, "4. Present Unit", common.PresentUnit);
                    AddRow("5. Parent Unit", common.ParentUnit, "6. Date of Birth", common.DateOfBirth?.ToString("dd-MM-yyyy"));
                    AddRow("7. Date of Commission", common.DateOfCommission?.ToString("dd-MM-yyyy"), "8.Date of Promotion", common.DateOfPromotion?.ToString("dd-MM-yyyy"));
                    AddRow("9. Date of Retirement", common.DateOfRetirement?.ToString("dd-MM-yyyy"), "10. PAN Card No", common.PanCardNo);
                    AddRow("11. Aadhaar Card No", common.AadharCardNo, "12. Mobile No", common.MobileNo);
                    AddRow("13. Email", common.Email, "14. Post Office", common.PostOffice);
                    AddRow("15. District", common.Distt, "16. State", common.State);
                    AddRow("17. Village/Town", common.Vill_Town, "18. Regt/Corps", common.RegtCorps);
                    AddRow("19. Army Post Office", common.ArmyPostOffice, "20. Present Unit PIN", common.PresentUnitPin);
                    AddRow("21. Salary Account No", common.SalaryAcctNo, "22. IFSC Code", common.IfsCode);
                    AddRow("23. Bank Name", common.NameOfBank, "24. Bank Branch", common.NameOfBankBranch);
                    AddRow("25. PCDA/PAO", common.pcda_pao, "", "");

                    document.Add(table);

                    // Footer
                    //document.Add(new Paragraph("This is a system-generated PDF using iText7.")
                    //    .SetFont(normalFont)
                    //    .SetFontSize(10)
                    //    .SetTextAlignment(TextAlignment.CENTER)
                    //    .SetMarginTop(30));

                    if (data.HbaApplicationResponse != null)
                    {
                        //document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                        document.Add(new Paragraph("HBA Loan Details")
                            .SetFont(boldFont).SetFontSize(14).SetTextAlignment(TextAlignment.CENTER).SetUnderline().SetMarginTop(20));

                        var hba = data.HbaApplicationResponse;
                        Table hbaTable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 5, 3, 5 })).UseAllAvailableWidth();

                        void AddLoanRow(string label1, string val1, string label2, string val2)
                        {
                            hbaTable.AddCell(new Cell().Add(new Paragraph(label1).SetFont(boldFont)).SetBorder(Border.NO_BORDER));
                            hbaTable.AddCell(new Cell().Add(new Paragraph(val1 ?? "").SetFont(normalFont)).SetBorder(Border.NO_BORDER));
                            hbaTable.AddCell(new Cell().Add(new Paragraph(label2).SetFont(boldFont)).SetBorder(Border.NO_BORDER));
                            hbaTable.AddCell(new Cell().Add(new Paragraph(val2 ?? "").SetFont(normalFont)).SetBorder(Border.NO_BORDER));
                        }

                        AddLoanRow("26. Property Type", hba.PropertyType, "27. Property Seller", hba.PropertySeller);
                        AddLoanRow("28. Property Address", hba.PropertyAddress, "29. Property Cost", hba.PropertyCost.ToString());
                        AddLoanRow("30. Loan Frequency", hba.HBA_LoanFreq.ToString(), "", "");

                        document.Add(hbaTable);

                        document.Add(new Paragraph("31. I have read the Terms & Conditions, Instructions and Rules regulating the grant of House Building Advance to AGIF members and agree to abide by them. I agree to pay the one time non-refundable insurance premium. I shall refund in one lump sum the full loan amount together with interest outstanding, in case, I wish to sell or transfer the house by way of gift deed.")
           .SetFont(normalFont).SetFontSize(10).SetMarginTop(10).SetMarginBottom(5));

                        document.Add(new Paragraph("32. I state and certify that:")
                            .SetFont(normalFont).SetFontSize(10).SetMarginTop(10).SetMarginBottom(5));

                        List<string> checkPara = new List<string>
                {
             "(a) If, at any time, it is found that I have obtained loan from AGIF by misrepresentation, fraud,  misstatement or have committed a breach of the terms & conditions issued from time to time by AGIF, I shall without prejudice, be liable to legal, disciplinary or any other action and will become liable to repay in one lump sum the full loan amount together with interest outstanding, for period of default at a rate 2 % higher than the rate of interest at which the loan was sanctioned, to AGIF without any demur.",
             "(b) I will maintain adequate credit in my IRLA to meet EMI payment during the duration and till complete repayment of loan. In case of any debit balance resulting in non-remittance of EMI by the CDA (O) / PAO (OR) to AGIF, I undertake to pay it directly along with interest for period of default at a rate 2 % higher than the rate of interest at which the loan was sanctioned, as levied by the AGIF for the delayed period.",
             "(c) In the event of my becoming non-effective due to any reason like retirement / dismissal / premature retirement / death preceding retirement etc, the AGIF shall be entitled to recover through the PAO (OR) / CDA(O) and/or receive the balance of the loan with interest remaining unpaid and any other dues from the whole or any specified part of the gratuity, Insurance / Disability / Maturity / survival benefit of AGIF, death benefits, DSOP Fund, DCRG, Commuted value of pension, leave encashment, service pension, that may be payable to me, without any demur from any quarter.",
             "(d) I hereby voluntarily authorize CDA(O) / PAO(OR) to deduct EMI on account of HBA taken from the AGIF from my pay and allowances on a monthly basis and remit the same to AGIF on my behalf. I hereby assign and transfer upto the AGIF the House by way of security for the said loan and the interest thereon.",
             "(e) In case of any dispute arising with regards to the rules, agreements and deeds executed there under, I am bound to the jurisdiction of Delhi Courts only."
                };

                        foreach (var point in checkPara)
                        {
                            document.Add(new Paragraph(point)
                                .SetFont(normalFont).SetFontSize(10).SetTextAlignment(TextAlignment.JUSTIFIED).SetMarginBottom(3).SetMarginLeft(20));
                        }

             document.Add(new Paragraph("33. I, solemnly declare that the details/information furnished by me and averments/certifications made herein are true to the best of my knowledge and belief and have not willfully suppressed any material information.")
              .SetFont(normalFont).SetFontSize(10).SetMarginTop(10).SetMarginBottom(5));

                        string domainInfo = $"Verified by - {""} IP Address – {""} Date Time  – {DateTime.Now:dd-MM-yyyy hh:mm tt} ";
                        document.Add(new Paragraph(domainInfo)
                            .SetFont(normalFont).SetFontSize(10).SetFontColor(ColorConstants.BLUE).SetTextAlignment(TextAlignment.JUSTIFIED).SetMarginBottom(10));

                        Table signatureTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 })).UseAllAvailableWidth();
                       // signatureTable.AddCell(new Cell().Add(new Paragraph("Date: " + carPcModel.DateTimeUpdated).SetFont(normalFont).SetFontSize(10)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT));
                        signatureTable.AddCell(new Cell().Add(new Paragraph(common.Number.ToString()).SetFont(normalFont).SetFontSize(10)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
                        signatureTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                        signatureTable.AddCell(new Cell().Add(new Paragraph(common.DdlRank + " " + common.ApplicantName).SetFont(normalFont).SetFontSize(10)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));

                        document.Add(signatureTable);
                    }


                    document.Close();
                }
            }
            return 1;
        }
    }
}
