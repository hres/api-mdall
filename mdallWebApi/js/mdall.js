//var mdall = "http://localhost:51195/Controllers/mdallController.ashx?";

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
     results = regex.exec(location.search);
     return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));    
}

function goMdallUrl(lang, status, term) {
    var searchUrl = mdall + "lang=" + lang + "&status=" + status + "&term=" + term;
    return searchUrl;
}

function goMdallLangUrl(lang, pType) {
    var term = getParameterByName("term");
    var langSwitch = lang == 'en' ? "fr" : "en";
    var langUrl = lang == 'fr' ? "instruments-medicaux-resultat.html" : "medical-device-result.html";
    langUrl += "?term=" + term + "&pType=" + pType + "&lang=" + langSwitch;
    return langUrl;
}

function goCompanyUrlByID(lang) {
    var companyId = getParameterByName("companyId");
    var goCompanyByIDUrl = mdall + "lang=" + lang + "&companyId=" + companyId;
    console.log(goCompanyByIDUrl);
    return goCompanyByIDUrl;
}

function goCompanyUrl(lang, term) {
    //http://localhost:51195/api-v1/company/?type=json&status=active&company_name=bayer
    var goCompanyUrl = mdall + "company/?type=json&status=active&company_name=" + term;
    console.log(goCompanyUrl);
    return goCompanyUrl;
}

function OnFail(result) {
    window.location.href = "./genericError.html";
}



function getDeviceListInfo(data) {
    if (data.length == 0) {
        return "";
    }
    var deviceDetail = "";

    //console.log("DeviceList :" + data.length);
    var txt = "";
    var i;
    for (i = 0; i < data.length; i++) {

        console.log("deviceIdentifierList" + i + ":" + data[i].deviceIdentifierList.length);
        if (data[i].deviceIdentifierList.length == 1) {
            if ($.trim(data[i].device_first_issue_dt) != '') {
                deviceDetail += "<tr><td>" + formatedDate(data[i].device_first_issue_dt) + "</td>";
            }
            if ($.trim(data[i].trade_name) != '') {
                deviceDetail += "<td>" + data[i].trade_name + "</td>";
            }
            if ($.trim(data[i].deviceIdentifierList[0].identifier_first_issue_dt) != '')
            {
                deviceDetail += "<td>" + formatedDate(data[i].deviceIdentifierList[0].identifier_first_issue_dt) + "</td>";
            }
            else
            {
                deviceDetail += "<td>&nbsp;</td>";
            }
            deviceDetail += "<td>" + data[i].deviceIdentifierList[0].device_identifier + "</td></tr>";
        }
        else
        {
            //console.log("Im here" + i + ":" + data[i].deviceIdentifierList);
            $.each(data[i].deviceIdentifierList, function (index, record) {
                if (index == 0) {
                    //console.log("Im here index =" + index);
                    if ($.trim(data[i].device_first_issue_dt) != '') {
                        deviceDetail += "<tr><td scope='rowgroup' rowspan='" + data[i].deviceIdentifierList.length + "'>" + formatedDate(data[i].device_first_issue_dt) + "</td>";

                    }
                    if ($.trim(data[i].trade_name) != '') {
                        deviceDetail += "<td scope='rowgroup' rowspan='" + data[i].deviceIdentifierList.length + "'>" + data[i].trade_name + "</td>";
                    }
                    if ($.trim(data[i].deviceIdentifierList[0].identifier_first_issue_dt) != '') {
                        deviceDetail += "<td>" + formatedDate(data[i].deviceIdentifierList[0].identifier_first_issue_dt) + "</td>";
                    }
                    else {
                        deviceDetail += "<td>&nbsp;</td>";
                    }

                    deviceDetail += "<td>" + data[i].deviceIdentifierList[0].device_identifier + "</td></tr>";
                    //console.log(i + "= " + deviceDetail);
                }
                else {
                        if (index > 0) {
                            //console.log("Im here index =" + index);
                            deviceDetail += "<tr><td>" + formatedDate(record.identifier_first_issue_dt) + "</td>";
                            deviceDetail += "<td>" + record.device_identifier + "</td></tr>";
                        }            
                }
                
            });
        }
        //console.log("i" + i +":" + deviceDetail);
    }
    
    if (deviceDetail != '') {
        deviceDetail = deviceDetail.replace("undefined", "");
        deviceDetail = deviceDetail.replace(/"/g, "");
    }

    var devieTable = "<table class='table'>" +
                            "<thead>" +
                                "<tr class='active'>" +
                                    "<th>Device first issue date</th>" +
                                    "<th>Device name</th>" +
                                    "<th>Identifier first issue date</th>" +
                                    "<th>Device identifier</th>" +
                                "</tr>" +
                            "</thead>" +
                            "<tbody>" + deviceDetail + "</tbody>" +
                        "</table>";
    //console.log(devieTable);
    return devieTable;
}


function formatedAddress(data) {
    var address;
    if ($.trim(data.company_name) == '') {
        return "";
    }
    address = data.company_name + "<br />";

    if (data.addr_line_1 != '') {
        address += data.addr_line_1 + "&nbsp;";

        if (data.addr_line_2 != '') {
            address += data.addr_line_2 + "&nbsp;";
        } 

        if (data.addr_line_3 != '') {
            address += data.addr_line_3 + "&nbsp;";
        }
        address +=  "<br />"
    }
    
    if (data.city != '') {
        address += data.city + ",&nbsp;";
    }
    if (data.region_cd != '') {
        address += data.region_cd + ",&nbsp;";
    }
    if (data.country_cd != '') {
        address += data.country_cd + ",&nbsp;";
    }

    if (data.postal_code != '') {
        address +=  data.postalCode;
    }
    if (address != '') {
        address = address.replace("undefined", "");
        return address;
    }
    return "&nbsp;";
}

function formatedDate(data) {
        if ($.trim(data) == '') {
            return "";
        }
        var data = data.replace("/Date(", "").replace(")/", "");
        if (data.indexOf("+") > 0) {
            data = data.substring(0, data.indexOf("+"));
        }
        else if (data.indexOf("-") > 0) {
            data = data.substring(0, data.indexOf("-"));
        }
        var date = new Date(parseInt(data, 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return date.getFullYear() + "-" + month + "-" + currentDate;
}

function formatedOrderedList(data) {
    var list;
    if ($.trim(data) == '') {
        return "";
    }
    $.each(data, function (index, record) {
        list += "<li>" + record.device_identifier + "</li>";
    });

    if (list != '') {
        list = list.replace("undefined", "");
        list = list.replace(/"/g, "");
        return "<ul>" + list + "</ul>";;
    }
    return "";
}


var serviceURL = "./MdallJson/GetAllListForJsonByCategory";
function defaultTab(lang, status, searchTerm, category) {
    window["wb-tables"] = {
        "processing": true,
        "destroy": true,
        "ajax": {
            "url": serviceURL,
            "data": { lang: lang, status: status, term: searchTerm, categoryType: 1 },
            "dataType": "json",
            "dataSrc": "companyResult",
            "error": function (xhr, err) {
                alert("readyState: " + xhr.readyState + "\nstatus: " + xhr.status);
                alert("responseText: " + xhr.responseText);
            },
        },
        "columns": [
                        { "data": "company_name" },
                        {

                            "data": null,
                            "render": function (data, type, full) {
                                return data.addr_line_1 + ' ' + data.addr_line_2 + ' ' + data.addr_line_3 + '<br>' + data.city + ', ' + data.region_cd + ', ' + data.country_cd + ', ' + data.postal_code;
                            }
                        },
                        {
                            "data": null,
                            "render": function (data, type, full) {
                                return '<a href=medical-device-detail.html?lang=en&id=' + data.company_id + '>' + data.company_id + '</a>';
                            }
                        }

        ]
    }
}

function licenceNameTab(lang, status, searchTerm, category) {
    alert("Im panel 222 ");
    //$('.wb-tables').on('xhr.dt', function (e, settings, json, xhr) {
       // var a = json.data.length;
    // console.log(a);

    //$(".wb-tables").on("wb-updated.wb-tables", function (event, settings) {
    //    alert("Im panel 2 updated ");
    //});
    window["wb-tables"] = {
        "processing": true,
        "destroy": true,
        "ajax": {
            "url": serviceURL,
            "data": { lang: lang, status: status, term: searchTerm, categoryType: 2 },
            "dataType": "json",
            "dataSrc": "licenceResult",
            "error": function (xhr, err) {
                alert("readyState: " + xhr.readyState + "\nstatus: " + xhr.status);
                alert("responseText: " + xhr.responseText);
            },
        },
        "columns": [
                        {
                            "data": null,
                            "render": function (data, type, full) {
                                return '<a href=medical-device-detail.html?lang=en&id=' + data.application_id + '>' + data.licence_name + '</a>';
                            }
                        },
                        { "data": "licence_type_desc" },
                        {
                            "data": null,
                            "render": function (data, type, full) {
                                return '<a href=medical-device-detail.html?lang=en&id=' + data.company_id + '>' + data.company_id + '</a>';
                            }
                        }

        ]
    }
   // });
}
