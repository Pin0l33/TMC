# TeKnikal May Cry (TMC)

> **TeKnikal May Cry (TMC)** es un proyecto académico e interdisciplinario que combina desarrollo de videojuegos, programación, diseño de interfaces e integración de hardware. Su núcleo es una experiencia 2D desarrollada en Unity, ambientada en un entorno escolar y conectada con distintos minijuegos, personajes, diálogos y materiales web y técnicos.

---

## Índice

- [Acerca del proyecto](#acerca-del-proyecto)
- [Objetivos](#objetivos)
- [Experiencia de juego](#experiencia-de-juego)
- [Sistemas principales](#sistemas-principales)
- [Integración con Arduino](#integración-con-arduino)
- [Compatibilidad de los minijuegos](#compatibilidad-de-los-minijuegos)
- [Tecnologías y herramientas](#tecnologías-y-herramientas)
- [Estructura del repositorio](#estructura-del-repositorio)
- [Requisitos](#requisitos)
- [Instalación y ejecución](#instalación-y-ejecución)
- [Controles](#controles)
- [Estado del desarrollo](#estado-del-desarrollo)
- [Documentación](#documentación)
- [Equipo](#equipo)
- [Contribuciones](#contribuciones)
- [Licencia](#licencia)

---

## Acerca del proyecto

TMC propone una aventura interactiva en 2D dentro de un establecimiento educativo. El jugador puede recorrer espacios, acercarse a personajes, iniciar conversaciones, elegir opciones y acceder a diferentes experiencias jugables. El proyecto reúne sistemas de exploración, interacción, navegación entre escenas, interfaces y minijuegos.

Además del control tradicional mediante teclado, el desarrollo contempla la conexión de un joystick físico construido con **Arduino UNO**. Esta integración busca que determinadas partes del videojuego puedan probarse y jugarse con un dispositivo físico, y que el propio proyecto informe al usuario qué tan compatible es cada experiencia antes de iniciarla.

El repositorio también reúne materiales web, informes académicos, manuales de uso y documentación técnica. Por eso, TMC no se limita al ejecutable del videojuego: incluye los recursos necesarios para comprender, instalar, probar y continuar el desarrollo.

## Objetivos

- Diseñar y desarrollar un videojuego 2D con una ambientación escolar.
- Construir una experiencia de exploración con escenarios, personajes y elementos interactivos.
- Implementar diálogos con líneas de texto, indicadores de continuación y selección de opciones.
- Conectar las distintas áreas del juego mediante transiciones y carga de escenas.
- Desarrollar minijuegos con mecánicas y requisitos de entrada propios.
- Integrar un control físico basado en Arduino y comunicación serial con Unity.
- Mantener alternativas de entrada mediante teclado cuando la escena lo permita.
- Informar al jugador sobre la compatibilidad del hardware antes de entrar a determinados minijuegos.
- Acompañar el desarrollo con documentación para usuarios, programadores y la presentación académica.

## Experiencia de juego

### Exploración del entorno

La experiencia principal se desarrolla en un mapa 2D. El personaje puede desplazarse por el escenario, recorrer distintas áreas y acercarse a elementos interactivos. El movimiento utiliza animaciones direccionales y efectos de sonido de pasos.

### Personajes y diálogos

Los personajes no jugables (NPC) funcionan como puntos de interacción y orientación. Al acercarse a un personaje, el juego puede mostrar un indicador que señala que existe una interacción disponible. El sistema de diálogo presenta el texto progresivamente y permite avanzar entre líneas.

Cuando una conversación incluye opciones, el jugador puede navegar por ellas y seleccionar una respuesta. Algunas elecciones permiten acceder a otra escena o iniciar una actividad.

### Puertas y transiciones

Las puertas y zonas de acceso conectan diferentes partes de la experiencia. Al interactuar con una puerta, el juego puede comprobar condiciones de acceso, iniciar un desafío o preparar una transición hacia otra escena. En los cambios de área se contempla la conservación de la posición del jugador para permitir el regreso al mapa principal.

### Minijuegos

El proyecto contiene distintas experiencias con mecánicas y requisitos de entrada particulares. Entre ellas se encuentran:

- **FNAE — Five Nights at Epifanio:** experiencia de vigilancia y supervivencia organizada alrededor de noches, cámaras y acciones de la oficina.
- **Combate contra Guardián (Boss Fight):** combate inspirado en juegos de esquiva y selección de acciones, con una secuencia de movimiento y opciones.
- **Taller Técnico:** minijuego de calibración y soldadura pensado para aprovechar el joystick físico de la propuesta de ExpoTécnica.
- **Galería Escolar:** área de exposición que puede recorrerse mediante el personaje.
- **Kiosco Escolar:** escena prototipo cuyo estado jugable y compatibilidad se encuentran limitados en la versión documentada.

La disponibilidad y el grado de terminación pueden variar entre escenas. La presencia de una escena o de sus recursos en el repositorio no significa necesariamente que todas sus mecánicas estén terminadas.

## Sistemas principales

| Sistema | Responsabilidad |
|---|---|
| Movimiento del jugador | Lee entradas, actualiza la dirección, reproduce animaciones y desplaza al personaje. |
| Diálogos | Gestiona conversaciones, texto progresivo, opciones y selección. |
| Interacción | Centraliza las acciones de interacción utilizadas por el mapa y los personajes. |
| Puertas y transiciones | Controla accesos, condiciones y cambios entre escenas. |
| Interfaz | Presenta menús, indicadores, opciones y elementos de navegación. |
| Pausa | Permite detener la partida y acceder a las opciones disponibles. |
| Entrada Arduino | Recibe datos del dispositivo físico y los distribuye a los sistemas compatibles. |
| Aviso de compatibilidad | Explica los controles disponibles y los periféricos adicionales necesarios antes de entrar a una experiencia. |
| Audio | Reproduce efectos de sonido asociados a acciones, pasos y diálogos. |

### Aviso de compatibilidad

Antes de acceder a determinadas experiencias, TMC puede mostrar una ventana de compatibilidad. El aviso identifica el minijuego, informa el nivel de compatibilidad y explica qué controles físicos están disponibles y qué otros dispositivos se necesitan.

Los niveles utilizados son:

- **Compatible:** la experiencia está preparada para jugarse con el control Arduino en las acciones principales indicadas.
- **Compatibilidad parcial:** algunas acciones funcionan con Arduino, pero se necesita otro periférico para completar la experiencia.
- **No compatible:** la escena todavía no cuenta con controles Arduino asignados o no tiene mecánicas activas compatibles.

El aviso también permite continuar o volver al mapa, evitando que el jugador ingrese a una escena sin conocer sus requisitos.

## Integración con Arduino

### Descripción general

TMC incorpora un sistema de entrada física basado en **Arduino UNO**. La placa lee el joystick analógico y los botones del control, y envía los datos a la computadora mediante una conexión USB y comunicación serial. Unity recibe esos mensajes a través del sistema `ArduinoJoystickInput`.

La integración contempla dos tipos de información:

1. **Movimiento:** dirección de la palanca analógica o mensajes direccionales.
2. **Acciones:** pulsación y liberación de botones, pausa e interacción, según el protocolo utilizado por el firmware.

La conexión serial configurada en la escena de inicio utiliza actualmente:

- Puerto de ejemplo/configuración: `COM5`.
- Velocidad: `9600 baudios`.
- Placa de referencia: Arduino UNO.

El puerto COM puede cambiar según la computadora. Si Windows asigna otro puerto, debe actualizarse la configuración correspondiente en Unity.

### Asignación de controles documentada

| Entrada física | Función |
|---|---|
| Joystick analógico (ejes X/Y) | Movimiento y navegación por interfaces compatibles. |
| Botón de interacción (D3 en el montaje documentado) | Interactuar, confirmar o ejecutar la acción que corresponda a la escena. |
| Botón de pausa (D2 en el montaje documentado) | Abrir o cerrar la pausa en el mapa principal. |
| Conexión USB | Alimentación y comunicación serial entre Arduino y la computadora. |

> **Importante:** la asignación de pines describe el montaje documentado para el proyecto. Antes de conectar o modificar el circuito, consultá el informe de hardware y verificá que el firmware cargado en la placa coincida con el protocolo que espera Unity.

### Protocolo de comunicación

El lector serial contempla mensajes de texto para movimiento y acciones, además de un paquete analógico. Los mensajes reconocidos por la implementación incluyen:

| Mensaje | Significado |
|---|---|
| `MOVE:UP` | Movimiento hacia arriba. |
| `MOVE:DOWN` | Movimiento hacia abajo. |
| `MOVE:LEFT` | Movimiento hacia la izquierda. |
| `MOVE:RIGHT` | Movimiento hacia la derecha. |
| `MOVE:STOP` | Detener el movimiento direccional. |
| `ACTION:PAUSE` | Pulsación del botón de pausa. |
| `ACTION:PAUSE_RELEASED` | Liberación del botón de pausa. |
| `ACTION:INTERACT` | Pulsación del botón de interacción. |
| `ACTION:INTERACT_RELEASED` | Liberación del botón de interacción. |
| `ACTION:INVENTORY` | Solicitud de acción de inventario, si la escena la contempla. |
| `JOY,x,y,button` | Lectura analógica con valores X/Y y estado del botón. |

En el paquete `JOY`, los ejes se expresan como valores numéricos entre -1 y 1, y el estado del botón como `0` o `1`. La implementación aplica una zona muerta (*dead zone*) para reducir movimientos involuntarios cerca del centro de la palanca.

El sistema procesa los mensajes seriales en una cola para separar la lectura del puerto de la actualización de los objetos de Unity. También contempla la pérdida de conexión y el restablecimiento de la lectura.

### Simulación sin Arduino

En el Editor de Unity y en compilaciones de desarrollo existe una modalidad de simulación para probar el sistema sin conectar la placa:

| Tecla | Acción |
|---|---|
| `F8` | Activar o desactivar la simulación. |
| `I` | Arriba. |
| `J` | Izquierda. |
| `K` | Abajo. |
| `L` | Derecha. |
| `Left Shift` | Movimiento reducido. |
| `Left Ctrl` | Movimiento lento alternativo. |
| `Right Shift` | Simular el botón de interacción. |

La simulación permite revisar el comportamiento de las entradas y la navegación sin depender del dispositivo físico. No reemplaza las pruebas finales con Arduino.

## Compatibilidad de los minijuegos

La siguiente tabla refleja la clasificación configurada en el sistema de aviso de compatibilidad del proyecto. No representa una certificación de funcionamiento en todas las computadoras o configuraciones.

| Experiencia | Estado informado | Uso del Arduino | Periféricos adicionales |
|---|---|---|---|
| Taller Técnico | Compatible | Palanca para calibración/movimiento; botón de interacción para acciones; pausa. | Ninguno obligatorio según la configuración del aviso. |
| Combate contra Guardián | Compatible | Palanca para esquivar y navegar; botón para acciones y selección. | Ninguno obligatorio; el teclado puede servir como alternativa. |
| FNAE | Parcial | Algunas acciones de interfaz y secuencias puntuales. | Mouse necesario para tablet, cámaras, bloqueo de puerta y vista de oficina. |
| Galería Escolar | Compatible | Palanca para caminar y botón para entrar por puertas. | Ninguno obligatorio según la configuración del aviso. |
| Kiosco Escolar | No compatible | Sin controles Arduino asignados. | Escena prototipo en desarrollo. |

La compatibilidad puede cambiar a medida que se implementen y prueben nuevas entradas. Para conocer el comportamiento de una versión concreta, consultá el aviso mostrado por el propio juego y los manuales correspondientes.

## Tecnologías y herramientas

| Tecnología / herramienta | Uso en el proyecto |
|---|---|
| **Unity 6** | Motor utilizado para construir y ejecutar el videojuego. |
| **Unity Editor 6000.3.11f1** | Versión de editor registrada para el proyecto. |
| **C#** | Scripts de movimiento, interacción, escenas, interfaces, minijuegos y comunicación con Arduino. |
| **Arduino UNO** | Lectura del joystick y los botones físicos. |
| **Comunicación serial** | Intercambio de datos entre Arduino y Unity por USB. |
| **HTML, CSS y JavaScript** | Recursos y componentes web del repositorio. |
| **PHP** | Material web y páginas del backend presentes en el repositorio. |
| **Git** | Control de versiones. |
| **GitHub** | Repositorio principal del proyecto. |
| **GitLab** | Remoto adicional utilizado para sincronización/respaldo del trabajo. |

## Estructura del repositorio

La organización general del repositorio es la siguiente:

```text
TMC/
├── Backend/
│   ├── unity/
│   │   ├── Assets/
│   │   │   ├── Scenes/
│   │   │   ├── scripts/
│   │   │   └── ...
│   │   ├── Packages/
│   │   └── ProjectSettings/
│   └── php/
├── FRONTEND/
│   └── Hardware/
├── docs/
│   └── manuales/
├── README.md
├── changelog.md
├── LICENSE
├── SECURITY.md
├── CONTRIBUTING.md
└── CODE_OF_CONDUCT.md
```

### Directorios principales

- **`Backend/unity/`:** proyecto principal de Unity. Contiene escenas, recursos, scripts, paquetes y configuración del editor.
- **`Backend/unity/Assets/Scenes/`:** escenas del mapa principal y de las experiencias asociadas.
- **`Backend/unity/Assets/scripts/`:** lógica del videojuego organizada en sistemas y áreas funcionales. Por ejemplo, los scripts de Arduino se encuentran en `scripts/Arduino/`, los del mapa principal en `scripts/JuegoPrincipal/` y los de FNAE en `scripts/FNAE/`.
- **`Backend/php/`:** páginas y recursos PHP del componente web.
- **`FRONTEND/`:** recursos del frontend y materiales asociados a la integración de hardware.
- **`FRONTEND/Hardware/`:** documentación e informe relacionados con el joystick y el hardware.
- **`docs/`:** informes, manuales y documentación complementaria.
- **`changelog.md`:** registro de cambios documentados del proyecto.

La estructura puede ampliarse durante el desarrollo. Para ubicar un archivo concreto, utilizá la ruta real presente en la rama que estés consultando.

## Requisitos

### Para abrir el videojuego

- Windows u otro sistema compatible con la versión de Unity utilizada.
- Unity Hub.
- Unity Editor **6000.3.11f1** (versión registrada).
- Espacio disponible para descargar el repositorio, importar recursos y generar archivos temporales.
- Teclado para las pruebas generales.
- Mouse para las escenas que requieren interacción de puntero, especialmente FNAE.

### Para utilizar el control físico

- Placa Arduino UNO.
- Joystick analógico.
- Botones y cableado correspondientes al circuito documentado.
- Cable USB de datos compatible con la placa.
- Driver/puerto serial reconocido por el sistema operativo.
- Firmware de Arduino compatible con el protocolo serial implementado en Unity.

## Instalación y ejecución

### 1. Obtener el repositorio

Cloná el repositorio desde una terminal:

```bash
git clone https://github.com/Pin0l33/TMC.git
```

Si ya tenés una copia local, actualizala desde la rama que utilice tu equipo antes de comenzar a trabajar. Evitá sobrescribir cambios locales que todavía no hayan sido guardados o sincronizados.

### 2. Abrir el proyecto de Unity

1. Instalá Unity Hub.
2. Instalá o seleccioná Unity Editor **6000.3.11f1**.
3. En Unity Hub, elegí **Add / Open project**.
4. Seleccioná la carpeta `Backend/unity/` del repositorio. No selecciones la raíz `TMC/` como proyecto de Unity.
5. Esperá a que Unity importe los recursos y resuelva los paquetes.
6. Abrí una escena desde `Assets/Scenes/` o utilizá las escenas configuradas en la lista de compilación del proyecto.
7. Presioná **Play** para ejecutar desde el Editor.

Si Unity informa que faltan paquetes o recursos, verificá primero la versión del Editor y la importación antes de modificar configuraciones del proyecto.

### 3. Configurar Arduino

1. Conectá el Arduino UNO mediante un cable USB de datos.
2. Comprobá en el sistema operativo qué puerto COM fue asignado.
3. Cargá en la placa el firmware correspondiente al circuito y al protocolo de TMC.
4. En la configuración de `ArduinoJoystickInput`, verificá el puerto serial y la velocidad.
5. Cerrá cualquier monitor serial u otro programa que esté utilizando el mismo puerto, ya que normalmente el puerto no puede ser abierto simultáneamente por varias aplicaciones.
6. Ejecutá el proyecto y comprobá que la entrada sea recibida.
7. Si no disponés del dispositivo, utilizá la simulación con `F8` en el Editor o en una compilación de desarrollo.

No conectes componentes ni cambies los pines basándote únicamente en este README: utilizá el informe de hardware para revisar el circuito y sus conexiones.

### 4. Ejecutar los componentes web

Los recursos PHP se encuentran en `Backend/php/` y los del frontend en `FRONTEND/`. Para ejecutar páginas PHP se necesita un entorno local que incluya un servidor web compatible con PHP. La configuración exacta de servidor, rutas y posibles servicios auxiliares debe verificarse en los archivos del componente correspondiente; este README no presupone una base de datos ni credenciales compartidas.

## Controles

Los controles pueden variar según la escena y el minijuego.

### Mapa principal

- **Teclado:** movimiento mediante los ejes configurados en Unity.
- **Interacción:** tecla de interacción configurada en el sistema de entrada.
- **Arduino:** palanca analógica para desplazamiento y botón físico para interactuar.
- **Pausa:** tecla configurada o botón físico D2 en el montaje documentado.

### Interfaces y minijuegos

En las escenas que lo admiten, la palanca permite navegar por opciones y el botón de interacción permite confirmar. Algunas experiencias siguen necesitando teclado o mouse para acciones específicas. El aviso de compatibilidad informa esas diferencias antes de entrar.

## Estado del desarrollo

TMC es un proyecto **en desarrollo**. El repositorio contiene el proyecto de Unity, escenas y recursos de distintas experiencias, sistemas de interfaz y scripts para la integración Arduino. Algunas funciones pueden estar completas en una escena y ser experimentales o estar pendientes en otra.

La documentación diferencia entre:

- **Recursos presentes:** archivos y componentes que ya están versionados.
- **Sistemas implementados:** lógica que existe en el código.
- **Compatibilidad configurada:** clasificación informada por el aviso del juego.
- **Funcionamiento validado:** comportamiento comprobado mediante pruebas en una configuración concreta.

Estas categorías no son equivalentes. Que una función aparezca en el código no garantiza que esté terminada ni que haya sido validada en todos los equipos.

### Versionado

No se identifica una versión de lanzamiento del videojuego mediante tags Git en el repositorio consultado. Por lo tanto, no debe interpretarse el número `v2.0.0` que aparece en el nombre de uno de los informes como una versión oficial del software: corresponde al documento.

Para conocer los cambios registrados, consultá [`changelog.md`](changelog.md) y el historial de commits del repositorio.

## Documentación

Los siguientes documentos amplían la información de instalación, uso, arquitectura y hardware:

- [Changelog del proyecto](changelog.md)
- [Manual del usuario y guía de instalación](docs/manuales/MANUAL%20DEL%20USUARIO%20Y%20GU%C3%8DA%20DE%20INSTALACI%C3%93N.pdf)
- [Manual técnico del programador y arquitectura de código](docs/manuales/MANUAL%20T%C3%89CNICO%20DEL%20PROGRAMADOR%20Y%20ARQUITECTURA%20DE%20C%C3%93DIGO%20%281%29.pdf)
- [Informe del proyecto](docs/InformeEEST1_LPR2026_PIA_G8_TeknicalMayCry_v2.0.0.pdf)
- [Informe de hardware y joystick](FRONTEND/Hardware/Informe_Proyecto_Teknical_May_Cry.pdf)
- [Política de seguridad](SECURITY.md)
- [Guía de contribución](CONTRIBUTING.md)
- [Código de conducta](CODE_OF_CONDUCT.md)

## Equipo

Integrantes identificados en la documentación del proyecto:

- Lucas Del Pino
- Maximo Orue
- Federico Matia
- Ulises Ferrari

## Contribuciones

Las contribuciones deben mantener la organización existente del repositorio y evitar cambios ajenos al objetivo de cada tarea.

1. Revisá el estado actual de la rama antes de modificar archivos.
2. Trabajá sobre una rama adecuada para el cambio cuando corresponda.
3. Realizá cambios acotados y comprobables.
4. Probá las escenas o componentes afectados.
5. Actualizá la documentación cuando cambien controles, requisitos, rutas, escenas o comportamiento.
6. Consultá [`CONTRIBUTING.md`](CONTRIBUTING.md) antes de enviar una contribución.

No incluyas contraseñas, tokens, claves privadas ni datos personales en commits, archivos de configuración o reportes de errores. Para informar una vulnerabilidad, seguí las indicaciones de [`SECURITY.md`](SECURITY.md).

## Licencia

La licencia y las condiciones de uso del proyecto se encuentran en el archivo [`LICENSE`](LICENSE). Consultalo antes de reutilizar, distribuir o modificar el contenido de este repositorio.
