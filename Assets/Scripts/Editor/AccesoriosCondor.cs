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

        public string vistaPrevia;

        public string frente;
        public string lado;
        public string tresCuartos;
    }

    private static readonly Opcion[] Opciones =
    {
        new Opcion
        {
            clave = "Predeterminado",
            vistaPrevia = CarpetaVistas + "/CondorFrente.png",
            frente = CarpetaVistas + "/CondorFrente.png",
            lado = CarpetaVistas + "/CondorLado.png",
            tresCuartos = CarpetaVistas + "/Condor3-4.png"
        },
        new Opcion
        {
            clave = "Poncho",
            vistaPrevia = CarpetaAccesorios + "/CondorAndina.png",
            frente = CarpetaVistas + "/CondorFrenteAndina.png",
            lado = CarpetaVistas + "/CondorLadoAndina.png",
            tresCuartos = CarpetaVistas + "/Condor3-4Andina.png"
        },
        new Opcion
        {
            clave = "SombreroVueltiao",
            vistaPrevia = CarpetaAccesorios + "/CondorCaribe.png",
            frente = CarpetaVistas + "/CondorFrenteCaribe.png",
            lado = CarpetaVistas + "/CondorLadoCaribe.png",
            tresCuartos = CarpetaVistas + "/Condor3-4Caribe.png"
        },
        new Opcion
        {
            clave = "VestidoDanza",
            vistaPrevia = CarpetaAccesorios + "/CondorPacifico.png",
            frente = CarpetaVistas + "/CondorFrentePacifico.png",
            lado = CarpetaVistas + "/CondorLadoPacifico.png",
            tresCuartos = CarpetaVistas + "/Condor3-4Pacifico.png"
        },
        new Opcion
        {
            clave = "SombreroLlanero",
            vistaPrevia = CarpetaAccesorios + "/CondorOrinoquia.png",
            frente = CarpetaVistas + "/CondorFrenteOrinoquia.png",
            lado = CarpetaVistas + "/CondorLadoOrinoquia.png",
            tresCuartos = CarpetaVistas + "/Condor3-4Orinoquia.png"
        },
        new Opcion
        {
            clave = "Plumas",
            vistaPrevia = CarpetaAccesorios + "/CondorAmazonica.png",
            frente = CarpetaVistas + "/CondorFrenteAmazonica.png",
            lado = CarpetaVistas + "/CondorLadoAmazonia.png",
            tresCuartos = CarpetaVistas + "/Condor3-4Amazonia.png"
        },
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

    private class Referencia
    {
        public Vector2 pivote;
        public float ppu;
        public Vector2 tamano;
    }

    private static bool LeerReferencia(string ruta, StringBuilder log, out Referencia referencia)
    {
        referencia = null;

        TextureImporter importador = AssetImporter.GetAtPath(ruta) as TextureImporter;

        if (importador == null)
        {
            log.AppendLine($"[error] No esta {ruta}");
            return false;
        }

        TextureImporterSettings ajustes = new TextureImporterSettings();
        importador.ReadTextureSettings(ajustes);

        if (ajustes.spriteAlignment != (int)SpriteAlignment.Custom)
        {
            log.AppendLine($"[error] {Path.GetFileNameWithoutExtension(ruta)} todavia no tiene el pivote calculado.");
            log.AppendLine("        Corre antes el paso 4 (Poner el condor animado).");
            return false;
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);

        referencia = new Referencia
        {
            pivote = ajustes.spritePivot,
            ppu = importador.spritePixelsPerUnit,
            tamano = sprite != null ? sprite.rect.size : Vector2.zero
        };

        return true;
    }

    private static bool ImportarAccesorios(StringBuilder log)
    {
        log.AppendLine("--- Vistas de cada accesorio ---");

        if (!LeerReferencia(AnimacionesCondor.RutaFrente, log, out Referencia frente) ||
            !LeerReferencia(AnimacionesCondor.RutaLado, log, out Referencia lado) ||
            !LeerReferencia(AnimacionesCondor.RutaTresCuartos, log, out Referencia tresCuartos))
        {
            return false;
        }

        foreach (Opcion opcion in Opciones)
        {
            int importadas = 0;

            importadas += Importar(opcion.frente, frente, log) ? 1 : 0;
            importadas += Importar(opcion.lado, lado, log) ? 1 : 0;
            importadas += Importar(opcion.tresCuartos, tresCuartos, log) ? 1 : 0;

            if (opcion.vistaPrevia != opcion.frente)
            {
                Importar(opcion.vistaPrevia, frente, log);
            }

            log.AppendLine($"  {opcion.clave}: {importadas}/3 vistas");
        }

        AssetDatabase.Refresh();
        return true;
    }

    private static bool Importar(string ruta, Referencia referencia, StringBuilder log)
    {
        TextureImporter importador = AssetImporter.GetAtPath(ruta) as TextureImporter;

        if (importador == null)
        {
            log.AppendLine($"[aviso] No esta {ruta}");
            return false;
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
        ajustes.spritePivot = referencia.pivote;
        importador.SetTextureSettings(ajustes);

        importador.spritePixelsPerUnit = referencia.ppu;
        importador.SaveAndReimport();

        Sprite importado = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);

        if (importado != null && referencia.tamano != Vector2.zero && importado.rect.size != referencia.tamano)
        {
            log.AppendLine($"[aviso] {Path.GetFileName(ruta)} mide {importado.rect.size}, " +
                           $"distinto de su vista base ({referencia.tamano}): puede quedar descuadrado");
        }

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

        imagen.sprite = Cargar(Opciones[0].vistaPrevia);
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;

        SerializedObject serializado = new SerializedObject(controlador);
        serializado.FindProperty("vistaPrevia").objectReferenceValue = imagen;
        LlenarVistasPrevias(serializado.FindProperty("accesorios"));
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
            serializado.FindProperty("spriteFrente").objectReferenceValue = Cargar(AnimacionesCondor.RutaFrente);
            serializado.FindProperty("spriteLado").objectReferenceValue = Cargar(AnimacionesCondor.RutaLado);
            serializado.FindProperty("spriteTresCuartos").objectReferenceValue = Cargar(AnimacionesCondor.RutaTresCuartos);
            LlenarSkins(serializado.FindProperty("skins"));
            serializado.ApplyModifiedProperties();

            log.AppendLine($"  {escena.name}: condor con las 3 vistas por accesorio");

            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena);
        }
    }

    private static void LlenarVistasPrevias(SerializedProperty lista)
    {
        lista.arraySize = Opciones.Length;

        for (int i = 0; i < Opciones.Length; i++)
        {
            SerializedProperty elemento = lista.GetArrayElementAtIndex(i);
            elemento.FindPropertyRelative("clave").stringValue = Opciones[i].clave;
            elemento.FindPropertyRelative("sprite").objectReferenceValue = Cargar(Opciones[i].vistaPrevia);
        }
    }

    private static void LlenarSkins(SerializedProperty lista)
    {
        lista.arraySize = Opciones.Length;

        for (int i = 0; i < Opciones.Length; i++)
        {
            SerializedProperty elemento = lista.GetArrayElementAtIndex(i);
            elemento.FindPropertyRelative("clave").stringValue = Opciones[i].clave;
            elemento.FindPropertyRelative("frente").objectReferenceValue = Cargar(Opciones[i].frente);
            elemento.FindPropertyRelative("lado").objectReferenceValue = Cargar(Opciones[i].lado);
            elemento.FindPropertyRelative("tresCuartos").objectReferenceValue = Cargar(Opciones[i].tresCuartos);
        }
    }

    public static string VistaPreviaDe(string clave)
    {
        foreach (Opcion opcion in Opciones)
        {
            if (opcion.clave == clave)
            {
                return opcion.vistaPrevia;
            }
        }

        return null;
    }

    private static Sprite Cargar(string ruta)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
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
