// mostrar/esconder senha
function togglePassword() {
    const passwordInput = document.getElementById('Senha');
    const toggleIcon = document.querySelector('.toggle-password i');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleIcon.classList.remove('fa-eye');
        toggleIcon.classList.add('fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        toggleIcon.classList.remove('fa-eye-slash');
        toggleIcon.classList.add('fa-eye');
    }
}

// função para mostrar mensagem
function showAlert(message, isSuccess = false) {
    const alertMessage = document.getElementById('alertMessage');
    const alertText = document.getElementById('alertText');
    
    alertMessage.className = isSuccess ? 'alert-message success' : 'alert-message';
    alertText.textContent = message;
    alertMessage.style.display = 'flex';
    
    if (isSuccess) {
        setTimeout(() => {
            alertMessage.style.opacity = '0';
            setTimeout(() => {
                alertMessage.style.display = 'none';
                alertMessage.style.opacity = '1';
            }, 300);
        }, 3000);
    } else {
        setTimeout(() => {
            alertMessage.style.display = 'none';
        }, 5000);
    }
}

// função de login
async function handleLogin(event) {
    event.preventDefault();
    
    const email = document.getElementById('Email').value;
    const password = document.getElementById('Senha').value;
    const lembrar = document.getElementById('Lembrar')?.checked || false;
    const urlRetorno = document.getElementById('UrlRetorno')?.value || '/';
    const btnLogin = document.getElementById('btnLogin');
    
    // validação básica
    if (!email || !password) {
        showAlert('Por favor, preencha todos os campos.');
        return;
    }
    
    // desabilitar botão durante a requisição
    btnLogin.disabled = true;
    btnLogin.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Entrando...';
    
    try {
        const response = await fetch('/Account/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email: email,
                senha: password,
                lembrar: lembrar,
                urlRetorno: urlRetorno
            })
        });
        
        const data = await response.json();
        
        if (data.success) {
            showAlert(data.message, true);
            
            // redirecionar após 2 segundos
            setTimeout(() => {
                window.location.href = data.redirectUrl || '/';
            }, 2000);
        } else {
            showAlert(data.message);
            
            // reabilitar botão
            btnLogin.disabled = false;
            btnLogin.innerHTML = '<i class="fas fa-sign-in-alt"></i> Entrar';
        }
    } catch (error) {
        console.error('Erro no login:', error);
        showAlert('Erro de conexão. Tente novamente.');
        
        // reabilitar botão
        btnLogin.disabled = false;
        btnLogin.innerHTML = '<i class="fas fa-sign-in-alt"></i> Entrar';
    }
}

// validação do formulário
document.getElementById('loginForm').addEventListener('submit', handleLogin);

// animação de foco nos inputs
const inputs = document.querySelectorAll('.form-control');
inputs.forEach(input => {
    input.addEventListener('focus', function () {
        this.closest('.form-group').classList.add('focused');
    });
    
    input.addEventListener('blur', function () {
        this.closest('.form-group').classList.remove('focused');
    });
});

// permitir submit com Enter
document.addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && document.activeElement.tagName !== 'BUTTON') {
        const form = document.getElementById('loginForm');
        if (form && document.activeElement.form === form) {
            e.preventDefault();
            handleLogin(e);
        }
    }
});