<?php
if (session_status() === PHP_SESSION_NONE) {
    session_start();
}
$logueado = isset($_SESSION['username']);
?>
<nav class="navbar">
    <div class="logo">
        <h1>Teknical May Cry</h1>
    </div>
    <ul class="nav-links">
        <li><a href="index.php">Inicio</a></li>
        <li><a href="galeria.php">Juegos</a></li>
        <li><a href="descarga.php">Descarga</a></li>
        <?php if ($logueado): ?>
            <li><a href="perfil.php">Perfil</a></li>
            <li><a href="perfil.php#logros">Logros</a></li>
            <li><a href="logout.php">Cerrar sesión</a></li>
        <?php else: ?>
            <li><a href="contacto.php">Contacto</a></li>
            <li><a href="login.php">Iniciar sesión</a></li>
            <li><a href="registro.php">Registrarse</a></li>
        <?php endif; ?>
    </ul>
</nav>
