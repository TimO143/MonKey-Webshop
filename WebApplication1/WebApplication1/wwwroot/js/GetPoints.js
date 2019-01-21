function GetPoints() {
    $.ajax({
        url: "/api/Points",
        method: "GET", // post is safer, but could also be GET
        data: {}, // any data (as a JSON object) you want to pass to the method
        contentType: "application/json;",
        dataType: "json",
        success: function (response) {
            document.getElementById("DiamondPoints").innerHTML = response;
        },
        failure: function (response) {
            alert(response.d);
            document.getElementById("DiamondPoints").innerHTML = response.d;
        }
    });
}