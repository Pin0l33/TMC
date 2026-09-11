<?php
header("Content-Type: application/json; charset=UTF-8");
include("conexion.php");

$data = json_decode(file_get_contents("php://input"), true);

$username = trim($data["username"] ?? '');
$email    = trim($data["email"] ?? '');
$password = $data["password"] ?? '';

if (!$username || !$email || !$password) {
    echo json_encode(["success" => false, "message" => "Faltan datos"]);
    exit;
}

if (strlen($username) > 30) {
    echo json_encode(["success" => false, "message" => "El usuario no puede tener más de 30 caracteres"]);
    exit;
}

$check = mysqli_prepare($conn, "SELECT username FROM usuario WHERE username = ? OR email = ?");
mysqli_stmt_bind_param($check, "ss", $username, $email);
mysqli_stmt_execute($check);
$existe = mysqli_stmt_get_result($check);

if ($existe && mysqli_num_rows($existe) > 0) {
    echo json_encode(["success" => false, "message" => "El usuario o el email ya existen"]);
    mysqli_stmt_close($check);
    mysqli_close($conn);
    exit;
}
mysqli_stmt_close($check);

$hash = password_hash($password, PASSWORD_DEFAULT);
$stmt = mysqli_prepare($conn, "INSERT INTO usuario (username, email, password_hash) VALUES (?, ?, ?)");
mysqli_stmt_bind_param($stmt, "sss", $username, $email, $hash);

if (mysqli_stmt_execute($stmt)) {
    echo json_encode(["success" => true, "username" => $username]);
} else {
    echo json_encode(["success" => false, "message" => "Error al registrar"]);
}

mysqli_stmt_close($stmt);
mysqli_close($conn);
