using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public static class IconoAplicacion
{
    private const string RutaPortada = "Assets/UI/PortadaAPK.png";

    private const string NombreJuego = "Cazador de Regiones";

    private const string IdentificadorAndroid = "com.cazadorderegiones.juego";

    [MenuItem("Herramientas/Cazador de Regiones/10 · Icono y nombre de la app", false, 109)]
    public static void MenuIcono()
    {
        StringBuilder log = new StringBuilder();
        log.AppendLine("=== 10 · Icono y nombre de la app ===");

        Nombres(log);

        Texture2D portada = PrepararPortada(log);

        if (portada != null)
        {
            IconosDeAndroid(portada, log);
            IconosDeEscritorio(portada, log);
        }

        AssetDatabase.SaveAssets();

        Debug.Log(log.ToString());
    }

    private static void Nombres(StringBuilder log)
    {
        PlayerSettings.productName = NombreJuego;
        PlayerSettings.companyName = NombreJuego;

        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, IdentificadorAndroid);

        log.AppendLine($"  Nombre: {NombreJuego}");
        log.AppendLine($"  Paquete de Android: {IdentificadorAndroid}");
    }

    private static Texture2D PrepararPortada(StringBuilder log)
    {
        TextureImporter importador = AssetImporter.GetAtPath(RutaPortada) as TextureImporter;

        if (importador == null)
        {
            log.AppendLine($"[aviso] No se encontro {RutaPortada}; el icono queda sin cambiar");
            return null;
        }

        if (importador.textureType != TextureImporterType.Default || !importador.isReadable)
        {

            importador.textureType = TextureImporterType.Default;
            importador.isReadable = true;
            importador.SaveAndReimport();

            log.AppendLine("  PortadaAPK reimportada como textura legible");
        }

        Texture2D portada = AssetDatabase.LoadAssetAtPath<Texture2D>(RutaPortada);

        if (portada == null)
        {
            log.AppendLine($"[aviso] {RutaPortada} no se pudo cargar como Texture2D");
        }

        return portada;
    }

    private static void IconosDeAndroid(Texture2D portada, StringBuilder log)
    {

        foreach (PlatformIconKind tipo in PlayerSettings.GetSupportedIconKinds(NamedBuildTarget.Android))
        {
            PlatformIcon[] iconos = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, tipo);

            foreach (PlatformIcon icono in iconos)
            {
                icono.SetTextures(Capas(portada, icono.maxLayerCount));
            }

            PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, tipo, iconos);

            log.AppendLine($"  Android · {tipo}: {iconos.Length} tamanos con la portada");
        }
    }

    private static Texture2D[] Capas(Texture2D portada, int cuantas)
    {
        Texture2D[] capas = new Texture2D[Mathf.Max(1, cuantas)];

        for (int i = 0; i < capas.Length; i++)
        {
            capas[i] = portada;
        }

        return capas;
    }

    private static void IconosDeEscritorio(Texture2D portada, StringBuilder log)
    {
        int[] tamanos = PlayerSettings.GetIconSizes(NamedBuildTarget.Standalone, IconKind.Application);

        Texture2D[] iconos = new Texture2D[tamanos.Length];

        for (int i = 0; i < iconos.Length; i++)
        {
            iconos[i] = portada;
        }

        PlayerSettings.SetIcons(NamedBuildTarget.Standalone, iconos, IconKind.Application);

        log.AppendLine($"  Escritorio: {iconos.Length} tamanos con la portada");
    }
}
