using DataAccessLayer.Interfaces;
using DocumentFormat.OpenXml.Office2013.Excel;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        public async Task<int> CreatePdfForOnlineApplication(int applicationId, string generatedPdfPath, bool isRejected, bool isApproved, string UserName, string IpAddress, string Name, string Mobile, string armyno)
        {
            var data = await _usersApplications.GetApplicationDetails(applicationId);
            var directory = Path.GetDirectoryName(generatedPdfPath);
            var dated = DateTime.Now.ToString("dd-MM-yyyy HH:mm");

            string formType = string.Empty;
            string formTypeName = string.Empty;


            // First, determine the form type
            if (data.CarApplicationResponse != null)
            {
                formType = "CAR / TWO WHEELER ADVANCE";
                if (data.CarApplicationResponse.Veh_Loan_Type == "Two Wheeler")
                {
                    formTypeName = "Two Wheeler";
                }
                else
                {
                    formTypeName = "Car";
                }

            }
            else if (data.PcaApplicationResponse != null)
            {
                formType = "PERSONAL COMPUTER LOAN";
                formTypeName = "PCA";
            }
            else if (data.HbaApplicationResponse != null)
            {
                formType = "HOUSE BUILDING ADVANCE";
                formTypeName = "HBA";
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
                    var document = new Document(pdf, pageSize); // Use PageSize instead of Rectangle
                    document.SetMargins(30f, 25f, 40f, 25f);

                    //Define Fonts
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

                    document.Add(new Paragraph($"APPLICATION FORM :{formType}")
                        .SetFont(boldFont)
                        .SetFontSize(12).SetMarginTop(0)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(20)
                        .SetUnderline());

                    Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();
                    // Helper method
                    void AddRow(string label1, string value1, string label2, string value2, bool isFirstRow = false)
                    {
                        Border topBorder = isFirstRow ? new SolidBorder(1) : Border.NO_BORDER;
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
                    AddRow("1. Type of Loan", formTypeName, "2. Army Number", common.Number);
                    AddRow("3.Old Army No", common.OldNumber, "4.Rank", common.DdlRank);
                    AddRow("5. Name", common.ApplicantName, "6. Date of Birth", common.DateOfBirth?.ToString("dd-MM-yyyy"));
                    AddRow("7. DOE/DOC", common.DateOfCommission?.ToString("dd-MM-yyyy"), "8.Date of Retirement", common.DateOfRetirement?.ToString("dd-MM-yyyy"));
                    AddRow("9. Mobile No", common.MobileNo, "10. Email ID", common.Email);
                    AddRow("11. Regt/Corps", common.RegtCorps, "12. PCDA(O)/PAO(OR)", common.pcda_pao);
                    AddRow("13.PCDA(O) Acct No", common.pcda_AcctNo, "14. Pan Card", common.PanCardNo);
                    AddRow("15. Aadhaar Card No", common.AadharCardNo, "16. Parent Unit", common.ParentUnit);
                    AddRow("17. Present Unit", common.PresentUnit, "18.Unit Pin", common.PresentUnitPin);
                    AddRow("19. Unit Address", common.ArmyPostOffice, "20. Civil Postal Address", common.CivilPostalAddress);
                    AddRow("21. Fmn HQ", common.NextFmnHQ, "22. Date of Prom", common.DateOfPromotion?.ToString("dd-MM-yyyy"));

                    table.AddCell(new Cell(1, 1).Add(new Paragraph("23. Permt Home Address").SetFont(normalFont).SetFontSize(smallFontSize))
                         .SetBorderTop(new SolidBorder(1))
                         .SetBorderBottom(new SolidBorder(1))
                         .SetBorderLeft(Border.NO_BORDER)
                         .SetBorderRight(Border.NO_BORDER)); // This is the heading cell

                    // The address will span 3 columns
                    table.AddCell(new Cell(1, 3).Add(new Paragraph((common.Vill_Town ?? "") + ", " + (common.PostOffice ?? "") + ", " +
                        (common.Distt ?? "") + ", " + (common.State ?? "") + ", " + (common.Code ?? ""))
                        .SetFont(normalFont)
                        .SetFontSize(smallFontSize))
                        .SetBorderTop(new SolidBorder(1))
                        .SetBorderBottom(new SolidBorder(1))
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER));

                    document.Add(table);

                    if (data.HbaApplicationResponse != null)
                    {
                        var hba = data.HbaApplicationResponse;
                        Table hbaTable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();

                        void AddLoanRow(string label1, string val1, string label2, string val2)
                        {
                            hbaTable.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            hbaTable.AddCell(new Cell().Add(new Paragraph(val1 ?? "").SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            hbaTable.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            hbaTable.AddCell(new Cell().Add(new Paragraph(val2 ?? "").SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                        }

                        AddLoanRow("24. Property Address", hba.PropertyAddress, "25.Property Type", hba.PropertyType);

                        AddLoanRow("26. Estimated Cost", hba.PropertyCost.ToString(), "27. Loan Amt Reqd", hba.HBA_Amount_Applied_For_Loan.ToString());


                        AddLoanRow("28. No of EMI (In Months)", hba.HBA_LoanFreq.ToString(), "29.Salary Acct No", common.SalaryAcctNo.ToString());


                        AddLoanRow("30. Bank IFSC Code", common.IfsCode, "", "");



                        document.Add(hbaTable);
                        PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                        Paragraph para28 = new Paragraph("31.  I have read the Terms & Conditions, Instructions and Rules regulating the grant of House Building Advance to AGIF members and agree to abide by them. I agree to pay the one time non-refundable insurance premium. I shall refund in one lump sum the full loan amount together with interest outstanding, in case, I wish to sell the house/flat or transfer the house/flat by way of gift deed.")
                        .SetFont(regularFont)
                        .SetFontSize(10)
                        .SetMarginTop(10).SetTextAlignment(TextAlignment.JUSTIFIED)
                        .SetMarginBottom(5);
                        document.Add(para28);
                        //document.Add(new Paragraph("\n"));

                        // Section 29
                        Paragraph para29 = new Paragraph("32. I state and certify that:-")
                            .SetFont(regularFont)
                            .SetFontSize(10)
                            .SetMarginTop(4)
                            .SetMarginBottom(5);
                        document.Add(para29);

                        List<string> checkPara = new List<string> {
                     "(a) If, at any time, it is found that I have obtained loan from AGIF by misrepresentation, fraud,  misstatement or have committed a breach of the terms & conditions issued from time to time by AGIF, I shall without prejudice, be liable to legal, disciplinary or any other action and will become liable to repay in one lump sum the full loan amount together with interest outstanding, for period of default at a rate 2 % higher than the rate of interest at which the loan was sanctioned, to AGIF without any demur.",
                     "(b) I will maintain adequate credit in my IRLA to meet EMI payment during the duration and till complete repayment of loan. In case of any debit balance resulting in non-remittance of EMI by the CDA (O) / PAO (OR) to AGIF, I undertake to pay it directly along with interest for period of default at a rate 2 % higher than the rate of interest at which the loan was sanctioned, as levied by the AGIF for the delayed period.",
                     "(c) In the event of my becoming non-effective due to any reason like retirement / dismissal / premature retirement / death preceding retirement etc, the AGIF shall be entitled to recover through the PAO (OR) / CDA(O) and/or receive the balance of the loan with interest remaining unpaid and any other dues from the whole or any specified part of the gratuity, Insurance / Disability / Maturity / survival benefit of AGIF, death benefits,AFPP Fund, DSOP Fund, DCRG, Commuted value of pension, leave encashment, service pension, that may be payable to me, without any demur from any quarter.",
                     "(d) I hereby voluntarily authorize CDA(O) / PAO(OR) to deduct EMI on account of HBA taken from the AGIF from my pay and allowances on a monthly basis and remit the same to AGIF on my behalf. I hereby assign and transfer upto the AGIF the House by way of security for the said loan and the interest thereon. ",
                     "(e) I will inform AGIF about any change in my present as well as permanent address/change in employement/release/disacharge/premature retirement and change in mobile number/Email.",
                     "(f) I will allow any person/agency authorised by AGIF to have free access to the property for the purpose of inspecting the progress of construction and the accounts of consturuction to ensure utilisation of the AGIF loan.",
                     "(g) I will depost the Title-Deed/Sale Deed/Conveyance Deed/Gift Deed/Partition Deed/Settlement Deed/Relinquish Deed/Transfer Deed with AGIF within 60 days from date of purchase(Date of possession in case of construction linked plan) of house/Flat being purchased by me with an intent to create an equitable mortgage. In case, if I fail to deposit title deed by date mentioned above. I will be liable to pay additional 2% interest.",
                     "(h) I understand and confirm that equitable mortage deed(EMD) will be created over the said property in favour of AGIF as security for the due repayment of all advances by AGIF to me in the loan account and for all my indebtedness and liablities whatsover to AGIF together with interest, costs, charges and expeneses thereon.I hereby agress to execute at my own costs in favour of the AGIF whenever requested by the AGIF to do, a registred mortgage over the said property in such form and with such powers of sale etc, as the AGIF may require for securing the above loan accounts.",
                     "(j) In case of any dispute arising with regards to the rules, agreements and deeds executed there under, I am bound to the jurisdiction of Delhi Courts only.",



                        };

                        foreach (var point in checkPara)
                        {
                            Paragraph p = new Paragraph(point)
                                .SetFont(regularFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(3)
                                .SetMarginLeft(20);
                            document.Add(p);
                        }

                        // Section 30
                        Paragraph para30 = new Paragraph("33.I, solemnly declare that the details/information furnished by me and averments/certifications made herein are true to the best of my knowledge and belief and have not willfully suppressed any material information. ")
                            .SetFont(regularFont)
                            .SetFontSize(10)
                            .SetMarginTop(10)
                            .SetMarginBottom(5);
                        document.Add(para30);
                        document.Add(new Paragraph("\n"));

                        string domainInfo = $"Verified by - {UserName}   IP Address – {IpAddress}   Date Time  – {DateTime.Now:dd-MM-yyyy hh:mm tt}";
                        document.Add(new Paragraph(domainInfo)
                            .SetFont(normalFont)
                            .SetFontColor(ColorConstants.BLUE)
                            .SetFontSize(12)
                            .SetTextAlignment(TextAlignment.JUSTIFIED)
                            .SetMarginBottom(10));

                        Table signatureTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();
                        signatureTable.AddCell(new Cell().Add(new Paragraph(common.Number))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.LEFT)
                            .SetFont(regularFont)
                            .SetFontSize(10));
                        signatureTable.AddCell(new Cell().SetBorder(Border.NO_BORDER)); // Empty left cell
                        signatureTable.AddCell(new Cell().Add(new Paragraph(common.DdlRank + " " + common.ApplicantName))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.LEFT)
                            .SetFont(regularFont)
                            .SetFontSize(10));
                        document.Add(signatureTable);

                        if (isApproved)
                        {
                            document.Add(new Paragraph("This is an electronically generated PDF")
                                .SetFont(boldFont)
                                .SetFontSize(10)
                                .SetFontColor(ColorConstants.BLUE)
                                .SetMarginTop(10)
                                .SetMarginBottom(5)
                                .SetTextAlignment(TextAlignment.LEFT));

                            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));


                            document.Add(new Paragraph("RECOMMENDATIONS AND COUNTERSIGNATURE")
                                .SetFont(boldFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetUnderline()
                                .SetMarginTop(10)
                                .SetMarginBottom(5));

                            // Paragraph 1
                            Paragraph r1 = new Paragraph()
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(3)
                                .SetMarginLeft(20)
                                .SetFont(regularFont)
                                .SetFontSize(10);

                            r1.Add(new Text("1. I certify that above ").SetFont(regularFont));
                            r1.Add(new Text(formType).SetFont(boldFont));
                            r1.Add(new Text(" Application has been submitted by ").SetFont(regularFont));
                            r1.Add(new Text(common.Number).SetFont(boldFont));
                            r1.Add(new Text(" ").SetFont(regularFont));
                            r1.Add(new Text(common.DdlRank).SetFont(boldFont));
                            r1.Add(new Text(" ").SetFont(regularFont));
                            r1.Add(new Text(common.ApplicantName).SetFont(boldFont));
                            r1.Add(new Text(" of my Unit ").SetFont(regularFont));
                            r1.Add(new Text(common.PresentUnit).SetFont(boldFont));
                            r1.Add(new Text(". I identify his signature documents as attested by him and certify them to be correct.")
                                .SetFont(regularFont));
                            document.Add(r1);

                            // Paragraphs 2–5
                            string[] paraTexts =
                            {
        $"2. It's certified that I am the CO/OC Tps of {common.Number} {common.DdlRank} {common.ApplicantName} and I am authorised to countersign financial documents of this individual.",
        "3. I have interviewed him and verified his financial condition and established need for taking this loan. Applicant will be using loan amount for intended purpose only.",
        $"4. It is certified that Bank A/c No {common.SalaryAcctNo} of Bank ({common.NameOfBank}) with IFSC {common.IfsCode} as given in the application...",
        "5. I have satisfied myself of the correctness of personal details given in application. I have perused the supporting documents and checked their correctness. Supporting documents uploaded are readable and latest.",
        "     Application is recommended for sanction and accordingly I countersign the same."
    };

                            foreach (var point in paraTexts)
                            {
                                Paragraph p = new Paragraph(point)
                                    .SetFont(regularFont)
                                    .SetFontSize(10)
                                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                                    .SetMarginBottom(3)
                                    .SetMarginLeft(20);
                                document.Add(p);
                            }

                            document.Add(new Paragraph("\n"));
                        }

                        if (isApproved || isRejected)
                        {
                                document.Add(new Paragraph("\n\n").SetMarginBottom(35));

                            Table signatureTable2 = new Table(UnitValue.CreatePercentArray(new float[] { 100 }))
                                .UseAllAvailableWidth();

                            signatureTable2.AddCell(new Cell().Add(new Paragraph("Digital Signature of CO")
                                    .SetFont(boldFont)
                                    .SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            signatureTable2.AddCell(new Cell().Add(new Paragraph(armyno)
                                    .SetFont(regularFont)
                                    .SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            signatureTable2.AddCell(new Cell().Add(new Paragraph(Name)
                                    .SetFont(regularFont)
                                    .SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            signatureTable2.AddCell(new Cell().Add(new Paragraph("Mobile No: " + Mobile)
                                    .SetFont(regularFont)
                                    .SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            signatureTable2.AddCell(new Cell().Add(new Paragraph("Digital Sign On: " + dated)
                                    .SetFont(regularFont)
                                    .SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            document.Add(signatureTable2);
                        }


                        if (isApproved)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Icon", "DigitalSign.png");
                            ImageData imageData = ImageDataFactory.Create(imagePath);
                            Image icon = new Image(imageData).ScaleToFit(60f, 60f);
                            icon.SetFixedPosition(pdf.GetNumberOfPages(), 30, 470);
                            document.Add(icon);
                        }
                        if (isRejected)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Icon", "RejectedIcon.png");
                            ImageData imageData = ImageDataFactory.Create(imagePath);
                            Image icon = new Image(imageData).ScaleToFit(80f, 80f);
                            icon.SetFixedPosition(pdf.GetNumberOfPages(), 30, 270);
                            document.Add(icon);
                        }


                    }

                    else if (data.CarApplicationResponse != null)
                    {
                        var car = data.CarApplicationResponse;
                        Table cartable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();


                        void AddLoanRow(string label1, string val1, string label2, string val2)
                        {
                            cartable.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            cartable.AddCell(new Cell().Add(new Paragraph(val1 ?? "").SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            cartable.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            cartable.AddCell(new Cell().Add(new Paragraph(val2 ?? "").SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                        }


                        AddLoanRow("22. Dealer Name", car.DealerName, "23. Model Name", car.ModelName);
                        AddLoanRow("24.Vehicle Cost", car.VehicleCost.ToString(), "25. Vehicle Type", car.Veh_Loan_Type.ToString());
                        AddLoanRow("26. Loan Amt Reqd:", car.CA_Amount_Applied_For_Loan.ToString(), "27. Loan Frequency", car.CA_LoanFreq.ToString());
                        AddLoanRow("28.Salary Acct No", common.SalaryAcctNo.ToString(), "29. Bank IFSC Code", common.IfsCode);
                        document.Add(cartable);


                        var normalFontforpara29 = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                        var boldFontforpara29 = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                        // Section 29 title
                        var titleParagraph = new Paragraph("30. I state and certify that:")
                            .SetFont(normalFontforpara29)
                            .SetFontSize(10)
                            .SetMarginTop(10)
                            .SetMarginBottom(5);
                        document.Add(titleParagraph);

                        // List of bullet points
                        var points = new List<string>
                        {
                            "(a) I have read the instr / rules regulating the grant of loan to AGIF members for purchase of four / two wheeler and agree to abide by the terms and conditions stipulated therein from time to time. I agree to pay the one-time non-refundable insurance premium and abide by its terms and conditions.",
                            "(b) I shall refund in one lump sum the full loan together with interest outstanding thereon, in case, I wish to sell the vehicle. The vehicle will be kept comprehensively insured from the date of its purchase till the loan together with interest, is fully repaid to AGIF. I will not use the vehicle for commercial purpose.",
                            "(c) Attested copy of "
                        };

                        foreach (var point in points)
                        {
                            var bulletPoint = new Paragraph().SetFont(normalFontforpara29).SetFontSize(10).SetMarginBottom(3).SetTextAlignment(TextAlignment.JUSTIFIED).SetMarginLeft(20);
                            bulletPoint.Add(point);

                            if (point.StartsWith("(c)"))
                            {
                                bulletPoint.Add(new Text("RC, Insurance, Bill / Invoice and Cash Receipt from Dealer").SetFont(boldFontforpara29));
                                bulletPoint.Add(new Text(" will be submitted within 180 days of sanction of loan. I shall refund the loan in one lump-sum with interest in the event of my failure to purchase the vehicle or to produce relevant documents of purchase within the stipulated time "));
                                bulletPoint.Add(new Text("180 days").SetFont(boldFontforpara29));
                                bulletPoint.Add(new Text(", failing which I authorise MD, AGIF to effect recovery of the entire outstanding amount together with additional interest of 2% for default period, through my IRLA."));
                            }
                            document.Add(bulletPoint);
                        }


                        // Section 30 title
                        var titleParagraph30 = new Paragraph("31. I further agree that:")
                            .SetFont(normalFontforpara29)
                            .SetFontSize(10)
                            .SetMarginTop(10)
                            .SetMarginBottom(5);
                        document.Add(titleParagraph30);

                        var points30 = new List<string>
                            {
                                "(a) If, at any time, it is found that I have obtained Car / Two Wheeler Loan from AGIF by misrepresentation, misstatement or fraud.",
                                "(b) I will ensure that I will maintain adequate credit in my IRLA during the duration of repayment of complete loan instalments.",
                                "(c) I will repay the loan amount with interest by monthly deductions from my salary.",
                                "(d) In case the vehicle for which loan has been obtained meets with an accident.",
                                "(e) In the event of my becoming non-effective / retirement / dismissal / premature retirement.",
                                "(f) I will pay the sum of loan amount or the balance remaining unpaid."
                            };

                        foreach (var point in points30)
                        {
                            var bulletPoint30 = new Paragraph(point)
                                .SetFont(normalFontforpara29)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(3)
                                .SetMarginLeft(20);
                            document.Add(bulletPoint30);
                        }

                        var titleParagraph31 = new Paragraph("32. I, solemnly declare that the details/information furnished by me and averments/certifications made herein are true to the best of my knowledge and belief and have not willfully suppressed any material information.")
                            .SetFont(normalFontforpara29)
                            .SetFontSize(10)
                            .SetMarginTop(10)
                            .SetMarginBottom(5);
                        document.Add(titleParagraph31);


                        string domainInfo = $"Verified by - {UserName} IP Address – {IpAddress} Date Time  – {DateTime.Now:dd-MM-yyyy hh:mm tt}";
                        document.Add(new Paragraph(domainInfo)
                            .SetFont(normalFont)
                            .SetFontColor(ColorConstants.BLUE)
                            .SetFontSize(12)
                            .SetTextAlignment(TextAlignment.JUSTIFIED)
                            .SetMarginBottom(10));

                        document.Add(new Paragraph("\n"));

                        // === Signature Table (Top) ===
                        Table signatureTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();
                        signatureTable.AddCell(new Cell()
                            .Add(new Paragraph(common.Number).SetFont(normalFont).SetFontSize(10))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.LEFT));

                        signatureTable.AddCell(new Cell()
                            .Add(new Paragraph("").SetFont(normalFont).SetFontSize(10))
                            .SetBorder(Border.NO_BORDER));

                        signatureTable.AddCell(new Cell()
                            .Add(new Paragraph(common.DdlRank + " " + common.ApplicantName)
                                .SetFont(normalFont).SetFontSize(10))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.LEFT));

                        document.Add(signatureTable);


                        // === Main Section ===
                        if (isApproved)
                        {
                            document.Add(new Paragraph("This is an electronically generated PDF")
                                .SetFont(boldFont)
                                .SetFontSize(10)
                                .SetFontColor(DeviceRgb.BLUE)
                                .SetMarginTop(10)
                                .SetMarginBottom(5));

                            // Page Break (Optional)
                            // document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                            document.Add(new Paragraph("RECOMMENDATIONS AND COUNTERSIGNATURE")
                                .SetFont(boldFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetUnderline()
                                .SetMarginTop(10)
                                .SetMarginBottom(5));

                            string strAppType = "Loan"; // Example dynamic value
                            string interviewDate = DateTime.Now.ToString("dd-MM-yyyy");

                            // Helper for consistent paragraphs
                            void AddRecParagraph(string text)
                            {
                                document.Add(new Paragraph(text)
                                    .SetFont(normalFont)
                                    .SetFontSize(10)
                                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                                    .SetMarginBottom(5));
                            }

                            Paragraph p1 = new Paragraph()
                                .Add(new Text("1. I certify that above ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(strAppType + " Application").SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" has been submitted by ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.Number + " ").SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(common.DdlRank + " ").SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(common.ApplicantName).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" of my Unit ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.PresentUnit).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(". I identify his signature on supporting documents as attested by him and certify them to be correct. ")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);

                            document.Add(p1);

                            Paragraph p2 = new Paragraph()
                                .Add(new Text("2. It's certified that I am the CO/OC Tps of ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.Number + " " + common.DdlRank + " " + common.ApplicantName).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" and I am authorised to countersign financial documents of this individual. ")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);

                            document.Add(p2);

                            Paragraph p3 = new Paragraph()
                                .Add(new Text("3. I have interviewed him on ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(interviewDate).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" and verified his financial condition and established need for taking this loan. Applicant will be using loan amount for intended purpose only.")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);

                            document.Add(p3);

                            string accountNo = common.ConfirmSalaryAcctNo ?? "";
                            string bankName = common.NameOfBank ?? "";
                            string branchName = common.NameOfBankBranch ?? "";

                            Paragraph p4 = new Paragraph()
                                .Add(new Text("4.     It is certified that Bank A/c No ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(accountNo).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" of Bank (").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(bankName).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(") with IFSC ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(branchName).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" as given in the application and cancelled cheque is of Salary account of ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.Number + " " + common.DdlRank + " " + common.ApplicantName).SetFont(boldFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);

                            document.Add(p4);

                            AddRecParagraph("5. I have satisfied myself of the correctness of personal details given in application. I have perused the supporting documents and checked their correctness. Supporting documents uploaded are readable and latest.");
                            AddRecParagraph("     Application is recommended for sanction and accordingly I countersign the same.");
                        }


                        // === Signature Section (Bottom) ===
                        if (isApproved || isRejected)
                        {
                            document.Add(new Paragraph("\n\n\n"));

                            Table signatureTable2 = new Table(UnitValue.CreatePercentArray(new float[] { 1 }))
                                .UseAllAvailableWidth();


                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph("Digital Signature of CO").SetFont(boldFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetBorder(Border.NO_BORDER));


                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph(armyno).SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetBorder(Border.NO_BORDER));


                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph(Name).SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetBorder(Border.NO_BORDER));


                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph("Mobile No: " + Mobile).SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetBorder(Border.NO_BORDER));


                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph("Digital Sign On: " + dated).SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetBorder(Border.NO_BORDER));

                            document.Add(signatureTable2);
                        }


                        if (isApproved)
                        {


                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Icon", "DigitalSign.png");
                            ImageData imageData = ImageDataFactory.Create(imagePath);
                            Image icon = new Image(imageData).ScaleToFit(60f, 60f);
                            icon.SetFixedPosition(pdf.GetNumberOfPages(), 30, 270);
                            document.Add(icon);
                        }

                        if (isRejected)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Icon", "RejectedIcon.png");
                            ImageData imageData = ImageDataFactory.Create(imagePath);
                            Image icon = new Image(imageData).ScaleToFit(80f, 80f);
                            icon.SetFixedPosition(pdf.GetNumberOfPages(), 30, 557);
                            document.Add(icon);
                        }


                    }
                    else if (data.PcaApplicationResponse != null)
                    {
                        var pca = data.PcaApplicationResponse;
                        Table pcatable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 4, 3, 4 })).UseAllAvailableWidth();

                        void AddLoanRow(string label1, string val1, string label2, string val2)
                        {
                            pcatable.AddCell(new Cell().Add(new Paragraph(label1).SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            pcatable.AddCell(new Cell().Add(new Paragraph(val1 ?? "").SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            pcatable.AddCell(new Cell().Add(new Paragraph(label2).SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                            pcatable.AddCell(new Cell().Add(new Paragraph(val2 ?? "").SetFont(normalFont)).SetFontSize(smallFontSize).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                        }

                        AddLoanRow("24. Dealer Name", pca.PCA_dealerName, "25. Model Name", pca.PCA_modelName);
                        AddLoanRow("26. Est Cost:", pca.computerCost.ToString(), "27. Loan Amt Reqd:", pca.PCA_Amount_Applied_For_Loan.ToString());
                        AddLoanRow("28. Loan Frequency", pca.PCA_LoanFreq.ToString(), "29.Salary Acct No", common.SalaryAcctNo.ToString());
                        AddLoanRow("30. Bank IFSC Code", common.IfsCode, "", "");

                        document.Add(pcatable);



                        // Section 29 Title
                        document.Add(new Paragraph("31. I state and certify that:")
                            .SetFont(normalFont)
                            .SetFontSize(10)
                            .SetMarginTop(10).SetMarginBottom(5));

                        // Section 29 Bullet Points
                        string[] points = new string[]
                        {
                      "(a) I have read the rules regulating the grant of loan to AGIF members for purchase of personal computer and agree to abide by the terms and conditions stipulated therein from time to time. ",
                     "(b) I will maintain adequate credit in my IRLA during the duration of repayment of complete loan instalments. In case of any debit balance resulting in non-remittance of EMI (instalment) by the CDA (O) / PAO(OR) to AGIF, I undertake to pay it directly alongwith interest for period of default. I shall refund in one lump sum the full loan together with interest outstanding thereon, in case, I wish to sell the computer prior to liquidation of Loan.",
                     "(c) Loan Amount being taken as PCA loan will be used for purchasing of Computers / Laptops only. The said amount will not be used for any other purposes. I will repay the loan amount with interest by monthly deductions from my salary and I hereby authorise the AGIF to make such deductions. ",
                     "(d) In the event of my becoming non-effective / retirement / dismissal / premature retirement and death preceding retirement, the AGIF shall be entitled to recover through the PAO (OR) / CDA(O) and / or receive the balance of the loan with interest remaining unpaid and any other dues from the whole or any specified part of the gratuity that may be sanctioned to me, Insurance / Disability / Maturity / survival benefit of AGIF, death benefits, DSOP Fund, DCRG, Commuted value of pension, leave encashment, Service pension, payable to the Applicant, without any demur from any quarter.  "
                        };

                        foreach (var pt in points)
                        {
                            document.Add(new Paragraph(pt).SetFont(normalFont).SetFontSize(10).SetTextAlignment(TextAlignment.JUSTIFIED).SetMarginBottom(3).SetMarginLeft(20));
                        }

                        // (e) Clause
                        string ePoint = "I will pay the sum of loan amount or the balance remaining unpaid at the date of retirement / dismissal / premature retirement from service by equal payments on the first day of every month and will pay interest on the sum remaining due as calculated according to the rules.  I authorise the PAO (OR) / CDA (O) to deduct the amount as conveyed by AGIF from my salary. ";
                        document.Add(new Paragraph(ePoint).SetFont(normalFont).SetFontSize(10).SetTextAlignment(TextAlignment.JUSTIFIED).SetMarginBottom(3).SetMarginLeft(20));

                        // Section 30
                        string section30 = "32.  I, solemnly declare that the details/information furnished by me and averments/certifications made herein are true to the best of my knowledge and belief and have not willfully suppressed any material information.";
                        document.Add(new Paragraph(section30).SetFont(normalFont).SetFontSize(10).SetMarginTop(10).SetMarginBottom(5));

                        string domainInfo = $"Verified by - {UserName} IP Address – {IpAddress} Date Time  – {DateTime.Now:dd-MM-yyyy hh:mm tt}";
                        document.Add(new Paragraph(domainInfo)
                            .SetFont(normalFont)
                            .SetFontColor(ColorConstants.BLUE)
                            .SetFontSize(12)
                            .SetTextAlignment(TextAlignment.JUSTIFIED)
                            .SetMarginBottom(10));

                        document.Add(new Paragraph("\n"));

                        // === Signature Table (Top) ===
                        Table signatureTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();

                        signatureTable.AddCell(new Cell()
                            .Add(new Paragraph(common.Number)
                                .SetFont(normalFont)
                                .SetFontSize(10))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.LEFT));

                        signatureTable.AddCell(new Cell()
                            .Add(new Paragraph("")
                                .SetFont(normalFont)
                                .SetFontSize(10))
                            .SetBorder(Border.NO_BORDER));

                        signatureTable.AddCell(new Cell()
                            .Add(new Paragraph(common.DdlRank + " " + common.ApplicantName)
                                .SetFont(normalFont)
                                .SetFontSize(10))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.LEFT));

                        document.Add(signatureTable);


                        // === Approval Section ===
                        if (isApproved)
                        {
                            document.Add(new Paragraph("This is an electronically generated PDF")
                                .SetFont(boldFont)
                                .SetFontColor(ColorConstants.BLUE)
                                .SetFontSize(10)
                                .SetMarginTop(10)
                                .SetMarginBottom(5));

                            document.Add(new Paragraph("RECOMMENDATIONS AND COUNTERSIGNATURE")
                                .SetFont(boldFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetUnderline()
                                .SetMarginTop(10)
                                .SetMarginBottom(5));

                            // Paragraph 1
                            Paragraph para1 = new Paragraph()
                                .Add(new Text("1.     I certify that above ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text("PCA").SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" Application has been submitted by ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.Number + " " + common.DdlRank + " " + common.ApplicantName)
                                    .SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" of my Unit ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.PresentUnit).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(". I identify his signature on supporting documents as attested by him and certify them to be correct.")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);
                            document.Add(para1);

                            // Paragraph 2
                            Paragraph para2 = new Paragraph()
                                .Add(new Text("2.     It's certified that I am the CO/OC Tps of ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.Number + " " + common.DdlRank + " " + common.ApplicantName)
                                    .SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" and I am authorised to countersign financial documents of this individual.")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);
                            document.Add(para2);

                            // Paragraph 3
                            Paragraph para3 = new Paragraph()
                                .Add(new Text("3.     I have interviewed him ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text("and verified his financial condition and established need for taking this loan. Applicant will be using loan amount for intended purpose only.")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);
                            document.Add(para3);

                            // Paragraph 4
                            Paragraph para4 = new Paragraph()
                                .Add(new Text("4.     It is certified that Bank A/c No ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.SalaryAcctNo).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" of Bank ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.NameOfBank).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" with IFSC ").SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.IfsCode).SetFont(boldFont).SetFontSize(10))
                                .Add(new Text(" as given in the application and cancelled cheque is of Salary account of ")
                                    .SetFont(normalFont).SetFontSize(10))
                                .Add(new Text(common.Number + " " + common.DdlRank + " " + common.ApplicantName)
                                    .SetFont(boldFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);
                            document.Add(para4);

                            // Paragraph 5
                            Paragraph para5 = new Paragraph()
                                .Add(new Text("5.     I have satisfied myself of the correctness of personal details given in application. ")
                                    .SetFont(normalFont).SetFontSize(10))
                                .Add(new Text("I have perused the supporting documents and checked their correctness. ")
                                    .SetFont(normalFont).SetFontSize(10))
                                .Add(new Text("Supporting documents uploaded are readable and latest.")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(10f);
                            document.Add(para5);

                            // Paragraph 6
                            Paragraph para6 = new Paragraph("     Application is recommended for sanction and accordingly I countersign the same.")
                                .SetFont(normalFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.JUSTIFIED)
                                .SetMarginBottom(5);
                            document.Add(para6);
                        }


                        // === Signature Section (Bottom) ===
                        if (isApproved || isRejected)
                        {
                            document.Add(new Paragraph("\n\n\n"));

                            Table signatureTable2 = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph("Digital Signature of CO")
                                    .SetFont(boldFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph("")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER));

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph(armyno)
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph("")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER));

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph(Name)
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph("")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER));

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph($"Mobile No: {Mobile}")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph("")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER));

                            signatureTable2.AddCell(new Cell()
                                .Add(new Paragraph($"Digital Sign On: {dated}")
                                    .SetFont(normalFont).SetFontSize(10))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.LEFT));

                            document.Add(signatureTable2);
                        }


                        if (isApproved)
                        {


                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Icon", "DigitalSign.png");
                            ImageData imageData = ImageDataFactory.Create(imagePath);
                            Image icon = new Image(imageData).ScaleToFit(60f, 60f);
                            icon.SetFixedPosition(pdf.GetNumberOfPages(), 30, 313);
                            document.Add(icon);
                        }

                        if (isRejected)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Icon", "RejectedIcon.png");
                            ImageData imageData = ImageDataFactory.Create(imagePath);
                            Image icon = new Image(imageData).ScaleToFit(80f, 80f);
                            icon.SetFixedPosition(pdf.GetNumberOfPages(), 30, 600);
                            document.Add(icon);
                        }

                    }
                    pdf.Close();
                    writer.Close();

                    document.Close();
                }
            }
            return 1;
        }
    }
}
