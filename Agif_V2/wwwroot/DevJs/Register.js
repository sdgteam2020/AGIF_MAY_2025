$(document).ready(function () {
    loadDropdown();
    const unitInput = $("input[name='UnitId']");

    unitInput.on("keyup", function () {
        ValInData(this);
    });

   
})

function loadDropdown() {

    const Rank = $('#rank').data('rank-prefix');
    const regtCorps = $('#regtCorps').data('regtcorps-prefix');
    const ApptId = $('#ApptId').data('apptid-prefix');

    mMsater(Rank, "rank", 3, 0);
    mMsater(regtCorps, "regtCorps", 8, 0);
    mMsater(ApptId, "ApptId", 1, 0);
}

$('#DteFmn').on('change', function () {
    if ($(this).is(':checked')) {
        Swal.fire({
            title: 'DTE/FMN Selected',
            text: 'You have checked the DTE/FMN option.',
            icon: 'success',
            confirmButtonText: 'OK'
        });
    }
});

$("#btnsignup").on("click", function (e) {
    e.preventDefault(); // Prevent form submission for now
    if ($("#UnitId").val() == 0 || $("#txtUnit").val() == "") {
        $("#UnitId").val(0);
        $("#txtUnit").val("");
        return false;
    }
    Swal.fire({
        title: 'Do you really want to save?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, save it!',
        cancelButtonText: 'No, cancel!',
    }).then((result) => {
        if (result.isConfirmed) {
            const formData = {};
            $.each($("#signupForm").serializeArray(), function () {
                formData[this.name] = this.value;
            });
            formData["DteFmn"] = $('#DteFmn').is(':checked');
            const jsonString = JSON.stringify(formData);
            const serverKey = $('#serverCryptoKey').val();
            const encryptedPayload = encryptPayload(jsonString, serverKey);
            $("<input>").attr({
                type: "hidden",
                name: "EncryptedData",
                value: encryptedPayload
            }).appendTo("#signupForm");
            $("#signupForm").find("input:not([name='EncryptedData'], [name='__RequestVerificationToken']), select, textarea").removeAttr("name");
            $("#signupForm")[0].submit();  // Or trigger your form submit action here
        } else {
            Swal.fire('Cancelled', 'Your Details was not saved.', 'info');
        }
    });
});


$("#txtUnit").autocomplete({
    source: function (request, response) {
        $("input[name='UnitId']").val(0);

      if (request.term.length > 0) {
            const param = { "UnitName": request.term };
            $("#UnitId").val(0);
            $.ajax({
                url: '/Account/GetALLByUnitName',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                headers: {
                    "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (data) {
                    if (data.length != 0) {
                        response($.map(data, function (item) {

                            return { label: item.pcda_Pao + ' ' + item.name, value: item.id };

                        }))
                    }
                    else {
                        $("#UnitId").val(0);
                        $("#txtUnit").val("");

                        showErrorMessage("Unit Not found.")
                    }

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        }
    },
    select: function (e, i) {
        e.preventDefault();
        $("#txtUnit").val(i.item.label);
        $("#UnitId").val(i.item.value);
    },
    appendTo: '#suggesstion-box'
});

$("#btnTokenDetails").on('click', function () {

    GetTokenDetails("ArmyNo", "Name", "errormsg", "btnsignup")
});

$("input, textarea").on("paste", function (e) {
    e.preventDefault();
});
function encryptPayload(plainText, keyBase64) {
    // 1. Parse the Base64 key
    var key = CryptoJS.enc.Base64.parse(keyBase64);

    // 2. Generate a random 16-byte IV
    var iv = CryptoJS.lib.WordArray.random(16);

    // 3. Encrypt the data (AES-CBC, PKCS7 Padding)
    var encrypted = CryptoJS.AES.encrypt(plainText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    // 4. Combine IV + CipherText (Need to clone to avoid mutating)
    var ivAndCipher = iv.clone().concat(encrypted.ciphertext);

    // 5. Generate HMAC-SHA256 of the combined IV + CipherText
    var hmac = CryptoJS.HmacSHA256(ivAndCipher, key);

    // 6. Combine IV + CipherText + HMAC
    var finalData = ivAndCipher.clone().concat(hmac);

    // 7. Return as Base64 string
    return CryptoJS.enc.Base64.stringify(finalData);
}