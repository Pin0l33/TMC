<?php
header("Content-Type: application/json; charset=UTF-8");
include("conexion.php");

$data = json_decode(file_get_contents("php://input"), true);

$username = trim($data["username"] ?? '');
$password = $data["password"] ?? '';

if (!$username || !$password) {
    echo json_encode(["success" => false, "message" => "Faltan datos"]);
    exit;
}

$stmt = mysqli_prepare($conn, "SELECT username, password_hash FROM usuario WHERE username = ?");
mysqli_stmt_bind_param($stmt, "s", $username);
mysqli_stmt_execute($stmt);
$resultado = mysqli_stmt_get_result($stmt);

if ($resultado && mysqli_num_rows($resultado) > 0) {
    $fila = mysqli_fetch_assoc($resultado);

    if (password_verify($password, $fila['password_hash'])) {
        echo json_encode(["success" => true, "username" => $fila['username']]);
    } else {
        echo json_encode(["success" => false, "message" => "Contraseña incorrecta"]);
    }
} else {
    echo json_encode(["success" => false, "message" => "Usuario no encontrado"]);
}

mysqli_stmt_close($stmt);
mysqli_close($conn);
