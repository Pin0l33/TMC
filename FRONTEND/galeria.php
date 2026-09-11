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
        <section class="gallery-title">
            <h2>Galería del Juego</h2>
        </section>
        <section class="games-gallery">
            <a href="FNAE.php" class="game-card">
                <img src="imagenes/periodico.png" alt="FNAE">
                <div class="game-overlay">
                    <h3>FNAE</h3>
                    <p>
                        Defiende el colegio de los peligros.
                    </p>
                </div>
            </a>
        </section>
    </main>
    <footer>
        <p>
            © 2026 Teknical May Cry
        </p>
    </footer>
</body>
</html>
