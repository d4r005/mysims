using System.Collections.Generic;

namespace MySims
{
    /// <summary>
    /// Datos de las ubicaciones del mundo. Usado por el mapa para mostrar
    /// opciones aunque la zona todavia no exista en escena. Las zonas de escena
    /// con el mismo id se conectan automaticamente.
    /// </summary>
    public static class LocationCatalog
    {
        public static readonly List<LocationInfo> Locations = new List<LocationInfo>
        {
            // Mexico
            new LocationInfo { id = "cdmx",         displayName = "Ciudad de Mexico", country = "Mexico", type = LocationType.Ciudad, travelCost = 120f, travelHours = 5f, description = "La capital. Museos, tacos y mucho movimiento." },
            new LocationInfo { id = "monterrey",    displayName = "Monterrey",        country = "Mexico", type = LocationType.Ciudad, travelCost = 0f,   travelHours = 0f, description = "La ciudad de las montañas. Negocios y birria." },
            new LocationInfo { id = "elcarmen",     displayName = "El Carmen",        country = "Mexico", type = LocationType.Pueblo, travelCost = 60f,  travelHours = 2f, description = "Pueblo tranquilo de Nuevo Leon, entre nogales." },
            new LocationInfo { id = "guadalajara",  displayName = "Guadalajara",       country = "Mexico", type = LocationType.Ciudad, travelCost = 90f,  travelHours = 3f, description = "Tierra del mariachi y la torta ahogada." },
            new LocationInfo { id = "cancun",       displayName = "Cancun",           country = "Mexico", type = LocationType.Playa,  travelCost = 250f, travelHours = 6f, description = "Playas turquesa del Caribe mexicano." },
            new LocationInfo { id = "acapulco",     displayName = "Acapulco",         country = "Mexico", type = LocationType.Playa,  travelCost = 180f, travelHours = 5f, description = "La playa clasica de la Costa Grande." },
            new LocationInfo { id = "tulum",        displayName = "Tulum",            country = "Mexico", type = LocationType.Playa,  travelCost = 220f, travelHours = 6f, description = "Ruinas mayas frente al mar." },

            // Francia
            new LocationInfo { id = "paris",        displayName = "Paris",            country = "Francia", type = LocationType.Ciudad, travelCost = 900f, travelHours = 12f, description = "La ciudad luz. Croissants y arte por doquier." },
            new LocationInfo { id = "niza",         displayName = "Niza",             country = "Francia", type = LocationType.Playa,  travelCost = 950f, travelHours = 13f, description = "La Riviera francesa. Playas y mercado de flores." },
            new LocationInfo { id = "marsella",     displayName = "Marsella",         country = "Francia", type = LocationType.Ciudad, travelCost = 880f, travelHours = 12f, description = "Puerto mediterraneo, bouillabaisse y calcio." },

            // El resto del mundo
            new LocationInfo { id = "madrid",       displayName = "Madrid",           country = "Espana", type = LocationType.Ciudad, travelCost = 800f, travelHours = 11f, description = "Churros con chocolate y noches largas." },
            new LocationInfo { id = "miami",        displayName = "Miami",            country = "EEUU",   type = LocationType.Playa,  travelCost = 500f, travelHours = 8f,  description = "Playa de South Beach y vida nocturna." },
            new LocationInfo { id = "rio",          displayName = "Rio de Janeiro",   country = "Brasil", type = LocationType.Playa,  travelCost = 700f, travelHours = 10f, description = "Copacabana, samba y el Pan de Azucar." },
            new LocationInfo { id = "tokio",        displayName = "Tokio",            country = "Japon", type = LocationType.Ciudad, travelCost = 1200f, travelHours = 14f, description = "Neones, ramen y santuarios tranquilos." },
            new LocationInfo { id = "roma",        displayName = "Roma",             country = "Italia", type = LocationType.Ciudad, travelCost = 850f, travelHours = 12f, description = "Historia en cada esquina y carbonara." }
        };

        public static LocationInfo Get(string id) => Locations.Find(l => l.id == id);
    }
}
