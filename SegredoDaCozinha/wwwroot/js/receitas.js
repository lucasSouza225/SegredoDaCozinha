const container = document.getElementById('ingredientsContainer');
const addBtn = document.getElementById('addIngredientBtn');

function createIngredientRow(selectedValue = '') {
    const row = document.createElement('div');
    row.className = 'ingredient-row';

    const select = document.createElement('select');
    select.className = 'ingredient-select';
    select.innerHTML = `
                <option value="">Selecione...</option>
                <option value="tomate">Tomate</option>
                <option value="cebola">Cebola</option>
                <option value="alho">Alho</option>
                <option value="cenoura">Cenoura</option>
                <option value="batata">Batata</option>
                <option value="frango">Frango</option>
                <option value="carne">Carne moída</option>
                <option value="queijo">Queijo</option>
                <option value="leite">Leite</option>
                <option value="ovos">Ovos</option>
                <option value="farinha">Farinha</option>
                <option value="acucar">Açúcar</option>
            `;
    if (selectedValue) select.value = selectedValue;

    const removeBtn = document.createElement('button');
    removeBtn.className = 'btn-remove-ingredient';
    removeBtn.innerHTML = '✕';
    removeBtn.onclick = function () { row.remove(); };

    row.appendChild(select);
    row.appendChild(removeBtn);
    return row;
}

addBtn.addEventListener('click', () => {
    const rows = container.querySelectorAll('.ingredient-row');
    // permite adicionar até 8 ingredientes (para não poluir)
    if (rows.length < 8) {
        container.appendChild(createIngredientRow());
    } else {
        alert('Você pode adicionar no máximo 8 ingredientes.');
    }
});

// Botão limpar: remove todos exceto o primeiro (deixa apenas um vazio)
document.getElementById('clearFilterBtn').addEventListener('click', () => {
    container.innerHTML = '';
    container.appendChild(createIngredientRow()); // um vazio
    // opcional: esconder botão remover do primeiro?
    const firstRemove = container.querySelector('.btn-remove-ingredient');
    if (firstRemove) firstRemove.style.visibility = 'hidden';
});

// Inicial: primeiro ingrediente com botão remover invisível
const firstRow = container.querySelector('.ingredient-row');
if (firstRow) {
    const firstRemove = firstRow.querySelector('.btn-remove-ingredient');
    if (firstRemove) firstRemove.style.visibility = 'hidden';
}

// Botão aplicar filtro (simulação)
document.getElementById('applyFilterBtn').addEventListener('click', () => {
    // coleta os ingredientes selecionados
    const selects = document.querySelectorAll('.ingredient-select');
    const ingredientes = [];
    selects.forEach(select => {
        if (select.value) ingredientes.push(select.value);
    });
    if (ingredientes.length > 0) {
        alert(`Filtrando por: ${ingredientes.join(', ')} (simulação)`);
    } else {
        alert('Nenhum ingrediente selecionado. Mostrando todas as receitas.');
    }
});