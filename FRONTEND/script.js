document.addEventListener('DOMContentLoaded', () => {
    const selectors = [
        '.card',
        '.achievement',
        '.game-card',
        '.contact-item',
        '.screenshot-grid img',
        '.download-card',
        '.contact-card',
        '.registro-container',
        '.hero-content'
    ].join(', ');

    const elementos = document.querySelectorAll(selectors);

    elementos.forEach((el, i) => {
        el.classList.add('reveal');
        // Pequeño desfasaje entre elementos cercanos para que no aparezcan todos juntos
        el.style.transitionDelay = (i % 6) * 0.15 + 's';
    });

    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.15 });

        elementos.forEach((el) => observer.observe(el));
    } else {
        // Si el navegador es muy viejo y no soporta IntersectionObserver,
        // mostramos todo directo sin animación.
        elementos.forEach((el) => el.classList.add('is-visible'));
    }
});