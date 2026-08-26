using UnityEngine;

public class AnimacionCondor : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerController jugador;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Arte")]
    [Tooltip("Marcado si el dibujo de perfil mira hacia la izquierda (es el caso de CondorLado).")]
    [SerializeField] private bool arteMiraIzquierda = true;

    [Header("Accesorio elegido en Personalización")]
    [Tooltip("Vistas base sin accesorio; son las que el Animator pone en cada clip.")]
    [SerializeField] private Sprite spriteFrente;
    [SerializeField] private Sprite spriteLado;
    [SerializeField] private Sprite spriteTresCuartos;

    [SerializeField] private SkinCondor[] skins;

    private const string ClaveAccesorio = "AccesorioSeleccionado";

    private SkinCondor skinElegido;

    private static readonly int ParametroCaminando = Animator.StringToHash("Caminando");
    private static readonly int ParametroEnSuelo = Animator.StringToHash("EnSuelo");

    private float direccion = -1f;

    private void Reset()
    {
        Resolver();
    }

    private void Awake()
    {
        Resolver();
        BuscarSkin();
    }

    private void BuscarSkin()
    {
        skinElegido = null;

        if (skins == null || skins.Length == 0)
        {
            return;
        }

        string elegido = PlayerPrefs.GetString(ClaveAccesorio, "Predeterminado");

        foreach (SkinCondor skin in skins)
        {
            if (skin != null && skin.clave == elegido)
            {
                skinElegido = skin;
                return;
            }
        }
    }

    private void Resolver()
    {
        if (jugador == null)
        {
            jugador = GetComponentInParent<PlayerController>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (jugador == null || animator == null)
        {
            return;
        }

        float movimiento = jugador.MovimientoHorizontal;
        bool caminando = Mathf.Abs(movimiento) > 0.01f;

        animator.SetBool(ParametroCaminando, caminando);
        animator.SetBool(ParametroEnSuelo, jugador.EstaEnSuelo);

        if (caminando)
        {
            direccion = Mathf.Sign(movimiento);
        }

        if (sprite != null)
        {
            sprite.flipX = arteMiraIzquierda ? direccion > 0f : direccion < 0f;
        }
    }

    private void LateUpdate()
    {
        if (skinElegido == null || sprite == null)
        {
            return;
        }

        Sprite reemplazo = Equivalente(sprite.sprite);

        if (reemplazo != null)
        {
            sprite.sprite = reemplazo;
        }
    }

    private Sprite Equivalente(Sprite actual)
    {
        if (actual == null)
        {
            return null;
        }

        if (actual == spriteFrente || actual == skinElegido.frente)
        {
            return skinElegido.frente;
        }

        if (actual == spriteLado || actual == skinElegido.lado)
        {
            return skinElegido.lado;
        }

        if (actual == spriteTresCuartos || actual == skinElegido.tresCuartos)
        {
            return skinElegido.tresCuartos;
        }

        return null;
    }
}
