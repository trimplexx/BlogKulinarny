window.addEventListener('DOMContentLoaded', (event) => {
    var difficultyRange = document.getElementById('difficultyRange');
    updateDifficultyLabel(difficultyRange);

    addEventListenersForExistingSteps();

    if (difficultyRange) {
        difficultyRange.addEventListener('input', function () {
            updateDifficultyLabel(difficultyRange);
        });
    }
});

// Funkcja do dodawania nasłuchiwania zdarzeń dla istniejących kroków
function addEventListenersForExistingSteps() {
    var stepElements = document.querySelectorAll("#stepContainer > div");
    stepElements.forEach(function (stepElement, index) {
        addEventListenersForStep(stepElement, index);
    });
}

// Funkcja do dodawania nasłuchiwania zdarzeń dla nowo utworzonych kroków
function addEventListenersForStep(stepElement, index) {
    var removeBtn = stepElement.querySelector('.btn-danger');
    removeBtn.addEventListener('click', function () {
        removeStepEdit(index);
    });
}

// suwak poziomu trudności
function updateDifficultyLabel(range) {
    var difficultyLabel = document.getElementById('difficultyLabel');
    var value = range.value;

    if (value <= 25) {
        difficultyLabel.textContent = 'Łatwy';
        difficultyLabel.style.color = 'green';
    } else if (value <= 50) {
        difficultyLabel.textContent = 'Średni';
        difficultyLabel.style.color = '#9c7c27';
    } else if (value <= 75) {
        difficultyLabel.textContent = 'Trudny';
        difficultyLabel.style.color = '#91411f';
    } else {
        difficultyLabel.textContent = 'Bardzo trudny';
        difficultyLabel.style.color = '#522121';
    }
}

function removeStepEdit(index) {
    var stepContainer = document.getElementById("stepContainer");
    var stepElement = document.getElementById("step-" + index);
    if (stepContainer && stepElement) {
        stepContainer.removeChild(stepElement);
    }
}

function saveStepsToViewModel() {
    try {
        var saveStepsInput = document.getElementById('editRecipe.saveSteps');
        var saveStepsInputValues = [];
        var stepElements = document.querySelectorAll("#stepContainer > div");

        stepElements.forEach(function (stepElement) {
            var imageInput = stepElement.querySelector('.form-control[id^="eleImage"]');
            var descriptionTextarea = stepElement.querySelector('.form-control[id^="eleDetails"]');

            if (imageInput && descriptionTextarea) {
                var imageInputValue = imageInput.value;
                var descriptionTextareaValue = descriptionTextarea.value;

                console.log("imageInput:", imageInputValue);
                console.log("descriptionTextarea:", descriptionTextareaValue);

                var stepInputValue = [descriptionTextareaValue, imageInputValue].join(',');
                saveStepsInputValues.push(stepInputValue);
            }
        });

        console.log("saveStepsInputValues:", saveStepsInputValues);

        if (saveStepsInput) {
            saveStepsInput.value = saveStepsInputValues.join(',');
            console.log("saveStepsInput.value:", saveStepsInput.value);
        }
    } catch (error) {
        console.error("Wystąpił błąd:", error);
    }
}

