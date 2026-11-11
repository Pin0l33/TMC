<?php
header("Content-Type: text/plain; charset=UTF-8");
include("conexion.php");

// Usá la MISMA clave que pusiste en desbloquear_logro.php
define("API_KEY", "cambia-esta-clave-1234");

$api_key  = $_POST['api_key'] ?? '';
$username = $_POST['username'] ?? '';
$minutos  = intval($_POST['minutos'] ?? 0);

if ($api_key !== API_KEY) {
    http_response_code(403);
    die("Acceso denegado");
}

if (!$username || $minutos <= 0) {
    die("Faltan datos");
}

$stmt = mysqli_prepare($conn, "UPDATE usuario SET tiempo_jugado_minutos = tiempo_jugado_minutos + ? WHERE username = ?");
mysqli_stmt_bind_param($stmt, "is", $minutos, $username);

if (mysqli_stmt_execute($stmt)) {
    echo "Tiempo actualizado";
} else {
    echo "Error: " . mysqli_error($conn);
}

mysqli_stmt_close($stmt);
mysqli_close($conn);
