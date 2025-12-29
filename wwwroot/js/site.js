// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    // User Dropdown Toggle
    const userBtn = document.getElementById('userDropdownBtn');
    const userMenu = document.getElementById('userDropdownMenu');

    if (userBtn && userMenu) {
        userBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            userMenu.classList.toggle('show');
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', function (e) {
            if (!userMenu.contains(e.target) && !userBtn.contains(e.target)) {
                userMenu.classList.remove('show');
            }
        });
    }


    document.addEventListener('DOMContentLoaded', function () {
        // Get current path
        const currentPath = window.location.pathname;

        // Get all nav links
        const navLinks = document.querySelectorAll('.nav-link');

        // Remove active class from all links
        navLinks.forEach(link => {
            link.classList.remove('active');

            // Add active class to matching link
            if (link.getAttribute('href') === currentPath ||
                link.pathname === currentPath) {
                link.classList.add('active');
            }
        });

        // Add click handlers for smooth transitions
        navLinks.forEach(link => {
            link.addEventListener('click', function (e) {
                // Remove active from all
                navLinks.forEach(l => l.classList.remove('active'));
                // Add to clicked
                this.classList.add('active');
            });
        });
    });

   

    
});