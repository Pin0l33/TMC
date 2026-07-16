<?php
session_start();
include("conexion.php");

$error = "";

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $username = trim($_POST['usuario'] ?? '');
    $email = trim($_POST['email'] ?? '');
    $password = $_POST['contraseña'] ?? '';

    if (!$username || !$email || !$password) {
        $error = "Completá todos los campos";
    } elseif (strlen($username) > 30) {
        $error = "El usuario no puede tener más de 30 caracteres";
    } else {
        $check = mysqli_prepare($conn, "SELECT username FROM usuario WHERE username = ? OR email = ?");
        mysqli_stmt_bind_param($check, "ss", $username, $email);
        mysqli_stmt_execute($check);
        $existe = mysqli_stmt_get_result($check);

        if ($existe && mysqli_num_rows($existe) > 0) {
            $error = "El usuario o el email ya están registrados";
            mysqli_stmt_close($check);
        } else {
            mysqli_stmt_close($check);

            $password_hash = password_hash($password, PASSWORD_DEFAULT);
            $stmt = mysqli_prepare($conn, "INSERT INTO usuario (username, email, password_hash) VALUES (?, ?, ?)");
            mysqli_stmt_bind_param($stmt, "sss", $username, $email, $password_hash);

            if (mysqli_stmt_execute($stmt)) {
                // Registro exitoso: logueamos al usuario automáticamente
                $_SESSION['username'] = $username;
                mysqli_stmt_close($stmt);
                mysqli_close($conn);
                header("Location: perfil.php");
                exit;
            } else {
                $error = "Error al registrar. Probá de nuevo.";
            }
            mysqli_stmt_close($stmt);
        }
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
            <h1>Crear Cuenta</h1>
            <?php if ($error): ?>
                <p style="color:#ff4d6d; margin-bottom:15px;"><?php echo htmlspecialchars($error); ?></p>
            <?php endif; ?>
            <form action="registro.php" method="POST">
                <input type="text" name="usuario" placeholder="Usuario" required>
                <input type="email" name="email" placeholder="Correo Electrónico" required>
                <input type="password" name="contraseña" placeholder="Contraseña" required>
                <button type="submit">Registrarse</button>
            </form>
            <p>¿Ya tienes una cuenta?</p>
            <a href="login.php">Iniciar sesión</a>
            <br><br>
            <a href="index.php">Volver al inicio</a>
        </div>
    </main>
    <footer>
        <p>© 2026 Teknical May Cry</p>
    </footer>
</body>
</html>
