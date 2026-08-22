using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AnimacionesCondor
{
    private const string CarpetaVistas = "Assets/UI/CondorVistas";
    private const string CarpetaAnimaciones = "Assets/Animaciones/Condor";

    private const string RutaControlador = CarpetaAnimaciones + "/Condor.controller";

    private const string VistaFrente = "CondorFrente";
    private const string VistaLado = "CondorLado";
    private const string VistaTresCuartos = "Condor3-4";

    private const float AlturaCondorUnidades = 3f;

    private const string ParametroCaminando = "Caminando";
    private const string ParametroEnSuelo = "EnSuelo";

    private const int OrdenCondor = 10;

    public static string RutaFrente => $"{CarpetaVistas}/{VistaFrente}.png";
    public static string RutaLado => $"{CarpetaVistas}/{VistaLado}.png";
    public static string RutaTresCuartos => $"{CarpetaVistas}/{VistaTresCuartos}.png";

    public static void ImportarVistas(StringBuilder log)
    {
        log.AppendLine("--- Condor · importar vistas ---");

        foreach (string vista in new[] { VistaFrente, VistaLado, VistaTresCuartos })
        {
            string ruta = $"{CarpetaVistas}/{vista}.png";
            TextureImporter importador = AssetImporter.GetAtPath(ruta) as TextureImporter;

            if (importador == null)
            {
                log.AppendLine($"[aviso] No se encontro {ruta}");
                continue;
            }

            AjustesBase(importador);

            importador.isReadable = true;
            importador.SaveAndReimport();

            Texture2D textura = AssetDatabase.LoadAssetAtPath<Texture2D>(ruta);

            if (textura == null || !MedirSilueta(textura, out Vector2 pivote, out float fraccionAlto))
            {
                log.AppendLine($"[aviso] {vista}: no se pudo medir la silueta, se deja centrado");
                importador.isReadable = false;
                importador.SaveAndReimport();
                continue;
            }

            TextureImporterSettings ajustes = new TextureImporterSettings();
            importador.ReadTextureSettings(ajustes);
            ajustes.spriteAlignment = (int)SpriteAlignment.Custom;
            ajustes.spritePivot = pivote;
            importador.SetTextureSettings(ajustes);

            importador.isReadable = false;
            importador.SaveAndReimport();

            AjustarTamano(ruta, fraccionAlto);

            float alturaReal = AlturaVisible(ruta, fraccionAlto);
            log.AppendLine($"  {vista}: pivote ({pivote.x:0.000}, {pivote.y:0.000}) · silueta {fraccionAlto * 100f:0}% · alto real {alturaReal:0.00}u");
        }

        AssetDatabase.Refresh();
    }

    private static void AjustarTamano(string ruta, float fraccionAlto)
    {
        float alturaActual = AlturaVisible(ruta, fraccionAlto);

        if (alturaActual <= 0.0001f || Mathf.Abs(alturaActual - AlturaCondorUnidades) < 0.001f)
        {
            return;
        }

        TextureImporter importador = AssetImporter.GetAtPath(ruta) as TextureImporter;

        if (importador == null)
        {
            return;
        }

        importador.spritePixelsPerUnit *= alturaActual / AlturaCondorUnidades;
        importador.SaveAndReimport();
    }

    private static float AlturaVisible(string ruta, float fraccionAlto)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        return sprite != null ? sprite.bounds.size.y * fraccionAlto : 0f;
    }

    private static void AjustesBase(TextureImporter importador)
    {
        importador.textureType = TextureImporterType.Sprite;
        importador.spriteImportMode = SpriteImportMode.Single;
        importador.alphaIsTransparency = true;
        importador.mipmapEnabled = false;
        importador.wrapMode = TextureWrapMode.Clamp;
        importador.maxTextureSize = 1024;
        importador.textureCompression = TextureImporterCompression.Uncompressed;
    }

    private static bool MedirSilueta(Texture2D textura, out Vector2 pivote, out float fraccionAlto)
    {
        pivote = new Vector2(0.5f, 0.5f);
        fraccionAlto = 1f;

        Color32[] pixeles;

        try
        {
            pixeles = textura.GetPixels32();
        }
        catch
        {
            return false;
        }

        int ancho = textura.width;
        int alto = textura.height;

        int xMin = ancho;
        int xMax = -1;
        int yMin = alto;
        int yMax = -1;

        for (int y = 0; y < alto; y++)
        {
            for (int x = 0; x < ancho; x++)
            {
                if (pixeles[y * ancho + x].a <= 10)
                {
                    continue;
                }

                if (x < xMin) xMin = x;
                if (x > xMax) xMax = x;
                if (y < yMin) yMin = y;
                if (y > yMax) yMax = y;
            }
        }

        if (xMax < 0 || yMax < 0)
        {
            return false;
        }

        pivote = new Vector2((xMin + xMax + 1) * 0.5f / ancho, yMin / (float)alto);
        fraccionAlto = (yMax - yMin + 1) / (float)alto;
        return true;
    }

    public static AnimatorController CrearAnimaciones(StringBuilder log)
    {
        log.AppendLine("--- Condor · clips y controlador ---");

        CrearCarpeta("Assets/Animaciones");
        CrearCarpeta(CarpetaAnimaciones);

        Sprite frente = CargarVista(VistaFrente);
        Sprite lado = CargarVista(VistaLado);
        Sprite tresCuartos = CargarVista(VistaTresCuartos);

        if (frente == null || lado == null || tresCuartos == null)
        {
            log.AppendLine("[error] Faltan vistas del condor; importa el arte primero");
            return null;
        }

        AnimationClip quieto = ClipQuieto(frente);
        AnimationClip caminando = ClipCaminando(lado);
        AnimationClip saltando = ClipSaltando(tresCuartos);

        log.AppendLine("  Clips: CondorQuieto (respira) · CondorCaminando (contoneo) · CondorSaltando (estirado)");

        AnimatorController controlador = MontarControlador(quieto, caminando, saltando);
        AssetDatabase.SaveAssets();

        log.AppendLine($"  Controlador: {RutaControlador}");
        return controlador;
    }

    private static AnimationClip ClipQuieto(Sprite frente)
    {
        AnimationClip clip = Nuevo(frente,
            posicionY: Curva(0f, 0f, 0.8f, 0.03f, 1.6f, 0f),
            escalaX: Curva(0f, 1f, 0.8f, 0.98f, 1.6f, 1f),
            escalaY: Curva(0f, 1f, 0.8f, 1.04f, 1.6f, 1f),
            rotacionZ: Curva(0f, 0f));

        return Guardar(clip, "CondorQuieto", true);
    }

    private static AnimationClip ClipCaminando(Sprite lado)
    {
        AnimationClip clip = Nuevo(lado,
            posicionY: Curva(0f, 0f, 0.125f, 0.06f, 0.25f, 0f, 0.375f, 0.06f, 0.5f, 0f),
            escalaX: Curva(0f, 1f),
            escalaY: Curva(0f, 1f),
            rotacionZ: Curva(0f, 0f, 0.125f, 7f, 0.25f, 0f, 0.375f, -7f, 0.5f, 0f));

        return Guardar(clip, "CondorCaminando", true);
    }

    private static AnimationClip ClipSaltando(Sprite tresCuartos)
    {
        AnimationClip clip = Nuevo(tresCuartos,
            posicionY: Curva(0f, 0f),
            escalaX: Curva(0f, 0.92f, 0.35f, 1f),
            escalaY: Curva(0f, 1.1f, 0.35f, 1f),
            rotacionZ: Curva(0f, 0f));

        return Guardar(clip, "CondorSaltando", false);
    }

    private static AnimationClip Nuevo(Sprite sprite, AnimationCurve posicionY, AnimationCurve escalaX,
        AnimationCurve escalaY, AnimationCurve rotacionZ)
    {
        AnimationClip clip = new AnimationClip { frameRate = 24f };

        PonerSprite(clip, sprite);
        clip.SetCurve("", typeof(Transform), "localPosition.y", posicionY);
        clip.SetCurve("", typeof(Transform), "localScale.x", escalaX);
        clip.SetCurve("", typeof(Transform), "localScale.y", escalaY);
        clip.SetCurve("", typeof(Transform), "localEulerAnglesRaw.z", rotacionZ);

        return clip;
    }

    private static void PonerSprite(AnimationClip clip, Sprite sprite)
    {
        EditorCurveBinding enlace = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        ObjectReferenceKeyframe[] claves = { new ObjectReferenceKeyframe { time = 0f, value = sprite } };
        AnimationUtility.SetObjectReferenceCurve(clip, enlace, claves);
    }

    private static AnimationCurve Curva(params float[] tiemposYValores)
    {
        Keyframe[] claves = new Keyframe[tiemposYValores.Length / 2];

        for (int i = 0; i < claves.Length; i++)
        {
            claves[i] = new Keyframe(tiemposYValores[i * 2], tiemposYValores[i * 2 + 1]);
        }

        AnimationCurve curva = new AnimationCurve(claves);

        for (int i = 0; i < curva.length; i++)
        {
            curva.SmoothTangents(i, 0f);
        }

        return curva;
    }

    private static AnimationClip Guardar(AnimationClip clip, string nombre, bool enBucle)
    {
        AnimationClipSettings ajustes = AnimationUtility.GetAnimationClipSettings(clip);
        ajustes.loopTime = enBucle;
        AnimationUtility.SetAnimationClipSettings(clip, ajustes);

        string ruta = $"{CarpetaAnimaciones}/{nombre}.anim";
        AnimationClip existente = AssetDatabase.LoadAssetAtPath<AnimationClip>(ruta);

        if (existente != null)
        {
            EditorUtility.CopySerialized(clip, existente);
            Object.DestroyImmediate(clip);
            EditorUtility.SetDirty(existente);
            return existente;
        }

        AssetDatabase.CreateAsset(clip, ruta);
        return clip;
    }

    private static AnimatorController MontarControlador(AnimationClip quieto, AnimationClip caminando, AnimationClip saltando)
    {
        AnimatorController controlador = AssetDatabase.LoadAssetAtPath<AnimatorController>(RutaControlador);

        if (controlador == null)
        {
            controlador = AnimatorController.CreateAnimatorControllerAtPath(RutaControlador);
        }

        while (controlador.parameters.Length > 0)
        {
            controlador.RemoveParameter(0);
        }

        controlador.AddParameter(ParametroCaminando, AnimatorControllerParameterType.Bool);
        controlador.AddParameter(ParametroEnSuelo, AnimatorControllerParameterType.Bool);

        AnimatorStateMachine maquina = controlador.layers[0].stateMachine;

        foreach (AnimatorStateTransition transicion in maquina.anyStateTransitions)
        {
            maquina.RemoveAnyStateTransition(transicion);
        }

        foreach (ChildAnimatorState hijo in maquina.states)
        {
            maquina.RemoveState(hijo.state);
        }

        AnimatorState estadoQuieto = maquina.AddState("Quieto");
        estadoQuieto.motion = quieto;

        AnimatorState estadoCaminando = maquina.AddState("Caminando");
        estadoCaminando.motion = caminando;

        AnimatorState estadoSaltando = maquina.AddState("Saltando");
        estadoSaltando.motion = saltando;

        maquina.defaultState = estadoQuieto;

        Transicion(estadoQuieto.AddTransition(estadoCaminando),
            (AnimatorConditionMode.If, ParametroCaminando), (AnimatorConditionMode.If, ParametroEnSuelo));

        Transicion(estadoCaminando.AddTransition(estadoQuieto),
            (AnimatorConditionMode.IfNot, ParametroCaminando), (AnimatorConditionMode.If, ParametroEnSuelo));

        AnimatorStateTransition alAire = maquina.AddAnyStateTransition(estadoSaltando);
        alAire.canTransitionToSelf = false;
        Transicion(alAire, (AnimatorConditionMode.IfNot, ParametroEnSuelo));

        Transicion(estadoSaltando.AddTransition(estadoQuieto),
            (AnimatorConditionMode.If, ParametroEnSuelo), (AnimatorConditionMode.IfNot, ParametroCaminando));

        Transicion(estadoSaltando.AddTransition(estadoCaminando),
            (AnimatorConditionMode.If, ParametroEnSuelo), (AnimatorConditionMode.If, ParametroCaminando));

        EditorUtility.SetDirty(controlador);
        return controlador;
    }

    private static void Transicion(AnimatorStateTransition transicion, params (AnimatorConditionMode modo, string parametro)[] condiciones)
    {
        transicion.hasExitTime = false;
        transicion.duration = 0f;
        transicion.hasFixedDuration = true;

        foreach ((AnimatorConditionMode modo, string parametro) in condiciones)
        {
            transicion.AddCondition(modo, 0f, parametro);
        }
    }

    public static string PonerCondorEnJugador(Scene escena, AnimatorController controlador)
    {
        GameObject jugador = null;

        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            if (raiz.name == "Jugador")
            {
                jugador = raiz;
                break;
            }
        }

        if (jugador == null)
        {
            return "sin Jugador";
        }

        MeshRenderer capsula = jugador.GetComponent<MeshRenderer>();

        if (capsula != null)
        {
            capsula.enabled = false;
        }

        Transform visual = BuscarOCrear(jugador.transform, "Visual");

        CapsuleCollider collider = jugador.GetComponent<CapsuleCollider>();
        float alturaPies = collider != null ? collider.center.y - collider.height * 0.5f : -1f;

        visual.localPosition = new Vector3(0f, alturaPies, 0f);
        visual.localRotation = Quaternion.identity;
        visual.localScale = Vector3.one;

        Transform sprite = BuscarOCrear(visual, "Sprite");
        sprite.localPosition = Vector3.zero;
        sprite.localRotation = Quaternion.identity;
        sprite.localScale = Vector3.one;

        SpriteRenderer renderer = Asegurar<SpriteRenderer>(sprite.gameObject);
        renderer.sprite = CargarVista(VistaFrente);
        renderer.sortingOrder = OrdenCondor;

        Animator animator = Asegurar<Animator>(sprite.gameObject);
        animator.runtimeAnimatorController = controlador;
        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        AnimacionCondor animacion = Asegurar<AnimacionCondor>(visual.gameObject);

        SerializedObject serializado = new SerializedObject(animacion);
        serializado.FindProperty("jugador").objectReferenceValue = jugador.GetComponent<PlayerController>();
        serializado.FindProperty("animator").objectReferenceValue = animator;
        serializado.FindProperty("sprite").objectReferenceValue = renderer;
        serializado.FindProperty("arteMiraIzquierda").boolValue = true;
        serializado.ApplyModifiedProperties();

        return $"condor {AlturaCondorUnidades:0.0}u (capsula oculta)";
    }

    private static Transform BuscarOCrear(Transform padre, string nombre)
    {
        Transform hijo = padre.Find(nombre);

        if (hijo != null)
        {
            return hijo;
        }

        GameObject nuevo = new GameObject(nombre);
        nuevo.transform.SetParent(padre, false);
        return nuevo.transform;
    }

    private static T Asegurar<T>(GameObject objeto) where T : Component
    {
        T componente = objeto.GetComponent<T>();
        return componente != null ? componente : objeto.AddComponent<T>();
    }

    private static Sprite CargarVista(string nombre)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>($"{CarpetaVistas}/{nombre}.png");
    }

    private static void CrearCarpeta(string ruta)
    {
        if (AssetDatabase.IsValidFolder(ruta))
        {
            return;
        }

        int corte = ruta.LastIndexOf('/');
        AssetDatabase.CreateFolder(ruta.Substring(0, corte), ruta.Substring(corte + 1));
    }
}
