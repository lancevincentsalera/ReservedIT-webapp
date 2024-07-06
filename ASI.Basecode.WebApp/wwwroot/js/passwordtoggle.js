const togglePasswordVisibility = (id, span) => {
    const passwordInput = document.getElementById(id);
    const eyeButton = span.querySelector(`img`);
    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        eyeButton.src = '/img/crossed-eye.svg' // Ensure this path is correct
    } else {
        passwordInput.type = 'password';
        eyeButton.src = '/img/eye.svg';
    }
}