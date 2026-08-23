using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public static class TipografiaProyecto
{
    private const string CarpetaFuentes = "Assets/Fonts";
    private const string CarpetaEscenas = "Assets/Scenes";

    private class Familia
    {
        public string archivo;
        public string asset;
        public string uso;
    }

    private static readonly Familia Titulos = new Familia
    {
        archivo = "Merienda-VariableFont_wght", asset = "Merienda SDF", uso = "titulos"
    };

    private static readonly Familia Botones = new Familia
    {
        archivo = "Boogaloo-Regular", asset = "Boogaloo SDF", uso = "botones"
    };

    private static readonly Familia Cuerpo = new Familia
    {
        archivo = "Nunito-VariableFont_wght", asset = "Nunito SDF", uso = "cuerpo"
    };

    private static readonly HashSet<string> TitularesDePanel = new HashSet<string>
    {
        "TextoDerrota", "TextoVictoria", "TextoPausa"
    };

    private static readonly HashSet<string> ConLetraDeBotones = new HashSet<string>
    {
        "NombreIntegrante", "RolIntegrante"
    };

    [MenuItem("Herramientas/Cazador de Regiones/5 · Aplicar tipografia", false, 104)]
    public static void MenuAplicarTipografia()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        StringBuilder log = new StringBuilder();
        log.AppendLine("=== Tipografia ===");

        TMP_FontAsset titulos = ObtenerFuente(Titulos, log);
        TMP_FontAsset botones = ObtenerFuente(Botones, log);
        TMP_FontAsset cuerpo = ObtenerFuente(Cuerpo, log);

        if (titulos == null || botones == null || cuerpo == null)
        {
            log.AppendLine("[error] Faltan fuentes, no se aplica nada.");
            Debug.LogError(log.ToString());
            return;
        }

        AplicarEnEscenas(titulos, botones, cuerpo, log);
        Debug.Log(log.ToString());
    }

    private static TMP_FontAsset ObtenerFuente(Familia familia, StringBuilder log)
    {
        string rutaAsset = $"{CarpetaFuentes}/{familia.asset}.asset";
        TMP_FontAsset existente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(rutaAsset);

        if (existente != null)
        {
            log.AppendLine($"  {familia.uso}: {familia.asset} (ya existia)");
            return existente;
        }

        Font origen = AssetDatabase.LoadAssetAtPath<Font>($"{CarpetaFuentes}/{familia.archivo}.ttf");

        if (origen == null)
        {
            log.AppendLine($"[error] No esta {CarpetaFuentes}/{familia.archivo}.ttf");
            return null;
        }

        TMP_FontAsset creada;

        try
        {
            creada = TMP_FontAsset.CreateFontAsset(origen, 90, 9, GlyphRenderMode.SDFAA,
                1024, 1024, AtlasPopulationMode.Dynamic, true);
        }
        catch (System.Exception error)
        {
            log.AppendLine($"[error] No se pudo generar {familia.asset}: {error.Message}");
            log.AppendLine($"        Generalo a mano con Window > TextMeshPro > Font Asset Creator");
            log.AppendLine($"        y guardalo como {rutaAsset}");
            return null;
        }

        if (creada == null)
        {
            log.AppendLine($"[error] TMP no genero {familia.asset}");
            return null;
        }

        creada.name = familia.asset;
        AssetDatabase.CreateAsset(creada, rutaAsset);

        if (creada.atlasTextures != null && creada.atlasTextures.Length > 0 && creada.atlasTextures[0] != null)
        {
            creada.atlasTextures[0].name = $"{familia.asset} Atlas";
            AssetDatabase.AddObjectToAsset(creada.atlasTextures[0], creada);
        }

        if (creada.material != null)
        {
            creada.material.name = $"{familia.asset} Material";
            AssetDatabase.AddObjectToAsset(creada.material, creada);
        }

        EditorUtility.SetDirty(creada);
        AssetDatabase.SaveAssets();

        log.AppendLine($"  {familia.uso}: {familia.asset} (generada desde {familia.archivo}.ttf)");
        return creada;
    }

    private static void AplicarEnEscenas(TMP_FontAsset titulos, TMP_FontAsset botones, TMP_FontAsset cuerpo, StringBuilder log)
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { CarpetaEscenas });

        foreach (string guid in guids)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);
            Scene escena = EditorSceneManager.OpenScene(ruta, OpenSceneMode.Single);

            int enTitulos = 0;
            int enBotones = 0;
            int enCuerpo = 0;

            foreach (GameObject raiz in escena.GetRootGameObjects())
            {
                foreach (TMP_Text texto in raiz.GetComponentsInChildren<TMP_Text>(true))
                {
                    TMP_FontAsset elegida;

                    if (EsDeBoton(texto))
                    {
                        elegida = botones;
                        enBotones++;
                    }
                    else if (EsTitulo(texto))
                    {
                        elegida = titulos;
                        enTitulos++;
                    }
                    else
                    {
                        elegida = cuerpo;
                        enCuerpo++;
                    }

                    if (texto.font != elegida)
                    {
                        texto.font = elegida;
                        EditorUtility.SetDirty(texto);
                    }
                }
            }

            log.AppendLine($"  {escena.name}: {enTitulos} titulos · {enBotones} botones · {enCuerpo} cuerpo");

            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena);
        }
    }

    private static bool EsDeBoton(TMP_Text texto)
    {
        return ConLetraDeBotones.Contains(texto.name)
            || texto.GetComponentInParent<Selectable>(true) != null;
    }

    private static bool EsTitulo(TMP_Text texto)
    {
        return texto.name.StartsWith("Titulo") || TitularesDePanel.Contains(texto.name);
    }
}
