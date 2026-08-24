using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UtilesInterfaz
{

    public const float AnchoReferencia = 1080f;
    public const float AltoReferencia = 1920f;

    public static readonly Color Tinta = new Color(0.16f, 0.16f, 0.14f);
    public static readonly Color TintaClara = new Color(0.99f, 0.99f, 0.96f);

    public static Sprite CargarSprite(string ruta)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }

    public static Canvas AsegurarLienzo(GameObject objeto)
    {
        Canvas lienzo = Componente<Canvas>(objeto);
        lienzo.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler escalador = Componente<CanvasScaler>(objeto);
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(AnchoReferencia, AltoReferencia);
        escalador.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        escalador.matchWidthOrHeight = 0f;

        Componente<GraphicRaycaster>(objeto);

        return lienzo;
    }

    public static RectTransform AsegurarAreaSegura(Scene escena, Transform lienzo)
    {

        RectTransform area = BuscarRect(escena, "SafeArea");

        if (area == null)
        {
            area = Asegurar(lienzo, "SafeArea");
        }

        if (area.parent != lienzo)
        {
            area.SetParent(lienzo, false);
        }

        Estirar(area);
        Componente<SafeAreaController>(area.gameObject);

        return area;
    }

    public static RectTransform Columna(Scene escena, Transform padre, string nombre, float espaciado,
        int rellenoArriba, int rellenoAbajo, int rellenoLados = 40)
    {
        return Armar(Adoptar(escena, padre, nombre), espaciado, rellenoArriba, rellenoAbajo, rellenoLados);
    }

    public static RectTransform ColumnaEn(Transform padre, string nombre, float espaciado,
        int rellenoArriba, int rellenoAbajo, int rellenoLados = 40)
    {

        return Armar(Asegurar(padre, nombre), espaciado, rellenoArriba, rellenoAbajo, rellenoLados);
    }

    private static RectTransform Armar(RectTransform rect, float espaciado,
        int rellenoArriba, int rellenoAbajo, int rellenoLados)
    {
        Estirar(rect);

        VerticalLayoutGroup grupo = Componente<VerticalLayoutGroup>(rect.gameObject);
        grupo.spacing = espaciado;
        grupo.padding = new RectOffset(rellenoLados, rellenoLados, rellenoArriba, rellenoAbajo);
        grupo.childAlignment = TextAnchor.UpperCenter;
        grupo.childControlWidth = false;
        grupo.childControlHeight = true;
        grupo.childForceExpandWidth = false;
        grupo.childForceExpandHeight = false;
        grupo.childScaleWidth = false;
        grupo.childScaleHeight = false;

        return rect;
    }

    public static RectTransform Rejilla(Scene escena, Transform padre, string nombre, Vector2 celda,
        Vector2 espaciado, int columnas, int filas)
    {
        RectTransform rect = Adoptar(escena, padre, nombre);

        GridLayoutGroup grupo = Componente<GridLayoutGroup>(rect.gameObject);
        grupo.cellSize = celda;
        grupo.spacing = espaciado;
        grupo.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grupo.constraintCount = columnas;
        grupo.childAlignment = TextAnchor.UpperCenter;
        grupo.padding = new RectOffset(0, 0, 0, 0);

        rect.sizeDelta = new Vector2(
            columnas * celda.x + (columnas - 1) * espaciado.x,
            filas * celda.y + (filas - 1) * espaciado.y);

        return rect;
    }

    public static RectTransform FilaHorizontal(Scene escena, Transform padre, string nombre, float espaciado)
    {
        RectTransform rect = Adoptar(escena, padre, nombre);

        HorizontalLayoutGroup grupo = Componente<HorizontalLayoutGroup>(rect.gameObject);
        grupo.spacing = espaciado;
        grupo.padding = new RectOffset(0, 0, 0, 0);
        grupo.childAlignment = TextAnchor.MiddleCenter;
        grupo.childControlWidth = false;
        grupo.childControlHeight = false;
        grupo.childForceExpandWidth = false;
        grupo.childForceExpandHeight = false;
        grupo.childScaleWidth = false;
        grupo.childScaleHeight = false;

        return rect;
    }

    public static AspectRatioFitter AjustarProporcion(RectTransform rect, Sprite sprite)
    {
        AspectRatioFitter ajuste = Componente<AspectRatioFitter>(rect.gameObject);
        ajuste.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        ajuste.aspectRatio = sprite != null && sprite.rect.height > 0f
            ? sprite.rect.width / sprite.rect.height
            : 1f;
        return ajuste;
    }

    public static void EnColumna(RectTransform rect, Transform columna, int orden, float alto,
        float ancho = 0f, float peso = 1f, float minimo = 0f)
    {
        if (rect == null)
        {
            return;
        }

        rect.SetParent(columna, false);
        rect.SetSiblingIndex(orden);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        rect.pivot = new Vector2(0.5f, 0.5f);

        if (ancho > 0f)
        {
            rect.sizeDelta = new Vector2(ancho, rect.sizeDelta.y);
        }

        LayoutElement elemento = Componente<LayoutElement>(rect.gameObject);
        elemento.preferredHeight = alto;
        elemento.minHeight = minimo > 0f ? minimo : alto * 0.55f;
        elemento.flexibleHeight = peso;
        elemento.preferredWidth = -1f;
        elemento.minWidth = -1f;
        elemento.flexibleWidth = 0f;
    }

    public static RectTransform Espaciador(Scene escena, Transform columna, string nombre, int orden, float peso)
    {
        return Flexible(Adoptar(escena, columna, nombre), orden, peso);
    }

    public static RectTransform EspaciadorEn(Transform columna, string nombre, int orden, float peso)
    {

        return Flexible(Asegurar(columna, nombre), orden, peso);
    }

    private static RectTransform Flexible(RectTransform rect, int orden, float peso)
    {
        rect.SetSiblingIndex(orden);

        LayoutElement elemento = Componente<LayoutElement>(rect.gameObject);
        elemento.minHeight = 0f;
        elemento.preferredHeight = 0f;
        elemento.flexibleHeight = peso;
        elemento.flexibleWidth = 0f;

        return rect;
    }

    public static Image ZonaClicable(RectTransform rect)
    {
        Image imagen = Componente<Image>(rect.gameObject);
        imagen.sprite = null;
        imagen.type = Image.Type.Simple;
        imagen.preserveAspect = false;
        imagen.color = new Color(1f, 1f, 1f, 0f);
        imagen.raycastTarget = true;
        imagen.enabled = true;
        return imagen;
    }

    public static GameObject BuscarRaiz(Scene escena, string nombre)
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

    public static GameObject Buscar(Scene escena, string nombre)
    {
        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            if (raiz.name == nombre)
            {
                return raiz;
            }

            foreach (Transform hijo in raiz.GetComponentsInChildren<Transform>(true))
            {
                if (hijo.name == nombre)
                {
                    return hijo.gameObject;
                }
            }
        }

        return null;
    }

    public static RectTransform BuscarRect(Scene escena, string nombre)
    {
        GameObject objeto = Buscar(escena, nombre);
        return objeto != null ? objeto.transform as RectTransform : null;
    }

    public static RectTransform Asegurar(Transform padre, string nombre)
    {
        Transform existente = padre.Find(nombre);

        if (existente is RectTransform encontrado)
        {
            return encontrado;
        }

        GameObject nuevo = new GameObject(nombre, typeof(RectTransform));
        nuevo.transform.SetParent(padre, false);
        return nuevo.GetComponent<RectTransform>();
    }

    public static RectTransform Adoptar(Scene escena, Transform padre, string nombre)
    {
        RectTransform existente = BuscarRect(escena, nombre);

        if (existente == null)
        {
            return Asegurar(padre, nombre);
        }

        if (existente.parent != padre)
        {
            existente.SetParent(padre, false);
        }

        return existente;
    }

    public static void Borrar(Scene escena, string nombre)
    {
        GameObject objeto = Buscar(escena, nombre);

        if (objeto != null)
        {
            Object.DestroyImmediate(objeto);
        }
    }

    public static T Componente<T>(GameObject objeto) where T : Component
    {
        T componente = objeto.GetComponent<T>();
        return componente != null ? componente : objeto.AddComponent<T>();
    }

    public static void Colocar(RectTransform rect, Vector2 ancla, Vector2 posicion, Vector2 tamano)
    {
        rect.anchorMin = ancla;
        rect.anchorMax = ancla;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = posicion;
        rect.sizeDelta = tamano;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    public static void Estirar(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    public static Vector2 TamanoPorAlto(Sprite sprite, float alto)
    {
        if (sprite == null || sprite.rect.height <= 0f)
        {
            return new Vector2(alto, alto);
        }

        return new Vector2(alto * sprite.rect.width / sprite.rect.height, alto);
    }

    public static Vector2 TamanoPorAncho(Sprite sprite, float ancho)
    {
        if (sprite == null || sprite.rect.width <= 0f)
        {
            return new Vector2(ancho, ancho);
        }

        return new Vector2(ancho, ancho * sprite.rect.height / sprite.rect.width);
    }

    public static Image PonerImagen(RectTransform rect, Sprite sprite, bool recibeClicks = false,
        bool respetarProporcion = true)
    {
        Image imagen = Componente<Image>(rect.gameObject);
        imagen.sprite = sprite;
        imagen.type = Image.Type.Simple;
        imagen.preserveAspect = respetarProporcion;
        imagen.color = Color.white;
        imagen.raycastTarget = recibeClicks;
        imagen.enabled = sprite != null;
        return imagen;
    }

    public static Image Icono(Transform padre, string nombre, Sprite sprite, Vector2 posicion, float alto)
    {
        RectTransform rect = Asegurar(padre, nombre);
        Colocar(rect, new Vector2(0.5f, 0.5f), posicion, TamanoPorAlto(sprite, alto));
        return PonerImagen(rect, sprite);
    }

    public static void Formato(TMP_Text texto, float tamano, TextAlignmentOptions alineacion, Color color)
    {
        if (texto == null)
        {
            return;
        }

        texto.alignment = alineacion;
        texto.color = color;
        texto.enableWordWrapping = false;
        texto.overflowMode = TextOverflowModes.Overflow;

        texto.enableAutoSizing = true;
        texto.fontSizeMax = tamano;
        texto.fontSizeMin = tamano * 0.5f;
        texto.fontSize = tamano;
    }

    public static TMP_Text PonerTexto(RectTransform rect, string contenido, float tamano,
        TextAlignmentOptions alineacion, Color color)
    {
        TextMeshProUGUI texto = Componente<TextMeshProUGUI>(rect.gameObject);
        texto.text = contenido;
        texto.raycastTarget = false;
        Formato(texto, tamano, alineacion, color);
        return texto;
    }

    public static TMP_Text Etiqueta(Transform padre, string nombre, string contenido, Vector2 ancla,
        Vector2 posicion, Vector2 tamano, float tamanoLetra, TextAlignmentOptions alineacion, Color color)
    {
        RectTransform rect = Asegurar(padre, nombre);
        Colocar(rect, ancla, posicion, tamano);
        return PonerTexto(rect, contenido, tamanoLetra, alineacion, color);
    }

    public static Image Velo(RectTransform rect, float opacidad = 0.72f)
    {
        Image imagen = Componente<Image>(rect.gameObject);
        imagen.sprite = null;
        imagen.type = Image.Type.Simple;
        imagen.preserveAspect = false;
        imagen.color = new Color(0f, 0f, 0f, opacidad);
        imagen.raycastTarget = true;
        imagen.enabled = true;
        return imagen;
    }

    public static void LimpiarOnClick(Button boton)
    {
        for (int i = boton.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
        {
            UnityEventTools.RemovePersistentListener(boton.onClick, i);
        }
    }

    public static bool Conectar(Button boton, Object destino, string metodo)
    {
        if (boton == null || destino == null)
        {
            return false;
        }

        MethodInfo info = destino.GetType().GetMethod(metodo,
            BindingFlags.Public | BindingFlags.Instance, null, System.Type.EmptyTypes, null);

        if (info == null)
        {
            return false;
        }

        UnityAction accion = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), destino, info);
        UnityEventTools.AddPersistentListener(boton.onClick, accion);
        return true;
    }

    public static void Reconectar(Button boton, Object destino, string metodo)
    {
        LimpiarOnClick(boton);
        Conectar(boton, destino, metodo);
    }

    public static TMP_Text TextoDeBoton(Button boton)
    {
        return boton != null ? boton.GetComponentInChildren<TMP_Text>(true) : null;
    }
}
