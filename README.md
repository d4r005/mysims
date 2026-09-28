# MySims 🏠

Juego de simulación social estilo *Los Sims* para **Android**, construido con **Unity 2022.3 LTS (C#)**.

## Estado actual: Fase 4 (editor de casa, personalización, logros y mapa mundial)

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

Ver [`docs/ROADMAP.md`](docs/ROADMAP.md) para las fases completas (editor de casa, personalización, nube y publicación en Google Play).

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
| 🏆 Logros | `Progress/AchievementSystem.cs` | 6 logros conectados a todos los sistemas, con toasts para UI |

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
└── Save/       Serialización de la partida
```

## Licencia

Uso personal/educativo.
