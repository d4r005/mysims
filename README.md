# MySims 🏠

Juego de simulación social estilo *Los Sims* para **Android**, construido con **Unity 2022.3 LTS (C#)**.

## Estado actual: Fase 1 (prototipo base)

Sistemas ya implementados como código base:

| Sistema | Script | Qué hace |
|---|---|---|
| ⏰ Tiempo | `Core/TimeSystem.cs` | Reloj día/noche, calendario, velocidad ajustable (x1 a x8) |
| 🍽️ Necesidades | `Needs/NeedsSystem.cs` | 5 necesidades (hambre, energía, social, diversión, higiene) que decaen por hora de juego |
| 🚶 IA del personaje | `NPC/NPCController.cs` | FSM: detecta la necesidad más baja, camina al objeto que la resuelve y lo usa |
| 🛋️ Objetos interactivos | `Building/PlaceableObject.cs` | Muebles que satisfacen necesidades (cama, refri, ducha, TV...) |
| 🏗️ Construcción | `Building/GridPlacement.cs` | Colocación de muebles en cuadrícula con snap y rotación (tecla R o botón táctil) |
| 💾 Guardado | `Save/SaveManager.cs` | Partida local en JSON, se guarda al pausar/cerrar la app en Android |

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

Ver [`docs/ROADMAP.md`](docs/ROADMAP.md) para las fases completas (interacción táctil, casa editable, economía, NPCs con rutinas,multiplayer y publicación en Google Play).

## Estructura

```
Assets/Scripts/
├── Core/       GameManager, TimeSystem (reloj del juego)
├── Needs/      Sistema de necesidades (el corazón del gameplay)
├── NPC/        IA de movimiento y decisiones
├── Building/   Colocación de objetos y modo construcción
└── Save/       Serialización de la partida
```

## Licencia

Uso personal/educativo.
