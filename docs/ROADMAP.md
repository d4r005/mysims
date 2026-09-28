# Hoja de ruta MySims

## Fase 1 — Prototipo base (completada)
- [x] Sistema de tiempo (día/noche, velocidad)
- [x] Sistema de necesidades (5 necesidades con decaimiento)
- [x] IA del personaje (FSM: detectar necesidad → caminar → usar objeto)
- [x] Objetos interactivos (PlaceableObject)
- [x] Modo construcción básico (grid + snap + rotación)
- [x] Guardado local en JSON

## Fase 2 — Loop jugable (completada)
- [x] Interacción táctil real en Android (tap para mover/usar muebles)
- [x] Cámara orbital táctil (orbita, pinch zoom, pan) y controles en editor
- [x] UI de necesidades (barras estilo Sims con color por nivel)
- [x] Controles de velocidad del tiempo (pausa / x1 / x2 / x4)
- [x] Restaurar muebles colocados al cargar partida (PrefabRegistry por nombre)
- [x] Animación procedural básica (bob al caminar, respiración en idle)
- [ ] Sonido ambiente y música

## Fase 3 — Mundo vivo (actual)
- [x] Economía: dinero con eventos para UI y guardado
- [x] Trabajos: horario, conmutar al punto de trabajo, pago por hora
- [x] Calendario: días de la semana, fin de semana y festivos configurables
- [x] NPCs independientes con rutinas diarias (deambular, usar muebles, dormir, conversar)
- [x] Sistema de conversación por turnos con temas y relación por NPC
- [x] Habilidades que suben con la práctica (muebles que entrenan, Carisma al conversar)
- [ ] Tienda de muebles conectada a Economía (usar `price` de PlaceableObject + TrySpend)
- [ ] Menú de empleos con requisitos de nivel de habilidad

## Fase 4 — Contenido y pulido
- [ ] Editor de casa completo (paredes, pisos, múltiples habitaciones)
- [ ] Personalización del personaje (colores, ropa)
- [ ] Logros y misiones diarias
- [ ] Guardado en la nube (opcional)
- [ ] Optimización móvil (draw calls, LOD, occlusion culling)

## Fase 5 — Publicación
- [ ] Cuenta Google Play Developer (pago único 25 USD)
- [ ] Ícono, capturas, ficha de tienda
- [ ] Build AAB firmado + splash screen
- [ ] Prueba interna → producción

## Assets gratuitos recomendados
- [Kenney.nl](https://kenney.nl) — muebles y personajes low-poly
- [itch.io](https://itch.io/game-assets/free) — packs de casas y UI
- [OpenGameArt](https://opengameart.org) — música y SFX
