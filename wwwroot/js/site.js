


document.addEventListener('DOMContentLoaded', function () {

    // ==========================================
    // user dropdown
    // ==========================================
    const userBtn = document.getElementById('userDropdownBtn');
    const userMenu = document.getElementById('userDropdownMenu');

    if (userBtn && userMenu) {
        userBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            userMenu.classList.toggle('show');
        });

        
        document.addEventListener('click', function (e) {
            if (!userMenu.contains(e.target) && !userBtn.contains(e.target)) {
                userMenu.classList.remove('show');
            }
        });
    }

    // ==========================================
    //  navigation active state
    // ==========================================
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        // Remove 'active' from hardcoded HTML just in case
        link.classList.remove('active');

        // Get the clean path from the link (ignoring query strings like ?culture=en)
        const linkPath = link.getAttribute('href');

        // Skip buttons or empty links
        if (!linkPath) return;

        // Logic:
        // 1. Exact match (e.g. "/" or "/Events")
        // 2. Sub-path match (e.g. "/Events/Details/1" matches "/Events"), BUT ignore "/" home link for this check
        if (linkPath === currentPath) {
            link.classList.add('active');
        }
        else if (linkPath !== "/" && currentPath.startsWith(linkPath)) {
            link.classList.add('active');
        }
    });

    // ==========================================
    // mobile menu
    // ==========================================

   
    const mobileBtn = document.querySelector('.mobile-menu-toggle');
    const closeBtn = document.querySelector('.close-menu-btn');
    const mobileMenu = document.querySelector('.mobile-menu');
    const body = document.body; 

    
    function toggleMobileMenu() {
        if (mobileMenu) {
            mobileMenu.classList.toggle('open');
            if (mobileMenu.classList.contains('open')) {
                body.style.overflow = 'hidden';
            } else {
                body.style.overflow = '';
            }
        }
    }

    // open burger
    if (mobileBtn) {
        mobileBtn.addEventListener('click', function (e) {
            e.stopPropagation(); 
            toggleMobileMenu();
        });
    }

    // close burger
    if (closeBtn) {
        closeBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            toggleMobileMenu();
        });
    }

    // for closing
    document.addEventListener('click', function (event) {
       
        if (mobileMenu && mobileMenu.classList.contains('open')) {
            if (!mobileMenu.contains(event.target) && !mobileBtn.contains(event.target)) {
                mobileMenu.classList.remove('open');
                body.style.overflow = '';
            }
        }
    });
});