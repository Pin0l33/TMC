<?php

include("conexion.php");

$username = $_POST['usuario'];
$password = $_POST['contraseña'];

$sql = "SELECT * FROM usuario
        WHERE username = '$username'";

$resultado = mysqli_query($conn, $sql);

if(mysqli_num_rows($resultado) > 0)
{
    $fila = mysqli_fetch_assoc($resultado);

    if(password_verify($password, $fila['password_hash']))
    {
        echo "Inicio de sesión correcto";
    }
    else
    {
        echo "Contraseña incorrecta";
    }
}
else
{
    echo "Usuario no encontrado";
}

mysqli_close($conn);

?>
