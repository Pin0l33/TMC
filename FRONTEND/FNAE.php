<?php session_start(); ?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>FNAE</title>
    <link rel="icon" type="image/png" href="Logo TmC bg.png">
    <link rel="stylesheet" href="style.css">
    <script src="script.js" defer></script>
</head>
<body>
    <header>
        <?php include 'nav.php'; ?>
    </header>
    <section class="game-page">
        <img src="imagenes/oficina.jpeg" class="game-banner">
        <div class="game-info">
            <h2>FNAE</h2>
            <p>
                Defiende el colegio de criaturas misteriosas mientras sobrevives a noches cada vez más peligrosas.
            </p>
            <div class="tags">
                <div class="tag">Terror</div>
                <div class="tag">Supervivencia</div>
                <div class="tag">Psicológico</div>
            </div>
            <a href="descarga.php" class="btn">
                JUGAR AHORA
            </a>
            <h3 class="subtitle">
                Logros del juego
            </h3>
            <div class="achievements">
                <div class="achievement">
                    <h4>Primer Paso</h4>
                    <p>
                        Completa la primera noche.
                    </p>
                </div>
                <div class="achievement">
                    <h4>Superviviente</h4>
                    <p>
                        Sobrevive cinco noches.
                    </p>
                </div>
                <div class="achievement">
                    <h4>Explorador</h4>
                    <p>
                        Habla con todos los profesores.
                    </p>
                </div>
            </div>

            <section class="screenshots">
                <h3 class="subtitle">
                    Screenshots
                </h3>
                <div class="screenshot-grid">
                    <img src="imagenes/camara 1.jpeg">
                    <img src="imagenes/camara 2.jpeg">
                    <img src="imagenes/camara 3.jpeg">
                </div>
            </section>
        </div>
    </section>
    <footer>
        <p>
            © 2026 Teknical May Cry
        </p>
    </footer>
</body>
</html>
