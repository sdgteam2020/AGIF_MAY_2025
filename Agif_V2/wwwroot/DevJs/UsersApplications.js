$(document).ready(function () {
    let rawValue = $("#Status").val();
    let value = (rawValue === "0" || !rawValue) ? 1 : rawValue;


    GetApplicationList(value, "/ApplicationRequest/GetUsersApplicationList");

    $('#Loan').click(function () {
        $("#UserType").val('Loan');
        $("#PdfViwerFOrDigital").attr("data", "");
        GetApplicationList(value, "/ApplicationRequest/GetUsersApplicationList");
    });

    $('#Maturity').click(function () {
        $("#UserType").val('Maturity');
        $("#PdfViwerFOrDigital").attr("data", "");

        let Mvalue = 0;
        if (value == 1)
            Mvalue = 101;
        else if (value == 2)
            Mvalue = 102;
        else if (value == 3)
            Mvalue = 103;

        GetApplicationList(Mvalue, "/ApplicationRequest/GetMaturityUsersApplicationList");
    });


    //const GlobalApplnId;
    $('#acceptButton').on('click', function () {
        var applnId = $("#spnapplicationId").html();
        var icNo = $("#IcNo").data("id");
        let value = true;
        let type = $("#UserType").val() || "Loan"; // Default to "Loan" if not set
        var remarkField = $("#txtRemark");
        var remarkValue = remarkField.val().trim();
        if (remarkValue === "") {
            remarkField.val("Accepted");
        }

        if (value == true) {
            Swal.fire({
                title: "Are you sure?",
                text: "Do you want to approve!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, approve it!"
            }).then((result) => {
                if (result.isConfirmed) {
                    GetTokenvalidatepersid2fa(icNo, applnId, type);
                }
            });
            

        }
    });
    $("#RejectButton").on('click', function () {
        var applnId = $("#spnapplicationId").html();
        var icNo = $("#IcNo").data("id");
        let type = $("#UserType").val() || "Loan"; // Default to "Loan" if not set
        var remarkField = $("#txtRemark");
        var remarkValue = remarkField.val().trim();
        if (remarkValue === "") {
            remarkField.val("Rejected");
        }
        Swal.fire({
            title: "Are you sure?",
            text: "Do you want to reject!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, reject it!"
        }).then((result) => {
            if (result.isConfirmed) {
                rejectedApplication(applnId, type);
            }
        });

    });

    function updateButtonState() {
        var allChecked = $('input[name="instr"]').length === $('input[name="instr"]:checked').length;
        $('#acceptButton, #RejectButton').prop('disabled', !allChecked);
    }
    updateButtonState();
    $('input[name="instr"]').on('change', function () {
        updateButtonState();
    });

    $(document).on('click', '#btnProcess', function () {
        $("#ApplicationAction").modal("show");
        populateRecommendationModal(currentApplicationData);
    });
});


var currentApplicationData = {};



function populateRecommendationModal(applicationData) {
    if (!applicationData || Object.keys(applicationData).length === 0) {
        console.warn('No application data available for binding');
        return;
    }

    $('#ApplicationAction').find('label').each(function () {
        let template = $(this).attr('data-template');
        if (!template) return;

        let updatedText = template
            .replace(/\[Applicant Name\]/g, `<strong class="text-primary">${applicationData.armyNo || 'N/A'} ${applicationData.rank || 'N/A'} ${applicationData.name || 'N/A'}</strong >`)
            .replace(/\[Application type\]/g, `<strong class="text-primary">${applicationData.applicationType || 'N/A'}</strong>`)
            .replace(/\[Unit Name\]/g, `<strong class="text-primary">${applicationData.unitName || 'N/A'}</strong>`)
            .replace(/\[Account Number\]/g, `<strong class="text-primary">${applicationData.accountNumber || 'N/A'}</strong>`)
            .replace(/\[IFSC Code\]/g, `<strong class="text-primary">${applicationData.ifscCode || 'N/A'}</strong>`)
            .replace(/\[Bank\]/g, `<strong class="text-primary">${applicationData.bank || 'N/A'}</strong>`);

        $(this).html(updatedText);
    });

    $('#ApplicationActionLabel').html(`
        <i class="bi bi-file-earmark-check me-2"></i>
        Application Review & Approval - ${applicationData.name || 'Unknown Applicant'}
    `);
}

function GetApplicationList(status,endpoint) {


    if (status == 1 || status == 101) {
        $("#DigitalSignatureforaction").html(`
        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">
                            <i class="bi bi-x-lg me-2"></i>
                            Close
                        </button>
                        <button type="button" class="btn btn-primary" id="btnProcess">
                            <i class="bi bi-gear-fill me-2"></i>
                            Process Application
                        </button>`);
    }
    // Destroy existing DataTable if it exists
    if ($.fn.DataTable.isDataTable('#tblApplications')) {
        $('#tblApplications').DataTable().clear().destroy();
    }

    // Initialize DataTable with server-side processing
    var table = $('#tblApplications').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: {
            url: endpoint,
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            data: function (data) {
                return {
                    draw: data.draw,
                    start: data.start,
                    length: data.length,
                    searchValue: data.search.value,
                    sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',
                    sortDirection: data.order.length > 0 ? data.order[0].dir : '',
                    status: status // Pass the status parameter
                };

            },
            error: function (xhr, error, code) {
                console.error('Error loading data:', error);
                alert('Error loading data. Please try again.');
            }
        },
        columns: [
            {
                data: null,
                name: "SerialNumber",
                orderable: false,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "armyNo",
                name: "ArmyNo",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "name",
                name: "Name",
                render: function (data, type, row) {
                    return data ? `<a href='mailto:${data}'>${data}</a>` : 'N/A';
                }
            },
            {
                data: "applicationType",
                name: "ApplicationType",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "dateOfBirth",
                name: "DateOfBirth",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "appliedDate",
                name: "AppliedDate",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: null,
                orderable: false,
                className: 'noExport',
                render: function (data, type, row) {
                    if (row.isMergePdf == false) {
                        var categorytype = $("#UserType").val() || "Loan"; 

                        return `
                        <div class='action action-container d-flex'>
                            <button class='btn btn-sm btn-outline-warning  align-items-center mx-2' onclick='mergePdf(${row.applicationId}, 0, 0, "${categorytype === "Loan" ? "/OnlineApplication/MergePdf" : "/Claim/MergePdf"}",${categorytype})'>
                                <i class="bi bi-eye"></i>
                        </div>
                       
                    `;
                    }
                    else {
                        var category = $("#UserType").val() || "Loan"; 

                        return `
                        <div class='action action-container'>
                            <button class='btn btn-sm btn-outline-warning d-flex align-items-center  mx-2' onclick='OpenAction(${row.applicationId}, "${category === "Loan" ? "/OnlineApplication/GetPdfFilePath" : "/Claim/GetPdfFilePath"}",${category})'>
                                <i class="bi bi-eye"></i>
                               
                            </button>
                        </div>
                    `;
                    }

                }
            }
        ],
        language: {
            search: "",
            searchPlaceholder: "Search applications...",
            processing: "Loading applications...",
            emptyTable: "No applications found",
            info: "Showing _START_ to _END_ of _TOTAL_ applications",
            infoEmpty: "Showing 0 to 0 of 0 applications",
            infoFiltered: "(filtered from _MAX_ total applications)",
            lengthMenu: "Show _MENU_ applications per page",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        },


        pageLength: 10,
        lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
        //dom: 'lBfrtip',
        dom: '<"row"<"col-md-6"l><"col-md-6"f>>rt<"row"<"col-md-6"i><"col-md-6"p>>',
    });
}

function OpenAction(applicationId, endpoint, category) {
    //HBA_SL12345671Y_1007_Merged
    $("#spnapplicationId").html(applicationId);

    var val = $("#UserType").val() || "Loan";

    $.ajax({
        type: "POST",
        url: endpoint,
        data: { applicationId: applicationId },
        dataType: 'json',
        success: function (response) {
          
            if (response != null) {
                $("#pdfContainer").empty();

                // Create a new object element dynamically
                var objectElement = document.createElement('object');
                objectElement.id = 'PdfViwerFOrDigital'; // Optional: Set ID for the object
                objectElement.type = 'application/pdf';
                objectElement.classList.add('w-100', 'h-100', 'border-0', 'rounded'); // Add your styling classes

                // Set the 'data' attribute to load the PDF
                objectElement.setAttribute('data', response);

                // Append the object element to the container
                document.getElementById('pdfContainer').appendChild(objectElement);

                // Show the modal with the PDF viewer
                $("#ViewPdf").modal("show");


                if (val === "Loan")
                    fetchApplicationDetails(applicationId, "/OnlineApplication/GetApplicationDetails");
                else if (val === "Maturity")
                    fetchApplicationDetails(applicationId, "/Claim/GetApplicationDetails");

            } else {
                alert('Error retrieving PDF file path: ' + response.message);
                console.error('Error:', response.message);
            }
        },
    });
   
 
}

// Function to fetch application details from server
function fetchApplicationDetails(applicationId,endpoint) {
    currentApplicationData = {};// Clear previous application data
    $.ajax({
        url: endpoint, // Adjust URL as per your controller
        type: 'POST',
        data: { applicationId: applicationId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                // Store the application data globally
                currentApplicationData = response.data;

                // Update PDF viewer with application info if needed
                updatePdfViewerInfo(response.data);
            } else {
                console.error('Failed to fetch application details:', response.message);
                // Don't show error popup as PDF is still loading successfully
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching application details:', error);
            // Don't show error popup as PDF functionality should still work
        }
    });
}

// Function to update PDF viewer with application information (optional)
function updatePdfViewerInfo(applicationData) {
    // Update applicant name in the PDF modal if you want
    $('#ViewPdf .text-muted strong').text(applicationData.name || 'Unknown Applicant');

    // Update date in the PDF modal if you want
    $('#ViewPdf .badge.bg-info').html(`
        <i class="bi bi-calendar3 me-1"></i>
        ${applicationData.appliedDate || new Date().toLocaleDateString()}
    `);
}
function mergePdf(applicationId, isRejected, isApproved, endpoint, category) {
    var val = $("#UserType").val() || "Loan";


    $.ajax({
        type: "POST",
        url: endpoint,
        data: { applicationId: applicationId, isRejected: isRejected, isApproved: isApproved },
        dataType: 'json',
        success: function (response) {
            if (isApproved) {
                Swal.fire({
                    title: "Approved!",
                    text: "Signed succesfull and Saved",
                    icon: "success"
                }).then(() => {
                    window.location.href = "/ApplicationRequest/UserApplicationList";
                });
            } else if (isRejected) {
                Swal.fire({
                    title: "Rejected!",
                    text: "Application Rejected!",
                    icon: "warning"
                }).then(() => {
                    window.location.href = "/ApplicationRequest/UserApplicationList";
                });
            } else {
                if (response.success) {
                    let url = "";

                    if (val === "Maturity") {
                        url = "/Claim/GetPdfFilePath";
                    } else if (val === "Loan") {
                        url = "/OnlineApplication/GetPdfFilePath";
                    } else {
                        console.warn("Unknown value for 'val':", val);
                        return;
                    }

                    OpenAction(applicationId, url, val);
                } else {
                    alert('Error generating PDF: ' + response.message);
                    console.error('PDF merge failed:', response.message);
                }
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX Error Details:');
            console.error('Status:', status);
            console.error('Error:', error);
            console.error('Response Text:', xhr.responseText);
            console.error('Status Code:', xhr.status);

            var errorMessage = 'An error occurred while generating the PDF.';
            if (xhr.responseText) {
                try {
                    var errorResponse = JSON.parse(xhr.responseText);
                    if (errorResponse.message) {
                        errorMessage += ' Details: ' + errorResponse.message;
                    }
                } catch (e) {
                    errorMessage += ' Server response: ' + xhr.responseText;
                }
            }

            alert(errorMessage);
        }
    });
}



async function GetTokenvalidatepersid2fa(IcNo, applnId, type) {
    var URL = '';
    $.ajax({

       url: "http://localhost/Temporary_Listen_Addresses/ValidatePersID2FA",
        //url:"https://dgisapp.army.mil:55102/Temporary_Listen_Addresses/ValidatePersID2FA",
        type: "POST",
        contentType: 'application/json', // Set content type to XML

        data: JSON.stringify({
            //"inputPersID": IcNo
            //"inputPersID": "A2A7D3ED10E454CDD66285EBDFCC293549762148F74D4A65221250769C8E6448"
            "inputPersID": "9A4BEB14B87DE35D6BBA98E2B16AD4EB341D52BDA2BB3B7EADB064BAF676CBD3"
            //"inputPersID": "9A4BEB14B87DE35D6BBA98E2B16AD4EB341D52BDA2BB3B7EADB064BAF676CBD3"
        }),

        success: function (response) {
            if (response) {
                const validationResult = response.ValidatePersID2FAResult;

                if (validationResult === true) {

                    if (type === "Loan")
                        URL = "/ApplicationRequest/DataDigitalXmlSign";
                    else if (type === "Maturity")
                        URL = "/ApplicationRequest/ClaimDataDigitalXmlSign";

                    DataSignDigitaly(applnId, URL,type);

                } else {
                    Swal.fire({
                        title: "Alert!",
                        text: "Army No Not Matched Pl Insert Valid Token!",
                        icon: "error"
                    });

                }
            }
        },
        error: function () {
            Swal.fire({
                title: "Alert!",
                text: "Please Ensure that DGIS App has been installed and running at the time of Digital Signature.",
                icon: "error"
            });
        }
    });



}

function DataSignDigitaly(applicationId, endpoint,Usertype) {
    $.ajax({
        type: "get",
        url: endpoint,
        data: { applicationId: applicationId },
        type: 'POST',
        success: function (data) {

            if (data != null) {
                GetTokenSignXml(data,Usertype)
            }
        },
        error: function () {
            Swal.fire({
                title: "Alert!",
                text: "Please Ensure that DGIS App has been installed and running at the time of Digital Signature.",
                icon: "error"
            });
        }
    });

}

function GetTokenSignXml(xml, Usertype) {
    var URL = '';

    $.ajax({
        url: 'http://localhost/Temporary_Listen_Addresses/SignXml',
        //url: 'https://dgisapp.army.mil:55102/Temporary_Listen_Addresses/SignXml',
        type: "POST",
        contentType: 'application/xml', // Set content type to XML
        data: xml, // Set the XML data
        success: function (response) {
            if (response) {
                var xmlContent = new XMLSerializer().serializeToString(response);

                // No Token Found
                if (xmlContent.indexOf("<Root>No Token Found</Root>") == -1) {

                    if (Usertype === "Loan")
                        URL = "/ApplicationRequest/SaveXML";
                    else if (Usertype === "Maturity")
                        URL = "/ApplicationRequest/SaveClaimXML";


                    SignXmlSendTOdatabase(xmlContent, URL, Usertype);

                } else {
                    Swal.fire({
                        title: "Error!",
                        text: "No Token Found!",
                        icon: "error"
                    });
                }
            }
        },
        error: function (result) {
            Swal.fire({
                title: "Alert!",
                text: "DGIS App Not running!",
                icon: "error"
            });
        }
    });
}

function SignXmlSendTOdatabase(xmlString, endpoint, Usertype) {
    var applnId = $('#spnapplicationId').html();
    var remarks = $('#txtRemark').val();
    var URL = '';
    $.ajax({
        url: endpoint,
        data: { applId: applnId, xmlResString: xmlString, remarks: remarks },
        type: 'POST',
        success: function () {
            if (Usertype === 'Loan')
                URL = "/OnlineApplication/MergePdf";
            else if (Usertype === 'Maturity')
                URL = "/Claim/MergePdf";


            mergePdf(applnId, false, true,URL,Usertype)

        },
        error: function () {
            alert("Data Not Saved!")
        }
    });
}

function rejectedApplication(applicationId, type) {
    var URL = '';
    let remarks = $("#txtRemark").val();
    if (type === 'Loan') {
        URL = "/ApplicationRequest/RejectXML";
    }
    else if(type === 'Maturity') {
        URL = "/ApplicationRequest/ClaimRejectXML";
    }

    $.ajax({
        url: URL,
        data: { applId: applicationId, rem: remarks },
        type: 'POST',
        success: function (data) {

            if (type === 'Loan')
                URL = "/OnlineApplication/MergePdf";
            else if (type === 'Maturity')
                URL = "/Claim/MergePdf";

            mergePdf(applicationId, true, false, URL, type)
        },
        error: function () {
            swal.fire("Error!", "Something went wrong. Please try again.", "error");
        }
    });
}