// Formularios con data-confirmar piden confirmación antes de enviarse (por ejemplo, eliminar).
document.addEventListener('submit', (e) => {
  const mensaje = e.target.dataset.confirmar;
  if (mensaje && !confirm(mensaje)) {
    e.preventDefault();
  }
});
