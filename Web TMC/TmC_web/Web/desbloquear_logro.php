<?php

include("conexion.php");

$username = $_POST['username'];
$id_logro = $_POST['id_logro'];

$sql = "INSERT INTO USUARIO_LOGRO
(
    username,
    id_logro,
    fecha_desbloqueo
)

VALUES
(
    '$username',
    '$id_logro',
    NOW()
)";

if(mysqli_query($conn, $sql))
{
    echo "Logro desbloqueado";
}
else
{
    echo "Error: " . mysqli_error($conn);
}

mysqli_close($conn);

?>