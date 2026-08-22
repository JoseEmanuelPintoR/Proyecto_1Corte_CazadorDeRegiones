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
        CarpetaMenu, CarpetaJuego, CarpetaPersonalizar, CarpetaPrendas,
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
        Vector2 posicion, float alto, StringBuilder log, Transform padre = null)
    {
        RectTransform rect = UtilesInterfaz.BuscarRect(escena, nombre);

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

    private static void Creditos(StringBuilder log)
    {
        Scene escena = AbrirEscena("Creditos");
        Transform lienzo = UtilesInterfaz.BuscarRaiz(escena, "Canvas").transform;

        RectTransform columna = Armazon(escena, lienzo, $"{CarpetaMenu}/Marco.png", 26f, 230, 120, out RectTransform marco);

        UtilesInterfaz.EnColumna(Titulo(escena, "TituloCreditos", 76f, log), columna, 0, 170f, AnchoInterior, 0f);
        UtilesInterfaz.EnColumna(UtilesInterfaz.BuscarRect(escena, "ImagenCreditos"),
            columna, 1, 900f, AnchoInterior, 1f, 300f);

        BotonIcono(escena, "BotonVolver", $"{CarpetaPersonalizar}/FlechaRegresar.png",
            new Vector2(0f, 1f), new Vector2(105f, -105f), 125f, log, marco);

        log.AppendLine("  Creditos: columna con titulo e imagen + flecha de regreso");
        Guardar(escena);
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

            AjustarSalto(escena, log);
            RehacerHUD(escena, log);
            RehacerPaneles(escena, log);

            Guardar(escena);
        }

        Debug.Log(log.ToString());
    }

    private static void AjustarSalto(Scene escena, StringBuilder log)
    {
        PlayerController jugador = Object.FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);

        if (jugador == null)
        {
            log.AppendLine($"[aviso] {escena.name}: sin PlayerController");
            return;
        }

        SerializedObject serializado = new SerializedObject(jugador);
        serializado.FindProperty("fuerzaSalto").floatValue = 11f;
        serializado.FindProperty("multiplicadorCaida").floatValue = 2.5f;
        serializado.FindProperty("multiplicadorSaltoCorto").floatValue = 2f;
        serializado.ApplyModifiedProperties();

        log.AppendLine($"  {escena.name}: salto 11 · caida x2.5");
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

        RectTransform contenedorCorazones = UtilesInterfaz.Asegurar(area, "Corazones");
        UtilesInterfaz.Colocar(contenedorCorazones, new Vector2(0f, 1f), new Vector2(250f, -130f), new Vector2(360f, 100f));

        Image[] corazones = new Image[3];

        for (int i = 0; i < corazones.Length; i++)
        {
            RectTransform rect = UtilesInterfaz.Asegurar(contenedorCorazones, $"Corazon{i + 1}");
            UtilesInterfaz.Colocar(rect, new Vector2(0.5f, 0.5f), new Vector2((i - 1) * 110f, 0f), new Vector2(88f, 88f));
            corazones[i] = UtilesInterfaz.PonerImagen(rect, lleno);
        }

        Sprite marcoTiempo = UtilesInterfaz.CargarSprite($"{CarpetaJuego}/MarcoTiempo.png");
        RectTransform contenedorTiempo = UtilesInterfaz.Asegurar(area, "MarcoTiempo");
        UtilesInterfaz.Colocar(contenedorTiempo, new Vector2(0.5f, 1f), new Vector2(0f, -130f),
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

        BotonIcono(escena, "BotonIzquierda", $"{CarpetaJuego}/Izquierda.png",
            new Vector2(0f, 0f), new Vector2(200f, 210f), 150f, log, area);
        BotonIcono(escena, "BotonDerecha", $"{CarpetaJuego}/Derecha.png",
            new Vector2(0f, 0f), new Vector2(430f, 210f), 150f, log, area);
        BotonIcono(escena, "BotonSalto", $"{CarpetaJuego}/Arriba.png",
            new Vector2(1f, 0f), new Vector2(-200f, 210f), 150f, log, area);
        BotonIcono(escena, "BotonPowerUp", $"{CarpetaJuego}/Energia.png",
            new Vector2(1f, 0f), new Vector2(-200f, 420f), 150f, log, area);

        Sostener(escena, "BotonIzquierda", BotonMovimiento.Direccion.Izquierda, jugador, log);
        Sostener(escena, "BotonDerecha", BotonMovimiento.Direccion.Derecha, jugador, log);
        Sostener(escena, "BotonSalto", BotonMovimiento.Direccion.Salto, jugador, log);

        RectTransform rectPowerUp = UtilesInterfaz.BuscarRect(escena, "TextoPowerUp");
        TMP_Text textoPowerUp = null;
        RectTransform botonPowerUp = UtilesInterfaz.BuscarRect(escena, "BotonPowerUp");

        if (rectPowerUp != null && botonPowerUp != null)
        {
            rectPowerUp.SetParent(botonPowerUp, false);
            UtilesInterfaz.Colocar(rectPowerUp, new Vector2(0.5f, 1f), new Vector2(0f, 46f), new Vector2(240f, 60f));
            textoPowerUp = UtilesInterfaz.PonerTexto(rectPowerUp, "0/3", 40f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        }

        ConectarHUD(textoPuntos, textoTiempo, textoPowerUp, textoNombre, corazones, lleno, vacio, escena, log);
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

        RectTransform velo = UtilesInterfaz.Asegurar(panel, "Velo");
        UtilesInterfaz.Estirar(velo);
        UtilesInterfaz.Velo(velo);
        velo.SetSiblingIndex(0);

        Sprite spriteMarco = UtilesInterfaz.CargarSprite($"{CarpetaMenu}/Marco.png");
        RectTransform marco = UtilesInterfaz.Asegurar(panel, "Marco");
        UtilesInterfaz.Colocar(marco, new Vector2(0.5f, 0.5f), Vector2.zero,
            UtilesInterfaz.TamanoPorAlto(spriteMarco, 1000f));
        UtilesInterfaz.PonerImagen(marco, spriteMarco);
        marco.SetSiblingIndex(1);

        RectTransform titulo = UtilesInterfaz.BuscarRect(escena, nombreTitulo);

        if (titulo != null)
        {
            UtilesInterfaz.Colocar(titulo, new Vector2(0.5f, 0.5f), new Vector2(0f, 260f), new Vector2(880f, 180f));
            UtilesInterfaz.Formato(titulo.GetComponent<TMP_Text>(), 82f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        }

        string empezar = $"{CarpetaComun}/BotonEmpezar.png";
        string menu = $"{CarpetaComun}/BotonMenu.png";

        if (esPausa)
        {
            BotonDePanel(panel, "BotonContinuar", empezar, new Vector2(0f, -180f), 100f, log);
            BotonDePanel(panel, "BotonVolver", menu, new Vector2(0f, -320f), 100f, log);

            RectTransform botonMenu = BotonDePanel(panel, "BotonMenuPrincipal", menu, new Vector2(0f, -460f), 100f, log);

            if (botonMenu != null && gestor != null)
            {
                Button boton = UtilesInterfaz.Componente<Button>(botonMenu.gameObject);
                UtilesInterfaz.Reconectar(boton, gestor, "VolverMenuPrincipal");
                Etiquetar(boton, "Menu principal", 44f);
            }
        }
        else
        {
            BotonDePanel(panel, "BotonReintentar", empezar, new Vector2(0f, -180f), 100f, log);
            BotonDePanel(panel, "BotonVolver", menu, new Vector2(0f, -320f), 100f, log);
        }
    }

    private static RectTransform BotonDePanel(RectTransform panel, string nombre, string rutaSprite,
        Vector2 posicion, float alto, StringBuilder log)
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

            RectTransform etiqueta = UtilesInterfaz.Asegurar(rect, "Texto");
            UtilesInterfaz.Estirar(etiqueta);
            UtilesInterfaz.PonerTexto(etiqueta, nombre, 44f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        }

        Sprite sprite = UtilesInterfaz.CargarSprite(rutaSprite);
        UtilesInterfaz.Colocar(rect, new Vector2(0.5f, 0.5f), posicion, UtilesInterfaz.TamanoPorAlto(sprite, alto));

        Image imagen = UtilesInterfaz.PonerImagen(rect, sprite, true);
        Button boton = UtilesInterfaz.Componente<Button>(rect.gameObject);
        boton.targetGraphic = imagen;

        TMP_Text texto = UtilesInterfaz.TextoDeBoton(boton);

        if (texto != null)
        {
            UtilesInterfaz.Estirar(texto.rectTransform);
            UtilesInterfaz.Formato(texto, 44f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        }

        return rect;
    }

    private static void Etiquetar(Button boton, string contenido, float tamano)
    {
        TMP_Text texto = UtilesInterfaz.TextoDeBoton(boton);

        if (texto == null)
        {
            return;
        }

        texto.text = contenido;
        texto.fontSize = tamano;
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
