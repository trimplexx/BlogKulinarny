
window.addEventListener('DOMContentLoaded', (event) => {
    var difficultyLabel = document.getElementById('difficultyLabel');
    difficultyLabel.style.color = '#9c7c27';
    difficultyLabel.style.fontWeight = 'bold';
});

function updateDifficultyLabel(range) {
    var difficultyLabel = document.getElementById('difficultyLabel');
    var value = range.value;

    if (value <= 25) {
        difficultyLabel.textContent = 'Łatwy';
        difficultyLabel.style.color = 'green';
    } else if (value <= 60) {
        difficultyLabel.textContent = 'Średni';
        difficultyLabel.style.color = '#9c7c27';
    } else if (value <= 80) {
        difficultyLabel.textContent = 'Trudny';
        difficultyLabel.style.color = '#91411f';
    } else {
        difficultyLabel.textContent = 'Bardzo trudny';
        difficultyLabel.style.color = '#522121';
    }
}


var stepsCount = 1;

function addStep() {

    stepsCount++;
    event.preventDefault();
    var stepTile = document.getElementById("stepTile");
    var clone = stepTile.cloneNode(true);
    document.getElementById("stepTile").after(clone);
}


function removeStep(button) {

    var stepTile = button.closest(".border");
    var stepTiles = document.querySelectorAll(".border");

    if (stepsCount > 1) {
        stepsCount--;
        stepTile.remove();
    } else {
        alert("Musisz zachować przynajmniej jeden kafelek.");
    }
}