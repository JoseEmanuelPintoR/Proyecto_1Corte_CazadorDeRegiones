using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class AccesoriosCondor
{
    private const string CarpetaAccesorios = "Assets/UI/Personalizacion";
    private const string CarpetaVistas = "Assets/UI/CondorVistas";
    private const string CarpetaEscenas = "Assets/Scenes";

    private const string NombreVistaPrevia = "VistaPreviaCondor";
    private const string EscenaPersonalizacion = "Personalizacion";

    private class Opcion
    {
        public string clave;
        public string ruta;
    }

    private static readonly Opcion[] Opciones =
    {
        new Opcion { clave = "Predeterminado",   ruta = CarpetaVistas + "/CondorFrente.png" },
        new Opcion { clave = "Poncho",           ruta = CarpetaAccesorios + "/CondorAndina.png" },
        new Opcion { clave = "SombreroVueltiao", ruta = CarpetaAccesorios + "/CondorCaribe.png" },
        new Opcion { clave = "VestidoDanza",     ruta = CarpetaAccesorios + "/CondorPacifico.png" },
        new Opcion { clave = "SombreroLlanero",  ruta = CarpetaAccesorios + "/CondorOrinoquia.png" },
        new Opcion { clave = "Plumas",           ruta = CarpetaAccesorios + "/CondorAmazonica.png" },
    };

    [MenuItem("Herramientas/Cazador de Regiones/6 · Accesorios del condor", false, 105)]
    public static void MenuAccesorios()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        StringBuilder log = new StringBuilder();
        log.AppendLine("=== Accesorios del condor ===");

        if (!ImportarAccesorios(log))
        {
            Debug.LogError(log.ToString());
            return;
        }

        ConfigurarPantallaPersonalizacion(log);
        ConfigurarNiveles(log);

        Debug.Log(log.ToString());
    }

    private static bool ImportarAccesorios(StringBuilder log)
    {
        TextureImporter referencia = AssetImporter.GetAtPath(AnimacionesCondor.RutaFrente) as TextureImporter;

        if (referencia == null)
        {
            log.AppendLine($"[error] No esta {AnimacionesCondor.RutaFrente}");
            return false;
        }

        TextureImporterSettings ajustesReferencia = new TextureImporterSettings();
        referencia.ReadTextureSettings(ajustesReferencia);

        if (ajustesReferencia.spriteAlignment != (int)SpriteAlignment.Custom)
        {
            log.AppendLine("[error] CondorFrente todavia no tiene el pivote calculado.");
            log.AppendLine("        Corre antes el paso 4 (Poner el condor animado).");
            return false;
        }

        Vector2 pivote = ajustesReferencia.spritePivot;
        float ppu = referencia.spritePixelsPerUnit;

        Sprite spriteReferencia = AssetDatabase.LoadAssetAtPath<Sprite>(AnimacionesCondor.RutaFrente);
        Vector2 tamanoReferencia = spriteReferencia != null ? spriteReferencia.rect.size : Vector2.zero;

        foreach (Opcion opcion in Opciones)
        {
            if (opcion.ruta == AnimacionesCondor.RutaFrente)
            {
                continue;
            }

            TextureImporter importador = AssetImporter.GetAtPath(opcion.ruta) as TextureImporter;

            if (importador == null)
            {
                log.AppendLine($"[aviso] No esta {opcion.ruta}");
                continue;
            }

            importador.textureType = TextureImporterType.Sprite;
            importador.spriteImportMode = SpriteImportMode.Single;
            importador.alphaIsTransparency = true;
            importador.mipmapEnabled = false;
            importador.wrapMode = TextureWrapMode.Clamp;
            importador.maxTextureSize = 1024;
            importador.textureCompression = TextureImporterCompression.Uncompressed;

            TextureImporterSettings ajustes = new TextureImporterSettings();
            importador.ReadTextureSettings(ajustes);
            ajustes.spriteAlignment = (int)SpriteAlignment.Custom;
            ajustes.spritePivot = pivote;
            importador.SetTextureSettings(ajustes);

            importador.spritePixelsPerUnit = ppu;
            importador.SaveAndReimport();

            Sprite importado = AssetDatabase.LoadAssetAtPath<Sprite>(opcion.ruta);

            if (importado != null && tamanoReferencia != Vector2.zero && importado.rect.size != tamanoReferencia)
            {
                log.AppendLine($"[aviso] {Path.GetFileName(opcion.ruta)} mide {importado.rect.size}, " +
                               $"distinto de CondorFrente ({tamanoReferencia}): puede quedar descuadrado");
            }

            log.AppendLine($"  {opcion.clave}: {Path.GetFileNameWithoutExtension(opcion.ruta)}");
        }

        AssetDatabase.Refresh();
        return true;
    }

    private static void ConfigurarPantallaPersonalizacion(StringBuilder log)
    {
        string ruta = $"{CarpetaEscenas}/{EscenaPersonalizacion}.unity";

        if (!File.Exists(ruta))
        {
            log.AppendLine($"[aviso] No existe {ruta}");
            return;
        }

        Scene escena = EditorSceneManager.OpenScene(ruta, OpenSceneMode.Single);
        PersonalizacionController controlador = Object.FindFirstObjectByType<PersonalizacionController>(FindObjectsInactive.Include);

        if (controlador == null)
        {
            log.AppendLine("[aviso] La escena Personalizacion no tiene PersonalizacionController");
            return;
        }

        RectTransform campo = BuscarRect(escena, "CampoNombre");
        Transform padre = campo != null ? campo.parent : controlador.transform;

        RectTransform vista = BuscarOCrearVista(padre);

        vista.anchorMin = new Vector2(0.5f, 0.5f);
        vista.anchorMax = new Vector2(0.5f, 0.5f);
        vista.pivot = new Vector2(0.5f, 0.5f);
        vista.anchoredPosition = new Vector2(0f, 570f);
        vista.sizeDelta = new Vector2(420f, 453f);
        vista.localScale = Vector3.one;
        vista.localRotation = Quaternion.identity;

        Image imagen = vista.GetComponent<Image>();

        if (imagen == null)
        {
            imagen = vista.gameObject.AddComponent<Image>();
        }

        imagen.sprite = CargarSprite(Opciones[0]);
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;

        SerializedObject serializado = new SerializedObject(controlador);
        serializado.FindProperty("vistaPrevia").objectReferenceValue = imagen;
        LlenarAccesorios(serializado.FindProperty("accesorios"));
        serializado.ApplyModifiedProperties();

        log.AppendLine($"  Personalizacion: vista previa encima del nombre + {Opciones.Length} accesorios");

        EditorSceneManager.MarkSceneDirty(escena);
        EditorSceneManager.SaveScene(escena);
    }

    private static RectTransform BuscarOCrearVista(Transform padre)
    {
        Transform existente = padre.Find(NombreVistaPrevia);

        if (existente is RectTransform encontrada)
        {
            return encontrada;
        }

        GameObject nuevo = new GameObject(NombreVistaPrevia, typeof(RectTransform), typeof(Image));
        nuevo.transform.SetParent(padre, false);
        return nuevo.GetComponent<RectTransform>();
    }

    private static void ConfigurarNiveles(StringBuilder log)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { CarpetaEscenas }))
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);

            if (!Path.GetFileName(ruta).StartsWith("Nivel"))
            {
                continue;
            }

            Scene escena = EditorSceneManager.OpenScene(ruta, OpenSceneMode.Single);
            AnimacionCondor animacion = Object.FindFirstObjectByType<AnimacionCondor>(FindObjectsInactive.Include);

            if (animacion == null)
            {
                log.AppendLine($"[aviso] {escena.name}: sin AnimacionCondor, corre antes el paso 4");
                continue;
            }

            SerializedObject serializado = new SerializedObject(animacion);
            serializado.FindProperty("spriteFrente").objectReferenceValue = CargarSprite(Opciones[0]);
            LlenarAccesorios(serializado.FindProperty("accesorios"));
            serializado.ApplyModifiedProperties();

            log.AppendLine($"  {escena.name}: condor conectado a PlayerPrefs");

            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena);
        }
    }

    private static void LlenarAccesorios(SerializedProperty lista)
    {
        lista.arraySize = Opciones.Length;

        for (int i = 0; i < Opciones.Length; i++)
        {
            SerializedProperty elemento = lista.GetArrayElementAtIndex(i);
            elemento.FindPropertyRelative("clave").stringValue = Opciones[i].clave;
            elemento.FindPropertyRelative("sprite").objectReferenceValue = CargarSprite(Opciones[i]);
        }
    }

    private static Sprite CargarSprite(Opcion opcion)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(opcion.ruta);
    }

    private static RectTransform BuscarRect(Scene escena, string nombre)
    {
        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            foreach (RectTransform rect in raiz.GetComponentsInChildren<RectTransform>(true))
            {
                if (rect.name == nombre)
                {
                    return rect;
                }
            }
        }

        return null;
    }
}
