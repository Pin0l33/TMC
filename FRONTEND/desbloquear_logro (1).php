<?php
header("Content-Type: text/plain; charset=UTF-8");
include("conexion.php");

// Clave simple para que solo tu juego pueda llamar a este endpoint.
// Cambiala por algo propio y usá el MISMO valor en Unity.
define("API_KEY", "cambia-esta-clave-1234");

$api_key  = $_POST['api_key'] ?? '';
$username = $_POST['username'] ?? '';
$id_logro = intval($_POST['id_logro'] ?? 0);

if ($api_key !== API_KEY) {
    http_response_code(403);
    die("Acceso denegado");
}

if (!$username || !$id_logro) {
    die("Faltan datos");
}

// Evitar duplicados
$check = mysqli_prepare($conn, "SELECT 1 FROM usuario_logro WHERE username = ? AND id_logro = ?");
mysqli_stmt_bind_param($check, "si", $username, $id_logro);
mysqli_stmt_execute($check);
mysqli_stmt_store_result($check);

if (mysqli_stmt_num_rows($check) > 0) {
    echo "El logro ya estaba desbloqueado";
    mysqli_stmt_close($check);
    mysqli_close($conn);
    exit;
}
mysqli_stmt_close($check);

$stmt = mysqli_prepare($conn, "INSERT INTO usuario_logro (username, id_logro, fecha_desbloqueo) VALUES (?, ?, CURDATE())");
mysqli_stmt_bind_param($stmt, "si", $username, $id_logro);

if (mysqli_stmt_execute($stmt)) {
    echo "Logro desbloqueado";
} else {
    echo "Error: " . mysqli_error($conn);
}

mysqli_stmt_close($stmt);
mysqli_close($conn);
