
function JSONtoCSV(fileName, url) {

    var json = $.getJSON(url, function (json) {
      
        var row = "";
        var csv = '';
        
        var arrData = typeof json != 'object' ? JSON.parse(json) : json;

        

        //extract the first row of data as headers
        for (var index in arrData[0]) {
            row += index + ',';
        }

        row.slice(0, -1);

        csv += row + '\r\n';

        for (var i = 0; i < arrData.length; i++) {
            var row = "";
            for (var index in arrData[i]) {
                row += '"' + arrData[i][index] + '",';          //create rows from the JSON data objects
            }
            row.slice(0, row.length);

            csv += row + '\r\n';                               //add it to the CSV string
        }

        

        blob = new Blob([csv], { type: 'text/csv' });

        var csvUrl = window.URL.createObjectURL(blob);

        

        $("<a />", {
            "download": fileName + '.csv',
            "href": csvUrl,
        }).appendTo("body")     //create, append and remove a download link
                        .click(function () {
                            $(this).remove()
                        })[0].click()
    });


}


function csvPackage(fileName, url)
{
    var zip = new JSZip();
    
    var zipName = "";

    var completedFiles = 0; 

    $.each(url, function (j) {

        zipName += fileName[j] + ", ";
        
        $.getJSON(url[j], function (json) {

            var row = "";
            var csv = '';

            var arrData = typeof json != 'object' ? JSON.parse(json) : json;

            //extract the first row of data as headers
            for (var index in arrData[0]) {
                row += index + ',';
            }

            row.slice(0, -1);
            csv += row + '\r\n';

            for (var i = 0; i < arrData.length; i++) {
                var row = "";
                for (var index in arrData[i]) {
                    row += '"' + arrData[i][index] + '",';          //create rows from the JSON data objects
                }
                row.slice(0, row.length);

                csv += row + '\r\n';                               //add it to the CSV string
            }

            blob = new Blob([csv], { type: 'text/csv' });

            var csvUrl = window.URL.createObjectURL(blob);

            zip.file(fileName[j] + ".csv", blob);

        })
            .error(function()
            {
                console.log("Could not download " + fileName[j]);
                completedFiles++;
            })

            .done(function () {

            completedFiles++;

            if (completedFiles == url.length) {

                zipName = zipName.slice(0, (zipName.length -2));
                zipName += " Data";

                zip.generateAsync({
                    type: "blob"
                }).then(function (content) {
                    saveAs(content, zipName + ".zip");
                    //window.location.href = "data:application/zip;base64," + content;
                });

            }



        });
    }); //end .each();

}
