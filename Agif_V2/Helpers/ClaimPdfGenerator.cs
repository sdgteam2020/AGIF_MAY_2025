using DataAccessLayer.Interfaces;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Office2013.Excel;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.Net.NetworkInformation;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Path = System.IO.Path;


namespace Agif_V2.Helpers
{

    public class ClaimPdfGenerator
    {
        private readonly IClaimOnlineApplication _usersApplications;
        public ClaimPdfGenerator(IClaimOnlineApplication usersApplications)
        {
            _usersApplications = usersApplications;
        }

        public async Task<int> CreatePdfForOnlineApplication(int applicationId, string generatedPdfPath, bool isRejected, bool isApproved, string UserName, string IpAddress, string Name)
        {
            var data = await _usersApplications.GetApplicationDetails(applicationId);
            var directory = Path.GetDirectoryName(generatedPdfPath);
            var dated = DateTime.Now.ToString("dd-MM-yyyy HH:mm");

            string formType = string.Empty;
            int counter = 0;

            // First, determine the form type
            if (data.EducationDetailsResponse != null)
            {
                formType = "Education of Ward";
            }
            else if (data.MarraigeWardResponse != null)
            {
                formType = "Marriage of Ward";
            }
            else if (data.PropertyRenovationResponse != null)
            {
                formType = "Renovation-Repair of House";
            }
            else if (data.SplWaiverResponse != null)
            {
                formType = "Special Waiver";
            }


            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var writer = new PdfWriter(generatedPdfPath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var pageSize = PageSize.A4; // Use PageSize directly
                    var document = new iText.Layout.Document(pdf, pageSize); // Use PageSize instead of Rectangle
                    document.SetMargins(30f, 25f, 40f, 25f);

                    //Define Fonts
                    PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    PdfFont subtitleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    float smallFontSize = 9f;

                    Table headerTable = new Table(2).UseAllAvailableWidth();
                    headerTable.AddCell(new Cell().Add(new Paragraph(DateTime.Now.ToString("dd-MM-yyyy hh:mm tt")).SetFont(normalFont))
                                                 .SetTextAlignment(TextAlignment.LEFT).SetBorder(Border.NO_BORDER));
                    headerTable.AddCell(new Cell().Add(new Paragraph($"{data.OnlineApplicationResponse.Number}_AGIF_Loan_ApplForm").SetFont(normalFont))
                                                 .SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER));
                    document.Add(headerTable);

                    // Add a title
                    document.Add(new Paragraph("ARMY GROUP INSURANCE FUND")
                        .SetFont(boldFont)
                        .SetFontSize(12)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(20)
                        .SetUnderline());

                    document.Add(new Paragraph($"APPLICATION FORM :Maturity")
                        .SetFont(boldFont)
                        .SetFontSize(12).SetMarginTop(0)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(20)
                        .SetUnderline());

                    LineSeparator line = new LineSeparator(new SolidLine());

                    // Add the line to the document
                    document.Add(line);

                    // Optionally, you can adjust the space after the line using SpacingAfter:
                    line.SetMarginBottom(5f); // Adjust the space after the line if needed

                    document.Add(new Paragraph("PART 1")
                        .SetFont(boldFont)
                        .SetFontSize(12).SetMarginTop(5)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(20)
                        .SetUnderline());


                    // 4-column table
                    //Table table = new Table(new float[] { 8f, 7f, 8f, 7f });
                    //table.SetWidth(UnitValue.CreatePercentValue(100));

                    Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();
                    // Helper method
                    void AddRow(string label1, string value1, string label2, string value2, bool isFirstRow = false)
                    {
                        Border topBorder = isFirstRow ? new SolidBorder(1) : Border.NO_BORDER;
                        Border bottomBorder = new SolidBorder(1); // Always apply bottom border
                        table.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont).SetFontSize(smallFontSize))
                            .SetBorderTop(topBorder)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderRight(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(value1 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                            .SetBorderTop(topBorder)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderRight(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont).SetFontSize(smallFontSize))
                            .SetBorderTop(topBorder)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderRight(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(value2 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                            .SetBorderTop(topBorder)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderRight(Border.NO_BORDER));
                    }

                    var common = data.OnlineApplicationResponse;

                    // Populate rows
                    //AddRow("1. Type of Loan", formType, "2. Army Number", common.Number);
                    //AddRow("3.Old Army No", common.OldNumber, "4.Rank", common.DdlRank);
                    //AddRow("5. Name", common.ApplicantName, "6. Date of Birth", common.DateOfBirth?.ToString("dd-MM-yyyy"));
                    //AddRow("7. DOE/DOC", common.DateOfCommission?.ToString("dd-MM-yyyy"), "8.Date of Retirement", common.DateOfRetirement?.ToString("dd-MM-yyyy"));
                    //AddRow("9. Mobile No", common.MobileNo, "10. Email ID", common.Email);
                    //AddRow("11. Regt/Corps", common.RegtCorps, "12. PCDA(O)/PAO(OR)", common.pcda_pao);
                    //AddRow("13.PCDA(O) Acct No", common.pcda_AcctNo, "14. PAN Card", common.PanCardNo);
                    //AddRow("15. Aadhaar Card No", common.AadharCardNo, "16. Parent Unit", common.ParentUnit);
                    //AddRow("17. Present Unit", common.PresentUnit, "18.Unit PIN", common.PresentUnitPin);
                    //AddRow("19. Unit Address", common.ArmyPostOffice, "20. Civil Postal Address", common.CivilPostalAddress);
                    //AddRow("21. Fmn HQ", common.NextFmnHQ, "22. Date of Prom", common.DateOfPromotion?.ToString("dd-MM-yyyy"));


                    AddRow("1. Army No", common.Number, "2. Old Army No", common.OldNumber);
                    AddRow("3. Rank", common.DdlRank, "4. Name", common.ApplicantName);
                    AddRow("5. Regt/Corps", common.RegtCorps, "6. Present Unit", common.PresentUnit);
                    AddRow("7. Unit Pin Code", common.PresentUnitPin, "8. Unit Address", common.ArmyPostOffice);
                    AddRow("9. Fmn HQ", common.NextFmnHQ, "10. Permanent Home Address", common.CivilPostalAddress);
                    AddRow("11. Date of Enrollment", common.DateOfCommission?.ToString("dd-MM-yyyy"), "12. Date of Birth", common.DateOfBirth?.ToString("dd-MM-yyyy"));
                    AddRow("13. Date of Retirement", common.DateOfRetirement?.ToString("dd-MM-yyyy"), "14. Total Service (In Years)", common.TotalService.ToString());
                    AddRow("15. E-Mail", common.Email+common.EmailDomain, "16. Aadhaar No", common.AadharCardNo);
                    AddRow("17. Pan No", common.PanCardNo, "18. Mob No", common.MobileNo);
                    AddRow("19. Salary Account No", common.SalaryAcctNo, "20. IFSC Code", common.IfsCode);
                    AddRow("21. Bank Branch", common.NameOfBankBranch, "22. Purpose of Withdrawal", formType);
                    AddRow("23. Amount of Withdrawal Reqd.", common.AmountwithdrwalRequired.ToString(), "24. No of Withdrawals", common.NoOfwithdrwal);

                    document.Add(table);




                    Table table2 = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();
                    // Helper method
                    void AddRow2(string label1, string value1, string label2, string value2, bool isFirstRow = false)
                    {
                        Border topBorder = isFirstRow ? new SolidBorder(1) : Border.NO_BORDER;
                        Border bottomBorder = new SolidBorder(1); // Always apply bottom border
                        table2.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont).SetFontSize(smallFontSize))
                            .SetBorderTop(topBorder)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderRight(Border.NO_BORDER));
                        table2.AddCell(new Cell().Add(new Paragraph(value1 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                            .SetBorderTop(topBorder)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderRight(Border.NO_BORDER));
                        table2.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont).SetFontSize(smallFontSize))
                            .SetBorderTop(topBorder)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderRight(Border.NO_BORDER));
                        table2.AddCell(new Cell().Add(new Paragraph(value2 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                            .SetBorderTop(topBorder)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderRight(Border.NO_BORDER));
                    }





                    if (common.House_Building_Advance_Loan || common.House_Repair_Advance_Loan || common.Conveyance_Advance_Loan || common.Computer_Advance_Loan)
                    {
                        counter = 25;
                        Paragraph point14 = new Paragraph($"{counter}. Details of Existing Agif Loans:")
                                            .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                            .SetFontSize(14)
                                            .SetBold().SetMarginTop(10);

                        document.Add(point14);
                        AddRow2("S/No & Type of Loan", "Date of Loan taken", " Duration of loan", "Amount Taken");
                    }
                    else
                    counter= 24;


                    if (common.House_Building_Advance_Loan)
                    {
                        AddRow2("House Building Advance", common.House_Building_Date_of_Loan_taken?.ToString("dd-MM-yyyy"),common.House_Building_Duration_of_Loan.ToString() , common.House_Building_Amount_Taken.ToString());
                    }
                    if (common.House_Repair_Advance_Loan)
                    {
                        AddRow2("House Repair Advance", common.House_Repair_Advance_Date_of_Loan_taken?.ToString("dd-MM-yyyy"), common.House_Repair_Advance_Duration_of_Loan.ToString(), common.House_Repair_Advance_Amount_Taken.ToString());
                    }
                    if (common.Computer_Advance_Loan)
                    {
                        AddRow2("Computer Advance", common.Computer_Date_of_Loan_taken?.ToString("dd-MM-yyyy"), common.Computer_Duration_of_Loan.ToString(), common.Computer_Amount_Taken.ToString());
                    }
                    if (common.Conveyance_Advance_Loan)
                    {
                        AddRow2("Conveyance Advance", common.Conveyance_Date_of_Loan_taken?.ToString("dd-MM-yyyy"), common.Conveyance_Duration_of_Loan.ToString(), common.Conveyance_Amount_Taken.ToString());
                    }

                    document.Add(table2);


                    document.Add(new Paragraph("PART 2")
                       .SetFont(boldFont)
                       .SetFontSize(12).SetMarginTop(10)
                       .SetTextAlignment(TextAlignment.CENTER)
                       .SetMarginBottom(5)
                       .SetUnderline());

                    if(data.EducationDetailsResponse!=null)
                    {
                        var EducationDetailsDTO = data.EducationDetailsResponse;

                        Paragraph point15 = new Paragraph($"{++counter}. For Education of Child(Applicable for children studying in 12th Class and above")
                                            .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                            .SetFontSize(14)
                                            .SetBold().SetMarginTop(10);

                        document.Add(point15);


                        Table table3 = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();
                        // Helper method
                        void AddRow3(string label1, string value1, string label2, string value2, bool isFirstRow = false)
                        {
                            Border topBorder = isFirstRow ? new SolidBorder(1) : Border.NO_BORDER;
                            Border bottomBorder = new SolidBorder(1); // Always apply bottom border
                            table3.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table3.AddCell(new Cell().Add(new Paragraph(value1 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table3.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table3.AddCell(new Cell().Add(new Paragraph(value2 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                        }

                        AddRow3("(a) Name of Child", EducationDetailsDTO.ChildName, "(b) Date of Birth", EducationDetailsDTO.DateOfBirth?.ToString("dd-MM-yyyy"));
                        AddRow3("(b) DO Part-II No", EducationDetailsDTO.DOPartIINo, "(d) DO Part-II Date ", EducationDetailsDTO.DoPartIIDate?.ToString("dd-MM-yyyy"));
                        AddRow3("(e) Course ", EducationDetailsDTO.CourseForWithdrawal, "(f)Name & Address of College", EducationDetailsDTO.CollegeInstitution);
                        AddRow3("(h) Total Expenditure ", EducationDetailsDTO.TotalExpenditure.ToString(),"","");

                        document.Add(table3);

                    }

                    else if (data.MarraigeWardResponse != null)
                    {
                        var MarraigeWardDTO = data.MarraigeWardResponse;

                        Paragraph point16 = new Paragraph($"{++counter}. For Marriage of Ward")
                            .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                            .SetFontSize(14)
                            .SetBold().SetMarginTop(10);

                        document.Add(point16);

                        Table table4 = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();
                        // Helper method
                        void AddRow4(string label1, string value1, string label2, string value2, bool isFirstRow = false)
                        {
                            Border topBorder = isFirstRow ? new SolidBorder(1) : Border.NO_BORDER;
                            Border bottomBorder = new SolidBorder(1); // Always apply bottom border
                            table4.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table4.AddCell(new Cell().Add(new Paragraph(value1 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table4.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table4.AddCell(new Cell().Add(new Paragraph(value2 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                        }

                        AddRow4("(a) Name of Ward", MarraigeWardDTO.NameOfChild, "(b) Date of Birth", MarraigeWardDTO.DateOfBirth?.ToString("dd-MM-yyyy"));
                        AddRow4("(c) DO Part-II No", MarraigeWardDTO.DOPartIINo, "(d) Do Part II Date", MarraigeWardDTO.DoPartIIDate?.ToString("dd-MM-yyyy"));
                        AddRow4("(e) Age of Ward", MarraigeWardDTO.AgeOfWard.ToString(), "(f) Date of Marriage", MarraigeWardDTO.DateofMarriage?.ToString("dd-MM-yyyy"));

                        document.Add(table4);
                    }

                    else if (data.PropertyRenovationResponse != null)
                    {
                        var PropertyRenovationDTO = data.PropertyRenovationResponse;

                        Paragraph point17 = new Paragraph($"{++counter}. For Renovation/Repair of House")
                            .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                            .SetFontSize(14)
                            .SetBold().SetMarginTop(10);

                        document.Add(point17);

                        Table table5 = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();
                        // Helper method
                        void AddRow5(string label1, string value1, string label2, string value2, bool isFirstRow = false)
                        {
                            Border topBorder = isFirstRow ? new SolidBorder(1) : Border.NO_BORDER;
                            Border bottomBorder = new SolidBorder(1); // Always apply bottom border
                            table5.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table5.AddCell(new Cell().Add(new Paragraph(value1 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table5.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table5.AddCell(new Cell().Add(new Paragraph(value2 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                        }

                        AddRow5("(a) Address of property", PropertyRenovationDTO.AddressOfProperty, "(b)  Name of property holder(s)", PropertyRenovationDTO.PropertyHolderName);
                        AddRow5("(e) Estimated cost of expenditure", PropertyRenovationDTO.EstimatedCost.ToString(), "", "");

                        document.Add(table5);
                    }

                    else if (data.SplWaiverResponse != null)
                    {
                        var SplWaiverResponseDTO = data.SplWaiverResponse;

                        Paragraph point17 = new Paragraph($"{++counter}. For Special Waiver")
                            .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                            .SetFontSize(14)
                            .SetBold().SetMarginTop(10);

                        document.Add(point17);

                        Table table5 = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();
                        // Helper method
                        void AddRow5(string label1, string value1, string label2, string value2, bool isFirstRow = false)
                        {
                            Border topBorder = isFirstRow ? new SolidBorder(1) : Border.NO_BORDER;
                            Border bottomBorder = new SolidBorder(1); // Always apply bottom border
                            table5.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table5.AddCell(new Cell().Add(new Paragraph(value1 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table5.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                            table5.AddCell(new Cell().Add(new Paragraph(value2 ?? "").SetFont(normalFont).SetFontSize(smallFontSize))
                                .SetBorderTop(topBorder)
                                .SetBorderBottom(new SolidBorder(1))
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER));
                        }

                        AddRow5("(a) Other Reason", SplWaiverResponseDTO.OtherReasons, "","");
                        

                        document.Add(table5);
                    }

                    Paragraph point18 = new Paragraph($"{++counter}. Certificate")
                                           .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                           .SetFontSize(14)
                                           .SetBold().SetMarginTop(10);
                    document.Add(point18);
               
                            List<string> checkPara = new List<string>
                            {
                            "(a) Certified that the particulars given are correct & the amount will be utilized for the purpose mentioned in application",
                            "(b) I understand that , if I withdraw money from maturity amount , it will reduce my ultimate saving amount receivable at the time of retirement/release/discharge as Member Maturity Fund.",
                            "(c) Following documents signed by me are attached with the application (Strike out whichever is not applicable):-"
                            };
        
                            for (int i = 0; i < checkPara.Count; i++)
                            {
                                var para = new Paragraph(checkPara[i])
                                    .SetFont(normalFont)
                                    .SetFontSize(10)
                                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                                    .SetMarginLeft(20)
                                    .SetMarginBottom(3);
                                //if (i == checkPara.Count - 1)
                                //    para.SetBold();
                                document.Add(para);
                            }
        

                    
                            List<string> documentsRequired = new List<string>
                            {
                            "(i) Attested copy of Birth Pt II Order of child(In case of edn/marriage of child).",
                            "(ii) Attested copy of Fee details of child (For Edn of Child) attested by OC unit",
                            "(iii) Cancelled cheque or first page of passbook duly autheticated by bank.",
                            "(iv) Latest Pay Slip.",
                            "(v) Marriage invitation Card(In case of marriage of child) duly attested by OC unit",
                            "(vi) Estimate of cost of expdr(For renovation of House in the last two years of service).",
                            "(vii) Personal application & sp docus(for seeking of spl waiver)."
                            };
        
                            foreach (var point in documentsRequired)
                            {
                                document.Add(new Paragraph(point)
                                    .SetFont(normalFont)
                                    .SetFontSize(10)
                                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                                    .SetMarginLeft(30)
                                    .SetMarginBottom(3));
                            }

                    // Add part (d) and (e) after documentsRequired
                    List<string> checkParaDE = new List<string>
                    {
                        "(d) If the withdrawal is for second time, gaps after first withdrawal should be more than six months.",                        
                    };

                    foreach (var paraText in checkParaDE)
                    {
                        var para = new Paragraph(paraText)
                            .SetFont(normalFont)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.JUSTIFIED)
                            .SetMarginLeft(20)
                            .SetMarginBottom(3);
                        document.Add(para);
                    }

                    var parae = new Paragraph("(e) I hearby declare that i have paid all my AGI subscriptions to AGIF on time upto the present date.")
                        .SetFont(normalFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.JUSTIFIED)
                        .SetMarginLeft(20)
                        .SetMarginBottom(3)
                        .SetBold()
                        .SetFontColor(ColorConstants.BLACK); // Bold with light font weight (normal color)
                    document.Add(parae);

                    // Verified by and Date Time
                    string domainInfo = $"Verified by - {Name}   IP Address – {IpAddress} Date Time  – {DateTime.Now:dd-MM-yyyy HH:mm}";
                            document.Add(new Paragraph(domainInfo)
                                .SetFont(normalFont)
                                .SetFontSize(12)
                                .SetFontColor(ColorConstants.BLUE)
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(10));
                            document.Add(new Paragraph("\n"));
        
                            // Date & Signature Alignment using Table
                            Table signatureTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 })).UseAllAvailableWidth();
        
                            // Left: Date
                            signatureTable.AddCell(new Cell().Add(new Paragraph("Date: " + DateTime.Now.ToString("dd-MM-yyyy"))
                                .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));
        
                            // Right: Signature (Army No)
                            signatureTable.AddCell(new Cell().Add(new Paragraph(common.Number ?? "")
                                .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.RIGHT));
        
                            // Left: Empty
                            signatureTable.AddCell(new Cell().Add(new Paragraph("")
                                .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));
        
                            // Right: Signature (Rank + Name)
                            signatureTable.AddCell(new Cell().Add(new Paragraph((common.DdlRank?.ToString() ?? "") + " " + (common.ApplicantName ?? ""))
                                .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.RIGHT));
        
                            document.Add(signatureTable);



                    if (isApproved)
                    {
                        document.Add(new Paragraph("This is an electronially generated PDF")
                                 .SetFont(boldFont).SetFontColor(ColorConstants.BLUE).SetFontSize(10).SetMarginTop(10).SetMarginBottom(5));

                        document.Add(new Paragraph("RECOMMENDATIONS AND COUNTERSIGNATURE")
                            .SetFont(boldFont).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER).SetUnderline().SetMarginTop(10).SetMarginBottom(5));

                        Paragraph para = new Paragraph();
                        para.Add(new Text("1.  I certify that above MAWD application ").SetFont(normalFont));
                        para.Add(new Text("has been submitted by No ").SetFont(normalFont));
                        para.Add(new Text(common.Number).SetFont(boldFont));
                        para.Add(new Text(" Rank ").SetFont(normalFont));
                        para.Add(new Text(common.DdlRank).SetFont(boldFont));
                        para.Add(new Text(" Name ").SetFont(normalFont));
                        para.Add(new Text(common.ApplicantName).SetFont(normalFont));
                        para.Add(new Text(" of my Unit ").SetFont(normalFont));
                        para.Add(new Text(common.PresentUnit).SetFont(boldFont));
                        para.Add(new Text(". I identify his signature on supporting documents as attested by him and certify them to be correct.").SetFont(normalFont));
                        document.Add(para);

                        Paragraph para2 = new Paragraph();
                        para2.Add(new Text("2. It's certified that I am the CO/OC Tps of No ").SetFont(normalFont));
                        para2.Add(new Text(common.Number).SetFont(boldFont));
                        para2.Add(new Text(" Rank ").SetFont(normalFont));
                        para2.Add(new Text(common.DdlRank).SetFont(boldFont));
                        para2.Add(new Text(" Name ").SetFont(normalFont));
                        para2.Add(new Text(common.ApplicantName).SetFont(normalFont));
                        para2.Add(new Text(". and I am authorised to countersign financial documents of this individual.").SetFont(normalFont));
                        document.Add(para2);


                        Paragraph para3 = new Paragraph();
                        para3.Add(new Text("3.I have interviewed him and verified his financial condition and established need for taking this MAWD. Applicant will be using MAWD amount for intended purpose only.").SetFont(normalFont));
                        document.Add(para3);

                        Paragraph para4 = new Paragraph();
                        para4.Add(new Text("4. It is certified that Bank A/c No ").SetFont(normalFont));
                        para4.Add(new Text(common.SalaryAcctNo).SetFont(boldFont));
                        para4.Add(new Text(" of Bank ").SetFont(normalFont));
                        para4.Add(new Text(common.NameOfBank).SetFont(boldFont));
                        para4.Add(new Text(" with IFSC ").SetFont(normalFont));
                        para4.Add(new Text(common.IfsCode).SetFont(boldFont));
                        para4.Add(new Text(" as given in the application and cancelled cheque is of Salary account of ").SetFont(normalFont));
                        para4.Add(new Text(common.Number + " " + common.DdlRank + " " + common.ApplicantName).SetFont(boldFont));
                        document.Add(para4);

                        Paragraph para5 = new Paragraph("5. I have satisfied myself of the correctness of personal details given in application. I have perused the supporting documents and checked their correctness. Supporting documents uploaded are readable and latest.")
                            .SetFont(normalFont);
                        document.Add(para5);

                        Paragraph para6 = new Paragraph();
                        para6 = new Paragraph("Application is recommended for sanction and accordingly I countersign the same.")
                            .SetFont(normalFont)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.JUSTIFIED)
                            .SetMarginBottom(5f);


                        document.Add(para6);

                    }

                    if (isApproved || isRejected)
                    {
                        // Replace the selected block with the following (iTextSharp code removed, iText7 style used, variable names fixed)

                        if (isApproved || isRejected)
                        {
                            if (isRejected)
                            {
                                document.Add(new Paragraph("\n\r\n\r"));
                            }

                            // Signature and Rank details table (iText7)
                            Table signatureTable3 = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 })).UseAllAvailableWidth();

                            // Left: Unit Stamp (empty)
                            signatureTable3.AddCell(new Cell().Add(new Paragraph(""))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            // Right: Signature of CO with Stamp
                            signatureTable3.AddCell(new Cell().Add(new Paragraph("Digital Signature of CO/OC Tps")
                                .SetFont(normalFont).SetFontSize(11))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.RIGHT));

                            // Left: Date (empty)
                            signatureTable3.AddCell(new Cell().Add(new Paragraph(""))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            // Right: Army No
                            signatureTable3.AddCell(new Cell().Add(new Paragraph($"{common.Number ?? ""}")
                                .SetFont(normalFont).SetFontSize(11))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.RIGHT));

                            // Left: Empty
                            signatureTable3.AddCell(new Cell().Add(new Paragraph(""))
                                .SetBorder(Border.NO_BORDER));

                            // Right: Rank & Name
                            signatureTable3.AddCell(new Cell().Add(new Paragraph($"{common.DdlRank} {common.ApplicantName ?? ""}")
                                .SetFont(normalFont).SetFontSize(11))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.RIGHT));

                            // Left: Empty
                            signatureTable3.AddCell(new Cell().Add(new Paragraph(""))
                                .SetBorder(Border.NO_BORDER));

                            // Right: Mobile No
                            signatureTable3.AddCell(new Cell().Add(new Paragraph($"Mobile No: {common.MobileNo ?? ""}")
                                .SetFont(normalFont).SetFontSize(11))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.RIGHT));

                            // Left: Empty
                            signatureTable3.AddCell(new Cell().Add(new Paragraph(""))
                                .SetBorder(Border.NO_BORDER));

                            // Right: Digital Sign On
                            signatureTable3.AddCell(new Cell().Add(new Paragraph($"Digital Sign On: {dated}")
                                .SetFont(normalFont).SetFontSize(11))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.RIGHT));

                            document.Add(signatureTable3);

                            if (isApproved)
                            {
                                document.Add(new Paragraph("\n"));
                                document.Add(new Paragraph("\n"));

                                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Icon", "DigitalSign.png");
                                if (File.Exists(imagePath))
                                {
                                    //var imgData = ImageDataFactory.Create(imagePath);
                                    //var icon = new iText.Layout.Element.Image(imgData);
                                    //icon.ScaleAbsolute(60f, 60f);
                                    //icon.SetFixedPosition(pdf.GetNumberOfPages(), 480, 270);
                                    //document.Add(icon);

                                    ImageData imageData = ImageDataFactory.Create(imagePath);
                                    Image icon = new Image(imageData).ScaleToFit(60f, 60f);
                                    icon.SetFixedPosition(pdf.GetNumberOfPages(), 480, 270);
                                    document.Add(icon);
                                }
                            }
                            if (isRejected)
                            {
                                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Icon", "RejectedIcon.png");
                                if (File.Exists(imagePath))
                                {
                                    //var imgData = ImageDataFactory.Create(imagePath);
                                    //var icon = new iText.Layout.Element.Image(imgData);
                                    //icon.ScaleAbsolute(80f, 80f);
                                    //icon.SetFixedPosition(pdf.GetNumberOfPages(), 480, 557);
                                    //document.Add(icon);


                                    ImageData imageData = ImageDataFactory.Create(imagePath);
                                    Image icon = new Image(imageData).ScaleToFit(80f, 80f);
                                    icon.SetFixedPosition(pdf.GetNumberOfPages(), 480, 557);
                                    document.Add(icon);
                                }
                            }
                        }
                       
                    }




                    pdf.Close();
                    writer.Close();

                    document.Close();
                }
           

                return 1;
            }
        }
    }
}
