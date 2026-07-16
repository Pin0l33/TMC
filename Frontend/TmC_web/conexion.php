<?php
// Reemplazá estos 4 valores por los que te dio InfinityFree en "MySQL Databases"
$host = "sql309.infinityfree.com";           // ej: sql200.epizy.com
$usuario = "if0_42388684";     // ej: if0_12345678
$password = "Mpgpalomagorda";   // la que elegiste al crear la base
$base_datos = "if0_42388684_tmc"; // ej: if0_12345678_tmc
 
// Evita que mysqli lance una excepción no controlada (que da 500 en blanco)
// y en su lugar nos deja mostrar un mensaje de error legible.
mysqli_report(MYSQLI_REPORT_OFF);
 
$conn = mysqli_connect($host, $usuario, $password, $base_datos);
 
if (!$conn) {
    http_response_code(500);
    die("Error de conexión: " . mysqli_connect_error());
}
 
mysqli_set_charset($conn, "utf8mb4");
