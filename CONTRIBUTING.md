# Guía de contribución

Gracias por colaborar con TMC. El proyecto usa Unity y contiene además documentación y componentes web. Coordiná con el grupo cualquier cambio que afecte escenas compartidas, paquetes o archivos grandes.

## Requisitos previos

- Git.
- Unity Hub y Unity Editor `6000.3.11f1` para abrir el videojuego, según `Backend/unity/ProjectSettings/ProjectVersion.txt`.
- Arduino y el hardware correspondiente solo si vas a trabajar o verificar esas funciones.
- Un entorno local con PHP solo si vas a ejecutar el componente web PHP.

## Obtener el proyecto

```sh
git clone https://github.com/Pin0l33/TMC.git
cd TMC
```

Abrí `Backend/unity/` desde Unity Hub. Antes de cambiar de rama, guardá tu trabajo y coordiná con el equipo; no compartas una misma escena en edición simultánea.

## Ramas y remotos

- `main` es la rama principal de referencia del proyecto.
- `origin` corresponde al repositorio principal de GitHub; `gitlab` corresponde al segundo remoto en GitLab.
- `development` y `master` existen en ambos remotos, pero sus historiales presentan divergencias respecto de `main`.
- No se establece aquí cuál de esas ramas debe usarse como base de trabajo ni cómo deben integrarse sus cambios.
- Cualquier unificación de historiales, merge entre esas ramas o cambio del flujo de trabajo debe acordarse entre los integrantes después de revisar las diferencias.

Antes de proponer una integración o sincronización, consultá el estado actual de las ramas y revisá sus diferencias. No publiques cambios hasta contar con el acuerdo del grupo.

## Commits

Usá mensajes breves, en español o inglés, con un prefijo consistente del tipo Conventional Commits:

- `docs: actualizar instrucciones de instalación`
- `feat: agregar compatibilidad de entrada`
- `fix: corregir navegación del menú`
- `chore: ordenar recursos del proyecto`

Describí el cambio real. Evitá agrupar tareas independientes en un solo commit.

## Buenas prácticas con Unity

- Respetá los `.meta` y los identificadores de recursos generados por Unity; mové/renombrá assets desde el editor cuando corresponda.
- No edites ni subas cachés del editor como `Library/`, `Temp/` o `UserSettings/` salvo que el equipo solicite revisar esos archivos.
- Abrí y verificá las escenas afectadas antes de proponer el cambio; no reemplaces escenas o prefabs compartidos sin coordinar.
- Mantené el cambio acotado y describí escenas, paquetes y hardware implicados.
- No subas ejecutables, credenciales, archivos personales ni builds innecesarios.

## Errores y propuestas

Abrí un issue en GitHub con pasos para reproducir, resultado esperado y observado, escena/commit afectado y capturas o registros sin datos sensibles. Para propuestas, explicá el problema que resuelven, alcance y alternativa. Para vulnerabilidades, seguí `SECURITY.md` y no publiques detalles explotables.

## Evitar conflictos

Avisá qué escenas, prefabs o archivos vas a tocar; dividí tareas por escena o subsistema; coordiná cambios concurrentes; no resuelvas conflictos descartando cambios ajenos sin hablar con sus autores.

## GitHub y GitLab

GitHub es el repositorio principal y GitLab el segundo remoto. Revisá las referencias y diferencias de las ramas antes de proponer actualizaciones entre repositorios. El método de sincronización y el orden de publicación deben acordarse entre los integrantes. No uses `--force` ni sobrescribas historial compartido.
