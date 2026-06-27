<?php

include("conexion.php");

$username = $_POST['usuario'];
$email = $_POST['email'];
$password_hash = password_hash(
    $_POST['contraseña'],
    PASSWORD_DEFAULT
);

$sql = "INSERT INTO USUARIO
(
    username,
    email,
    password_hash
)
VALUES
(
    '$username',
    '$email',
    '$password_hash'
)";

if(mysqli_query($conn, $sql))
{
    echo "Usuario registrado correctamente";
}
else
{
    echo "Error: " . mysqli_error($conn);
}

mysqli_close($conn);

?>