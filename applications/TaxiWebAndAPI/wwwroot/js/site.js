function randomTaxi(idResult) {
    $("#" + idResult).text('please wait');
    var urlTaxi = "/api/Taxi/GetRandomTaxiNumber";
    $.ajax({
        url: urlTaxi,
        method: "GET"
    })
        .done(function (data) {
            displayResult(data, idResult);
        })
        .fail(function (xhr) {
            window.alert('error' + xhr);
            displayResult(data, idResult);
        });
}
function findTaxi(idPlateText, idResult) {

    $("#" + idResult).text('please wait');
    var plate = $("#" + idPlateText).val();
    if (plate== null || plate.length == 0)
        return;

    var urlTaxi = "/api/Taxi/GetTaxi?plateNumber=" + plate;
    $.ajax({
        url: urlTaxi,
        method: "GET"
        })
        .done(function (data) {
            displayResult(data, idResult);
        })
        .fail(function (xhr) {
            window.alert('error' + xhr);
            displayResult(data, idResult);
        });
}
function findTaxiPicture(base64Text, idResult) {

    $("#" + idResult).text('please wait');
    
    if (base64Text == null || base64Text.length == 0)
        return;

    var urlTaxi = "/api/Taxi/GetFromPicture";
    $.ajax({
        url: urlTaxi,
        method: "POST",
        data: { base64Picture: base64Text}
    })
        .done(function (data) {
            displayResult(data, idResult);
        })
        .fail(function (xhr) {
            window.alert('error' + xhr);
            displayResult(data, idResult);
        });
}
function displayResult(data, idResult) {
    
    if (data === undefined) {
        window.alert('no taxi');
        $("#result").html('no taxi registered');
    }
    $("#result").html("<b>"+JSON.stringify(data).replace(/["']/g,"")+"</b>");

}


