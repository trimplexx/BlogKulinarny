$(document).ready(function () {
    // Obsługa zdarzenia kliknięcia na awatar
    $(".user-avatar").on("click", function () {
        $("#avatarModal").modal("show");
    });

    // Obsługa zdarzenia wyboru awatara
    $("#avatarsContainer").on("click", ".avatar-item", function () {
        let avatarUrl = $(this).data("url");
        updateAvatar(avatarUrl);
    });
});

function loadAvatars() {
    // Przykładowa tablica z URL-ami awatarów.
    const avatars = [
        '/images/avatars/avatar0.png',
        '/images/avatars/avatar2.png',
        '/images/avatars/avatar3.png',
        '/images/avatars/avatar4.png',
        '/images/avatars/avatar6.png',
        '/images/avatars/avatar8.png',
        '/images/avatars/avatar9.png',
        '/images/avatars/avatar10.png',
        '/images/avatars/avatar11.png',
        '/images/avatars/avatar12.png',
    ];

    // Pobierz kontener awatarów z modalu.
    const avatarsContainer = document.getElementById('avatarsContainer');

    // Wyczyść kontener awatarów.
    avatarsContainer.innerHTML = '';

    // Przeiteruj przez URL-e awatarów i dodaj dla każdego element img do kontenera.
    avatars.forEach((avatarUrl) => {
        const img = document.createElement('img');
        img.src = avatarUrl;
        img.classList.add('avatar-option');
        img.style.width = '200px';
        img.style.height = '200px';
        img.style.cursor = 'pointer';
        img.style.margin = '10px'; // Dodaj marginesy
        img.style.borderRadius = '50%'; // Dodaj okrągłe przycinanie

        // Dodaj funkcję obsługującą kliknięcie na awatarze.
        img.onclick = () => {
            // Zaktualizuj awatar użytkownika za pomocą wybranej opcji.
            // Zaimplementuj logikę aktualizacji awataru w tej funkcji.
            updateAvatar(avatarUrl);
        };

        avatarsContainer.appendChild(img);
    });

    // Wyświetl modal z awatarami.
    const avatarModal = new bootstrap.Modal(document.getElementById('avatarModal'), {
        keyboard: false
    });
    avatarModal.show();
}

async function updateAvatar(avatarUrl) {
    // Wywołaj API do aktualizacji awatara użytkownika
    // Tutaj powinieneś przekazywać userId oraz avatarUrl jako parametry
    const updateAvatarResponse = await fetch("/User/UpdateAvatar", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({avatarUrl: avatarUrl}),
    });

    if (updateAvatarResponse.ok) {
        // Aktualizuj awatar na stronie
        $(".user-avatar").html(`<img src="${avatarUrl}" style="width: 100px; height: 100px; border-radius: 50%;">`);
        $("#avatarModal").modal("hide");
        location.reload(); // Odśwież stronę po zaktualizowaniu awatara
    } else {
        // Obsłuż błąd aktualizacji awatara
        console.error("Błąd podczas aktualizacji awatara");
    }
}