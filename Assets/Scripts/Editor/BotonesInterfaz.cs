using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BotonesInterfaz
{
    private const string CarpetaMenu = "Assets/UI/PantallaMenu";
    private const string CarpetaJuego = "Assets/UI/PantallaJuego";
    private const string CarpetaPersonalizar = "Assets/UI/PantallaPersonalizar";
    private const string CarpetaCreditos = "Assets/UI/PantallaCreditos";
    private const string CarpetaPrendas = "Assets/UI/Prendas";
    private const string CarpetaEscenas = "Assets/Scenes";

    public const string CarpetaComun = "Assets/UI/PANTALLA-INSTRUCCIONES-ANDINA";

    public static string RutaFondoComun => $"{CarpetaComun}/Fondo.png";

    private const float MargenPantalla = 34f;

    private const int RellenoLados = 74;

    private const float AnchoInterior = 770f;

    private const float AnchoInteriorPersonalizar = 700f;

    private static readonly string[] CarpetasArte =
    {
        CarpetaMenu, CarpetaJuego, CarpetaPersonalizar, CarpetaCreditos, CarpetaPrendas,
        "Assets/UI/PANTALLA-INSTRUCCIONES-ANDINA",
        "Assets/UI/PANTALLA-INSTRUCCIONES-CARIBE",
        "Assets/UI/PANTALLA-INSTRUCCIONES-PACIFICO",
        "Assets/UI/PANTALLA-INSTRUCCIONES-ORINOQUIA",
        "Assets/UI/PANTALLA-INSTRUCCIONES-AMAZONIA",
    };

    private static readonly string[] EscenasNivel =
    {
        "Nivel1_Andina", "Nivel2_Caribe", "Nivel3_Pacifica", "Nivel4_Orinoquia", "Nivel5_Amazonia"
    };

    private const float AltoControl = 120f;

    private const float FuerzaSalto = 9.5f;

    private const float MargenBordeJugador = 0.02f;

    private const float PorcionFiguraJugador = 0.55f;

    private const float AltoTarjeta = 1120f;

    private const int RellenoTarjeta = 46;

    private const float LetraBoton = 44f;

    [MenuItem("Herramientas/Cazador de Regiones/7 · Aplicar botones nuevos", false, 106)]
    public static void MenuBotones()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        StringBuilder log = new StringBuilder();
        log.AppendLine("=== 7 · Botones nuevos ===");

        ReimportarArteDeInterfaz(log);

        MenuPrincipal(log);
        SeleccionNiveles(log);
        Personalizacion(log);
        Creditos(log);

        Debug.Log(log.ToString());
    }

    public static void ReimportarArteDeInterfaz(StringBuilder log)
    {
        log.AppendLine("--- Arte de interfaz como Sprite ---");

        int cambiados = 0;

        foreach (string carpeta in CarpetasArte)
        {
            cambiados += ConfigurarEscenarios.ReimportarCarpeta(carpeta, 2048, log);
        }

        AssetDatabase.Refresh();
        log.AppendLine($"Texturas reimportadas: {cambiados}");
    }

    private static Scene AbrirEscena(string nombre)
    {
        return EditorSceneManager.OpenScene($"{CarpetaEscenas}/{nombre}.unity", OpenSceneMode.Single);
    }

    private static void Guardar(Scene escena)
    {
        EditorSceneManager.MarkSceneDirty(escena);
        EditorSceneManager.SaveScene(escena);
    }

    public static RectTransform FondoYMarco(Scene escena, Transform lienzo, string rutaMarco)
    {
        RectTransform fondo = UtilesInterfaz.Adoptar(escena, lienzo, "Fondo");
        UtilesInterfaz.Estirar(fondo);
        UtilesInterfaz.PonerImagen(fondo, UtilesInterfaz.CargarSprite(RutaFondoComun), false, false);
        fondo.SetSiblingIndex(0);

        RectTransform area = UtilesInterfaz.AsegurarAreaSegura(escena, lienzo);

        RectTransform interior = UtilesInterfaz.Adoptar(escena, area, "Interior");
        UtilesInterfaz.Estirar(interior);
        interior.offsetMin = new Vector2(MargenPantalla, MargenPantalla);
        interior.offsetMax = new Vector2(-MargenPantalla, -MargenPantalla);
        interior.SetSiblingIndex(0);

        Sprite spriteMarco = UtilesInterfaz.CargarSprite(rutaMarco);
        RectTransform marco = UtilesInterfaz.Adoptar(escena, interior, "Marco");
        UtilesInterfaz.Estirar(marco);
        UtilesInterfaz.PonerImagen(marco, spriteMarco, false, false);
        UtilesInterfaz.AjustarProporcion(marco, spriteMarco);

        return marco;
    }

    private static RectTransform Boton(Scene escena, string nombre, string rutaSprite,
        float tamanoLetra, StringBuilder log, Color? tinte = null, string rotulo = null)
    {
        RectTransform rect = UtilesInterfaz.BuscarRect(escena, nombre);

        if (rect == null)
        {
            log.AppendLine($"[aviso] {escena.name}: no existe {nombre}");
            return null;
        }

        Sprite sprite = UtilesInterfaz.CargarSprite(rutaSprite);

        Image imagen = UtilesInterfaz.PonerImagen(rect, sprite, true);
        imagen.color = tinte ?? Color.white;

        Button boton = UtilesInterfaz.Componente<Button>(rect.gameObject);
        boton.targetGraphic = imagen;

        TMP_Text etiqueta = UtilesInterfaz.TextoDeBoton(boton);

        if (etiqueta != null)
        {
            UtilesInterfaz.Estirar(etiqueta.rectTransform);

            if (rotulo != null)
            {
                etiqueta.text = rotulo;
            }

            UtilesInterfaz.Formato(etiqueta, tamanoLetra, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
            etiqueta.gameObject.SetActive(true);
        }

        return rect;
    }

    private static RectTransform BotonIcono(Scene escena, string nombre, string rutaSprite, Vector2 ancla,
        Vector2 posicion, float alto, StringBuilder log, Transform padre = null, bool crear = false)
    {
        RectTransform rect = UtilesInterfaz.BuscarRect(escena, nombre);

        if (rect == null && crear && padre != null)
        {

            rect = UtilesInterfaz.Asegurar(padre, nombre);
            UtilesInterfaz.Componente<Button>(rect.gameObject);
        }

        if (rect == null)
        {
            log.AppendLine($"[aviso] {escena.name}: no existe {nombre}");
            return null;
        }

        if (padre != null && rect.parent != padre)
        {
            rect.SetParent(padre, false);
        }

        Sprite sprite = UtilesInterfaz.CargarSprite(rutaSprite);
        UtilesInterfaz.Colocar(rect, ancla, posicion, UtilesInterfaz.TamanoPorAlto(sprite, alto));
        UtilesInterfaz.PonerImagen(rect, sprite, true);

        TMP_Text etiqueta = UtilesInterfaz.TextoDeBoton(rect.GetComponent<Button>());

        if (etiqueta != null)
        {
            etiqueta.gameObject.SetActive(false);
        }

        return rect;
    }

    private static RectTransform Titulo(Scene escena, string nombre, float tamanoLetra, StringBuilder log)
    {
        RectTransform rect = UtilesInterfaz.BuscarRect(escena, nombre);

        if (rect == null)
        {
            log.AppendLine($"[aviso] {escena.name}: no existe {nombre}");
            return null;
        }

        UtilesInterfaz.Formato(rect.GetComponent<TMP_Text>(), tamanoLetra,
            TextAlignmentOptions.Center, UtilesInterfaz.Tinta);

        return rect;
    }

    private static RectTransform Armazon(Scene escena, Transform lienzo, string rutaMarco,
        float espaciado, int rellenoArriba, int rellenoAbajo, out RectTransform marco)
    {
        UtilesInterfaz.AsegurarLienzo(lienzo.gameObject);
        marco = FondoYMarco(escena, lienzo, rutaMarco);
        return UtilesInterfaz.Columna(escena, marco, "Columna", espaciado, rellenoArriba, rellenoAbajo, RellenoLados);
    }

    private static void MenuPrincipal(StringBuilder log)
    {
        Scene escena = AbrirEscena("MenuPrincipal");
        Transform lienzo = UtilesInterfaz.BuscarRaiz(escena, "Canvas").transform;

        RectTransform columna = Armazon(escena, lienzo, $"{CarpetaMenu}/Marco.png", 26f, 150, 130, out _);

        UtilesInterfaz.EnColumna(Titulo(escena, "TituloJuego", 84f, log), columna, 0, 210f, AnchoInterior, 0f);

        Sprite linea = UtilesInterfaz.CargarSprite($"{CarpetaMenu}/LineaTitulo.png");
        RectTransform rectLinea = UtilesInterfaz.Adoptar(escena, columna, "LineaTitulo");
        UtilesInterfaz.PonerImagen(rectLinea, linea);
        UtilesInterfaz.EnColumna(rectLinea, columna, 1, 50f, 600f, 0f);

        UtilesInterfaz.Espaciador(escena, columna, "Aire", 2, 1f);

        UtilesInterfaz.EnColumna(Boton(escena, "BotonJugar", $"{CarpetaMenu}/BotonJugar.png", 60f, log),
            columna, 3, 190f, AnchoInterior, 0f, 140f);
        UtilesInterfaz.EnColumna(Boton(escena, "BotonPersonalizar", $"{CarpetaMenu}/BotonPersonalizar.png", 54f, log),
            columna, 4, 190f, AnchoInterior, 0f, 140f);
        UtilesInterfaz.EnColumna(Boton(escena, "BotonCreditos", $"{CarpetaMenu}/BotonCreditos.png", 60f, log),
            columna, 5, 190f, AnchoInterior, 0f, 140f);

        UtilesInterfaz.EnColumna(
            Boton(escena, "BotonSalir", $"{CarpetaMenu}/BotonCreditos.png", 60f, log,
                new Color(0.82f, 0.82f, 0.82f)),
            columna, 6, 190f, AnchoInterior, 0f, 140f);

        UtilesInterfaz.Espaciador(escena, columna, "AireAbajo", 7, 1.1f);

        log.AppendLine("  MenuPrincipal: columna elastica con titulo y 4 botones");
        Guardar(escena);
    }

    private static readonly (string boton, string sprite, int fila)[] Regiones =
    {
        ("BotonAndina",    "ElegirRegionAndina",    0),
        ("BotonCaribe",    "ElegirRegionCaribe",    0),
        ("BotonPacifica",  "ElegirRegionPacifica",  1),
        ("BotonOrinoquia", "ElegirRegionOrinoquia", 1),
        ("BotonAmazonia",  "ElegirRegionAmazonia",  2),
    };

    private static void SeleccionNiveles(StringBuilder log)
    {
        Scene escena = AbrirEscena("SeleccionNiveles");
        Transform lienzo = UtilesInterfaz.BuscarRaiz(escena, "Canvas").transform;

        RectTransform columna = Armazon(escena, lienzo, $"{CarpetaMenu}/Marco.png", 24f, 190, 110, out RectTransform marco);

        UtilesInterfaz.EnColumna(Titulo(escena, "Titulo", 76f, log), columna, 0, 170f, AnchoInterior, 0f);
        UtilesInterfaz.Espaciador(escena, columna, "Aire", 1, 0.5f);

        Vector2 celda = new Vector2(370f, 380f);

        RectTransform[] filas = new RectTransform[3];

        for (int f = 0; f < filas.Length; f++)
        {
            filas[f] = UtilesInterfaz.FilaHorizontal(escena, columna, $"FilaRegiones{f + 1}", 30f);
            UtilesInterfaz.EnColumna(filas[f], columna, 2 + f, celda.y, AnchoInterior, 0f, celda.y);
        }

        foreach ((string boton, string sprite, int fila) in Regiones)
        {
            RectTransform rect = UtilesInterfaz.BuscarRect(escena, boton);

            if (rect == null)
            {
                log.AppendLine($"[aviso] SeleccionNiveles: no existe {boton}");
                continue;
            }

            rect.SetParent(filas[fila], false);
            rect.sizeDelta = celda;
            rect.localScale = Vector3.one;

            Button control = UtilesInterfaz.Componente<Button>(rect.gameObject);
            control.targetGraphic = UtilesInterfaz.ZonaClicable(rect);

            Sprite mapa = UtilesInterfaz.CargarSprite($"{CarpetaMenu}/{sprite}.png");
            RectTransform dibujo = UtilesInterfaz.Asegurar(rect, "Mapa");
            UtilesInterfaz.Colocar(dibujo, new Vector2(0.5f, 1f), new Vector2(0f, -145f), new Vector2(260f, 260f));
            UtilesInterfaz.PonerImagen(dibujo, mapa);

            TMP_Text etiqueta = UtilesInterfaz.TextoDeBoton(control);

            if (etiqueta != null)
            {
                UtilesInterfaz.Colocar(etiqueta.rectTransform, new Vector2(0.5f, 0f),
                    new Vector2(0f, 52f), new Vector2(celda.x - 20f, 76f));
                UtilesInterfaz.Formato(etiqueta, 40f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
                etiqueta.gameObject.SetActive(true);
            }
        }

        UtilesInterfaz.Borrar(escena, "Regiones");

        UtilesInterfaz.Espaciador(escena, columna, "AireAbajo", 5, 0.8f);

        BotonIcono(escena, "BotonVolver", $"{CarpetaPersonalizar}/FlechaRegresar.png",
            new Vector2(0f, 1f), new Vector2(105f, -105f), 125f, log, marco);

        log.AppendLine("  SeleccionNiveles: 2+2+1 dentro del marco + flecha de regreso");
        Guardar(escena);
    }

    private static readonly (string boton, string prenda, string rotulo)[] Accesorios =
    {
        ("BotonPredeterminado",   null,                          "Predeterminado"),
        ("BotonPoncho",           "PrendaAndina",                "Ruana"),
        ("BotonSombreroVueltiao", "PrendaCaribe",                "Sombrero vueltiao"),
        ("BotonVestidoDanza",     "PrendaPacifico_CondorFrente", "Vestido de danza"),
        ("BotonSombreroLlanero",  "PrendaOrinoquia",             "Sombrero llanero"),
        ("BotonPlumas",           "PrendaAmazonia",              "Adorno de plumas"),
    };

    private static void Personalizacion(StringBuilder log)
    {
        Scene escena = AbrirEscena("Personalizacion");
        Transform lienzo = UtilesInterfaz.BuscarRaiz(escena, "Canvas").transform;

        RectTransform columna = Armazon(escena, lienzo, $"{CarpetaPersonalizar}/Marco.png", 20f, 170, 70, out RectTransform marco);

        UtilesInterfaz.EnColumna(Titulo(escena, "TituloPersonalizar", 72f, log), columna, 0, 130f, AnchoInteriorPersonalizar, 0f);

        UtilesInterfaz.EnColumna(UtilesInterfaz.BuscarRect(escena, "VistaPreviaCondor"),
            columna, 1, 300f, 380f, 1f, 200f);

        RectTransform rectNombre = UtilesInterfaz.Adoptar(escena, columna, "TituloNombre");
        UtilesInterfaz.PonerTexto(rectNombre, "Nombre del Cazador", 42f,
            TextAlignmentOptions.Left, UtilesInterfaz.Tinta);
        UtilesInterfaz.EnColumna(rectNombre, columna, 2, 56f, AnchoInteriorPersonalizar, 0f);

        UtilesInterfaz.EnColumna(UtilesInterfaz.BuscarRect(escena, "CampoNombre"),
            columna, 3, 110f, AnchoInteriorPersonalizar, 0f, 100f);

        RectTransform tituloAccesorios = UtilesInterfaz.BuscarRect(escena, "TituloAccesorios");

        if (tituloAccesorios != null)
        {
            UtilesInterfaz.Formato(tituloAccesorios.GetComponent<TMP_Text>(), 46f,
                TextAlignmentOptions.Left, UtilesInterfaz.Tinta);
            UtilesInterfaz.EnColumna(tituloAccesorios, columna, 4, 56f, AnchoInteriorPersonalizar, 0f);
        }

        Vector2 celda = new Vector2(336f, 175f);
        RectTransform rejilla = UtilesInterfaz.Rejilla(escena, columna, "Prendas", celda, new Vector2(28f, 22f), 2, 3);
        float altoRejilla = rejilla.sizeDelta.y;

        UtilesInterfaz.EnColumna(rejilla, columna, 5, altoRejilla, rejilla.sizeDelta.x, 0f, altoRejilla);

        for (int i = 0; i < Accesorios.Length; i++)
        {
            (string boton, string prenda, string rotulo) = Accesorios[i];
            RectTransform rect = UtilesInterfaz.BuscarRect(escena, boton);

            if (rect == null)
            {
                log.AppendLine($"[aviso] Personalizacion: no existe {boton}");
                continue;
            }

            rect.SetParent(rejilla, false);
            rect.SetSiblingIndex(i);

            Image fondoBoton = UtilesInterfaz.Componente<Image>(rect.gameObject);
            fondoBoton.type = Image.Type.Sliced;
            fondoBoton.preserveAspect = false;
            fondoBoton.raycastTarget = true;

            string rutaIcono = prenda != null
                ? $"{CarpetaPrendas}/{prenda}.png"
                : AccesoriosCondor.VistaPreviaDe("Predeterminado");

            UtilesInterfaz.Icono(rect, "Icono", UtilesInterfaz.CargarSprite(rutaIcono), new Vector2(0f, 33f), 98f);

            TMP_Text etiqueta = UtilesInterfaz.TextoDeBoton(rect.GetComponent<Button>());

            if (etiqueta != null)
            {
                UtilesInterfaz.Colocar(etiqueta.rectTransform, new Vector2(0.5f, 0f),
                    new Vector2(0f, 36f), new Vector2(celda.x - 24f, 56f));
                etiqueta.text = rotulo;
                UtilesInterfaz.Formato(etiqueta, 34f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
                etiqueta.gameObject.SetActive(true);
            }
        }

        UtilesInterfaz.Espaciador(escena, columna, "Aire", 6, 0.5f);

        UtilesInterfaz.EnColumna(
            Boton(escena, "BotonGuardar", $"{CarpetaPersonalizar}/BotonGuardar.png", 54f, log),
            columna, 7, 145f, 600f, 0f, 120f);

        BotonIcono(escena, "BotonVolver", $"{CarpetaPersonalizar}/FlechaRegresar.png",
            new Vector2(0f, 1f), new Vector2(105f, -105f), 125f, log, marco);

        log.AppendLine("  Personalizacion: columna con rejilla 2x3 de prendas");
        Guardar(escena);
    }

    private class Integrante
    {
        public string rol;
        public string nombre;
        public string descripcion;
        public string icono;
    }

    private static readonly Integrante[] Integrantes =
    {
        new Integrante
        {
            rol = "Programador", nombre = "José Emanuel Pinto", icono = "IconoProgramador",
            descripcion = "Construcción de la lógica funcional del videojuego."
        },
        new Integrante
        {
            rol = "Diseñador UX/UI", nombre = "Manuel Velasco", icono = "IconoUX-UI",
            descripcion = "Diseño de pantallas, HUD, controles táctiles y experiencia del jugador."
        },
        new Integrante
        {
            rol = "Ilustradora", nombre = "Laura Delgado", icono = "IconoIlustradora",
            descripcion = "Creación de personajes, animaciones, objetos, fondos y efectos visuales."
        },
        new Integrante
        {
            rol = "Documentador y coordinador", nombre = "Lowell Ortiz", icono = "IconoDocumentador",
            descripcion = "Organización del proyecto, documentos, pruebas, evidencias y presentación."
        },
        new Integrante
        {
            rol = "Musica y Sonidos", nombre = "YouTube Music", icono = "IconoMusica",
            descripcion = "Toda la música y los efectos de sonido se obtuvieron de esta fuente."
        },
    };

    private static readonly Color TintaCreditos = new Color(0.33f, 0.17f, 0.05f);

    private const float AnchoCreditos = 700f;

    private const float AltoIntegrante = 222f;

    private const float AlturaEncabezado = 250f;

    private static void Creditos(StringBuilder log)
    {
        Scene escena = AbrirEscena("Creditos");
        GameObject objetoLienzo = UtilesInterfaz.BuscarRaiz(escena, "Canvas");
        Transform lienzo = objetoLienzo.transform;

        UtilesInterfaz.AsegurarLienzo(objetoLienzo);

        LimpiarCreditos(escena);

        RectTransform fondo = UtilesInterfaz.Adoptar(escena, lienzo, "Fondo");
        UtilesInterfaz.Estirar(fondo);
        UtilesInterfaz.PonerImagen(fondo, UtilesInterfaz.CargarSprite(RutaFondoComun), false, false);
        fondo.SetSiblingIndex(0);

        RectTransform area = UtilesInterfaz.AsegurarAreaSegura(escena, lienzo);

        RectTransform interior = UtilesInterfaz.Adoptar(escena, area, "Interior");
        UtilesInterfaz.Estirar(interior);
        interior.offsetMin = new Vector2(MargenPantalla, MargenPantalla);
        interior.offsetMax = new Vector2(-MargenPantalla, -MargenPantalla);
        interior.SetSiblingIndex(0);

        RectTransform titulo = UtilesInterfaz.Asegurar(interior, "TituloCreditos");
        UtilesInterfaz.Colocar(titulo, new Vector2(0.5f, 1f), new Vector2(0f, -165f), new Vector2(860f, 150f));
        UtilesInterfaz.PonerTexto(titulo, "CREDITOS", 96f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);

        RectTransform zona = UtilesInterfaz.Adoptar(escena, interior, "ZonaMarco");
        UtilesInterfaz.Estirar(zona);
        zona.offsetMin = Vector2.zero;
        zona.offsetMax = new Vector2(0f, -AlturaEncabezado);

        Sprite spriteMarco = UtilesInterfaz.CargarSprite($"{CarpetaCreditos}/MarcoCreditos.png");
        RectTransform marco = UtilesInterfaz.Adoptar(escena, zona, "Marco");
        UtilesInterfaz.Estirar(marco);
        UtilesInterfaz.PonerImagen(marco, spriteMarco, false, false);
        UtilesInterfaz.AjustarProporcion(marco, spriteMarco);

        RectTransform columna = UtilesInterfaz.Columna(escena, marco, "Columna", 14f, 130, 40, 70);

        for (int i = 0; i < Integrantes.Length; i++)
        {
            RectTransform entrada = UtilesInterfaz.Adoptar(escena, columna, $"Integrante{i + 1}");
            float alto = EntradaCreditos(entrada, Integrantes[i], log);

            UtilesInterfaz.EnColumna(entrada, columna, i, alto, AnchoCreditos, 0f, alto);
        }

        Sprite condor = UtilesInterfaz.CargarSprite($"{CarpetaCreditos}/CondorCreditos.png");
        RectTransform rectCondor = UtilesInterfaz.Adoptar(escena, columna, "CondorCreditos");
        UtilesInterfaz.PonerImagen(rectCondor, condor);

        UtilesInterfaz.EnColumna(rectCondor, columna, Integrantes.Length, 300f, AnchoCreditos, 1f, 180f);

        BotonIcono(escena, "BotonVolver", $"{CarpetaCreditos}/FlechaRegresar.png",
            new Vector2(0f, 1f), new Vector2(78f, -68f), 108f, log, interior);

        log.AppendLine($"  Creditos: titulo fuera del pergamino, {Integrantes.Length} entradas y el condor abajo");
        Guardar(escena);
    }

    private static void LimpiarCreditos(Scene escena)
    {

        UtilesInterfaz.Borrar(escena, "Columna");
        UtilesInterfaz.Borrar(escena, "TituloCreditos");
        UtilesInterfaz.Borrar(escena, "ImagenCreditos");
        UtilesInterfaz.Borrar(escena, "Lista");

        for (int i = 1; i <= 12; i++)
        {
            UtilesInterfaz.Borrar(escena, $"Integrante{i}");
        }
    }

    private static float EntradaCreditos(RectTransform entrada, Integrante datos, StringBuilder log)
    {
        bool conNombre = !string.IsNullOrEmpty(datos.nombre);

        Encabezado(entrada, datos, log);

        RectTransform rectNombre = UtilesInterfaz.Asegurar(entrada, "NombreIntegrante");
        UtilesInterfaz.Colocar(rectNombre, new Vector2(0.5f, 1f), new Vector2(0f, -104f),
            new Vector2(AnchoCreditos, 64f));

        TMP_Text nombre = UtilesInterfaz.PonerTexto(rectNombre, datos.nombre, 54f,
            TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        nombre.fontStyle = FontStyles.Bold;
        nombre.fontSizeMin = 42f;
        rectNombre.gameObject.SetActive(conNombre);

        float yDescripcion = conNombre ? -174f : -112f;

        RectTransform rectDescripcion = UtilesInterfaz.Asegurar(entrada, "Descripcion");
        UtilesInterfaz.Colocar(rectDescripcion, new Vector2(0.5f, 1f), new Vector2(0f, yDescripcion),
            new Vector2(660f, 90f));

        TMP_Text descripcion = UtilesInterfaz.PonerTexto(rectDescripcion, datos.descripcion, 34f,
            TextAlignmentOptions.Top, TintaCreditos);

        descripcion.enableWordWrapping = true;
        descripcion.fontSizeMin = 30f;
        descripcion.fontStyle = FontStyles.Bold;

        return conNombre ? AltoIntegrante : AltoIntegrante - 62f;
    }

    private static void Encabezado(RectTransform entrada, Integrante datos, StringBuilder log)
    {

        RectTransform fila = UtilesInterfaz.Asegurar(entrada, "Encabezado");
        UtilesInterfaz.Colocar(fila, new Vector2(0.5f, 1f), new Vector2(0f, -36f),
            new Vector2(AnchoCreditos, 68f));

        HorizontalLayoutGroup grupo = UtilesInterfaz.Componente<HorizontalLayoutGroup>(fila.gameObject);
        grupo.spacing = 16f;
        grupo.padding = new RectOffset(0, 0, 0, 0);
        grupo.childAlignment = TextAnchor.MiddleCenter;
        grupo.childControlWidth = true;
        grupo.childControlHeight = true;
        grupo.childForceExpandWidth = false;
        grupo.childForceExpandHeight = false;
        grupo.childScaleWidth = false;
        grupo.childScaleHeight = false;

        Sprite sprite = UtilesInterfaz.CargarSprite($"{CarpetaCreditos}/{datos.icono}.png");

        if (sprite == null)
        {
            log.AppendLine($"[aviso] Creditos: falta {CarpetaCreditos}/{datos.icono}.png");
        }

        RectTransform rectIcono = UtilesInterfaz.Asegurar(fila, "Icono");
        rectIcono.SetSiblingIndex(0);
        UtilesInterfaz.PonerImagen(rectIcono, sprite);

        LayoutElement medidaIcono = UtilesInterfaz.Componente<LayoutElement>(rectIcono.gameObject);
        medidaIcono.preferredWidth = 62f;
        medidaIcono.preferredHeight = 62f;
        medidaIcono.flexibleWidth = 0f;
        medidaIcono.flexibleHeight = 0f;

        RectTransform rectRol = UtilesInterfaz.Asegurar(fila, "RolIntegrante");
        rectRol.SetSiblingIndex(1);

        TMP_Text rol = UtilesInterfaz.PonerTexto(rectRol, datos.rol, 44f,
            TextAlignmentOptions.Left, UtilesInterfaz.Tinta);

        rol.enableAutoSizing = false;
        rol.fontStyle = FontStyles.Bold;

        ContentSizeFitter ajuste = UtilesInterfaz.Componente<ContentSizeFitter>(rectRol.gameObject);
        ajuste.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        ajuste.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        LayoutElement medidaRol = UtilesInterfaz.Componente<LayoutElement>(rectRol.gameObject);
        medidaRol.preferredHeight = 64f;
        medidaRol.flexibleWidth = 0f;
        medidaRol.flexibleHeight = 0f;
    }

    [MenuItem("Herramientas/Cazador de Regiones/8 · Rehacer HUD de niveles", false, 107)]
    public static void MenuHUD()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        StringBuilder log = new StringBuilder();
        log.AppendLine("=== 8 · HUD de niveles ===");

        ReimportarArteDeInterfaz(log);

        foreach (string nombre in EscenasNivel)
        {
            if (!File.Exists($"{CarpetaEscenas}/{nombre}.unity"))
            {
                log.AppendLine($"[aviso] No existe la escena {nombre}");
                continue;
            }

            Scene escena = AbrirEscena(nombre);

            AjustarJugador(escena, log);
            RehacerHUD(escena, log);
            RehacerPaneles(escena, log);

            Guardar(escena);
        }

        Debug.Log(log.ToString());
    }

    private static void AjustarJugador(Scene escena, StringBuilder log)
    {
        PlayerController jugador = Object.FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);

        if (jugador == null)
        {
            log.AppendLine($"[aviso] {escena.name}: sin PlayerController");
            return;
        }

        SerializedObject serializado = new SerializedObject(jugador);
        serializado.FindProperty("fuerzaSalto").floatValue = FuerzaSalto;
        serializado.FindProperty("multiplicadorCaida").floatValue = 2.5f;
        serializado.FindProperty("multiplicadorSaltoCorto").floatValue = 2f;

        serializado.FindProperty("margenBorde").floatValue = MargenBordeJugador;
        serializado.FindProperty("porcionFigura").floatValue = PorcionFiguraJugador;
        serializado.ApplyModifiedProperties();

        log.AppendLine($"  {escena.name}: salto {FuerzaSalto} · caida x2.5 · alcance hasta el borde");
    }

    private static int VidasDeLaEscena(Scene escena, StringBuilder log)
    {
        GameManager gestor = Object.FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);

        if (gestor == null)
        {
            log.AppendLine($"[aviso] {escena.name}: sin GameManager, se asumen 3 corazones");
            return 3;
        }

        SerializedObject serializado = new SerializedObject(gestor);
        return Mathf.Clamp(serializado.FindProperty("vidas").intValue, 1, 6);
    }

    private static void BorrarSobrantes(Transform padre, string prefijo, int cuantos)
    {

        for (int i = padre.childCount - 1; i >= 0; i--)
        {
            Transform hijo = padre.GetChild(i);

            if (!hijo.name.StartsWith(prefijo))
            {
                continue;
            }

            if (int.TryParse(hijo.name.Substring(prefijo.Length), out int numero) && numero > cuantos)
            {
                Object.DestroyImmediate(hijo.gameObject);
            }
        }
    }

    private static void RehacerHUD(Scene escena, StringBuilder log)
    {
        GameObject objetoLienzo = UtilesInterfaz.BuscarRaiz(escena, "Canvas");

        if (objetoLienzo == null)
        {
            log.AppendLine($"[aviso] {escena.name}: sin Canvas");
            return;
        }

        UtilesInterfaz.AsegurarLienzo(objetoLienzo);
        RectTransform area = UtilesInterfaz.AsegurarAreaSegura(escena, objetoLienzo.transform);

        Sprite lleno = UtilesInterfaz.CargarSprite($"{CarpetaJuego}/CorazonRojo.png");
        Sprite vacio = UtilesInterfaz.CargarSprite($"{CarpetaJuego}/CorazonBlanco.png");

        int vidas = VidasDeLaEscena(escena, log);

        float paso = vidas <= 3 ? 110f : 96f;
        float ladoCorazon = vidas <= 3 ? 88f : 82f;

        RectTransform contenedorCorazones = UtilesInterfaz.Asegurar(area, "Corazones");
        UtilesInterfaz.Colocar(contenedorCorazones, new Vector2(0f, 1f), new Vector2(250f, -130f),
            new Vector2(vidas * paso, 100f));

        Image[] corazones = new Image[vidas];

        for (int i = 0; i < corazones.Length; i++)
        {
            RectTransform rect = UtilesInterfaz.Asegurar(contenedorCorazones, $"Corazon{i + 1}");
            UtilesInterfaz.Colocar(rect, new Vector2(0.5f, 0.5f),
                new Vector2((i - (vidas - 1) * 0.5f) * paso, 0f), new Vector2(ladoCorazon, ladoCorazon));
            corazones[i] = UtilesInterfaz.PonerImagen(rect, lleno);
        }

        BorrarSobrantes(contenedorCorazones, "Corazon", vidas);

        Sprite marcoTiempo = UtilesInterfaz.CargarSprite($"{CarpetaJuego}/MarcoTiempo.png");
        RectTransform contenedorTiempo = UtilesInterfaz.Asegurar(area, "MarcoTiempo");
        UtilesInterfaz.Colocar(contenedorTiempo, new Vector2(0.5f, 1f), new Vector2(120f, -130f),
            UtilesInterfaz.TamanoPorAlto(marcoTiempo, 130f));
        UtilesInterfaz.PonerImagen(contenedorTiempo, marcoTiempo);

        RectTransform rectTiempo = UtilesInterfaz.BuscarRect(escena, "TextoTiempo");
        TMP_Text textoTiempo = null;

        if (rectTiempo != null)
        {
            rectTiempo.SetParent(contenedorTiempo, false);
            UtilesInterfaz.Estirar(rectTiempo);
            textoTiempo = UtilesInterfaz.PonerTexto(rectTiempo, "00:00", 56f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        }

        BotonIcono(escena, "BotonPausa", $"{CarpetaJuego}/BotonPausa.png",
            new Vector2(1f, 1f), new Vector2(-140f, -130f), 110f, log, area);

        TMP_Text textoNombre = UtilesInterfaz.Etiqueta(area, "TextoNombre", "Cazador",
            new Vector2(0.5f, 1f), new Vector2(0f, -270f), new Vector2(900f, 90f),
            56f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);

        RectTransform rectPuntos = UtilesInterfaz.BuscarRect(escena, "TextoPuntos");
        TMP_Text textoPuntos = null;

        if (rectPuntos != null)
        {
            UtilesInterfaz.Colocar(rectPuntos, new Vector2(0.5f, 1f), new Vector2(0f, -370f), new Vector2(900f, 90f));
            textoPuntos = UtilesInterfaz.PonerTexto(rectPuntos, "Puntos: 0", 60f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        }

        GameObject viejoVidas = UtilesInterfaz.Buscar(escena, "TextoVidas");

        if (viejoVidas != null)
        {
            Object.DestroyImmediate(viejoVidas);
        }

        RectTransform controles = UtilesInterfaz.BuscarRect(escena, "ControlesInferiores");

        if (controles != null)
        {
            controles.SetParent(area, false);
            UtilesInterfaz.Estirar(controles);
        }

        PlayerController jugador = Object.FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);

        RectTransform joystick = Joystick(escena, area, jugador, log);

        RectTransform izquierda = BotonIcono(escena, "BotonIzquierda", $"{CarpetaJuego}/Izquierda.png",
            new Vector2(0f, 0f), new Vector2(165f, 210f), AltoControl, log, area, true);
        RectTransform derecha = BotonIcono(escena, "BotonDerecha", $"{CarpetaJuego}/Derecha.png",
            new Vector2(0f, 0f), new Vector2(445f, 210f), AltoControl, log, area, true);

        Sostener(escena, "BotonIzquierda", BotonMovimiento.Direccion.Izquierda, jugador, log);
        Sostener(escena, "BotonDerecha", BotonMovimiento.Direccion.Derecha, jugador, log);

        ConectarSelector(escena, objetoLienzo, joystick, izquierda, derecha, log);

        BotonIcono(escena, "BotonSalto", $"{CarpetaJuego}/Arriba.png",
            new Vector2(1f, 0f), new Vector2(-165f, 210f), AltoControl, log, area);
        Sostener(escena, "BotonSalto", BotonMovimiento.Direccion.Salto, jugador, log);

        TMP_Text textoPowerUp = Medidor(escena, area, log);

        AjustarHojas(escena, log);

        ConectarHUD(textoPuntos, textoTiempo, textoPowerUp, textoNombre, corazones, lleno, vacio, escena, log);
    }

    private static void AjustarHojas(Scene escena, StringBuilder log)
    {

        RectTransform hojas = UtilesInterfaz.BuscarRect(escena, "PanelHojas");

        if (hojas == null)
        {
            return;
        }

        Image imagen = hojas.GetComponent<Image>();

        if (imagen != null)
        {

            imagen.raycastTarget = false;
        }

        hojas.SetAsFirstSibling();

        log.AppendLine($"  {escena.name}: las hojas ya no tapan los toques ni los paneles");
    }

    private static RectTransform Joystick(Scene escena, RectTransform area, PlayerController jugador, StringBuilder log)
    {
        Sprite circulo = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        RectTransform baseJoystick = UtilesInterfaz.Adoptar(escena, area, "Joystick");
        UtilesInterfaz.Colocar(baseJoystick, new Vector2(0f, 0f), new Vector2(250f, 260f), new Vector2(300f, 300f));

        Image fondo = UtilesInterfaz.PonerImagen(baseJoystick, circulo, true);
        fondo.color = new Color(1f, 1f, 1f, 0.32f);

        fondo.enabled = true;

        RectTransform mango = UtilesInterfaz.Asegurar(baseJoystick, "Mango");
        UtilesInterfaz.Colocar(mango, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(130f, 130f));

        Image imagenMango = UtilesInterfaz.PonerImagen(mango, circulo);
        imagenMango.color = new Color(1f, 0.97f, 0.88f, 0.95f);
        imagenMango.enabled = true;

        JoystickVirtual control = UtilesInterfaz.Componente<JoystickVirtual>(baseJoystick.gameObject);

        SerializedObject serializado = new SerializedObject(control);
        serializado.FindProperty("jugador").objectReferenceValue = jugador;
        serializado.FindProperty("mango").objectReferenceValue = mango;
        serializado.FindProperty("radio").floatValue = 110f;
        serializado.ApplyModifiedProperties();

        if (jugador == null)
        {
            log.AppendLine($"[aviso] {escena.name}: el joystick quedo sin referencia al jugador");
        }

        return baseJoystick;
    }

    private static void ConectarSelector(Scene escena, GameObject objetoLienzo, RectTransform joystick,
        RectTransform izquierda, RectTransform derecha, StringBuilder log)
    {

        SelectorControles selector = UtilesInterfaz.Componente<SelectorControles>(objetoLienzo);

        SerializedObject serializado = new SerializedObject(selector);
        serializado.FindProperty("joystick").objectReferenceValue = joystick != null ? joystick.gameObject : null;
        serializado.FindProperty("botonIzquierda").objectReferenceValue = izquierda != null ? izquierda.gameObject : null;
        serializado.FindProperty("botonDerecha").objectReferenceValue = derecha != null ? derecha.gameObject : null;
        serializado.ApplyModifiedProperties();

        if (joystick != null)
        {
            joystick.gameObject.SetActive(true);
        }

        if (izquierda != null)
        {
            izquierda.gameObject.SetActive(true);
        }

        if (derecha != null)
        {
            derecha.gameObject.SetActive(true);
        }

        log.AppendLine($"  {escena.name}: joystick y flechas; el jugador elige en Instrucciones");
    }

    private static TMP_Text Medidor(Scene escena, RectTransform area, StringBuilder log)
    {
        RectTransform boton = UtilesInterfaz.BuscarRect(escena, "BotonPowerUp");

        if (boton == null)
        {
            log.AppendLine($"[aviso] {escena.name}: no existe BotonPowerUp");
            return null;
        }

        if (boton.parent != area)
        {
            boton.SetParent(area, false);
        }

        Sprite energia = UtilesInterfaz.CargarSprite($"{CarpetaJuego}/Energia.png");
        Vector2 tamano = UtilesInterfaz.TamanoPorAlto(energia, AltoControl);

        UtilesInterfaz.Colocar(boton, new Vector2(1f, 0f), new Vector2(-390f, 210f), tamano);

        Button control = UtilesInterfaz.Componente<Button>(boton.gameObject);
        control.targetGraphic = UtilesInterfaz.ZonaClicable(boton);

        GameManager gestor = Object.FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);

        if (gestor != null)
        {
            UtilesInterfaz.Reconectar(control, gestor, "ActivarPowerUp");
        }
        else
        {
            log.AppendLine($"[aviso] {escena.name}: BotonPowerUp quedo sin GameManager");
        }

        RectTransform rectMarco = UtilesInterfaz.Asegurar(boton, "Marco");
        UtilesInterfaz.Colocar(rectMarco, new Vector2(0.5f, 0.5f), Vector2.zero,
            new Vector2(AltoControl * 1.28f, AltoControl * 1.28f));

        Image placa = UtilesInterfaz.PonerImagen(rectMarco,
            AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"));
        placa.color = new Color(0.08f, 0.08f, 0.07f, 0.55f);
        placa.enabled = true;
        rectMarco.SetSiblingIndex(0);

        RectTransform rectHalo = UtilesInterfaz.Asegurar(boton, "Halo");
        UtilesInterfaz.Colocar(rectHalo, new Vector2(0.5f, 0.5f), Vector2.zero,
            UtilesInterfaz.TamanoPorAlto(energia, AltoControl * 1.55f));
        Image halo = UtilesInterfaz.PonerImagen(rectHalo, energia);
        halo.color = new Color(1f, 0.86f, 0.35f, 0f);
        rectHalo.SetSiblingIndex(1);

        RectTransform rectBase = UtilesInterfaz.Asegurar(boton, "Base");
        UtilesInterfaz.Colocar(rectBase, new Vector2(0.5f, 0.5f), Vector2.zero, tamano);
        Image vacia = UtilesInterfaz.PonerImagen(rectBase, energia);
        vacia.color = new Color(0.72f, 0.72f, 0.68f, 0.8f);
        rectBase.SetSiblingIndex(2);

        RectTransform rectRelleno = UtilesInterfaz.Asegurar(boton, "Relleno");
        UtilesInterfaz.Colocar(rectRelleno, new Vector2(0.5f, 0.5f), Vector2.zero, tamano);
        Image relleno = UtilesInterfaz.PonerImagen(rectRelleno, energia);

        relleno.type = Image.Type.Filled;
        relleno.fillMethod = Image.FillMethod.Vertical;
        relleno.fillOrigin = (int)Image.OriginVertical.Bottom;
        relleno.fillAmount = 0f;
        rectRelleno.SetSiblingIndex(3);

        RectTransform rectTexto = UtilesInterfaz.BuscarRect(escena, "TextoPowerUp");
        TMP_Text texto = null;

        if (rectTexto != null)
        {
            rectTexto.SetParent(boton, false);
            UtilesInterfaz.Colocar(rectTexto, new Vector2(0.5f, 1f), new Vector2(0f, 46f), new Vector2(240f, 60f));
            texto = UtilesInterfaz.PonerTexto(rectTexto, "0/3", 40f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
            rectTexto.SetSiblingIndex(4);
        }

        MedidorPowerUp medidor = UtilesInterfaz.Componente<MedidorPowerUp>(boton.gameObject);

        SerializedObject serializado = new SerializedObject(medidor);
        serializado.FindProperty("relleno").objectReferenceValue = relleno;
        serializado.FindProperty("halo").objectReferenceValue = halo;
        serializado.ApplyModifiedProperties();

        log.AppendLine($"  {escena.name}: medidor de power up a la izquierda del salto");
        return texto;
    }

    private static void Sostener(Scene escena, string nombre, BotonMovimiento.Direccion direccion,
        PlayerController jugador, StringBuilder log)
    {
        RectTransform rect = UtilesInterfaz.BuscarRect(escena, nombre);

        if (rect == null)
        {
            return;
        }

        Button boton = rect.GetComponent<Button>();

        if (boton != null)
        {
            UtilesInterfaz.LimpiarOnClick(boton);
        }

        BotonMovimiento movimiento = UtilesInterfaz.Componente<BotonMovimiento>(rect.gameObject);

        SerializedObject serializado = new SerializedObject(movimiento);
        serializado.FindProperty("direccion").enumValueIndex = (int)direccion;
        serializado.FindProperty("jugador").objectReferenceValue = jugador;
        serializado.ApplyModifiedProperties();

        if (jugador == null)
        {
            log.AppendLine($"[aviso] {escena.name}: {nombre} quedo sin referencia al jugador");
        }
    }

    private static void ConectarHUD(TMP_Text puntos, TMP_Text tiempo, TMP_Text powerUp, TMP_Text nombre,
        Image[] corazones, Sprite lleno, Sprite vacio, Scene escena, StringBuilder log)
    {
        HUDController hud = Object.FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);

        if (hud == null)
        {
            GameObject lienzo = UtilesInterfaz.BuscarRaiz(escena, "Canvas");

            if (lienzo == null)
            {
                log.AppendLine($"[aviso] {escena.name}: sin HUDController ni Canvas");
                return;
            }

            hud = lienzo.AddComponent<HUDController>();
            log.AppendLine($"  {escena.name}: HUDController agregado al Canvas");
        }

        SerializedObject serializado = new SerializedObject(hud);
        serializado.FindProperty("textoPuntos").objectReferenceValue = puntos;
        serializado.FindProperty("textoTiempo").objectReferenceValue = tiempo;
        serializado.FindProperty("textoPowerUp").objectReferenceValue = powerUp;
        serializado.FindProperty("textoNombre").objectReferenceValue = nombre;
        serializado.FindProperty("corazonLleno").objectReferenceValue = lleno;
        serializado.FindProperty("corazonVacio").objectReferenceValue = vacio;

        SerializedProperty lista = serializado.FindProperty("corazones");
        lista.arraySize = corazones.Length;

        for (int i = 0; i < corazones.Length; i++)
        {
            lista.GetArrayElementAtIndex(i).objectReferenceValue = corazones[i];
        }

        serializado.ApplyModifiedProperties();

        log.AppendLine($"  {escena.name}: HUD con corazones, reloj y nombre");
    }

    private static readonly (string nombre, string sprite, string rotulo)[] BotonesPausa =
    {
        ("BotonContinuar",     "BotonEmpezar", "CONTINUAR"),
        ("BotonVolver",        "BotonMenu",    "VOLVER"),
        ("BotonMenuPrincipal", "BotonMenu",    "MENU PRINCIPAL"),
    };

    private static readonly (string nombre, string sprite, string rotulo)[] BotonesFinal =
    {
        ("BotonReintentar", "BotonEmpezar", "REINTENTAR"),
        ("BotonVolver",     "BotonMenu",    "VOLVER"),
    };

    private static void RehacerPaneles(Scene escena, StringBuilder log)
    {
        GameManager gestor = Object.FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);

        Panel(escena, "PanelVictoria", "TextoVictoria", gestor, false, log);
        Panel(escena, "PanelDerrota", "TextoDerrota", gestor, false, log);
        Panel(escena, "PanelPausa", "TextoPausa", gestor, true, log);
    }

    private static void Panel(Scene escena, string nombrePanel, string nombreTitulo,
        GameManager gestor, bool esPausa, StringBuilder log)
    {
        RectTransform panel = UtilesInterfaz.BuscarRect(escena, nombrePanel);

        if (panel == null)
        {
            log.AppendLine($"[aviso] {escena.name}: no existe {nombrePanel}");
            return;
        }

        UtilesInterfaz.Estirar(panel);

        panel.SetAsLastSibling();

        RectTransform velo = AsegurarEnPanel(panel, panel, "Velo");
        UtilesInterfaz.Estirar(velo);
        UtilesInterfaz.Velo(velo);
        velo.SetSiblingIndex(0);

        Sprite spriteMarco = UtilesInterfaz.CargarSprite($"{CarpetaMenu}/Marco.png");
        Vector2 tamano = UtilesInterfaz.TamanoPorAlto(spriteMarco, AltoTarjeta);

        RectTransform tarjeta = AsegurarEnPanel(panel, panel, "Tarjeta");
        UtilesInterfaz.Colocar(tarjeta, new Vector2(0.5f, 0.5f), Vector2.zero, tamano);
        tarjeta.SetSiblingIndex(1);

        RectTransform fondo = AsegurarEnPanel(panel, tarjeta, "Fondo");
        UtilesInterfaz.Estirar(fondo);
        UtilesInterfaz.PonerImagen(fondo, UtilesInterfaz.CargarSprite(RutaFondoComun), false, false);
        fondo.SetSiblingIndex(0);

        RectTransform marco = AsegurarEnPanel(panel, tarjeta, "Marco");
        UtilesInterfaz.Estirar(marco);
        UtilesInterfaz.PonerImagen(marco, spriteMarco, false, false);
        UtilesInterfaz.AjustarProporcion(marco, spriteMarco);
        marco.SetSiblingIndex(1);

        AsegurarEnPanel(panel, tarjeta, "Columna");

        BorrarEnPanel(panel, "Interior");

        RectTransform columna = UtilesInterfaz.ColumnaEn(tarjeta, "Columna", 18f, 90, 80, RellenoTarjeta);
        columna.SetSiblingIndex(2);

        float ancho = tamano.x - RellenoTarjeta * 2f;

        RectTransform titulo = UtilesInterfaz.BuscarRect(escena, nombreTitulo);

        if (titulo != null)
        {
            UtilesInterfaz.Formato(titulo.GetComponent<TMP_Text>(), 68f,
                TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
            UtilesInterfaz.EnColumna(titulo, columna, 0, 150f, ancho, 0f);
        }

        UtilesInterfaz.EspaciadorEn(columna, "Aire", 1, 1f);

        (string nombre, string sprite, string rotulo)[] lista = esPausa ? BotonesPausa : BotonesFinal;

        for (int i = 0; i < lista.Length; i++)
        {
            RectTransform boton = BotonDePanel(panel, lista[i].nombre,
                $"{CarpetaComun}/{lista[i].sprite}.png", lista[i].rotulo, log);

            UtilesInterfaz.EnColumna(boton, columna, 2 + i, 150f, ancho, 0f, 115f);

            if (lista[i].nombre == "BotonMenuPrincipal" && boton != null && gestor != null)
            {
                UtilesInterfaz.Reconectar(boton.GetComponent<Button>(), gestor, "VolverMenuPrincipal");
            }
        }

        UtilesInterfaz.EspaciadorEn(columna, "AireAbajo", 2 + lista.Length, 1.1f);

        log.AppendLine($"  {escena.name}: {nombrePanel} en tarjeta de {tamano.x:0}x{tamano.y:0} sobre el velo");
    }

    private static void BorrarEnPanel(RectTransform panel, string nombre)
    {

        Transform sobrante = BuscarEnHijos(panel, nombre);

        if (sobrante != null && sobrante != panel)
        {
            Object.DestroyImmediate(sobrante.gameObject);
        }
    }

    private static RectTransform AsegurarEnPanel(RectTransform panel, Transform padre, string nombre)
    {

        Transform existente = BuscarEnHijos(panel, nombre);

        if (existente is RectTransform encontrado)
        {
            if (encontrado.parent != padre)
            {
                encontrado.SetParent(padre, false);
            }

            return encontrado;
        }

        return UtilesInterfaz.Asegurar(padre, nombre);
    }

    private static RectTransform BotonDePanel(RectTransform panel, string nombre, string rutaSprite,
        string rotulo, StringBuilder log)
    {
        Transform existente = BuscarEnHijos(panel, nombre);
        RectTransform rect;

        if (existente is RectTransform encontrado)
        {
            rect = encontrado;
        }
        else
        {
            GameObject nuevo = new GameObject(nombre, typeof(RectTransform), typeof(Image), typeof(Button));
            nuevo.transform.SetParent(panel, false);
            rect = nuevo.GetComponent<RectTransform>();

            log.AppendLine($"[aviso] {panel.name}: se creo {nombre}; revisa su OnClick en la escena");
        }

        Sprite sprite = UtilesInterfaz.CargarSprite(rutaSprite);

        Image imagen = UtilesInterfaz.PonerImagen(rect, sprite, true);
        Button boton = UtilesInterfaz.Componente<Button>(rect.gameObject);
        boton.targetGraphic = imagen;

        TMP_Text texto = UtilesInterfaz.TextoDeBoton(boton);

        if (texto == null)
        {
            texto = UtilesInterfaz.PonerTexto(UtilesInterfaz.Asegurar(rect, "Texto"), rotulo, LetraBoton,
                TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        }

        UtilesInterfaz.Estirar(texto.rectTransform);
        texto.text = rotulo;
        UtilesInterfaz.Formato(texto, LetraBoton, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        texto.gameObject.SetActive(true);

        return rect;
    }

    private static Transform BuscarEnHijos(Transform padre, string nombre)
    {
        foreach (Transform hijo in padre.GetComponentsInChildren<Transform>(true))
        {
            if (hijo.name == nombre)
            {
                return hijo;
            }
        }

        return null;
    }
}
