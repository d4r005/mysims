# MySims 🏠

Juego de simulación social estilo *Los Sims* para **Android**, construido con **Unity 2022.3 LTS (C#)**.

## Estado actual: las 40/40 expansiones + mundo global + arte base procedural + escena jugable en un clic

Sistemas ya implementados como código base:

| Sistema | Script | Qué hace |
|---|---|---|
| ⏰ Tiempo | `Core/TimeSystem.cs` | Reloj día/noche, calendario, velocidad ajustable (x1 a x8) |
| 🍽️ Necesidades | `Needs/NeedsSystem.cs` | 5 necesidades (hambre, energía, social, diversión, higiene) que decaen por hora de juego |
| 🚶 IA del personaje | `NPC/NPCController.cs` | FSM: detecta la necesidad más baja, camina al objeto que la resuelve y lo usa |
| 🛋️ Objetos interactivos | `Building/PlaceableObject.cs` | Muebles que satisfacen necesidades (cama, refri, ducha, TV...) |
| 🏗️ Construcción | `Building/GridPlacement.cs` | Colocación de muebles en cuadrícula con snap y rotación (tecla R o botón táctil) |
| 💾 Guardado | `Save/SaveManager.cs` | Partida local en JSON + restauración de muebles colocados (vía `PrefabRegistry`) |
| 📱 Entrada táctil | `Input/TouchInputController.cs` | Tap para mover al personaje o usar un mueble (también con mouse en editor) |
| 🎥 Cámara orbital | `Camera/TouchCameraController.cs` | Orbita con un dedo, zoom pinch con dos, pan; rueda/botón derecho en editor |
| 📊 UI necesidades | `UI/NeedsUIController.cs` | Barras estilo Sims con color por nivel (verde/amarillo/rojo) |
| ⏩ Controles de tiempo | `UI/TimeControlsUI.cs` | Botones pausa / x1 / x2 / x4 |
| 🏃 Animación procedural | `NPC/SimpleLocomotion.cs` | Bob al caminar, respiración en idle, giro suave (sin necesidad de rig) |

## Puesta en marcha rápida (arte incluido)

El proyecto genera su propio arte placeholder y la escena completa desde el editor:

1. Abre el proyecto en Unity 2022.3 LTS.
2. En la barra de menú: **MySims → 1. Generar prefabs base** (crea muebles, mascotas, niño, robot y pared en `Assets/Generated`).
3. Luego **MySims → 2. Crear escena base** (crea `Assets/Scenes/Main.unity` con todos los sistemas, zonas de Monterrey/Cancún/NeoCiudad, vecinos, mascota, NavMesh y UI de barras).
4. Dale Play. Reemplaza después los cubos por modelos reales cuando quieras.

## Cómo abrir el proyecto

1. Instala **Unity Hub** y el editor **Unity 2022.3 LTS** con módulo *Android Build Support* (SDK + NDK).
2. Clona este repo y ábrelo desde Unity Hub (`Add` → carpeta del proyecto).
3. Al abrir, Unity genera los archivos de escena y configuración faltantes.
4. Configura el proyecto para Android: `File → Build Settings → Android → Switch Platform`.
5. Crea la escena `Assets/Scenes/Main.unity` con:
   - Un plano (suelo), un cubo o cápsula con `NavMeshAgent` (el personaje) con el script `NPCController`.
   - GameObject `GameManager` con `GameManager`, `TimeSystem` y `NeedsSystem`.
   - `Window → AI → Navigation` para hornear el NavMesh del suelo.
   - Unos muebles de prueba con `PlaceableObject` (asignar `satisfies` y `interactionPoint`).

## Hoja de ruta

Ver [`docs/ROADMAP.md`](docs/ROADMAP.md) para las fases y [`docs/EXPANSIONS.md`](docs/EXPANSIONS.md) para el mapeo completo de las 40 expansiones de Sims 2/3/4 a sistemas de este repo.

**Packs de contenido nuevos (equivalentes a expansiones):**

| Pack | Script | Cubre temas de |
|---|---|---|
| 🌦️ Estaciones y clima | `World/SeasonSystem.cs` | Cuatro Estaciones (x3), Escapada en la Nieve |
| 🏪 Negocios | `Economy/BusinessSystem.cs` | Abren Negocios, Ocio y Negocio, A Trabajar |
| ⭐ Fama | `Progress/FameSystem.cs` | Salto a la Fama, Rumbo a la Fama, Al Caer la Noche |
| 🧛 Sobrenatural | `Supernatural/SupernaturalSystem.cs` | Noctámbulos, Criaturas Sobrenaturales, Naturaleza Encantada |
| 🎨 Hobbies y clubes | `Social/HobbyClubSystem.cs` | Y Sus Hobbies, ¿Quedamos?, Ocio y Negocio |
| 🏢 Vivienda compartida | `Housing/HousingSystem.cs` | Comparten Piso, Se Alquila, Urbanitas |
| ♻️ Ecología | `Progress/EcoSystem.cs` | Vida Ecológica |
| 🐴 Rancho y pueblo | `Pets/FarmSystem.cs` | Rancho de Caballos, Vida en el Pueblo |
| 🏫 Instituto | `Education/HighSchoolSystem.cs` | Años High School |
| 👻 Más allá | `Family/AfterlifeSystem.cs` | Vida y Más Allá |
| ✈️ Actividades de viaje | `World/VacationActivity.cs` | Bon Voyage, Vida Isleña, Aventura en la Isla, A la Aventura |
| 👑 Dinastías | `Family/FamilySystem.cs` (generation, legacyScore) | Dinastías y Linajes, Menuda Familia, Creciendo en Familia |
| 🤖 Hacia el Futuro | `Future/FutureSystem.cs` + `Future/RobotCompanion.cs` | La expansión 40: NeoCiudad, máquina del tiempo con intereses y robots que ayudan en casa |
| 🎨 Arte procedural | `Editor/MySimsAssetBuilder.cs` | Prefabs base y escena completa generados desde el menú MySims del editor |

**Novedades Fase 3:**

| Sistema | Script | Qué hace |
|---|---|---|
| 💰 Economía | `Economy/EconomySystem.cs` | Billetera con eventos de cambio para UI y guardado |
| 💼 Trabajos | `Economy/JobSystem.cs` | Horario laboral, conmuta al punto de trabajo y cobra por hora |
| 📅 Calendario | `Core/CalendarSystem.cs` | Días de la semana, fin de semana y festivos |
| 🚶 NPC con rutina | `NPC/NPCRoutine.cs` | NPCs secundarios con horario diario (deambular, usar muebles, dormir, conversar) |
| 💬 Diálogos | `Social/DialogueSystem.cs` | Conversaciones por turnos con temas y relación por NPC |
| 🌟 Habilidades | `Skills/SkillSystem.cs` | 5 habilidades con niveles y XP; los muebles pueden entrenar |
| 🛒 Tienda | `UI/StoreUI.cs` | Catálogo generado del PrefabRegistry, compra con validación de dinero |
| 💼 Menú de empleos | `UI/JobsMenuUI.cs` | Ofertas con requisitos de habilidad, bloqueadas si no cumples |
| 🗺️ Mapa mundial | `World/TravelSystem.cs` + `UI/WorldMapUI.cs` | Ubicaciones por país, viajes con costo y horas, elegir casa |
| 🏖️ Zonas | `World/LocationZone.cs` | Cada ciudad/playa es una zona con contenido propio y spawn |
| 🧱 Editor de casa | `Building/WallBuilder.cs` | Paredes celda a celda con costo por metro y modo demoler |
| 🎨 Personalización | `Character/CharacterCustomizer.cs` | Piel, cabello, playera y pantalón; se guarda como hex |
| 🏆 Logros | `Progress/AchievementSystem.cs` | 10 logros conectados a todos los sistemas, con toasts para UI |
| 🎓 Universidad | `Education/UniversitySystem.cs` | 4 carreras, inscripción con colegiatura, clases con créditos y título que desbloquea empleos |
| 💍 Vida familiar | `Family/FamilySystem.cs` | Cortejo, compromiso, boda e hijos que crecen día a día |
| 🐶 Mascotas | `Pets/Pet.cs` + `Pets/PetSystem.cs` | Perros y gatos con hambre y cariño, siguen al sim, adopción y acariciar con tap |

## Estructura

```
Assets/Scripts/
├── Core/       GameManager, TimeSystem, PrefabRegistry
├── Needs/      Sistema de necesidades (el corazón del gameplay)
├── NPC/        IA de decisiones + animación procedural
├── Building/   Colocación de objetos, modo construcción y tienda
├── Economy/    Billetera y sistema de trabajos
├── Social/     Diálogos y relaciones
├── Skills/     Habilidades con XP
├── Input/      Entrada táctil (tap para mover/usar)
├── Camera/     Cámara orbital táctil
├── UI/         Barras de necesidad y controles de tiempo
├── Save/       Serialización de la partida
└── Editor/     Menú MySims: generador de prefabs y constructor de escena
```

## Licencia

Uso personal/educativo.
