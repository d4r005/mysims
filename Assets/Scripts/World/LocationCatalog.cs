using System.Collections.Generic;

namespace MySims
{
    /// <summary>
    /// Datos de las ubicaciones del mundo: ciudades, pueblos y playas de todos
    /// los continentes. Las zonas de escena con el mismo id se conectan solas.
    /// </summary>
    public static class LocationCatalog
    {
        public static readonly List<LocationInfo> Locations = new List<LocationInfo>
        {
            // ===== Mexico =====
            new LocationInfo { id = "cdmx",         displayName = "Ciudad de Mexico", country = "Mexico",      type = LocationType.Ciudad, travelCost = 120f,  travelHours = 5f,  description = "La capital. Museos, tacos y mucho movimiento." },
            new LocationInfo { id = "monterrey",    displayName = "Monterrey",        country = "Mexico",      type = LocationType.Ciudad, travelCost = 0f,    travelHours = 0f,  description = "La ciudad de las montanas. Negocios y birria." },
            new LocationInfo { id = "elcarmen",     displayName = "El Carmen",        country = "Mexico",      type = LocationType.Pueblo, travelCost = 60f,   travelHours = 2f,  description = "Pueblo tranquilo de Nuevo Leon, entre nogales." },
            new LocationInfo { id = "guadalajara",  displayName = "Guadalajara",        country = "Mexico",      type = LocationType.Ciudad, travelCost = 90f,   travelHours = 3f,  description = "Tierra del mariachi y la torta ahogada." },
            new LocationInfo { id = "cancun",       displayName = "Cancun",            country = "Mexico",      type = LocationType.Playa,  travelCost = 250f,  travelHours = 6f,  description = "Playas turquesa del Caribe mexicano." },
            new LocationInfo { id = "acapulco",     displayName = "Acapulco",          country = "Mexico",      type = LocationType.Playa,  travelCost = 180f,  travelHours = 5f,  description = "La playa clasica de la Costa Grande." },
            new LocationInfo { id = "tulum",        displayName = "Tulum",             country = "Mexico",      type = LocationType.Playa,  travelCost = 220f,  travelHours = 6f,  description = "Ruinas mayas frente al mar." },
            new LocationInfo { id = "merida",       displayName = "Merida",            country = "Mexico",      type = LocationType.Ciudad, travelCost = 150f,  travelHours = 4f,  description = "La ciudad blanca y su sopa de lima." },
            new LocationInfo { id = "puebla",       displayName = "Puebla",            country = "Mexico",      type = LocationType.Ciudad, travelCost = 100f,  travelHours = 3f,  description = "Cemitas, chiles en nogada y talavera." },
            new LocationInfo { id = "vallarta",     displayName = "Puerto Vallarta",   country = "Mexico",      type = LocationType.Playa,  travelCost = 160f,  travelHours = 4f,  description = "Bahia, malecon y atardeceres." },

            // ===== America =====
            new LocationInfo { id = "nyc",          displayName = "Nueva York",        country = "EEUU",        type = LocationType.Ciudad, travelCost = 600f,  travelHours = 9f,  description = "La ciudad que nunca duerme." },
            new LocationInfo { id = "losangeles",   displayName = "Los Angeles",       country = "EEUU",        type = LocationType.Ciudad, travelCost = 550f,  travelHours = 8f,  description = "Hollywood, palmeras y tacones de sol." },
            new LocationInfo { id = "miami",        displayName = "Miami",             country = "EEUU",        type = LocationType.Playa,  travelCost = 500f,  travelHours = 8f,  description = "South Beach y vida nocturna." },
            new LocationInfo { id = "sanfran",      displayName = "San Francisco",     country = "EEUU",        type = LocationType.Ciudad, travelCost = 580f,  travelHours = 9f,  description = "El Golden Gate y la niebla de la bahia." },
            new LocationInfo { id = "honolulu",    displayName = "Honolulu",          country = "EEUU",        type = LocationType.Playa,  travelCost = 800f,  travelHours = 12f, description = "Waikiki y surf en Oahu." },
            new LocationInfo { id = "toronto",      displayName = "Toronto",           country = "Canada",      type = LocationType.Ciudad, travelCost = 620f,  travelHours = 9f,  description = "La torre CN y su centro multicultural." },
            new LocationInfo { id = "vancouver",    displayName = "Vancouver",         country = "Canada",      type = LocationType.Ciudad, travelCost = 650f,  travelHours = 10f, description = "Mar y montanas en la misma ciudad." },
            new LocationInfo { id = "habana",       displayName = "La Habana",         country = "Cuba",        type = LocationType.Playa,  travelCost = 300f,  travelHours = 6f,  description = "Malecon, classicos y son cubano." },
            new LocationInfo { id = "buenosaires",  displayName = "Buenos Aires",     country = "Argentina",  type = LocationType.Ciudad, travelCost = 750f,  travelHours = 11f, description = "El tango y el Obelisco de la Reina del Plata." },
            new LocationInfo { id = "lima",        displayName = "Lima",             country = "Peru",       type = LocationType.Ciudad, travelCost = 600f,  travelHours = 9f,  description = "Gastronomia de altura y el malecon de Miraflores." },
            new LocationInfo { id = "cartagena",    displayName = "Cartagena",         country = "Colombia",    type = LocationType.Playa,  travelCost = 550f,  travelHours = 8f,  description = "Ciudad amurallada y mar Caribe." },
            new LocationInfo { id = "saopaulo",    displayName = "Sao Paulo",         country = "Brasil",      type = LocationType.Ciudad, travelCost = 700f,  travelHours = 10f, description = "La gigante del sur y su gastronomia." },
            new LocationInfo { id = "rio",          displayName = "Rio de Janeiro",     country = "Brasil",      type = LocationType.Playa,  travelCost = 700f,  travelHours = 10f, description = "Copacabana, samba y el Pan de Azucar." },
            new LocationInfo { id = "santiago",     displayName = "Santiago de Chile", country = "Chile",      type = LocationType.Ciudad, travelCost = 780f,  travelHours = 11f, description = "Los Andes de fondo en cada mirador." },
            new LocationInfo { id = "patagonia",    displayName = "Patagonia",         country = "Argentina",   type = LocationType.Campo,  travelCost = 900f,  travelHours = 13f, description = "Glaciares, guanacos y cielo infinito." },

            // ===== Europa =====
            new LocationInfo { id = "madrid",       displayName = "Madrid",            country = "Espana",      type = LocationType.Ciudad, travelCost = 800f,  travelHours = 11f, description = "Churros con chocolate y noches largas." },
            new LocationInfo { id = "barcelona",    displayName = "Barcelona",        country = "Espana",      type = LocationType.Playa,  travelCost = 820f,  travelHours = 11f, description = "La Sagrada Familia y el Barceloneta." },
            new LocationInfo { id = "lisboa",       displayName = "Lisboa",            country = "Portugal",    type = LocationType.Ciudad, travelCost = 830f,  travelHours = 11f, description = "El Tajo, fado y pasteles de Belem." },
            new LocationInfo { id = "londres",      displayName = "Londres",           country = "Inglaterra",  type = LocationType.Ciudad, travelCost = 850f,  travelHours = 12f, description = "Big Ben, mercados y te en la tarde." },
            new LocationInfo { id = "paris",        displayName = "Paris",             country = "Francia",     type = LocationType.Ciudad, travelCost = 900f,  travelHours = 12f, description = "La ciudad luz. Croissants y arte por doquier." },
            new LocationInfo { id = "niza",        displayName = "Niza",             country = "Francia",     type = LocationType.Playa,  travelCost = 950f,  travelHours = 13f, description = "La Riviera francesa y su mercado de flores." },
            new LocationInfo { id = "marsella",     displayName = "Marsella",          country = "Francia",     type = LocationType.Ciudad, travelCost = 880f,  travelHours = 12f, description = "Puerto mediterraneo y bouillabaisse." },
            new LocationInfo { id = "roma",        displayName = "Roma",             country = "Italia",      type = LocationType.Ciudad, travelCost = 850f,  travelHours = 12f, description = "Historia en cada esquina y carbonara." },
            new LocationInfo { id = "venecia",      displayName = "Venecia",            country = "Italia",      type = LocationType.Ciudad, travelCost = 870f,  travelHours = 12f, description = "Canales, gondolas y carnival." },
            new LocationInfo { id = "berlin",       displayName = "Berlin",            country = "Alemania",    type = LocationType.Ciudad, travelCost = 890f,  travelHours = 12f, description = "Museos, arte callejero y currywurst." },
            new LocationInfo { id = "amsterdam",    displayName = "Amsterdam",          country = "Paises Bajos", type = LocationType.Ciudad, travelCost = 880f, travelHours = 12f, description = "Canales, bicicletas y tulipanes." },
            new LocationInfo { id = "atenas",       displayName = "Atenas",             country = "Grecia",      type = LocationType.Playa,  travelCost = 950f,  travelHours = 13f, description = "La Acrropolis y el mar Egeo." },
            new LocationInfo { id = "estocolmo",    displayName = "Estocolmo",          country = "Suecia",      type = LocationType.Ciudad, travelCost = 980f,  travelHours = 13f, description = "Islas, diseno y canal de niebla." },
            new LocationInfo { id = "praga",        displayName = "Praga",              country = "Chequia",     type = LocationType.Ciudad, travelCost = 900f,  travelHours = 12f, description = "Puentes, torres y cerveza centenaria." },
            new LocationInfo { id = "monaco",      displayName = "Monaco",            country = "Monaco",     type = LocationType.Ciudad, travelCost = 1000f, travelHours = 13f, description = "Yates, casino y la Formula 1." },
            new LocationInfo { id = "moscu",        displayName = "Moscu",              country = "Rusia",       type = LocationType.Ciudad, travelCost = 950f,  travelHours = 13f, description = "La Plaza Roja bajo la nieve." },
            new LocationInfo { id = "reikiavik",    displayName = "Reikiavik",         country = "Islandia",    type = LocationType.Ciudad, travelCost = 1100f, travelHours = 14f, description = "Auroras boreales y aguas termales." },

            // ===== Asia =====
            new LocationInfo { id = "tokio",        displayName = "Tokio",             country = "Japon",       type = LocationType.Ciudad, travelCost = 1200f, travelHours = 14f, description = "Neones, ramen y santuarios tranquilos." },
            new LocationInfo { id = "kioto",        displayName = "Kioto",             country = "Japon",       type = LocationType.Pueblo, travelCost = 1220f, travelHours = 14f, description = "Templos, cerezos y geishas." },
            new LocationInfo { id = "seul",        displayName = "Seul",              country = "Corea del Sur", type = LocationType.Ciudad, travelCost = 1180f, travelHours = 14f, description = "K-pop, palacios y cafes de tema." },
            new LocationInfo { id = "pekin",        displayName = "Pekin",             country = "China",       type = LocationType.Ciudad, travelCost = 1150f, travelHours = 14f, description = "La Gran Muralla y la Ciudad Prohibida." },
            new LocationInfo { id = "hongkong",     displayName = "Hong Kong",         country = "China",       type = LocationType.Ciudad, travelCost = 1150f, travelHours = 14f, description = "Rascacielos y dim sum junto a la bahia." },
            new LocationInfo { id = "bangkok",      displayName = "Bangkok",            country = "Tailandia",  type = LocationType.Ciudad, travelCost = 1100f, travelHours = 13f, description = "Templos dorados y pad thai de calle." },
            new LocationInfo { id = "bali",        displayName = "Bali",              country = "Indonesia",   type = LocationType.Playa,  travelCost = 1150f, travelHours = 14f, description = "Arrozales, templos y olas." },
            new LocationInfo { id = "bombay",       displayName = "Bombay",            country = "India",       type = LocationType.Ciudad, travelCost = 1100f, travelHours = 13f, description = "Bollywood, especias y la puerta de India." },
            new LocationInfo { id = "dubai",        displayName = "Dubai",             country = "Emiratos",    type = LocationType.Ciudad, travelCost = 1200f, travelHours = 14f, description = "Desierto, rascacielos y souks de oro." },
            new LocationInfo { id = "singapur",     displayName = "Singapur",          country = "Singapur",    type = LocationType.Ciudad, travelCost = 1180f, travelHours = 14f, description = "Jardines futuristas y hawker centers." },
            new LocationInfo { id = "katmandu",     displayName = "Katmandu",          country = "Nepal",       type = LocationType.Campo,  travelCost = 1200f, travelHours = 15f, description = "La puerta del Himalaya." },

            // ===== Africa y Oceania =====
            new LocationInfo { id = "elcairo",      displayName = "El Cairo",          country = "Egipto",      type = LocationType.Ciudad, travelCost = 950f,  travelHours = 13f, description = "Las piramides y el Nilo." },
            new LocationInfo { id = "marrakech",    displayName = "Marrakech",          country = "Marruecos",   type = LocationType.Ciudad, travelCost = 900f,  travelHours = 12f, description = "Zocos, teterias y la Koutoubia." },
            new LocationInfo { id = "capetown",     displayName = "Ciudad del Cabo",   country = "Sudafrica",   type = LocationType.Playa,  travelCost = 1150f, travelHours = 15f, description = "La Montana de la Mesa y sus playas." },
            new LocationInfo { id = "nairobi",      displayName = "Nairobi",            country = "Kenia",       type = LocationType.Campo,  travelCost = 1100f, travelHours = 14f, description = "Safari y la sabana a una hora del centro." },
            new LocationInfo { id = "sidney",       displayName = "Sidney",            country = "Australia",  type = LocationType.Playa,  travelCost = 1300f, travelHours = 16f, description = "La Opera y Bondi Beach." },
            new LocationInfo { id = "auckland",     displayName = "Auckland",           country = "Nueva Zelanda", type = LocationType.Ciudad, travelCost = 1350f, travelHours = 16f, description = "Veleros, volcanes y maori culture." },
            new LocationInfo { id = "borabora",     displayName = "Bora Bora",          country = "Polinesia",  type = LocationType.Playa,  travelCost = 1500f, travelHours = 18f, description = "Bungalows sobre agua turquesa." },

            // ===== El futuro. Pack Hacia el Futuro =====
            new LocationInfo { id = "neociudad",    displayName = "NeoCiudad",         country = "Futuro",     type = LocationType.Ciudad, travelCost = 2000f, travelHours = 20f, description = "La ciudad del manana. Robots, hologramas y la maquina del tiempo." }
        };

        public static LocationInfo Get(string id) => Locations.Find(l => l.id == id);
    }
}
