var serviceURL = "./MdallJson/GetAllListForJsonByCategory";

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
     results = regex.exec(location.search);
     return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));    
}


function OnFail(result) {
    window.location.href = "./genericError.html";
}



function getDeviceListInfo(data) {
    if (data.length == 0) {
        return "";
    }
    var deviceDetail = "";

    var txt = "";
    var i;
    for (i = 0; i < data.length; i++) {

       // console.log("deviceIdentifierList" + i + ":" + data[i].deviceIdentifierList.length);
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


