<?php

$host = "localhost";
$usuario = "root";
$password = "";
$base_datos = "tmc";

$conn = mysqli_connect($host, $usuario, $password, $base_datos);

if (!$conn) {
    die("Error de conexión: " . mysqli_connect_error());
}

?>
