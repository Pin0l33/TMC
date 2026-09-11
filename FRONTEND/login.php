<?php
session_start();
include("conexion.php");

$error = "";

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $username = trim($_POST['usuario'] ?? '');
    $password = $_POST['contraseña'] ?? '';

    if ($username && $password) {
        $stmt = mysqli_prepare($conn, "SELECT username, password_hash FROM usuario WHERE username = ?");
        mysqli_stmt_bind_param($stmt, "s", $username);
        mysqli_stmt_execute($stmt);
        $resultado = mysqli_stmt_get_result($stmt);

        if ($resultado && mysqli_num_rows($resultado) > 0) {
            $fila = mysqli_fetch_assoc($resultado);
            if (password_verify($password, $fila['password_hash'])) {
                $_SESSION['username'] = $fila['username'];
                mysqli_stmt_close($stmt);
                mysqli_close($conn);
                header("Location: perfil.php");
                exit;
            } else {
                $error = "Contraseña incorrecta";
            }
        } else {
            $error = "Usuario no encontrado";
        }
        mysqli_stmt_close($stmt);
    } else {
        $error = "Completá todos los campos";
    }
}
mysqli_close($conn);
?>
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
    <main class="registro">
        <div class="registro-container">
            <h1>Iniciar Sesión</h1>
            <?php if ($error): ?>
                <p style="color:#ff4d6d; margin-bottom:15px;"><?php echo htmlspecialchars($error); ?></p>
            <?php endif; ?>
            <form action="login.php" method="POST">
                <input type="text" name="usuario" placeholder="Usuario" required>
                <input type="password" name="contraseña" placeholder="Contraseña" required>
                <button type="submit">Ingresar</button>
            </form>
            <p>¿No tienes una cuenta?</p>
            <a href="registro.php">Registrarse</a>
            <br><br>
            <a href="index.php">Volver al inicio</a>
        </div>
    </main>
    <footer>
        <p>© 2026 Teknical May Cry</p>
    </footer>
</body>
</html>
