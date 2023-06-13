window.addEventListener('DOMContentLoaded', (event) => {
    var difficultyRange = document.getElementById('difficultyRange');
    updateDifficultyLabel(difficultyRange);

    var addBtn = document.getElementById("addBtn");
    addBtn.addEventListener('click', function () {
        createStep();
    });

});

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

function saveStepsToViewModel() {
    try {
        var saveStepsInput = document.getElementById('editRecipe.saveSteps');
        var saveStepsInputValues = [];
        var stepElements = document.querySelectorAll("#stepContainer > div");

        stepElements.forEach(function (stepElement, index) {
            var imageInput = stepElement.querySelector('.step-image');
            var descriptionTextarea = stepElement.querySelector('.step-description');

            if (imageInput && descriptionTextarea) {
                var imageInputValue = imageInput.value;
                var descriptionTextareaValue = descriptionTextarea.value;

                console.log("imageInput:", imageInputValue);
                console.log("descriptionTextarea:", descriptionTextareaValue);

                var stepInputValue = [imageInputValue, descriptionTextareaValue].join(',');
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

function createStep() {
    event.preventDefault();
    var index = loadLastStep();
    var stepTitle = document.getElementById("step-" + index);
    var clone = stepTitle.cloneNode(true);

    // Resetowanie wartości pól tekstowych
    var clonedImageInput = clone.querySelector(".step-image");
    clonedImageInput.value = "";

    var clonedDescriptionTextarea = clone.querySelector(".step-description");
    clonedDescriptionTextarea.value = "";

    var stepContainer = document.getElementById("stepContainer");
    stepContainer.insertAdjacentElement("beforeend", clone);

    // Dodawanie nasłuchiwania zdarzeń dla nowo utworzonych elementów
    addEventListenersForStep(clone);
}

function loadLastStep() {
    var lastStepElement = document.querySelector('#stepContainer > div:last-child');
    if (lastStepElement) {
        var lastStepId = lastStepElement.id;
        var lastStepIndex = lastStepId.split('-')[1];
        console.log("ID ostatniego elementu:", lastStepId);
        console.log("Indeks ostatniego elementu:", lastStepIndex);
        return lastStepIndex;
    }
}

function removeStepEdit(index) {
    var stepContainer = document.getElementById("stepContainer");
    var stepElement = document.getElementById("step-" + index);
    if (stepContainer && stepElement) {
        stepContainer.removeChild(stepElement);
    }
}
