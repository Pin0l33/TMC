<?php

$host = "sql300.byetcluster.com";
$usuario = "if0_42308583";
$password = "TmCyamil";
$base_datos = "if0_42308583_tmc";

$conn = mysqli_connect($host, $usuario, $password, $base_datos);

if (!$conn) {
    die("Error de conexión: " . mysqli_connect_error());
}

?>
