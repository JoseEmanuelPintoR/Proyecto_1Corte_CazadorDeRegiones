using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class HacerTodo
{

    private const string ClavePendiente = "CazadorDeRegiones.HacerTodo.Pendiente.v3";

    static HacerTodo()
    {
        if (!EditorPrefs.GetBool(ClavePendiente, true))
        {
            return;
        }

        EditorApplication.delayCall += Ejecutar;
    }

    private static void Ejecutar()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {

            EditorApplication.delayCall += Ejecutar;
            return;
        }

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        EditorPrefs.SetBool(ClavePendiente, false);

        Armar();
    }

    [MenuItem("Herramientas/Cazador de Regiones/0 · Armar todo el juego", false, 99)]
    public static void MenuArmar()
    {
        Armar();
    }

    private static void Armar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("Cazador de Regiones · armado cancelado: quedaba trabajo sin guardar.");
            return;
        }

        Debug.Log("=== Cazador de Regiones · armando todo ===");

        try
        {

            ConfigurarEscenarios.MenuEjecutarTodo();
            AccesoriosCondor.MenuAccesorios();
            BotonesInterfaz.MenuBotones();
            BotonesInterfaz.MenuHUD();
            PantallaInstrucciones.MenuConstruir();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("=== Cazador de Regiones · listo. Revisa los avisos de arriba. ===");
        }
        catch (System.Exception error)
        {
            Debug.LogError($"Cazador de Regiones · el armado se corto: {error}");
        }
    }

    [MenuItem("Herramientas/Cazador de Regiones/Volver a armar en la proxima recompilada", false, 201)]
    public static void Rearmar()
    {
        EditorPrefs.SetBool(ClavePendiente, true);
        Debug.Log("Cazador de Regiones · quedo armado para la proxima recompilada. " +
                  "Toca cualquier script (o Assets > Reimport All) para dispararlo.");
    }
}
