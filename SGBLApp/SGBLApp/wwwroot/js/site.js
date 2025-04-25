// Función para controlar la visibilidad de la sidebar
document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggle = document.querySelector('.sidebar-toggle');
    const sidebar = document.querySelector('.sidebar');
    const mainContent = document.querySelector('.main-content');

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            sidebar.classList.toggle('show');
            mainContent.classList.toggle('shift');
        });
    }

    // Cerrar sidebar al hacer clic en un enlace (en dispositivos móviles)
    const sidebarLinks = document.querySelectorAll('.sidebar .nav-link');
    sidebarLinks.forEach(link => {
        link.addEventListener('click', function () {
            if (window.innerWidth < 768) {
                sidebar.classList.remove('show');
                mainContent.classList.remove('shift');
            }
        });
    });
});