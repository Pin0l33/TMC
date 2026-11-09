# TeKnikal May Cry (TMC)

TeKnikal May Cry (TMC) es un proyecto académico que reúne un videojuego 2D desarrollado con Unity y materiales web y de documentación asociados. El juego propone explorar escenarios escolares e interactuar con personajes y minijuegos.

## Objetivos

- Desarrollar una experiencia interactiva ambientada en un entorno escolar.
- Integrar movimiento, escenas, interfaz, diálogos y minijuegos.
- Incorporar controles compatibles con joystick y hardware Arduino en las partes preparadas para ello.
- Documentar el desarrollo para Laboratorio de Programación (LPR).

## Estado

El proyecto continúa en desarrollo. El repositorio contiene escenas del juego principal y del minijuego FNAE, recursos y sistemas de interfaz, y scripts de compatibilidad con joystick/Arduino. La compatibilidad y los controles dependen de cada escena y del hardware disponible; consultá el Manual del Usuario y la Guía de Instalación antes de usar el dispositivo físico.

No hay una versión de software publicada identificada mediante tags Git. El número `v2.0.0` en el nombre del informe corresponde al documento, no constituye evidencia de una versión del juego.

## Tecnologías

- Unity 6, versión de editor registrada: `6000.3.11f1`.
- C# para la lógica del juego.
- PHP, HTML, CSS y JavaScript en los materiales web presentes en el repositorio.
- Arduino como dispositivo de entrada en las funciones compatibles.
- Git, con GitHub como remoto principal y GitLab como respaldo.

## Funcionalidades presentes

- Movimiento y animaciones del personaje.
- Escenas de menú, juego, kiosco, taller y galería.
- Escenas y secuencias del minijuego FNAE.
- Interfaz y recursos de audio.
- Configuración de entradas para joystick.
- Scripts que contemplan controles Arduino y compatibilidad por minijuego.

Estas características se describen a partir de escenas, configuración y recursos versionados; su disponibilidad jugable puede variar según la escena y el estado de desarrollo.

## Estructura general

- `Backend/unity/`: proyecto de Unity, con `Assets/`, `Packages/` y `ProjectSettings/`.
- `Backend/php/`: páginas y recursos PHP asociados al sitio.
- `FRONTEND/`: recursos del frontend y documentación de hardware.
- `docs/`: manuales, informes y material complementario.
- `README.md`, `changelog.md` y `LICENSE`: documentación principal y licencia.

## Instalación y ejecución

### Videojuego

1. Instalá Unity Hub y Unity Editor `6000.3.11f1`.
2. Cloná el repositorio y abrí `Backend/unity/` desde Unity Hub como proyecto existente.
3. Esperá la importación de recursos y paquetes.
4. Abrí una escena incluida en `Assets/Scenes/` o usá las escenas configuradas en `ProjectSettings/EditorBuildSettings.asset` y ejecutá el proyecto desde el editor.
5. Para controles Arduino, revisá primero el manual de instalación y el informe de hardware enlazados abajo. El repositorio no establece en este README un procedimiento de conexión/calibración universal.

### Sitio web

Los archivos PHP y frontend se encuentran en `Backend/php/` y `FRONTEND/`. Ejecutalos mediante un entorno local compatible con PHP y seguí las indicaciones propias de cada componente. No se documenta aquí una configuración de base de datos porque no se verificó una configuración vigente y común a todos los colaboradores.

## Integrantes

El README previo del repositorio identifica como integrantes a:

- Lucas Del Pino
- Maximo Orue
- Federico Matia
- Ulises Ferrari

## Documentación

- [Changelog](changelog.md)
- [Manual del usuario y guía de instalación](docs/manuales/MANUAL%20DEL%20USUARIO%20Y%20GU%C3%8DA%20DE%20INSTALACI%C3%93N.pdf)
- [Manual técnico del programador y arquitectura](docs/manuales/MANUAL%20T%C3%89CNICO%20DEL%20PROGRAMADOR%20Y%20ARQUITECTURA%20DE%20C%C3%93DIGO%20%281%29.pdf)
- [Informe del proyecto](docs/InformeEEST1_LPR2026_PIA_G8_TeknicalMayCry_v2.0.0.pdf)
- [Informe de hardware y joystick](FRONTEND/Hardware/Informe_Proyecto_Teknical_May_Cry.pdf)
- [Política de seguridad](SECURITY.md)
- [Guía de contribución](CONTRIBUTING.md)
- [Código de conducta](CODE_OF_CONDUCT.md)
