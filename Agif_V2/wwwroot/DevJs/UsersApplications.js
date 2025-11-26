$(document).ready(function () {
    addBlurEffect();
    let rawValue = $("#Status").val();
    let value = (rawValue === "0" || !rawValue) ? 1 : rawValue;

    if (value == 2) {
        $('#tblApplications thead tr th').eq(6).before('<th>Digital Sign On</th>');
    }

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
    $('#acceptButton').on('click', function () {
        const applnId = $("#spnapplicationId").html();
        const icNo = $("#IcNo").val();
        const type = $("#UserType").val() || "Loan"; // Default to "Loan" if not set
        const $remarkField = $("#txtRemark");
        let remarkValue = $remarkField.val().trim();
        if (remarkValue === "") {
            $remarkField.val("Accepted");
        }
        Swal.fire({
            title: "Are you sure?",
            text: "Do you want to approve!",
            icon: "warning",
            iconColor: "#f1c40f", // Yellow icon
            background: "#fff8e1", // Light yellow background
            color: "#333",         // Text color
            showCancelButton: true,
            confirmButtonColor: "#f39c12", // Enhanced yellow-orange for "Yes"
            cancelButtonColor: "#d33",     // Red for cancel
            confirmButtonText: "✅ Yes, approve it!",
            cancelButtonText: "❌ Cancel",
            customClass: {
                popup: 'border border-yellow-500 shadow-lg rounded-xl',
                confirmButton: 'swal2-confirm-button-custom',
                cancelButton: 'swal2-cancel-button-custom'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                showLoader();
                GetTokenvalidatepersid2fa(icNo, applnId, type);
            }
        });
    });
    $("#RejectButton").on('click', function () {
        const applnId = $("#spnapplicationId").html();
        const type = $("#UserType").val() || "Loan"; // Default to "Loan" if not set
        const $remarkField = $("#txtRemark");
        let remarkValue = $remarkField.val().trim();
        if (remarkValue === "") {
            $remarkField.val("Rejected");
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
        const allChecked = $('input[name="instr"]').length === $('input[name="instr"]:checked').length;
        $('#acceptButton').prop('disabled', !allChecked);
        if (allChecked) {
            $('#txtRemark').focus();
        }
    }
    updateButtonState();
    $('input[name="instr"]').on('change', function () {
        updateButtonState();
    });

    $(document).on('click', '#btnProcess', function () {
        $("#ApplicationAction").modal("show");
        populateRecommendationModal(currentApplicationData);
    });
    $('#applicantsHistory').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (!currentApplicationData.armyNo) {
            return;
        }
        const historyModal = new bootstrap.Modal($('#ApplicantHistoryModal')[0], {
            backdrop: 'static'
        });
        historyModal.show();
        fetchApplicantData(currentApplicationData.armyNo);
    })
});
let currentApplicationData = {};
function populateRecommendationModal(applicationData) {
    if (!applicationData || Object.keys(applicationData).length === 0) {
        console.warn('No application data available for binding');
        return;
    }

    $('#ApplicationAction').find('label').each(function () {
        let template = $(this).attr('data-template');
        if (!template) return;

        let updatedText = template
            .replace(/\[Applicant Name\]/g, `<strong class="text-dark">${applicationData.armyNo || 'N/A'} ${applicationData.rank || 'N/A'} ${applicationData.name || 'N/A'}</strong >`)
            .replace(/\[Application type\]/g, `<strong class="text-dark">${applicationData.applicationType || 'N/A'}</strong>`)
            .replace(/\[Unit Name\]/g, `<strong class="text-dark">${applicationData.unitName || 'N/A'}</strong>`)
            .replace(/\[Account Number\]/g, `<strong class="text-dark">${applicationData.accountNumber || 'N/A'}</strong>`)
            .replace(/\[IFSC Code\]/g, `<strong class="text-dark">${applicationData.ifscCode || 'N/A'}</strong>`)
            .replace(/\[Bank\]/g, `<strong class="text-dark">${applicationData.bank || 'N/A'}</strong>`);

        $(this).html(updatedText);
    });

    $('#ApplicationActionLabel').html(`
        <i class="bi bi-file-earmark-check me-2"></i>
        Application Review & Approval - ${applicationData.armyNo || 'N/A'} ${applicationData.rank || 'N/A'} ${applicationData.name || 'N/A'}
    `);
}
function GetApplicationList(status, endpoint) {
    const dynamicColumns = [
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
        }
    ];

    // Track the index for Digital Sign On column
    let digitalSignColumnIndex = -1;

    // Add "Digital Sign On" column dynamically if status == 2
    if (status == 2 || status == 102) {
        digitalSignColumnIndex = dynamicColumns.length; // This will be index 6
        dynamicColumns.push({
            data: "digitalSignDate", 
            name: "digitalSignDate", // Changed from "Digital Sign On" to avoid spaces
            orderable: true, // Make sure it's orderable
            render: function (data, type, row) {
                return data || 'N/A';
            }
        });
    }

    // Add the "Action" column dynamically
    dynamicColumns.push({
        data: null,
        orderable: false,
        className: 'noExport',
        render: function (data, type, row) {
            const categorytype = $("#UserType").val() || "Loan";
            console.log(categorytype);
            if (row.isMergePdf == false) {
                return `
                    <div class='action action-container d-flex'>
                        <button class='btn btn-sm btn-outline-danger align-items-center mx-2' onclick='mergePdf(${row.applicationId}, 0, 0, "${categorytype === "Loan" ? "/OnlineApplication/MergePdf" : "/Claim/MergePdf"}",${categorytype})' data-bs-toggle="tooltip" data-bs-placement="top" title="Merge Pdf">
                            <i class="bi bi-pencil-square"></i>
                        </button>
                    </div>
                `;
            } else {
                return `
                    <div class='action action-container'>
                        <button class='btn btn-sm btn-outline-danger mx-2' onclick='OpenAction(${row.applicationId}, "${categorytype === "Loan" ? "/OnlineApplication/GetPdfFilePath" : "/Claim/GetPdfFilePath"}",${categorytype})' data-bs-toggle="tooltip" data-bs-placement="top" title="View Pdf">
                            <i class="bi bi-pencil-square"></i>
                        </button>
                    </div>
                `;
            }
        }
    });

    // Set the correct sorting order
    let tableOrder;
    if (status == 2 && digitalSignColumnIndex !== -1) {
        tableOrder = [[digitalSignColumnIndex, 'desc']]; // Sort by Digital Sign On column
    } else {
        tableOrder = [[0, 'desc']]; // Default sort by first column
    }

    if (status == 1 || status == 101) {
        $("#DigitalSignatureforaction").html(`
        <button type="button" class="btn btn-outline-danger" data-bs-dismiss="modal">
                            <i class="bi bi-x-lg me-2"></i>
                            Close
                        </button>
                        <button type="button" class="btn btn-success" id="btnProcess">
                            <i class="bi bi-gear-fill me-2"></i>
                            Process Application
                        </button>`);
    }

    // Destroy existing DataTable if it exists
    if ($.fn.DataTable.isDataTable('#tblApplications')) {
        $('#tblApplications').DataTable().clear().destroy();
    }

    // Initialize DataTable with server-side processing
    $('#tblApplications').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: tableOrder,
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
                    status: status
                };
            },
            error: function (xhr, error, code) {
                console.error('Error loading data:', error);
                alert('Error loading data. Please try again.');
            }
        },
        columns: dynamicColumns,
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
        dom: '<"row"<"col-md-6"l><"col-md-6"f>>rt<"row"<"col-md-6"i><"col-md-6"p>>',
    });
}
//function OpenAction(applicationId, endpoint, category) {
//    //HBA_SL12345671Y_1007_Merged
//    $("#spnapplicationId").html(applicationId);

//    const val = $("#UserType").val() || "Loan";

//    $.ajax({
//        type: "POST",
//        url: endpoint,
//        data: { applicationId: applicationId },
//        dataType: 'json',
//        success: function (response) {

//            if (response != null) {
//                $("#pdfContainer").empty();


//                // Create a new object element dynamically
//                const objectElement = document.createElement('object');
//                objectElement.id = 'PdfViwerFOrDigital'; // Optional: Set ID for the object
//                objectElement.type = 'application/pdf';
//                objectElement.classList.add('w-100', 'h-100', 'border-0', 'rounded'); // Add your styling classes

//                // Set the 'data' attribute to load the PDF
//                const pdfUrl = response + '#toolbar=0&navpanes=0&scrollbar=0';

//                //objectElement.setAttribute('data', response);
//                objectElement.setAttribute('data', pdfUrl);

//                // Append the object element to the container
//                document.getElementById('pdfContainer').appendChild(objectElement);

//                // Show the modal with the PDF viewer
//                $("#ViewPdf").modal("show");


//                if (val === "Loan")
//                    fetchApplicationDetails(applicationId, "/OnlineApplication/GetApplicationDetails");
//                else if (val === "Maturity")
//                    fetchApplicationDetails(applicationId, "/Claim/GetApplicationDetails");

//            } else {
//                alert('Error retrieving PDF file path: ' + response.message);
//                console.error('Error:', response.message);
//            }
//        },
//    });


//}
function OpenAction(applicationId, endpoint, category) {
    $("#spnapplicationId").html(applicationId);
    const val = $("#UserType").val() || "Loan";

    // Show loading overlay (optional)
    $("#loadingOverlay").removeClass("d-none");

    fetch(endpoint, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: new URLSearchParams({ applicationId })
    })
        .then(resp => resp.json())
        .then(response => {
            $("#loadingOverlay").addClass("d-none");
            $("#pdfContainer").empty();

            if (!response) {
                alert("No PDF returned");
                return;
            }

            // Assume `response` is the full URL to your PDF
            return fetch(response);
        })
        .then(r => {
            if (!r || !r.ok) throw new Error("PDF not found");
            return r.blob();
        })
        .then(blob => {
            const pdfUrl = URL.createObjectURL(blob);

            const embed = document.createElement("embed");
            embed.src = pdfUrl + "#toolbar=0&navpanes=0&scrollbar=0";
            embed.type = "application/pdf";
            embed.classList.add("w-100", "h-100", "border-0", "rounded");

            document.getElementById("pdfContainer").appendChild(embed);

            $("#ViewPdf").modal("show");

            const val = $("#UserType").val() || "Loan";
            if (val === "Loan")
                fetchApplicationDetails(applicationId, "/OnlineApplication/GetApplicationDetails");
            else if (val === "Maturity")
                fetchApplicationDetails(applicationId, "/Claim/GetApplicationDetails");
        })
        .catch(err => {
            $("#loadingOverlay").addClass("d-none");
            console.error(err);
            alert("Error loading PDF: " + err.message);
        });
}

function fetchApplicationDetails(applicationId, endpoint) {
    currentApplicationData = {};// Clear previous application data
    $.ajax({
        url: endpoint, // Adjust URL as per your controller
        type: 'POST',
        data: { applicationId: applicationId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                currentApplicationData = response.data;
                
                updatePdfViewerInfo(response.data);
            } else {
                console.error('Failed to fetch application details:', response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching application details:', error);
        }
    });
}
function updatePdfViewerInfo(applicationData) {
    const applicantName = applicationData.armyNo + " " + applicationData.rank + " " + applicationData.name;
    $('#ViewPdf .text-muted strong').text(applicantName);
}
function mergePdf(applicationId, isRejected, isApproved, endpoint, category) {
    const val = $("#UserType").val() || "Loan";
    $.ajax({
        type: "POST",
        url: endpoint,
        data: { applicationId: applicationId, isRejected: isRejected, isApproved: isApproved },
        dataType: 'json',
        success: function (response) {
            if (isApproved) {
                DigitalSignByAPI(applicationId, val);
            } else if (isRejected) {
                Swal.fire({
                    title: "Rejected!",
                    text: "Application Rejected!",
                    icon: "warning"
                }).then(() => {
                    window.location.href = "/ApplicationRequest/UserApplicationList";
                });
            } else if (response.success) {
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

        },
        error: function (xhr, status, error) {
            console.error('AJAX Error Details:');
            console.error('Status:', status);
            console.error('Error:', error);
            console.error('Response Text:', xhr.responseText);
            console.error('Status Code:', xhr.status);

            let errorMessage = 'An error occurred while generating the PDF.';
            if (xhr.responseText) {
                try {
                    const errorResponse = JSON.parse(xhr.responseText);
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
    let URL = '';
    $.ajax({
        url: "https://dgisapp.army.mil:55102/Temporary_Listen_Addresses/ValidatePersID2FA",
        type: "POST",
        contentType: 'application/json',
        data: JSON.stringify({
            "inputPersID": "9a4beb14b87de35d6bba98e2b16ad4eb341d52bda2bb3b7eadb064baf676cbd3" //Amit kumar jha
            //"inputPersID": IcNo//HRMS Token
            //"inputPersID": "867a43f0ec5593ba28ebd8f8e765bb6f3886f45981193f3d2befdc72404e0484" //Comdt Token
        }),

        success: function (response) {
            if (response) {
               
                const validationResult = response.ValidatePersID2FAResult;

                if (validationResult === true) {

                    if (type === "Loan")
                        URL = "/ApplicationRequest/DataDigitalXmlSign";
                    else if (type === "Maturity")
                        URL = "/ApplicationRequest/ClaimDataDigitalXmlSign";

                    DataSignDigitaly(applnId, URL, type);

                } else {
                    hideLoader();

                    Swal.fire({
                        title: "Alert!",
                        text: "Army No is Not Matching. Please Insert Valid Token!",
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
            hideLoader();

            Swal.fire({
                title: "Error!",
                text: "Failed to communicate with signing service.",
                icon: "error"
            });
        }
    });
}

function DataSignDigitaly(applicationId, endpoint, userType) {
    $.ajax({
        url: endpoint,
        type: "POST",
        data: { applicationId },
        success: function (data) {
            if (data) { 
                GetTokenSignXml(data, userType, applicationId);
            }
        },
        error: function () {
            hideLoader();
            Swal.fire({
                title: "Alert!",
                text: "Please ensure that DGIS App is installed and running during the digital signature process.",
                icon: "error"
            });
        }
    });
}
function GetTokenSignXml(xml, Usertype, applicationId) {
    let URL = '';
    if (Usertype === "Loan")
        URL = "/ApplicationRequest/SaveXML";
    else if (Usertype === "Maturity")
        URL = "/ApplicationRequest/SaveClaimXML";
    SignXmlSendTOdatabase(xml, URL, Usertype);
}
//tez code start
function DigitalSignByAPI(applicationId, type) {
    GetThumbprint().then(function (tprint) {
        let URL = '';
        if (tprint) {
            if (type === "Loan")
                URL = "/OnlineApplication/GetPdfFilePath";
            else if (type === "Maturity")
                URL = "/Claim/GetPdfFilePath";
            getPdfFilePath(applicationId, tprint, URL,type);
        } else {
            console.error('No thumbprint received.');
        }
    }).catch(function (error) {
        hideLoader();
        console.error('Failed to fetch thumbprint:', error);
    });
}
function getPdfFilePath(applicationId, thumbprint, endpoint, type) {

    console.log(JSON.stringify({ applicationId: applicationId }));

    $.ajax({
        url: endpoint,
        type: 'POST',
        data: { applicationId: applicationId },
        dataType: 'json',
        success: function (response) {
            if (response) {
                sendPDFToServer(response, thumbprint, type);
            } else {
                console.log('No file path returned.');
            }
        },
        error: function (xhr, status, error) {
            hideLoader();
            console.error('Error fetching PDF file path:', error);
        }
    });
}
function sendPDFToServer(filepath, thumbprint, type) {
    console.log("Thumbprint used:", thumbprint);

    const baseUrl = window.location.origin;
    const fullPath = `${baseUrl.replace(/\/+$/, '')}/${filepath.replace(/^\/+/, '')}`;
    let URL = '';

    console.log("Full PDF Path:", fullPath)

    $.ajax({
        //url: 'http://localhost/Temporary_Listen_Addresses/ByteDigitalSignAsync',
        url: 'https://dgisapp.army.mil:55102/Temporary_Listen_Addresses/ByteDigitalSignAsync',
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify([{
            Thumbprint: thumbprint,
            pdfpath: fullPath,
            XCoordinate: "450",
            YCoordinate: "20",
            Page: "1",
            CustomText: "Digital Signature"
        }]),
        success: function (response) {
            hideLoader();
            if (response) {
                Swal.fire({
                    title: "Application Approved",
                    text: "Application has been digitally signed successfully.",
                    icon: "success",
                    confirmButtonText: "OK",
                    customClass: {
                        popup: 'swal-success-theme',
                        confirmButton: 'swal-confirm-green'
                    },
                    buttonsStyling: false
                }).then(() => {
                    const fileName = filepath.split('/').pop();
                    if (type === "Loan")
                        URL = "/OnlineApplication/SaveBase64ToFile";
                    else if (type === "Maturity")
                        URL = "/Claim/SaveBase64ToFile";

                    SaveSignedPdf(response.Message, fileName,URL);
                });
            } else {
                Swal.fire({
                    title: "Error!",
                    text: "Failed to sign PDF.",
                    icon: "error"
                });
            }
        },
        error: function (error) {
            console.error('Error sending PDF:', error);
            hideLoader();

            Swal.fire({
                title: "Error!",
                text: "Failed to communicate with signing service.",
                icon: "error"
            });
        }
    });
}
function GetThumbprint() {
    return $.ajax({
        //url: 'http://localhost/Temporary_Listen_Addresses/FetchUniqueTokenDetails',
        url: 'https://dgisapp.army.mil:55102/Temporary_Listen_Addresses/FetchUniqueTokenDetails',
        type: 'GET'
    }).then(function (response) {
        if (response && response.length > 0 && response[0].Thumbprint) {
            return response[0].Thumbprint;
        } else {
            throw new Error('Thumbprint not found in response');
        }
    }).catch(function (error) {
        console.error('Error fetching thumbprint:', error);
        return null;
    });
}
function SaveSignedPdf(base64String, fn, endpoint) {
    $.ajax({
        url: endpoint,
        type: 'POST',        
        dataType: 'json',
        data:{
            base64String: base64String, 
            fileName: `${fn}`
        },
        success: function (response) {
            window.location.href = "/ApplicationRequest/UserApplicationList";
        },
        error: function (error) {
            console.error('Error saving signed PDF:', error);
        }
    });
}
//tez code end
function SignXmlSendTOdatabase(xmlString, endpoint, userType) {
    const applnId = $('#spnapplicationId').html();
    const remarks = $('#txtRemark').val();
    let url = '';

    $.ajax({
        url: endpoint,
        type: 'POST',
        data: { applId: applnId, xmlResString: xmlString, remarks: remarks },
        success: function () {
            if (userType === 'Loan') {
                url = "/OnlineApplication/MergePdf";
            } else if (userType === 'Maturity') {
                url = "/Claim/MergePdf";
            }

            mergePdf(applnId, false, true, url, userType);
        },
        error: function () {
            alert("Data Not Saved!");
        }
    });
}
function rejectedApplication(applicationId, type) {
    let URL = '';
    let remarks = $("#txtRemark").val();
    if (type === 'Loan') {
        URL = "/ApplicationRequest/RejectXML";
    }
    else if (type === 'Maturity') {
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
// Function to add blur effect when ApplicationAction modal opens
function addBlurEffect() {
    const $modal1 = $('#ViewPdf');
    const $modal2 = $('#ApplicationAction');
    const $modal3 = $('#ApplicantHistoryModal');
    
    $modal2.on('show.bs.modal', function () {
        $modal1.find('.modal-content').addClass('modal-blur');
    });
    
    $modal2.on('hidden.bs.modal', function () {
        $modal1.find('.modal-content').removeClass('modal-blur');
    });

    $modal3.on('show.bs.modal', function () {
        $modal2.find('.modal-content').addClass('modal-blur');
    });
    $modal3.on('hidden.bs.modal', function () {
        $modal2.find('.modal-content').removeClass('modal-blur');
    });
}
function fetchApplicantData(armyNo) {
    console.log(armyNo);
    const usertype = $("#UserType").val();
    console.log(usertype);
    $.ajax({
        url: '/ApplicationRequest/GetApplicantHistory',
        type: 'POST',
        data: { armyNo: armyNo, usertype: usertype },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                console.log(response.data);
                const applicantData = response.data[0] || {};
                $('#applicantName').text(applicantData.name || 'N/A');
                $('#applicantArmyNo').text(applicantData.armyNo || 'N/A');
                populateHistoryTable(response.data);
                //$('#ApplicantHistoryModal').modal('show');
               
            } else {
                console.error('Failed to fetch applicant history:', response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching applicant history:', error);
        }
    });
}

function populateHistoryTable(data) {
    const $tbody = $('#historyTableBody');
    $tbody.empty();

    if (data.length === 0) {
        $tbody.html('<tr><td colspan="6" class="text-center py-4 text-muted">No records found</td></tr>');
        return;
    }

    $.each(data, function (index, record) {
        const row = `
                        <tr>
                            <td class="text-center">${index + 1}.</td>
                            <td>${record.armyNo}</td>
                            <td>${record.name}</td>
                            <td>${record.applicationType}</td>
                            <td>
                                <span>
                                    ${record.presentStatus}
                                </span>
                            </td>
                            <td>${record.updatedOn}</td>
                        </tr>
                    `;
        $tbody.append(row);
    });

    $('#totalRecords').text(data.length);
}

function showLoader() {
    $("#global-loader").removeClass("d-none");
}

function hideLoader() {
    $("#global-loader").addClass("d-none");
}
