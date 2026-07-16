<<?php
session_start();
include("conexion.php");

if (!isset($_SESSION['username'])) {
    header("Location: login.php");
    exit;
}

$username = $_SESSION['username'];

// Datos de la cuenta
$stmtUsuario = mysqli_prepare($conn, "SELECT fecha_creacion, tiempo_jugado_minutos FROM usuario WHERE username = ?");
mysqli_stmt_bind_param($stmtUsuario, "s", $username);
mysqli_stmt_execute($stmtUsuario);
$datosUsuario = mysqli_fetch_assoc(mysqli_stmt_get_result($stmtUsuario));
mysqli_stmt_close($stmtUsuario);

// Formatear fecha en español
$meses = [1=>"enero","febrero","marzo","abril","mayo","junio","julio","agosto","septiembre","octubre","noviembre","diciembre"];
$fechaTimestamp = strtotime($datosUsuario['fecha_creacion']);
$fechaFormateada = date("j", $fechaTimestamp) . " de " . $meses[(int)date("n", $fechaTimestamp)] . " de " . date("Y", $fechaTimestamp);

// Formatear tiempo jugado
$minutos = (int)$datosUsuario['tiempo_jugado_minutos'];
$horas = intdiv($minutos, 60);
$minsRestantes = $minutos % 60;
$tiempoFormateado = $horas > 0 ? "{$horas}h {$minsRestantes}m" : "{$minsRestantes}m";

// Logros
$stmtLogros = mysqli_prepare($conn, "
    SELECT l.id_logro, l.nombre, l.descripcion, ul.fecha_desbloqueo,
           CASE WHEN ul.id_logro IS NULL THEN 0 ELSE 1 END AS desbloqueado
    FROM logro l
    LEFT JOIN usuario_logro ul
      ON l.id_logro = ul.id_logro AND ul.username = ?
    ORDER BY l.id_logro
");
mysqli_stmt_bind_param($stmtLogros, "s", $username);
mysqli_stmt_execute($stmtLogros);
$resultado = mysqli_stmt_get_result($stmtLogros);
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mi Perfil - Teknical May Cry</title>
    <link rel="icon" type="image/png" href="Logo TmC bg.png">
    <link rel="stylesheet" href="style.css">
    <script src="script.js" defer></script>
</head>
<body>
    <header>
        <?php include 'nav.php'; ?>
    </header>
    <main>
        <section class="game-page">
            <div class="game-info">
                <h2>Hola, <?php echo htmlspecialchars($username); ?></h2>

                <div class="tags">
                    <div class="tag">Cuenta creada el <?php echo $fechaFormateada; ?></div>
                    <div class="tag">Tiempo jugado: <?php echo $tiempoFormateado; ?></div>
                </div>

                <h3 class="subtitle" id="logros">Tus logros</h3>
                <div class="achievements">
                    <?php while ($logro = mysqli_fetch_assoc($resultado)): ?>
                        <div class="achievement" style="opacity: <?php echo $logro['desbloqueado'] ? '1' : '0.4'; ?>;">
                            <h4><?php echo htmlspecialchars($logro['nombre']); ?></h4>
                            <p><?php echo htmlspecialchars($logro['descripcion']); ?></p>
                            <?php if ($logro['desbloqueado']): ?>
                                <p><small>Desbloqueado el <?php echo $logro['fecha_desbloqueo']; ?></small></p>
                            <?php else: ?>
                                <p><small>Bloqueado</small></p>
                            <?php endif; ?>
                        </div>
                    <?php endwhile; ?>
                </div>
            </div>
        </section>
    </main>
    <footer>
        <p>© 2026 Teknical May Cry</p>
    </footer>
</body>
</html>
<?php
mysqli_stmt_close($stmtLogros);
mysqli_close($conn);
?>
