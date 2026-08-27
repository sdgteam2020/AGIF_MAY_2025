const errormsg002 = "Something Went Wrong";

function mMsater(sectid, ddl, TableId, ParentId) {
    const userdata = {
        id: TableId,
        ParentId: ParentId
    };

    var token = $('input[name="__RequestVerificationToken"]').val();

    var tokenRegex = /^[a-zA-Z0-9_\-]+$/;

    if (!token || !tokenRegex.test(token)) {
        console.error("Security Error: Invalid Anti-Forgery Token format.");
        Swal.fire({ text: errormsg002 });
        return; // Abort execution before the AJAX call is made
    }

    $.ajax({
        url: '/Master/GetAllMMaster',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        "headers": {
            "RequestVerificationToken": token
        },
        success: function (response) {
            if (response !== "null" && response != null) {
                if (response === 0 || response === -1) {
                    Swal.fire({ text: errormsg002 });
                } else {
                    let listItemddl = '<option value="">Please Select</option>';

                    if (Array.isArray(response)) {
                        response.forEach(item => {
                            listItemddl += `<option value="${item.id}">${item.name}</option>`;
                        });
                    }

                    //$("#" + ddl).html(listItemddl);

                    //if (sectid !== '') {
                    //    $("#" + ddl).val(sectid);
                    //}

                    $("#" + ddl).html(listItemddl);

                    if (sectid !== '') {
                      $("#" + ddl).prop('selectedIndex', 0);
                    }
                }
            }
        },
        error: function () {
            Swal.fire({ text: errormsg002 });
        }
    });
}

async function GetTokenDetails(txtArmyNo, txtName, msgid, btntoshow) {
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
                    $("#" + msgid).html('<div class="alert alert-danger register"><i class="fa fa-times" aria-hidden="true" ></i><span class="m-lg-2">Token Expired</span>.</div>');
                    $("#" + txtArmyNo).val("");
                    $("#" + txtName).val("");
                    $("#" + btntoshow).addClass('d-none');
                    return false;
                } else {
                    $("#" + msgid).html('<div class="alert alert-success register"><i class="fa fa-check" aria-hidden="true" ></i><span class="m-lg-2">Token Detected</span></div>');
                    $("#" + txtArmyNo).val(keyValuePairs.SERIALNUMBER.toUpperCase().trim());
                    $("#" + txtName).val(keyValuePairs.CN.toUpperCase().trim()).prop("readonly", true);
                    $("#" + btntoshow).removeClass('d-none');
                    return true;
                }
            }
            else if (data[0].Status === '404') {
                $("#" + msgid).html(`<div class="alert alert-danger register"><i class="fa fa-check" aria-hidden="true" ></i><span class="m-lg-2">${data[0].Remarks}</span></div>`);
                $("#" + txtArmyNo).val("");
                $("#" + txtName).val("");
                $("#" + btntoshow).addClass('d-none');
                return false;

            }
        }
        else {
            $("#" + msgid).html("Something Went Wrong");
            $("#" + txtArmyNo).val("");
            $("#" + txtName).val("");
            $("#" + btntoshow).addClass('d-none');
            //return 0;
            return false;

        }
    }
    catch (error) {
        $("#" + msgid).html(`<div class="alert alert-danger register"><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2 text-danger alert-danger tokenremarks">DGIS App Not running</span></div>
       <a class="alert-info register-msg" href="https://dgis.army.mil">Click To Download Dgis App For Digital Sign</a>`);
        $("#" + txtArmyNo).val("");
        $("#" + txtName).val("");
        $("#" + btntoshow).addClass('d-none');
    }
}

//let _0x9258f; const errormsg002 = "\u0053\u006F\u006D\u0065\u0074\u0068\u0069\u006E\u0067\u0020\u0057\u0065\u006E\u0074\u0020\u0057\u0072\u006F\u006E\u0067"; _0x9258f = (873930 ^ 873929) + (438878 ^ 438878); function mMsater(sectid, ddl, TableId, ParentId) { const _0x2f67df = { "id": TableId, '\u0050\u0061\u0072\u0065\u006E\u0074\u0049\u0064': ParentId }; var _0xage89g = $("\u0069\u006E\u0070\u0075\u0074\u005B\u006E\u0061\u006D\u0065\u003D\u0022\u005F\u005F\u0052\u0065\u0071\u0075\u0065\u0073\u0074\u0056\u0065\u0072\u0069\u0066\u0069\u0063\u0061\u0074\u0069\u006F\u006E\u0054\u006F\u006B\u0065\u006E\u0022\u005D")['\u0076\u0061\u006C'](); var _0x177d = new RegExp('\u005E\u005B\u0061\u002D\u007A\u0041\u002D\u005A\u0030\u002D\u0039\u005F\u005C\u002D\u005D\u002B\u0024', ""); if (!_0xage89g || !_0x177d['\u0074\u0065\u0073\u0074'](_0xage89g)) { console['\u0065\u0072\u0072\u006F\u0072']("\u0053\u0065\u0063\u0075\u0072\u0069\u0074\u0079\u0020\u0045\u0072\u0072\u006F\u0072\u003A\u0020\u0049\u006E\u0076\u0061\u006C\u0069\u0064\u0020\u0041\u006E\u0074\u0069\u002D\u0046\u006F\u0072\u0067\u0065\u0072\u0079\u0020\u0054\u006F\u006B\u0065\u006E\u0020\u0066\u006F\u0072\u006D\u0061\u0074\u002E"); Swal['\u0066\u0069\u0072\u0065']({ '\u0074\u0065\u0078\u0074': errormsg002 }); return; } $['\u0061\u006A\u0061\u0078']({ "url": "\u002F\u004D\u0061\u0073\u0074\u0065\u0072\u002F\u0047\u0065\u0074\u0041\u006C\u006C\u004D\u004D\u0061\u0073\u0074\u0065\u0072", "contentType": 'application/x-www-form-urlencoded', "data": _0x2f67df, '\u0074\u0079\u0070\u0065': "\u0050\u004F\u0053\u0054", "\u0068\u0065\u0061\u0064\u0065\u0072\u0073": { "\u0052\u0065\u0071\u0075\u0065\u0073\u0074\u0056\u0065\u0072\u0069\u0066\u0069\u0063\u0061\u0074\u0069\u006F\u006E\u0054\u006F\u006B\u0065\u006E": _0xage89g }, "success": function (response) { if (response !== "\u006E\u0075\u006C\u006C" && response != null) { if (response === (842480 ^ 842480) || response === -(362766 ^ 362767)) { Swal['\u0066\u0069\u0072\u0065']({ '\u0074\u0065\u0078\u0074': errormsg002 }); } else { var _0x031ae = (288251 ^ 288253) + (954523 ^ 954514); let _0x36603e = ">noitpo/<tceleS esaelP>\"\"=eulav noitpo<".split("").reverse().join(""); _0x031ae = (210839 ^ 210839) + (478845 ^ 478836); if (Array['\u0069\u0073\u0041\u0072\u0072\u0061\u0079'](response)) { response['\u0066\u006F\u0072\u0045\u0061\u0063\u0068'](item => { _0x36603e += `<option value="${item['\u0069\u0064']}">${item['\u006E\u0061\u006D\u0065']}</option>`; }); } $("\u0023" + ddl)['\u0068\u0074\u006D\u006C'](_0x36603e); if (sectid !== '') { $("\u0023" + ddl)['\u0070\u0072\u006F\u0070']("xednIdetceles".split("").reverse().join(""), 729649 ^ 729649); } } } }, '\u0065\u0072\u0072\u006F\u0072': function () { Swal['\u0066\u0069\u0072\u0065']({ '\u0074\u0065\u0078\u0074': errormsg002 }); } }); } async function GetTokenDetails(txtArmyNo, txtName, msgid, btntoshow) {
//    try { var _0x7bc53f = (367334 ^ 367335) + (178925 ^ 178921); const _0x179a2d = await fetch("\u0068\u0074\u0074\u0070\u0073\u003A\u002F\u002F\u0064\u0067\u0069\u0073\u0061\u0070\u0070\u002E\u0061\u0072\u006D\u0079\u002E\u006D\u0069\u006C\u003A\u0035\u0035\u0031\u0030\u0032\u002F\u0054\u0065\u006D\u0070\u006F\u0072\u0061\u0072\u0079\u005F\u004C\u0069\u0073\u0074\u0065\u006E\u005F\u0041\u0064\u0064\u0072\u0065\u0073\u0073\u0065\u0073\u002F\u0046\u0065\u0074\u0063\u0068\u0055\u006E\u0069\u0071\u0075\u0065\u0054\u006F\u006B\u0065\u006E\u0044\u0065\u0074\u0061\u0069\u006C\u0073", { '\u006D\u0065\u0074\u0068\u006F\u0064': "\u0047\u0045\u0054", "cache": "\u006E\u006F\u002D\u0063\u0061\u0063\u0068\u0065", "headers": { "\u0041\u0063\u0063\u0065\u0070\u0074": "application/json" } }); _0x7bc53f = (168090 ^ 168093) + (143605 ^ 143605); var _0xe2e4b = (289042 ^ 289040) + (307895 ^ 307891); const _0x53c48a = await _0x179a2d['\u006A\u0073\u006F\u006E'](); _0xe2e4b = 960160 ^ 960164; if (_0x53c48a && _0x53c48a['\u006C\u0065\u006E\u0067\u0074\u0068'] > (174864 ^ 174864)) { if (_0x53c48a[829329 ^ 829329]['\u0053\u0074\u0061\u0074\u0075\u0073'] === "\u0032\u0030\u0030") { let _0x669deg; let _0xc5c = _0x53c48a[429130 ^ 429130]['\u0073\u0075\u0062\u006A\u0065\u0063\u0074']['\u0073\u0070\u006C\u0069\u0074']("\u002C\u0020"); _0x669deg = '\u0063\u0068\u0062\u006C\u006E\u0067'; let _0x49d1c = {}; _0xc5c['\u0066\u006F\u0072\u0045\u0061\u0063\u0068'](pair => { let [k, v] = pair['\u0073\u0070\u006C\u0069\u0074']("\u003D"); _0x49d1c[k['\u0074\u0072\u0069\u006D']()] = v ? v['\u0074\u0072\u0069\u006D']() : ""; }); var _0xb1dg8d = (712016 ^ 712025) + (814619 ^ 814623); const _0x_0x6ga = new Date(); _0xb1dg8d = 458451 ^ 458455; if (_0x53c48a[603915 ^ 603915]['\u0056\u0061\u006C\u0069\u0064\u0054\u006F'] >= _0x_0x6ga) { $("\u0023" + msgid)['\u0068\u0074\u006D\u006C'](">vid/<.>naps/<deripxE nekoT>\"2-gl-m\"=ssalc naps<>i/<> \"eurt\"=neddih-aira \"semit-af af\"=ssalc i<>\";xp5:pot-nigram\"=elyts \"regnad-trela trela\"=ssalc vid<".split("").reverse().join("")); $("\u0023" + txtArmyNo)['\u0076\u0061\u006C'](""); $("\u0023" + txtName)['\u0076\u0061\u006C'](""); $("\u0023" + btntoshow)['\u0061\u0064\u0064\u0043\u006C\u0061\u0073\u0073']("\u0064\u002D\u006E\u006F\u006E\u0065"); } else { $("\u0023" + msgid)['\u0068\u0074\u006D\u006C'](">vid/<>naps/<detceteD nekoT>\"2-gl-m\"=ssalc naps<>i/<> \"eurt\"=neddih-aira \"kcehc-af af\"=ssalc i<>\";xp5:pot-nigram\"=elyts \" sseccus-trela trela\"=ssalc vid<".split("").reverse().join("")); $("\u0023" + txtArmyNo)['\u0076\u0061\u006C'](_0x49d1c['\u0053\u0045\u0052\u0049\u0041\u004C\u004E\u0055\u004D\u0042\u0045\u0052']['\u0074\u006F\u0055\u0070\u0070\u0065\u0072\u0043\u0061\u0073\u0065']()['\u0074\u0072\u0069\u006D']()); $("\u0023" + txtName)['\u0076\u0061\u006C'](_0x49d1c['\u0043\u004E']['\u0074\u006F\u0055\u0070\u0070\u0065\u0072\u0043\u0061\u0073\u0065']()['\u0074\u0072\u0069\u006D']())['\u0070\u0072\u006F\u0070']("\u0072\u0065\u0061\u0064\u006F\u006E\u006C\u0079", !![]); $("\u0023" + btntoshow)['\u0072\u0065\u006D\u006F\u0076\u0065\u0043\u006C\u0061\u0073\u0073']("\u0064\u002D\u006E\u006F\u006E\u0065"); } } else if (_0x53c48a[229453 ^ 229453]['\u0053\u0074\u0061\u0074\u0075\u0073'] === "404".split("").reverse().join("")) { $("\u0023" + msgid)['\u0068\u0074\u006D\u006C'](`<div class="alert alert-danger" style="margin-top:5px;"><i class="fa fa-check" aria-hidden="true" ></i><span class="m-lg-2">${_0x53c48a[678614 ^ 678614]['\u0052\u0065\u006D\u0061\u0072\u006B\u0073']}</span></div>`); $("\u0023" + txtArmyNo)['\u0076\u0061\u006C'](""); $("\u0023" + txtName)['\u0076\u0061\u006C'](""); $("\u0023" + btntoshow)['\u0061\u0064\u0064\u0043\u006C\u0061\u0073\u0073']("enon-d".split("").reverse().join("")); } } else { $("\u0023" + msgid)['\u0068\u0074\u006D\u006C']("gnorW tneW gnihtemoS".split("").reverse().join("")); $("\u0023" + txtArmyNo)['\u0076\u0061\u006C'](""); $("\u0023" + txtName)['\u0076\u0061\u006C'](""); $("\u0023" + btntoshow)['\u0061\u0064\u0064\u0043\u006C\u0061\u0073\u0073']("\u0064\u002D\u006E\u006F\u006E\u0065"); return 585551 ^ 585551; } } catch (error) {
//        $("\u0023" + msgid)['\u0068\u0074\u006D\u006C'](`<div class="alert alert-danger" style="margin-top:5px;"><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2 text-danger alert-danger tokenremarks">DGIS App Not running</span></div>
//       <a class="alert-info" href="https://dgis.army.mil" style="padding:5px; text-align:right; font-size:12px">Click To Download Dgis App For Digital Sign</a>`); $("\u0023" + txtArmyNo)['\u0076\u0061\u006C'](""); $("\u0023" + txtName)['\u0076\u0061\u006C'](""); $("\u0023" + btntoshow)['\u0061\u0064\u0064\u0043\u006C\u0061\u0073\u0073']("\u0064\u002D\u006E\u006F\u006E\u0065");
//    }
//}