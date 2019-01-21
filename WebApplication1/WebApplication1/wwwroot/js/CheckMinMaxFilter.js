function checkmin() {
    var minimum = parseInt(document.getElementById("min").value, 10);
    var maximum = parseInt(document.getElementById("max").value, 10);

    console.log("Minimum value: " + minimum);
    console.log("Maximum value: " + maximum);

    if ((minimum < maximum && minimum > 0 && maximum > 1) || (minimum == 0  && maximum == 0) || minimum.isNaN() || maximum.isNaN) {
        return true;
    } else {
        alert("The minimum value cannot be higher then the maximum value!");
        return false;
    }
}

function checkMinRealTime() {
    var minimum = parseInt(document.getElementById("min").value, 10);
    var maximum = parseInt(document.getElementById("max").value, 10);

    if (minimum > 0 && maximum > 0) {

        if (minimum < 1) {
            minimum = 1;
        }

        if (maximum < 2) {
            maximum = 2;
        }

        if (minimum > maximum) {
            minimum = maximum - 1;
        }
    }

    document.getElementById("min").value = minimum;
    document.getElementById("max").value = maximum;
}