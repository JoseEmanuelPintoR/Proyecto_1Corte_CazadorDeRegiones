using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ConfigurarEscenarios
{
    private const string CarpetaFondos = "Assets/UI/Fondos";
    private const string CarpetaElementos = "Assets/UI/Elementos";
    private const string CarpetaEscenas = "Assets/Scenes";
    private const string CarpetaPrefabs = "Assets/Prefabs/Objetos";

    private const string NombreFondo = "Fondo";
    private const string NombreVisual = "Visual";

    private static readonly string[] ObjetosSinDibujar = { "Piso", "ZonaFallo" };

    private const float FactorTamanoVisual = 2.2f;

    private const float MargenSpawnSobrePantalla = 1.5f;

    private static readonly Dictionary<string, string> SpritePorElemento = new Dictionary<string, string>
    {
        { "Cafe",             "ElementoAndina1"    },
        { "SombreroAguadeno", "ElementoAndina2"    },
        { "Tiple",            "ElementoAndina3"    },
        { "SombreroVueltiao", "ElementoCaribe1"    },
        { "TamborAlegre",     "ElementoCaribe2"    },
        { "FlorCayena",       "ElementoCaribe3"    },
        { "MarimbaChonta",    "ElementoPacifico1"  },
        { "BallenaJorobada",  "ElementoPacifico2"  },
        { "Chontaduro",       "ElementoPacifico3"  },
        { "SombreroLlanero",  "ElementoOrinoquia1" },
        { "CaballoCriollo",   "ElementoOrinoquia2" },
        { "ArpaLlanera",      "ElementoOrinoquia3" },
        { "Anaconda",         "ElementoAmazonica1" },
        { "RanaVenenosa",     "ElementoAmazonica2" },
        { "Maloca",           "ElementoAmazonica3" },
    };

    private class Nivel
    {
        public string escena;
        public string carpetaPrefabs;
        public string sufijo;
        public string fondo;
        public float lineaSuelo;
        public string[] elementos;
    }

    private static readonly Nivel[] Niveles =
    {
        new Nivel
        {
            escena = "Nivel1_Andina", carpetaPrefabs = "Nivel1_Andina", sufijo = "Andina",
            fondo = "RegionAndina", lineaSuelo = 0.18f,
            elementos = new[] { "Cafe", "SombreroAguadeno", "Tiple", "SombreroVueltiao", "BallenaJorobada", "Anaconda" }
        },
        new Nivel
        {
            escena = "Nivel2_Caribe", carpetaPrefabs = "Nivel2_Caribe", sufijo = "Caribe",
            fondo = "RegionCaribe", lineaSuelo = 0.14f,
            elementos = new[] { "SombreroVueltiao", "TamborAlegre", "FlorCayena", "Cafe", "ArpaLlanera", "Maloca" }
        },
        new Nivel
        {
            escena = "Nivel3_Pacifica", carpetaPrefabs = "Nivel3_Pacifica", sufijo = "Pacifica",
            fondo = "RegionPacifico", lineaSuelo = 0.16f,
            elementos = new[] { "MarimbaChonta", "Chontaduro", "BallenaJorobada", "SombreroAguadeno", "TamborAlegre", "CaballoCriollo" }
        },
        new Nivel
        {
            escena = "Nivel4_Orinoquia", carpetaPrefabs = "Nivel4_Orinoquia", sufijo = "Orinoquia",
            fondo = null, lineaSuelo = 0.16f,
            elementos = new[] { "ArpaLlanera", "SombreroLlanero", "CaballoCriollo", "Tiple", "Chontaduro", "RanaVenenosa" }
        },
        new Nivel
        {
            escena = "Nivel5_Amazonia", carpetaPrefabs = "Nivel5_Amazonia", sufijo = "Amazonia",
            fondo = "RegionAmazonica", lineaSuelo = 0.16f,
            elementos = new[] { "Anaconda", "RanaVenenosa", "Maloca", "FlorCayena", "MarimbaChonta", "SombreroLlanero" }
        },
    };

    [MenuItem("Herramientas/Cazador de Regiones/1 · Reimportar arte como Sprite", false, 100)]
    public static void MenuReimportarArte()
    {
        StringBuilder log = new StringBuilder();
        ReimportarArte(log);
        Debug.Log(log.ToString());
    }

    [MenuItem("Herramientas/Cazador de Regiones/2 · Colocar fondos en niveles", false, 101)]
    public static void MenuColocarFondos()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        StringBuilder log = new StringBuilder();
        ColocarFondos(log);
        Debug.Log(log.ToString());
    }

    [MenuItem("Herramientas/Cazador de Regiones/3 · Aplicar sprites a los objetos", false, 102)]
    public static void MenuAplicarSprites()
    {
        StringBuilder log = new StringBuilder();
        AplicarSpritesAObjetos(log);
        Debug.Log(log.ToString());
    }

    [MenuItem("Herramientas/Cazador de Regiones/4 · Poner el condor animado", false, 103)]
    public static void MenuCondor()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        StringBuilder log = new StringBuilder();
        ColocarCondor(log);
        Debug.Log(log.ToString());
    }

    [MenuItem("Herramientas/Cazador de Regiones/Ejecutar todo", false, 200)]
    public static void MenuEjecutarTodo()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        StringBuilder log = new StringBuilder();
        log.AppendLine("=== Cazador de Regiones · Ejecutar todo ===");
        ReimportarArte(log);
        AplicarSpritesAObjetos(log);
        ColocarFondos(log);
        ColocarCondor(log);
        Debug.Log(log.ToString());
    }

    private static void ReimportarArte(StringBuilder log)
    {
        log.AppendLine("--- 1 · Reimportar arte como Sprite ---");

        int cambiados = 0;
        int yaCorrectos = 0;

        cambiados += ReimportarCarpeta(CarpetaFondos, 2048, log, ref yaCorrectos);
        cambiados += ReimportarCarpeta(CarpetaElementos, 512, log, ref yaCorrectos);

        AssetDatabase.Refresh();

        log.AppendLine($"Texturas reimportadas: {cambiados} · ya estaban bien: {yaCorrectos}");
    }

    private static int ReimportarCarpeta(string carpeta, int tamanoMaximo, StringBuilder log, ref int yaCorrectos)
    {
        if (!AssetDatabase.IsValidFolder(carpeta))
        {
            log.AppendLine($"[aviso] No existe la carpeta {carpeta}");
            return 0;
        }

        int cambiados = 0;
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { carpeta });

        foreach (string guid in guids)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importador = AssetImporter.GetAtPath(ruta) as TextureImporter;

            if (importador == null)
            {
                continue;
            }

            bool correcto =
                importador.textureType == TextureImporterType.Sprite &&
                importador.spriteImportMode == SpriteImportMode.Single &&
                importador.alphaIsTransparency &&
                !importador.mipmapEnabled &&
                importador.wrapMode == TextureWrapMode.Clamp &&
                Mathf.Approximately(importador.spritePixelsPerUnit, 100f) &&
                importador.maxTextureSize == tamanoMaximo;

            if (correcto)
            {
                yaCorrectos++;
                continue;
            }

            importador.textureType = TextureImporterType.Sprite;
            importador.spriteImportMode = SpriteImportMode.Single;
            importador.alphaIsTransparency = true;
            importador.mipmapEnabled = false;
            importador.wrapMode = TextureWrapMode.Clamp;
            importador.spritePixelsPerUnit = 100f;
            importador.maxTextureSize = tamanoMaximo;
            importador.SaveAndReimport();

            cambiados++;
            log.AppendLine($"  Sprite: {Path.GetFileName(ruta)}");
        }

        return cambiados;
    }

    private static void ColocarFondos(StringBuilder log)
    {
        log.AppendLine("--- 2 · Colocar fondos en niveles ---");

        int guardadas = 0;

        foreach (Nivel nivel in Niveles)
        {
            string rutaEscena = $"{CarpetaEscenas}/{nivel.escena}.unity";

            if (!File.Exists(rutaEscena))
            {
                log.AppendLine($"[aviso] No existe la escena {rutaEscena}");
                continue;
            }

            Scene escena = EditorSceneManager.OpenScene(rutaEscena, OpenSceneMode.Single);

            GameObject objetoFondo = BuscarRaiz(escena, NombreFondo);
            bool recienCreado = objetoFondo == null;

            if (recienCreado)
            {
                objetoFondo = new GameObject(NombreFondo);
                SceneManager.MoveGameObjectToScene(objetoFondo, escena);
            }

            SpriteRenderer renderer = objetoFondo.GetComponent<SpriteRenderer>();

            if (renderer == null)
            {
                renderer = objetoFondo.AddComponent<SpriteRenderer>();
            }

            Sprite sprite = nivel.fondo != null ? CargarSprite($"{CarpetaFondos}/{nivel.fondo}.png") : null;
            renderer.sprite = sprite;
            renderer.sortingOrder = -100;

            FondoEscenario fondo = objetoFondo.GetComponent<FondoEscenario>();

            if (fondo == null)
            {
                fondo = objetoFondo.AddComponent<FondoEscenario>();
                fondo.distancia = 22f;
                fondo.zoomExtra = 1f;
            }

            fondo.lineaSueloNormalizada = nivel.lineaSuelo;
            fondo.camaraObjetivo = BuscarCamara(escena);
            fondo.Colocar();

            string estadoCamara = ReencuadrarCamara(fondo);
            string estadoSpawner = ReubicarSpawner(escena, fondo.camaraObjetivo);
            string estadoOcultos = OcultarUtileria(escena);
            string estadoFondo = sprite != null ? nivel.fondo : "sin sprite (falta el arte)";

            log.AppendLine($"  {nivel.escena}: {estadoFondo} · {estadoCamara} · {estadoSpawner} · {estadoOcultos}");

            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena);
            guardadas++;
        }

        log.AppendLine($"Escenas guardadas: {guardadas}");
    }

    private static string ReencuadrarCamara(FondoEscenario fondo)
    {
        Camera camara = fondo.camaraObjetivo;

        if (camara == null)
        {
            return "sin camara";
        }

        float nuevaY = fondo.AlturaCamaraParaLineaDeSuelo();
        Vector3 posicion = camara.transform.position;
        camara.transform.position = new Vector3(posicion.x, nuevaY, posicion.z);
        fondo.Colocar();

        return $"camara y={nuevaY:0.00}";
    }

    private static string ReubicarSpawner(Scene escena, Camera camara)
    {
        GameObject spawner = BuscarRaiz(escena, "Spawner");

        if (spawner == null || camara == null || camara.orthographic)
        {
            return "sin Spawner";
        }

        float profundidad = -camara.transform.position.z;
        float mitadAltura = profundidad * Mathf.Tan(camara.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float nuevaY = camara.transform.position.y + mitadAltura + MargenSpawnSobrePantalla;

        Vector3 posicion = spawner.transform.position;
        spawner.transform.position = new Vector3(posicion.x, nuevaY, posicion.z);

        return $"spawn y={nuevaY:0.0}";
    }

    private static string OcultarUtileria(Scene escena)
    {
        List<string> ocultados = new List<string>();

        foreach (string nombre in ObjetosSinDibujar)
        {
            GameObject objeto = BuscarRaiz(escena, nombre);

            if (objeto == null)
            {
                continue;
            }

            MeshRenderer renderer = objeto.GetComponent<MeshRenderer>();

            if (renderer != null)
            {
                renderer.enabled = false;
                ocultados.Add(nombre);
            }
        }

        return ocultados.Count > 0 ? $"ocultos: {string.Join(", ", ocultados)}" : "nada que ocultar";
    }

    private static Camera BuscarCamara(Scene escena)
    {
        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            Camera camara = raiz.GetComponentInChildren<Camera>(true);

            if (camara != null)
            {
                return camara;
            }
        }

        return null;
    }

    private static GameObject BuscarRaiz(Scene escena, string nombre)
    {
        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            if (raiz.name == nombre)
            {
                return raiz;
            }
        }

        return null;
    }

    private static void ColocarCondor(StringBuilder log)
    {
        AnimacionesCondor.ImportarVistas(log);
        AnimatorController controlador = AnimacionesCondor.CrearAnimaciones(log);

        if (controlador == null)
        {
            return;
        }

        log.AppendLine("--- 4 · Condor en las escenas ---");

        foreach (Nivel nivel in Niveles)
        {
            string rutaEscena = $"{CarpetaEscenas}/{nivel.escena}.unity";

            if (!File.Exists(rutaEscena))
            {
                continue;
            }

            Scene escena = EditorSceneManager.OpenScene(rutaEscena, OpenSceneMode.Single);
            log.AppendLine($"  {nivel.escena}: {AnimacionesCondor.PonerCondorEnJugador(escena, controlador)}");

            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena);
        }
    }

    private static void AplicarSpritesAObjetos(StringBuilder log)
    {
        log.AppendLine("--- 3 · Aplicar sprites a los objetos ---");

        int renombrados = 0;
        int reconstruidos = 0;

        foreach (Nivel nivel in Niveles)
        {
            string carpeta = $"{CarpetaPrefabs}/{nivel.carpetaPrefabs}";

            for (int i = 0; i < nivel.elementos.Length; i++)
            {
                int indice = i + 1;
                string elemento = nivel.elementos[i];
                string nombreDestino = $"{indice}{elemento}_{nivel.sufijo}";

                string rutaActual = BuscarPrefabPorIndice(carpeta, indice);

                if (rutaActual == null)
                {
                    log.AppendLine($"[aviso] No se encontro el prefab {indice} en {carpeta}");
                    continue;
                }

                if (Path.GetFileNameWithoutExtension(rutaActual) != nombreDestino)
                {
                    string error = AssetDatabase.RenameAsset(rutaActual, nombreDestino);

                    if (!string.IsNullOrEmpty(error))
                    {
                        log.AppendLine($"[error] Renombrando {rutaActual}: {error}");
                        continue;
                    }

                    rutaActual = $"{carpeta}/{nombreDestino}.prefab";
                    renombrados++;
                }

                if (ReconstruirPrefab(rutaActual, nombreDestino, elemento, log))
                {
                    reconstruidos++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        log.AppendLine($"Prefabs renombrados: {renombrados} · reconstruidos: {reconstruidos}");
    }

    private static string BuscarPrefabPorIndice(string carpeta, int indice)
    {
        if (!AssetDatabase.IsValidFolder(carpeta))
        {
            return null;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { carpeta });

        foreach (string guid in guids)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);

            if (Path.GetFileNameWithoutExtension(ruta).StartsWith(indice.ToString()))
            {
                return ruta;
            }
        }

        return null;
    }

    private static bool ReconstruirPrefab(string ruta, string nombreDestino, string elemento, StringBuilder log)
    {
        if (!SpritePorElemento.TryGetValue(elemento, out string nombreSprite))
        {
            log.AppendLine($"[error] Sin sprite mapeado para el elemento {elemento}");
            return false;
        }

        Sprite sprite = CargarSprite($"{CarpetaElementos}/{nombreSprite}.png");

        if (sprite == null)
        {
            log.AppendLine($"[error] No se pudo cargar el sprite {nombreSprite} (reimporta el arte primero)");
            return false;
        }

        GameObject contenido = PrefabUtility.LoadPrefabContents(ruta);

        try
        {
            contenido.name = nombreDestino;

            MeshRenderer meshRenderer = contenido.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                Object.DestroyImmediate(meshRenderer, true);
            }

            MeshFilter meshFilter = contenido.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Object.DestroyImmediate(meshFilter, true);
            }

            Transform visual = contenido.transform.Find(NombreVisual);

            if (visual == null)
            {
                GameObject nuevo = new GameObject(NombreVisual);
                visual = nuevo.transform;
                visual.SetParent(contenido.transform, false);
            }

            visual.localPosition = Vector3.zero;
            visual.localRotation = Quaternion.identity;

            SpriteRenderer renderer = visual.GetComponent<SpriteRenderer>();

            if (renderer == null)
            {
                renderer = visual.gameObject.AddComponent<SpriteRenderer>();
            }

            renderer.sprite = sprite;
            renderer.sortingOrder = 0;

            SphereCollider collider = contenido.GetComponent<SphereCollider>();
            float diametro = collider != null ? collider.radius * 2f : 1f;
            Vector3 tamanoSprite = sprite.bounds.size;
            float ladoMayor = Mathf.Max(tamanoSprite.x, tamanoSprite.y);

            if (ladoMayor > 0f)
            {
                float escala = diametro * FactorTamanoVisual / ladoMayor;
                visual.localScale = new Vector3(escala, escala, escala);
            }

            PrefabUtility.SaveAsPrefabAsset(contenido, ruta);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(contenido);
        }

        log.AppendLine($"  {nombreDestino} <- {nombreSprite}");
        return true;
    }

    private static Sprite CargarSprite(string ruta)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }
}
