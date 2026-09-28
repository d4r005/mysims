# Compilar el APK en Android Studio

El juego esta hecho en Unity 2022.3 LTS con C#. Android Studio no puede abrir
ni compilar los scripts directamente, pero Unity puede exportar el proyecto
como un proyecto Gradle que si se abre en Android Studio. Ahi si puedes
generar el APK.

## Flujo Unity → Android Studio

1. **Instalar**
   - Unity Hub + Unity 2022.3 LTS con el modulo **Android Build Support**
     (incluye Android SDK y NDK). Marcar tambien "OpenJDK".
   - Android Studio: https://developer.android.com/studio

2. **Preparar el proyecto Unity**
   - Crear un proyecto nuevo 3D y copiar la carpeta `Assets` de este repo.
   - Menu **MySims → 1. Generar prefabs base**.
   - Menu **MySims → 2. Crear escena base** (crea `Assets/Scenes/Main.unity`).
   - Agregar la escena a **File → Build Settings → Add Open Scenes**.

3. **Configurar Android**
   - En Build Settings, plataforma **Android → Switch Platform**.
   - **Player Settings**: minimo API 24, orientacion Landscape o Auto,
     "Optimized Frame Pacing" activado para movil.

4. **Exportar a Android Studio**
   - En Build Settings, marcar la casilla **"Export Project"**.
   - Click en **Export**. Unity genera una carpeta con proyecto Gradle.

5. **Generar el APK en Android Studio**
   - Abrir la carpeta exportada en Android Studio (File → Open).
   - Menu **Build → Build Bundle(s) / APK(s) → Build APK(s)**.
   - El APK queda en `app/build/outputs/apk/`.

## Alternativa mas rapida

El paso 4 y 5 se pueden saltar: en Unity, **Build Settings → Build** genera el
APK directamente, sin abrir Android Studio nunca. Android Studio solo hace
falta si quieres modificar el codigo Java/Kotlin nativo (permisos, plugins,
SDKs de anuncios, etc).

## Publicar en Google Play

Se requiere un **AAB** (Android App Bundle), no un APK:
Build → Build App Bundle en Unity, o `Build → Generate Signed Bundle / APK`
en Android Studio con una keystore creada por ti.
Cuenta de Google Play Console: 25 USD una sola vez.
