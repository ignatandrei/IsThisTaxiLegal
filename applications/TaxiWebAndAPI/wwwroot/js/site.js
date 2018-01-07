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
function displayResult(data, idResult) {
    
    if (data === undefined) {
        window.alert('no taxi');
        ("#result").text('no taxi registered');
    }
    $("#result").text(JSON.stringify(data));

}


