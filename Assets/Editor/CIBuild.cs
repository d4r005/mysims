#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Metodo de build para GitHub Actions (game-ci/unity-builder).
/// buildMethod: CIBuild.BuildAndroid
///
/// Como el repo no commitea la escena ni los prefabs generados, este metodo
/// primero corre los generadores del menu MySims y despues compila el APK.
/// </summary>
public static class CIBuild
{
    public static void BuildAndroid()
    {
        // 1. Generar prefabs y escena base (lo mismo que MySims/1 y MySims/2)
        MySimsAssetBuilder.GeneratePrefabs();
        MySimsAssetBuilder.BuildScene();

        // 2. Abrir la escena recien creada
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);

        // 3. Configurar el player para Android
        PlayerSettings.companyName = "Dario Robles";
        PlayerSettings.productName = "MySims";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.dario.mysims");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        // 4. Cambiar de plataforma si hace falta
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        // 5. Compilar el APK
        var levels = new[] { scene.path };
        var report = BuildPipeline.BuildPlayer(levels, "builds/MySims.apk", BuildTarget.Android, BuildOptions.None);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception("Fallo el build de Android: " + report.summary.totalErrors + " errores.");

        Debug.Log("CIBuild: APK generado en builds/MySims.apk (" + report.summary.totalSize / 1024 / 1024 + " MB)");
    }
}
#endif
