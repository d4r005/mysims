# Mapeo de expansiones (Sims 2, 3 y 4) a sistemas de MySims

Cada expansion oficial de EA se mapeo a un sistema de C# ya implementado en este
repo. La idea no es clonar el arte de EA, sino recrear la mecanica de juego con
sistemas propios y gratuitos.

## Los Sims 2 (8 expansiones)

| # | Expansion | Tema | Sistema implementado |
|---|---|---|---|
| 1 | Universitarios | Universidad, carreras | `Education/UniversitySystem.cs` |
| 2 | Noctambulos | Citas, vampiros | `Family/FamilySystem.cs` (GoOnDate) + `Supernatural/SupernaturalSystem.cs` (Vampiro) |
| 3 | Abren Negocios | Negocios, empleados | `Economy/BusinessSystem.cs` |
| 4 | Mascotas | Perros, gatos, hombres lobo | `Pets/Pet.cs` + `Supernatural/SupernaturalSystem.cs` (HombreLobo) |
| 5 | Y las Cuatro Estaciones | Estaciones del año | `World/SeasonSystem.cs` |
| 6 | Bon Voyage | Vacaciones, hoteles | `World/VacationActivity.cs` (CheckInHotel) + `World/TravelSystem.cs` |
| 7 | Y Sus Hobbies | Hobbies, clubes | `Social/HobbyClubSystem.cs` |
| 8 | Comparten Piso | Departamentos, roommates | `Housing/HousingSystem.cs` |

## Los Sims 3 (11 expansiones)

| # | Expansion | Tema | Sistema implementado |
|---|---|---|---|
| 1 | Trotamundos | Viajes, Egipto, Francia, China | `World/TravelSystem.cs` + `World/LocationCatalog.cs` (60 ciudades) |
| 2 | Triunfadores | Profesiones activas, bombero, detective | `Economy/JobCatalog.cs` (Bombero, Detective) |
| 3 | Al Caer la Noche | Fama, clubes, vampiros | `Progress/FameSystem.cs` + `Social/HobbyClubSystem.cs` (clubes) |
| 4 | Menuda Familia | Etapas de vida, familia, bodas | `Family/FamilySystem.cs` |
| 5 | Vaya Fauna | Perros, gatos, caballos | `Pets/Pet.cs` + `Pets/FarmSystem.cs` (Horse) |
| 6 | Salto a la Fama | Cantantes, artistas | `Progress/FameSystem.cs` |
| 7 | Criaturas Sobrenaturales | Brujas, hadas, hombres lobo | `Supernatural/SupernaturalSystem.cs` |
| 8 | Y las Cuatro Estaciones | Clima, festividades | `World/SeasonSystem.cs` |
| 9 | Movida en la Universidad | Universidad, vida social | `Education/UniversitySystem.cs` |
| 10 | Aventura en la Isla | Islas, buceo | `World/VacationActivity.cs` (Snorkel, Excursion) |
| 11 | Hacia el Futuro | Viajes al futuro, robots | `Future/FutureSystem.cs` + `Future/RobotCompanion.cs` + zona NeoCiudad |

## Los Sims 4 (21 expansiones)

| # | Expansion | Tema | Sistema implementado |
|---|---|---|---|
| 1 | A Trabajar | Profesiones activas, negocios | `Economy/JobSystem.cs` + `Economy/BusinessSystem.cs` |
| 2 | ¿Quedamos? | Clubes, vida social | `Social/HobbyClubSystem.cs` |
| 3 | Urbanitas | Departamentos, ciudad | `Housing/HousingSystem.cs` |
| 4 | Perros y Gatos | Mascotas, veterinaria | `Pets/Pet.cs` + `Pets/PetSystem.cs` |
| 5 | Y las Cuatro Estaciones | Clima, festividades | `World/SeasonSystem.cs` |
| 6 | ¡Rumbo a la Fama! | Celebridades | `Progress/FameSystem.cs` |
| 7 | Vida Isleña | Islas, playas, buceo | `World/VacationActivity.cs` + zonas tipo Playa en `LocationCatalog.cs` |
| 8 | Días de Universidad | Universidad, carreras | `Education/UniversitySystem.cs` |
| 9 | Vida Ecológica | Ecologia, reciclaje | `Progress/EcoSystem.cs` |
| 10 | Escapada en la Nieve | Nieve, esqui | `World/SeasonSystem.cs` (Invierno) + `World/VacationActivity.cs` (Esqui) |
| 11 | Vida en el Pueblo | Campo, agricultura | `Pets/FarmSystem.cs` (cultivos) |
| 12 | Años High School | Instituto, adolescencia | `Education/HighSchoolSystem.cs` |
| 13 | Creciendo en Familia | Familias, bebes, crianza | `Family/FamilySystem.cs` (hijos que crecen) |
| 14 | Rancho de Caballos | Caballos, ranchos | `Pets/FarmSystem.cs` (Horse) |
| 15 | Se Alquila | Departamentos, alquileres | `Housing/HousingSystem.cs` |
| 16 | ¡Viva el Amor! | Romance, citas | `Family/FamilySystem.cs` (GoOnDate) |
| 17 | Vida y Más Allá | Muerte, fantasmas | `Family/AfterlifeSystem.cs` |
| 18 | Ocio y Negocio | Negocios, hobbies | `Economy/BusinessSystem.cs` + `Social/HobbyClubSystem.cs` |
| 19 | Naturaleza Encantada | Hadas, magia | `Supernatural/SupernaturalSystem.cs` (Hada, Bruja) |
| 20 | ¡A la Aventura! | Aventuras, exploracion | `World/VacationActivity.cs` (Excursion) |
| 21 | Dinastías y Linajes | Generaciones, legado | `Family/FamilySystem.cs` (generation, legacyScore, AdvanceGeneration) |

## Notas

- Cada sistema es un `MonoBehaviour` singleton (`Instance`) que se puede colocar
  en el `GameManager` o su propio GameObject en la escena.
- El guardado (`Save/SaveManager.cs`) ya incluye los campos principales de cada
  pack (estacion, clima, negocio, fama, forma sobrenatural, ecologia, vivienda,
  generacion familiar).
- Las **40/40** expansiones ya están mapeadas a sistemas funcionales.
- El arte placeholder se genera desde el editor: menú **MySims** en Unity
  (`Assets/Editor/MySimsAssetBuilder.cs`).
