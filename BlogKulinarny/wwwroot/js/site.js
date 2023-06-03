// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/* Funkcja odblokowująca i blokujący przyciski radio jeżeli checkbox nie jest check. */
function enableRadioButtons() {
    var checkbox = document.getElementById("flexCheckDefault3");
    var radioAlphabetical = document.getElementById("sortEasiest");
    var radioDifficulty = document.getElementById("sortHardest");

    if (checkbox.checked) {
        radioAlphabetical.disabled = false;
        radioDifficulty.disabled = false;
    } else {
        radioAlphabetical.disabled = true;
        radioDifficulty.disabled = true;
    }
}