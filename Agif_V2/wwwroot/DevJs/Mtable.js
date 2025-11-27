const errormsg002 = "Something Went Wrong";

function mMsater(sectid, ddl, TableId, ParentId) {
    const userdata = {
        id: TableId,
        ParentId: ParentId
    };

    $.ajax({
        url: '/Master/GetAllMMaster',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response !== "null" && response != null) {
                if (response === 0 || response === -1) {
                    Swal.fire({ text: errormsg002 });
                } else {
                    let listItemddl = '<option value="">Please Select</option>';

                    // Guard if response isn't iterable
                    if (Array.isArray(response)) {
                        response.forEach(item => {
                            listItemddl += `<option value="${item.id}">${item.name}</option>`;
                        });
                    }

                    $("#" + ddl).html(listItemddl);

                    if (sectid !== '') {
                        $("#" + ddl).val(sectid);
                    }
                }
            }
            // else: silently ignore null-like response (could add logging if needed)
        },
        error: function () {
            Swal.fire({ text: errormsg002 });
        }
    });
}

async function GetTokenDetails(txtArmyNo, txtName, msgid, btntoshow)
{
    try {
        const response = await fetch("https://dgisapp.army.mil:55102/Temporary_Listen_Addresses/FetchUniqueTokenDetails", {
            method: "GET",
            cache: "no-cache",
            headers: {
                "Accept": "application/json"
            }
        });

        const data = await response.json();

        if (data && data.length > 0) {
            if (data[0].Status === '200') {

                let pairs = data[0].subject.split(", ");
                let keyValuePairs = {};

                pairs.forEach(pair => {
                    let [k, v] = pair.split("=");
                    keyValuePairs[k.trim()] = v ? v.trim() : "";
                });


                const datef2 = new Date();
                if (data[0].ValidTo >= datef2) {
                    $("#" + msgid).html('<div class="alert alert-danger" style="margin-top:5px;><i class="fa fa-times" aria-hidden="true" ></i><span class="m-lg-2">Token Expired</span>.</div>');
                    $("#" + txtArmyNo).val("");
                    $("#" + txtName).val("");
                    $("#" + btntoshow).addClass('d-none');
                } else {
                    $("#" + msgid).html('<div class="alert alert-success " style="margin-top:5px;><i class="fa fa-check" aria-hidden="true" ></i><span class="m-lg-2">Token Detected</span></div>');
                    $("#" + txtArmyNo).val(keyValuePairs.SERIALNUMBER.toUpperCase().trim());
                    $("#" + txtName).val(keyValuePairs.CN.toUpperCase().trim()).prop("readonly", true);
                    $("#" + btntoshow).removeClass('d-none');

                }
            }
            else if (data[0].Status === '404') {
                $("#" + msgid).html(`<div class="alert alert-danger" style="margin-top:5px;><i class="fa fa-check" aria-hidden="true" ></i><span class="m-lg-2">${data[0].Remarks}</span></div>`);
                $("#" + txtArmyNo).val("");
                $("#" + txtName).val("");
                $("#" + btntoshow).addClass('d-none');
            }
        }
        else {
            $("#" + msgid).html("Someting Went Wrong");
            $("#" + txtArmyNo).val("");
            $("#" + txtName).val("");
            $("#" + btntoshow).addClass('d-none');
            return 0;
        }
    }
    catch (error) {

        $("#" + msgid).html(`<div  class="alert alert-danger" style="margin-top:5px;><i class="fa fa-times" aria-hidden="true" >
        </i><span class="m-lg-2 text-danger alert-danger tokenremarks">DGIS App Not running
        </span </div>
       <a class="alert-info" href="https://dgis.army.mil" style="padding:5px; text-align:right; font-size:12px">Click To Download Dgis App For Digital Sign</a>
        `);
        $("#" + txtArmyNo).val("");
        $("#" + txtName).val("");
        $("#" + btntoshow).addClass('d-none');

    }
}