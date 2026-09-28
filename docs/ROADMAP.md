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

## Fase 3 — Mundo vivo (completada)
- [x] Economía: dinero con eventos para UI y guardado
- [x] Trabajos: horario, conmutar al punto de trabajo, pago por hora
- [x] Calendario: días de la semana, fin de semana y festivos configurables
- [x] NPCs independientes con rutinas diarias (deambular, usar muebles, dormir, conversar)
- [x] Sistema de conversación por turnos con temas y relación por NPC
- [x] Habilidades que suben con la práctica (muebles que entrenan, Carisma al conversar)
- [x] Tienda de muebles conectada a Economía (StoreUI + TrySpend al comprar)
- [x] Menú de empleos con requisitos de nivel de habilidad (JobCatalog + JobsMenuUI)

## Fase 4 — Contenido y pulido (actual)
- [x] Editor de casa: paredes celda a celda con costo y modo demoler (WallBuilder)
- [x] Personalización del personaje (colores de piel, cabello y ropa, guardado en la partida)
- [x] Logros conectados a todos los sistemas (AchievementSystem)
- [ ] Pisos y múltiples habitaciones
- [ ] Misiones diarias
- [ ] Guardado en la nube (opcional)
- [ ] Optimización móvil (draw calls, LOD, occlusion culling)

## Fase 4.5 — Mundo global (en progreso)
- [x] Catálogo mundial: Mexico. Ciudad de Mexico, Monterrey, El Carmen NL, Guadalajara, Cancun, Acapulco, Tulum. Francia. Paris, Niza, Marsella. Y mas
- [x] Zonas de ubicación por escena con contenido y spawn propio (LocationZone)
- [x] Viajes con costo en dinero y horas de juego; pasaje a casa a mitad de precio
- [x] Elegir en qué lugar del mundo vive el sim (TravelSystem.SetHome)
- [x] Mapa UI agrupado por país con costos y botón de vivir aquí (WorldMapUI)
- [x] Playas y todo el mundo: 60 ubicaciones de America, Europa, Asia, Africa y Oceania
- [ ] Playas con actividades especiales: nadar, bronceado, volley (nuevos PlaceableObject)
- [ ] Clima por país y más contenido visual por zona

## Packs de contenido estilo expansiones
- [x] Universidad: 4 carreras con colegiatura, clases por horario, creditos y graduacion con titulo que desbloquea empleos de nivel alto como Medico
- [x] Vida familiar: etapas Soltero, Cortejo, Comprometido y Casado con costos de anillo y boda, hijos que crecen cada dia
- [x] Mascotas: perros y gatos con hambre y carino, siguen al dueno, comen de su comedero y se acarician con un tap
- [ ] Universidades por ciudad, mascotas exoticas por país y eventos familiares, proxima ronda

## Las 40 expansiones de Sims 2/3/4 (ver docs/EXPANSIONS.md para el detalle completo)
- [x] 39 de 40 temas mapeados a sistemas funcionales: estaciones/clima, negocios, fama,
      sobrenatural (vampiros/hombres lobo/hadas/brujas), hobbies/clubes, vivienda compartida,
      ecologia, rancho/caballos/cultivos, instituto, mas alla/fantasmas, actividades de viaje
      (buceo, esqui, excursion, hotel), dinastias/generaciones, citas romanticas
- [x] "Hacia el Futuro" (Into the Future): FutureSystem con maquina del tiempo
      (viajar dias al futuro con interes del ahorro), robots companeros que ayudan
      en casa y la zona NeoCiudad que desbloquea el futuro al llegar
- [x] Arte base procedural: menú MySims en el editor genera 13 prefabs y monta la
      escena jugable completa (sistemas, zonas, vecinos, mascota, NavMesh y UI)

## Fase 5 — Publicación
- [ ] Cuenta Google Play Developer (pago único 25 USD)
- [ ] Ícono, capturas, ficha de tienda
- [ ] Build AAB firmado + splash screen
- [ ] Prueba interna → producción

## Assets gratuitos recomendados
- [Kenney.nl](https://kenney.nl) — muebles y personajes low-poly
- [itch.io](https://itch.io/game-assets/free) — packs de casas y UI
- [OpenGameArt](https://opengameart.org) — música y SFX
