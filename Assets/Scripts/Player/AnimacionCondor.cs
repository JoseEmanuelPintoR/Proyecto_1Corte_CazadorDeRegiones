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

    [Header("Accesorio elegido en Personalizacion")]
    [Tooltip("Vista de frente sin accesorio; es la unica que se puede sustituir.")]
    [SerializeField] private Sprite spriteFrente;

    [SerializeField] private AccesorioCondor[] accesorios;

    private const string ClaveAccesorio = "AccesorioSeleccionado";

    private Sprite spriteAccesorio;

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
        BuscarAccesorio();
    }

    private void BuscarAccesorio()
    {
        spriteAccesorio = null;

        if (accesorios == null || accesorios.Length == 0)
        {
            return;
        }

        string elegido = PlayerPrefs.GetString(ClaveAccesorio, "Predeterminado");

        foreach (AccesorioCondor accesorio in accesorios)
        {
            if (accesorio != null && accesorio.clave == elegido)
            {
                spriteAccesorio = accesorio.sprite;
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
        if (spriteAccesorio == null || sprite == null || spriteFrente == null)
        {
            return;
        }

        if (sprite.sprite == spriteFrente || sprite.sprite == spriteAccesorio)
        {
            sprite.sprite = spriteAccesorio;
        }
    }
}
