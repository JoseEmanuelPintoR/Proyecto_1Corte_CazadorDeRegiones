using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PantallaInstrucciones
{
    private const string CarpetaEscenas = "Assets/Scenes";
    private const string NombreEscena = "Instrucciones";
    private const string CarpetaMenu = "Assets/UI/PantallaMenu";

    private const int ElementosPorLista = 3;

    private const float AnchoInterior = 770f;

    private const float AltoBotonPie = 118f;

    private const string TextoPoder =
        "Recoge 3 elementos correctos para activar el poder.\n" +
        "Con el poder los objetos caen mas lento.";

    private class Arte
    {
        public string carpeta;
        public string cuadro;
        public string nombreMostrado;
    }

    private static readonly Arte[] ArtePorRegion =
    {
        new Arte { carpeta = "PANTALLA-INSTRUCCIONES-ANDINA",    cuadro = "CuadroAndina",    nombreMostrado = "Region Andina"    },
        new Arte { carpeta = "PANTALLA-INSTRUCCIONES-CARIBE",    cuadro = "CuadroCaribe",    nombreMostrado = "Region Caribe"    },
        new Arte { carpeta = "PANTALLA-INSTRUCCIONES-PACIFICO",  cuadro = "CuadroPacifico",  nombreMostrado = "Region Pacifica"  },
        new Arte { carpeta = "PANTALLA-INSTRUCCIONES-ORINOQUIA", cuadro = "CuadroOrinoquia", nombreMostrado = "Region Orinoquia" },
        new Arte { carpeta = "PANTALLA-INSTRUCCIONES-AMAZONIA",  cuadro = "CuadroAmazonia",  nombreMostrado = "Region Amazonia"  },
    };

    [MenuItem("Herramientas/Cazador de Regiones/9 · Construir pantalla de instrucciones", false, 108)]
    public static void MenuConstruir()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        StringBuilder log = new StringBuilder();
        log.AppendLine("=== 9 · Pantalla de instrucciones ===");

        BotonesInterfaz.ReimportarArteDeInterfaz(log);

        Scene escena = AbrirOCrear(log);
        InstruccionesController controlador = Montar(escena, log);

        if (controlador != null)
        {
            LlenarRegiones(controlador, log);
        }

        EditorSceneManager.MarkSceneDirty(escena);
        EditorSceneManager.SaveScene(escena, $"{CarpetaEscenas}/{NombreEscena}.unity");

        RegistrarEnBuild(log);

        Debug.Log(log.ToString());
    }

    private static Scene AbrirOCrear(StringBuilder log)
    {
        string ruta = $"{CarpetaEscenas}/{NombreEscena}.unity";

        if (File.Exists(ruta))
        {
            log.AppendLine($"  Escena existente: {ruta}");
            return EditorSceneManager.OpenScene(ruta, OpenSceneMode.Single);
        }

        log.AppendLine($"  Escena nueva: {ruta}");
        return EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }

    private static InstruccionesController Montar(Scene escena, StringBuilder log)
    {
        GameObject objetoLienzo = UtilesInterfaz.Buscar(escena, "Canvas");

        if (objetoLienzo == null)
        {
            objetoLienzo = new GameObject("Canvas", typeof(RectTransform));
            SceneManager.MoveGameObjectToScene(objetoLienzo, escena);
        }

        UtilesInterfaz.AsegurarLienzo(objetoLienzo);
        Transform lienzo = objetoLienzo.transform;
        AsegurarEventSystem(escena);

        GameObject control = UtilesInterfaz.Buscar(escena, "ControlInstrucciones");

        if (control == null)
        {
            control = new GameObject("ControlInstrucciones");
            SceneManager.MoveGameObjectToScene(control, escena);
        }

        InstruccionesController controlador = UtilesInterfaz.Componente<InstruccionesController>(control);

        RectTransform marco = BotonesInterfaz.FondoYMarco(escena, lienzo, $"{CarpetaMenu}/Marco.png");
        Image fondo = lienzo.Find("Fondo").GetComponent<Image>();

        RectTransform columna = UtilesInterfaz.Columna(escena, marco, "Columna", 16f, 170, 70, 74);

        Sprite flecha = UtilesInterfaz.CargarSprite(RutaArte(0, "FlechaRegresar"));

        Button regresar = Boton(escena, marco, "BotonRegresar", flecha, new Vector2(0f, 1f),
            new Vector2(105f, -105f), 125f, null, 0f, out Image imagenRegresar);
        UtilesInterfaz.Reconectar(regresar, controlador, "Regresar");

        Sprite cuadroInicial = UtilesInterfaz.CargarSprite(RutaCuadro(0));
        RectTransform cuadro = UtilesInterfaz.Adoptar(escena, columna, "Cuadro");
        Image imagenCuadro = UtilesInterfaz.PonerImagen(cuadro, cuadroInicial);

        UtilesInterfaz.EnColumna(cuadro, columna, 0, 640f, AnchoInterior, 0f, 430f);

        TMP_Text textoNivel = UtilesInterfaz.Etiqueta(cuadro, "TextoNivel", "Nivel 1 de 5",
            new Vector2(0.5f, 1f), new Vector2(0f, -68f), new Vector2(600f, 70f),
            46f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);

        TMP_Text textoRegion = UtilesInterfaz.Etiqueta(cuadro, "TextoRegion", "Region Andina",
            new Vector2(0.5f, 1f), new Vector2(0f, -150f), new Vector2(600f, 96f),
            62f, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);

        Button anterior = Boton(escena, cuadro, "BotonAnterior", flecha, new Vector2(0f, 0.5f),
            new Vector2(66f, -40f), 100f, null, 0f, out Image imagenAnterior);
        UtilesInterfaz.Reconectar(anterior, controlador, "Anterior");

        Button siguiente = Boton(escena, cuadro, "BotonSiguiente", flecha, new Vector2(1f, 0.5f),
            new Vector2(-66f, -40f), 100f, null, 0f, out Image imagenSiguiente);
        UtilesInterfaz.Reconectar(siguiente, controlador, "Siguiente");
        siguiente.transform.localScale = new Vector3(-1f, 1f, 1f);

        RectTransform listas = UtilesInterfaz.Adoptar(escena, columna, "Listas");
        float altoListas = 140f + ElementosPorLista * 118f;
        UtilesInterfaz.EnColumna(listas, columna, 1, altoListas, AnchoInterior, 0f, altoListas);

        Image imagenRecoge = Encabezado(escena, listas, "TituloRecoge", RutaArte(0, "Recoge"), 0.25f);
        Image imagenEvita = Encabezado(escena, listas, "TituloEvita", RutaArte(0, "Evita"), 0.75f);

        Sprite linea = UtilesInterfaz.CargarSprite(RutaArte(0, "Linea medio"));
        RectTransform divisor = UtilesInterfaz.Adoptar(escena, listas, "LineaMedio");
        divisor.anchorMin = new Vector2(0.5f, 0f);
        divisor.anchorMax = new Vector2(0.5f, 1f);
        divisor.pivot = new Vector2(0.5f, 0.5f);
        divisor.anchoredPosition = Vector2.zero;
        divisor.sizeDelta = new Vector2(4f, -20f);
        divisor.localScale = Vector3.one;
        Image imagenLinea = UtilesInterfaz.PonerImagen(divisor, linea, false, false);

        Image[] iconosRecoge = new Image[ElementosPorLista];
        TMP_Text[] textosRecoge = new TMP_Text[ElementosPorLista];
        Image[] iconosEvita = new Image[ElementosPorLista];
        TMP_Text[] textosEvita = new TMP_Text[ElementosPorLista];

        for (int i = 0; i < ElementosPorLista; i++)
        {
            float y = -140f - i * 118f;

            Fila(escena, listas, $"Recoge{i + 1}", 0.25f, y, out iconosRecoge[i], out textosRecoge[i]);
            Fila(escena, listas, $"Evita{i + 1}", 0.75f, y, out iconosEvita[i], out textosEvita[i]);
        }

        Poder(escena, columna);

        UtilesInterfaz.Espaciador(escena, columna, "Aire", 3, 0.3f);

        RectTransform fila = UtilesInterfaz.Adoptar(escena, columna, "Botones");
        UtilesInterfaz.EnColumna(fila, columna, 4, 130f, AnchoInterior, 0f, AltoBotonPie);

        Button menu = Boton(escena, fila, "BotonMenu", UtilesInterfaz.CargarSprite(RutaArte(0, "BotonMenu")),
            new Vector2(0.24f, 0.5f), Vector2.zero, AltoBotonPie, "Menu", 46f, out Image imagenMenu);
        UtilesInterfaz.Reconectar(menu, controlador, "VolverMenu");

        Button empezar = Boton(escena, fila, "BotonEmpezar", UtilesInterfaz.CargarSprite(RutaArte(0, "BotonEmpezar")),
            new Vector2(0.76f, 0.5f), Vector2.zero, AltoBotonPie, "Empezar", 46f, out Image imagenEmpezar);
        UtilesInterfaz.Reconectar(empezar, controlador, "Empezar");

        SerializedObject serializado = new SerializedObject(controlador);
        serializado.FindProperty("fondo").objectReferenceValue = fondo;
        serializado.FindProperty("cuadro").objectReferenceValue = imagenCuadro;
        serializado.FindProperty("textoNivel").objectReferenceValue = textoNivel;
        serializado.FindProperty("textoRegion").objectReferenceValue = textoRegion;
        serializado.FindProperty("imagenRecoge").objectReferenceValue = imagenRecoge;
        serializado.FindProperty("imagenEvita").objectReferenceValue = imagenEvita;
        serializado.FindProperty("imagenLinea").objectReferenceValue = imagenLinea;
        serializado.FindProperty("imagenBotonMenu").objectReferenceValue = imagenMenu;
        serializado.FindProperty("imagenBotonEmpezar").objectReferenceValue = imagenEmpezar;
        serializado.FindProperty("imagenFlechaRegresar").objectReferenceValue = imagenRegresar;
        serializado.FindProperty("imagenFlechaAnterior").objectReferenceValue = imagenAnterior;
        serializado.FindProperty("imagenFlechaSiguiente").objectReferenceValue = imagenSiguiente;
        LlenarArreglo(serializado.FindProperty("iconosRecoge"), iconosRecoge);
        LlenarArreglo(serializado.FindProperty("textosRecoge"), textosRecoge);
        LlenarArreglo(serializado.FindProperty("iconosEvita"), iconosEvita);
        LlenarArreglo(serializado.FindProperty("textosEvita"), textosEvita);
        serializado.ApplyModifiedProperties();

        log.AppendLine("  Pantalla montada: mapa, flechas, 3+3 elementos, Menu y Empezar");
        return controlador;
    }

    private static void Poder(Scene escena, Transform columna)
    {

        const float alto = 240f;

        RectTransform bloque = UtilesInterfaz.Adoptar(escena, columna, "Poder");
        UtilesInterfaz.EnColumna(bloque, columna, 2, alto, AnchoInterior, 0f, alto);

        Sprite energia = UtilesInterfaz.CargarSprite("Assets/UI/PantallaJuego/Energia.png");

        RectTransform rectIcono = UtilesInterfaz.Asegurar(bloque, "Icono");
        UtilesInterfaz.Colocar(rectIcono, new Vector2(0f, 0.5f), new Vector2(62f, 0f),
            UtilesInterfaz.TamanoPorAlto(energia, 100f));
        UtilesInterfaz.PonerImagen(rectIcono, energia);

        TMP_Text texto = UtilesInterfaz.Etiqueta(bloque, "Texto", TextoPoder, new Vector2(0f, 0.5f),
            new Vector2(430f, 0f), new Vector2(610f, 210f), 40f,
            TextAlignmentOptions.Left, UtilesInterfaz.Tinta);

        texto.enableWordWrapping = true;

        texto.fontSizeMin = 34f;
    }

    private static void AsegurarEventSystem(Scene escena)
    {
        GameObject objeto = UtilesInterfaz.Buscar(escena, "EventSystem");

        if (objeto == null)
        {
            objeto = new GameObject("EventSystem");
            SceneManager.MoveGameObjectToScene(objeto, escena);
        }

        UtilesInterfaz.Componente<EventSystem>(objeto);

        UtilesInterfaz.Componente<InputSystemUIInputModule>(objeto);
    }

    private static Image Encabezado(Scene escena, Transform padre, string nombre, string ruta, float fraccionX)
    {
        Sprite sprite = UtilesInterfaz.CargarSprite(ruta);
        RectTransform rect = UtilesInterfaz.Adoptar(escena, padre, nombre);
        UtilesInterfaz.Colocar(rect, new Vector2(fraccionX, 1f), new Vector2(0f, -58f),
            UtilesInterfaz.TamanoPorAlto(sprite, 120f));
        return UtilesInterfaz.PonerImagen(rect, sprite);
    }

    private static void Fila(Scene escena, Transform padre, string nombre, float fraccionX, float y,
        out Image icono, out TMP_Text texto)
    {
        RectTransform fila = UtilesInterfaz.Adoptar(escena, padre, nombre);
        UtilesInterfaz.Colocar(fila, new Vector2(fraccionX, 1f), new Vector2(0f, y), new Vector2(370f, 104f));

        RectTransform rectIcono = UtilesInterfaz.Asegurar(fila, "Icono");
        UtilesInterfaz.Colocar(rectIcono, new Vector2(0f, 0.5f), new Vector2(52f, 0f), new Vector2(94f, 94f));
        icono = UtilesInterfaz.PonerImagen(rectIcono, null);

        texto = UtilesInterfaz.Etiqueta(fila, "Nombre", "", new Vector2(0f, 0.5f),
            new Vector2(240f, 0f), new Vector2(260f, 92f), 40f, TextAlignmentOptions.Left, UtilesInterfaz.Tinta);
    }

    private static Button Boton(Scene escena, Transform padre, string nombre, Sprite sprite, Vector2 ancla,
        Vector2 posicion, float alto, string etiqueta, float tamanoLetra, out Image imagen)
    {
        RectTransform rect = UtilesInterfaz.Adoptar(escena, padre, nombre);
        UtilesInterfaz.Colocar(rect, ancla, posicion, UtilesInterfaz.TamanoPorAlto(sprite, alto));

        imagen = UtilesInterfaz.PonerImagen(rect, sprite, true);
        Button boton = UtilesInterfaz.Componente<Button>(rect.gameObject);
        boton.targetGraphic = imagen;

        if (etiqueta != null)
        {
            RectTransform rectTexto = UtilesInterfaz.Asegurar(rect, "Texto");
            UtilesInterfaz.Estirar(rectTexto);
            UtilesInterfaz.PonerTexto(rectTexto, etiqueta, tamanoLetra, TextAlignmentOptions.Center, UtilesInterfaz.Tinta);
        }

        return boton;
    }

    private static void LlenarArreglo(SerializedProperty lista, Object[] valores)
    {
        lista.arraySize = valores.Length;

        for (int i = 0; i < valores.Length; i++)
        {
            lista.GetArrayElementAtIndex(i).objectReferenceValue = valores[i];
        }
    }

    private static string RutaCuadro(int region)
    {
        return RutaArte(region, ArtePorRegion[region].cuadro);
    }

    private static string RutaArte(int region, string archivo)
    {
        return $"Assets/UI/{ArtePorRegion[region].carpeta}/{archivo}.png";
    }

    private static Sprite ArteDe(int region, string archivo)
    {
        return UtilesInterfaz.CargarSprite(RutaArte(region, archivo));
    }

    private static void LlenarRegiones(InstruccionesController controlador, StringBuilder log)
    {
        SerializedObject serializado = new SerializedObject(controlador);
        SerializedProperty lista = serializado.FindProperty("regiones");

        int total = Mathf.Min(ArtePorRegion.Length, ConfigurarEscenarios.Niveles.Length);
        lista.arraySize = total;

        for (int i = 0; i < total; i++)
        {
            Arte arte = ArtePorRegion[i];
            ConfigurarEscenarios.Nivel nivel = ConfigurarEscenarios.Niveles[i];

            SerializedProperty region = lista.GetArrayElementAtIndex(i);
            region.FindPropertyRelative("clave").stringValue = nivel.sufijo;
            region.FindPropertyRelative("nombreMostrado").stringValue = arte.nombreMostrado;
            region.FindPropertyRelative("escenaNivel").stringValue = nivel.escena;

            region.FindPropertyRelative("fondo").objectReferenceValue = ArteDe(i, "Fondo");
            region.FindPropertyRelative("cuadro").objectReferenceValue =
                UtilesInterfaz.CargarSprite(RutaCuadro(i));
            region.FindPropertyRelative("recoge").objectReferenceValue = ArteDe(i, "Recoge");
            region.FindPropertyRelative("evita").objectReferenceValue = ArteDe(i, "Evita");
            region.FindPropertyRelative("lineaMedio").objectReferenceValue = ArteDe(i, "Linea medio");
            region.FindPropertyRelative("botonMenu").objectReferenceValue = ArteDe(i, "BotonMenu");
            region.FindPropertyRelative("botonEmpezar").objectReferenceValue = ArteDe(i, "BotonEmpezar");
            region.FindPropertyRelative("flecha").objectReferenceValue = ArteDe(i, "FlechaRegresar");

            Rellenar(region, "iconosRecoge", "nombresRecoge", nivel.elementos, 0);
            Rellenar(region, "iconosEvita", "nombresEvita", nivel.elementos, ElementosPorLista);

            log.AppendLine($"  {arte.nombreMostrado}: {Resumen(nivel.elementos, 0)} / {Resumen(nivel.elementos, ElementosPorLista)}");
        }

        serializado.ApplyModifiedProperties();
    }

    private static void Rellenar(SerializedProperty region, string campoIconos, string campoNombres,
        string[] elementos, int desde)
    {
        SerializedProperty iconos = region.FindPropertyRelative(campoIconos);
        SerializedProperty nombres = region.FindPropertyRelative(campoNombres);

        List<string> tomados = new List<string>();

        for (int i = desde; i < Mathf.Min(desde + ElementosPorLista, elementos.Length); i++)
        {
            tomados.Add(elementos[i]);
        }

        iconos.arraySize = tomados.Count;
        nombres.arraySize = tomados.Count;

        for (int i = 0; i < tomados.Count; i++)
        {
            iconos.GetArrayElementAtIndex(i).objectReferenceValue = ConfigurarEscenarios.SpriteDeElemento(tomados[i]);
            nombres.GetArrayElementAtIndex(i).stringValue = ConfigurarEscenarios.NombreDeElemento(tomados[i]);
        }
    }

    private static string Resumen(string[] elementos, int desde)
    {
        List<string> nombres = new List<string>();

        for (int i = desde; i < Mathf.Min(desde + ElementosPorLista, elementos.Length); i++)
        {
            nombres.Add(ConfigurarEscenarios.NombreDeElemento(elementos[i]));
        }

        return string.Join(", ", nombres);
    }

    private static void RegistrarEnBuild(StringBuilder log)
    {
        string ruta = $"{CarpetaEscenas}/{NombreEscena}.unity";

        foreach (EditorBuildSettingsScene registrada in EditorBuildSettings.scenes)
        {
            if (registrada.path == ruta)
            {
                log.AppendLine("  Build Settings: ya estaba registrada");
                return;
            }
        }

        List<EditorBuildSettingsScene> escenas = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        escenas.Add(new EditorBuildSettingsScene(ruta, true));
        EditorBuildSettings.scenes = escenas.ToArray();

        log.AppendLine($"  Build Settings: agregada en el indice {escenas.Count - 1}");
    }
}
