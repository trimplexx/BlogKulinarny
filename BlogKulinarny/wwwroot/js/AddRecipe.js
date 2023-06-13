
//
window.addEventListener('DOMContentLoaded', (event) => {
    var difficultyLabel = document.getElementById('difficultyLabel');
    difficultyLabel.style.color = '#9c7c27';
    difficultyLabel.style.fontWeight = 'bold';
    var difficultyRange = document.getElementById('difficultyRange');
    updateDifficultyLabel(difficultyRange);
    var range = document.querySelector('input[type="range"]');
    range.value = 50;
    updateDifficultyLabel(range);

    var eleDetailsInput = document.getElementById("eleDetails");
    var eleImageInput = document.getElementById("eleImage");
    var addBtn = document.getElementById("addBtn");
    document.getElementById("eleDetails").addEventListener("input", validateFields);
    document.getElementById("eleImage").addEventListener("input", validateFields);

    // Wyłącz przycisk "Dodaj Krok" na początku, jeśli pola są puste
    addBtn.disabled = true;

    // Dodaj nasłuchiwanie na zdarzenie dblclick dla elementów tagu
    var tagElements = document.querySelectorAll('.tag');
    tagElements.forEach(function (tagElement) {
        tagElement.addEventListener('dblclick', function () {
            var tag = tagElement.textContent.trim();
            removeTag(tag);
        });
    });

    // Dodaj nasłuchiwanie na zdarzenie dla przycisku "Dodaj Krok"

});


// suwak poziomu trudnosci

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

//tagi

function addTag(tag) {
    // Sprawdź, czy tag nie został już dodany
    var tags = document.querySelectorAll('.tag');
    for (var i = 0; i < tags.length; i++) {
        if (tags[i].textContent.trim() === tag) {
            return; // Tag już istnieje, nie dodawaj go ponownie
        }
    }

    // Sprawdź, czy przekroczono limit tagów
    var tagContainer = document.getElementById('tagContainer');
    if (tagContainer.childElementCount >= 5) {
        alert('Osiągnięto maksymalny limit tagów.');
        return;
    }

    // Tworzenie nowego elementu tagu
    var tagElement = document.createElement('div');
    tagElement.className = 'col-auto border border-2 rounded-2 border-warning mx-1 tag';
    tagElement.innerHTML = '<div class="p-1">' + tag + '</div>';

    // Dodawanie obsługi zdarzenia dla usuwania tagu
    tagElement.addEventListener('dblclick', function () {
        removeTag(tag);
    });

    // Dodawanie tagu do kontenera tagów
    tagContainer.appendChild(tagElement);

    // Aktualizacja wartości pola selectedTags
    var selectedTagsInput = document.getElementById('selectedTags');
    var tags = document.querySelectorAll('.tag');
    var selectedTags = Array.from(tags).map((tag) => tag.textContent.trim());
    selectedTagsInput.value = selectedTags.join(',');

    // Ustawienie stylu flex dla kontenera tagów
    tagContainer.style.display = 'flex';
}

function selectTag(tag) {
    // Wywołanie funkcji do dodawania tagu
    addTag(tag);
}

function removeTag(tag) {
    // Usuń tag z widoku
    var tagElements = document.querySelectorAll('.tag');
    for (var i = 0; i < tagElements.length; i++) {
        var tagElement = tagElements[i];
        if (tagElement.textContent.trim() === tag) {
            tagElement.remove();
            break;
        }
    }

    // Zaktualizuj wartość pola selectedTags
    var selectedTagsInput = document.getElementById('selectedTags');
    var tags = document.querySelectorAll('.tag');
    var selectedTags = Array.from(tags).map((tag) => tag.textContent.trim());
    selectedTagsInput.value = selectedTags.join(',');
}


//zapisywanie krokow
var stepsCount = 1;

function addStep() {
    stepsCount++;
    event.preventDefault();
    var stepTitle = document.getElementById("stepTitle");
    var clone = stepTitle.cloneNode(true);

    // Resetowanie wartości pól tekstowych
    var clonedImageInput = clone.querySelector(".step-image");
    clonedImageInput.value = "";

    var clonedDescriptionTextarea = clone.querySelector(".step-description");
    clonedDescriptionTextarea.value = "";

    var stepsContainer = document.getElementById("stepsContainer");
    stepsContainer.insertAdjacentElement("beforeend", clone);

    // Dodawanie nasłuchiwania zdarzeń dla nowo utworzonych elementów
    addEventListenersForStep(clone);
}


function removeStep(button) {
    var stepTitle = button.closest(".border");
    var stepTitles = document.querySelectorAll(".border");

    if (stepsCount > 1) {
        stepsCount--;
        stepTitle.remove();
    } else {
        alert("Musisz zachować przynajmniej jeden kafelek.");
    }
}

function addEventListenersForStep(stepElement) {
    var eleDetailsInput = stepElement.querySelector(".step-description");
    var eleImageInput = stepElement.querySelector(".step-image");

    eleDetailsInput.addEventListener("input", validateFields);
    eleImageInput.addEventListener("input", validateFields);
}

function validateFields() {
    var eleDetailsValue = document.querySelector('.step-description').value.trim();
    var eleImageValue = document.querySelector('.step-image').value.trim();
    var addBtn = document.getElementById("addBtn");

    // Włącz/wyłącz przycisk "Dodaj Krok" na podstawie stanu pól
    if (eleDetailsValue !== "" && eleImageValue !== "") {
        addBtn.disabled = false;
    } else {
        addBtn.disabled = true;
    }
}


function saveStepsToViewModel() {

    var selectedTags = document.getElementById("selectedTags").value;
    if (selectedTags.trim() === "") {
        event.preventDefault();
        // Jeśli nie wybrano tagów, zatrzymaj zapis przepisu
        alert("Wybierz co najmniej jeden tag.");
        return;
    }

    try {
        var stepElements = document.querySelectorAll('.border');
        var saveStepsInput = document.getElementById('saveSteps');
        var saveStepsInputValues = [];

        stepElements.forEach(function (stepElement) {
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
