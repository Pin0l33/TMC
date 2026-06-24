<?php

include("conexion.php");

$usuario = $_POST['usuario'];
$email = $_POST['email'];
$contraseña = password_hash($_POST['contraseña'], PASSWORD_DEFAULT);

$sql = "INSERT INTO usuarios (usuario, email, contraseña)
        VALUES ('$usuario', '$email', '$contraseña')";

if (mysqli_query($conn, $sql)) {
    echo "Usuario registrado correctamente";
} else {
    echo "Error: " . mysqli_error($conn);
}

mysqli_close($conn);

?>
