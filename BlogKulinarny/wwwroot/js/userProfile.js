function matchTileHeight() {
    const leftTile = document.getElementById("leftTile");
    const rightTile = document.getElementById("rightTile");
    const maxHeight = Math.max(leftTile.offsetHeight, rightTile.offsetHeight);
    leftTile.style.minHeight = `${maxHeight}px`;
    rightTile.style.minHeight = `${maxHeight}px`;
}

async function loadPartialView(partialView) {
    const response = await fetch(`/User/LoadPartialView?viewName=${partialView}`);
    const content = await response.text();
    document.getElementById("rightTile").innerHTML = content;
    matchTileHeight(); // Dodajemy tę linię, aby dopasować wysokość kafelków po załadowaniu nowego widoku.
}

function showEditProfile() {
    loadPartialView("_EditProfilePartial");
}

function showChangePassword() {
    loadPartialView("_ChangePasswordPartial");
}

function showDeleteAccount() {
    loadPartialView("_DeleteAccountPartial");
}

window.addEventListener("load", matchTileHeight);

function togglePasswordVisibility() {
    const oldPassword = document.getElementById('oldPassword');
    const newPassword = document.getElementById('newPassword');
    const confirmNewPassword = document.getElementById('confirmNewPassword');
    const showPasswordCheckbox = document.getElementById('showPassword');

    if (showPasswordCheckbox.checked) {
        oldPassword.type = 'text';
        newPassword.type = 'text';
        confirmNewPassword.type = 'text';
    } else {
        oldPassword.type = 'password';
        newPassword.type = 'password';
        confirmNewPassword.type = 'password';
    }
}