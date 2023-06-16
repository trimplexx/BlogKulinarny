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

function updateBorders() {
    var checkboxes = document.getElementsByClassName('checkbox-class');
    for (var i = 0; i < checkboxes.length; i++) {
        var checkbox = checkboxes[i];
        var container = checkbox.closest('.border');

        if (checkbox.checked) {
            container.classList.remove('border-warning');
            container.classList.remove('bg-light');
            container.classList.add('border-success');
            container.classList.add('bg-secondary');
        } else {
            container.classList.remove('border-success');
            container.classList.remove('bg-secondary');
            container.classList.add('border-warning');
            container.classList.add('bg-light');
        }

        var childContainers = container.querySelectorAll('.border');
        for (var j = 0; j < childContainers.length; j++) {
            var childContainer = childContainers[j];
            if (checkbox.checked) {
                childContainer.classList.remove('border-warning');
                childContainer.classList.remove('bg-light');
                childContainer.classList.add('border-success');
                childContainer.classList.add('bg-secondary');
                childContainer.classList.add('text-decoration-line-through');
            } else {
                childContainer.classList.remove('border-success');
                childContainer.classList.remove('bg-secondary');
                childContainer.classList.remove('text-decoration-line-through');
                childContainer.classList.add('border-warning');
                childContainer.classList.add('bg-light');
            }
        }
    }
}