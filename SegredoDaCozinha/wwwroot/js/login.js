function togglePassword() {
    const passwordInput = document.getElementById('Senha');
    const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordInput.setAttribute('type', type);
    
    const icon = document.querySelector('.toggle-password i');
    icon.classList.toggle('fa-eye');
    icon.classList.toggle('fa-eye-slash');
}

document.getElementById('loginForm')?.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const btnLogin = document.getElementById('btnLogin');
    const originalText = btnLogin.innerHTML;
    btnLogin.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Entrando...';
    btnLogin.classList.add('loading');
    btnLogin.disabled = true;
    
    const formData = {
        Email: document.getElementById('Email').value,
        Senha: document.getElementById('Senha').value,
        Lembrar: document.getElementById('Lembrar').checked,
        UrlRetorno: document.getElementById('UrlRetorno')?.value || ''
    };
    
    try {
        const response = await fetch('/Account/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });
        
        const result = await response.json();
        
        if (result.success) {
            showAlert(result.message, 'success');
            setTimeout(() => {
                window.location.href = result.redirectUrl;
            }, 1500);
        } else {
            showAlert(result.message, 'error');
            btnLogin.innerHTML = originalText;
            btnLogin.classList.remove('loading');
            btnLogin.disabled = false;
        }
    } catch (error) {
        showAlert('Erro ao conectar com o servidor', 'error');
        btnLogin.innerHTML = originalText;
        btnLogin.classList.remove('loading');
        btnLogin.disabled = false;
    }
});

function showAlert(message, type) {
    const alertDiv = document.getElementById('alertMessage');
    const alertText = document.getElementById('alertText');
    
    alertText.textContent = message;
    alertDiv.className = `alert-message ${type}`;
    alertDiv.style.display = 'flex';
    
    // Scroll suave até o alerta
    alertDiv.scrollIntoView({ behavior: 'smooth', block: 'center' });
    
    setTimeout(() => {
        alertDiv.style.animation = 'fadeOut 0.3s ease-out';
        setTimeout(() => {
            alertDiv.style.display = 'none';
            alertDiv.style.animation = '';
        }, 300);
    }, 5000);
}

// Adicionar efeito de foco nos campos
document.querySelectorAll('.form-control').forEach(input => {
    input.addEventListener('focus', () => {
        input.parentElement.classList.add('focused');
    });
    
    input.addEventListener('blur', () => {
        input.parentElement.classList.remove('focused');
    });
});