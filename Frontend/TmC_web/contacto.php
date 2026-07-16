<?php session_start(); ?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Teknical may cry</title>
    <link rel="icon" type="image/png" href="Logo TmC bg.png">
    <link rel="stylesheet" href="style.css">
    <script src="script.js" defer></script>
</head>
<body>
    <header>
        <?php include 'nav.php'; ?>
    </header>
    <main>
        <section class="contact-page">
            <div class="contact-card">
                <h2>Contacto</h2>
                <p class="contact-text">
                    Seguinos en nuestras redes y mantenete
                    al día con novedades, actualizaciones
                    y futuros proyectos.
                </p>
                <div class="contact-links">
                    <div class="contact-item">
                        <h3>Email</h3>
                        <p>contacto@videojuego.com</p>
                    </div>
                    <div class="contact-item">
                        <h3>Discord</h3>
                        <a href="https://discord.gg/TULINK" target="_blank">
                            Unirse al Discord
                        </a>
                    </div>
                    <div class="contact-item">
                        <h3>Instagram</h3>
                        <p>@mijuego</p>
                    </div>
                </div>
            </div>
        </section>
    </main>
    <footer>
        <p>
            © 2026 Teknical May Cry
        </p>
    </footer>
</body>
</html>