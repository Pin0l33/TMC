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
        <section class="download-page">
            <div class="download-card">
                <img src="imagenes/Logo TmC bg.png" class="download-image">
                <div class="download-info">
                    <h2>TmC</h2>
                    <p class="game-description">
                        Defiende el colegio mientras sobrevives
                        a criaturas misteriosas y noches cada
                        vez más peligrosas.
                    </p>
                    <div class="requirements-box">
                        <h3>Requisitos mínimos</h3>
                        <ul>
                            <li>Windows 10</li>
                            <li>2GB RAM</li>
                            <li>GTX 1050</li>
                            <li>5GB almacenamiento</li>
                            <li>Procesador i5</li>
                        </ul>
                    </div>
                    <a href="#" class="download-btn">
                        DESCARGAR AHORA
                    </a>
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
