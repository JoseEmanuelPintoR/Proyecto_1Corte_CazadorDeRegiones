using UnityEngine;

public class SpawnerObjetos : MonoBehaviour
{
    [Header("Objetos correctos")]
    [SerializeField] private GameObject[] objetosCorrectos;

    [Header("Objetos incorrectos")]
    [SerializeField] private GameObject[] objetosIncorrectos;

    [Header("Tiempo de aparicion")]
    [SerializeField] private float tiempoMinimo = 1f;
    [SerializeField] private float tiempoMaximo = 2f;

    [Header("Limites horizontales")]
    [Tooltip("Calcula los limites con lo que la camara ve de verdad, para que ningun objeto " +
             "caiga fuera de la pantalla. Al desmarcarlo se usan los valores fijos de abajo.")]
    [SerializeField] private bool limitesDesdeCamara = true;

    [Tooltip("Radio del objeto que cae, para que no quede medio afuera al borde.")]
    [SerializeField] private float radioObjeto = 0.5f;

    [Tooltip("Cuanto se permite que un objeto caiga mas afuera de donde llega el centro del " +
             "condor. Con el cuerpo del condor alcanza a atraparlo.")]
    [SerializeField] private float toleranciaAlcance = 0.4f;

    [SerializeField] private float limiteIzquierdo = -4f;
    [SerializeField] private float limiteDerecho = 4f;

    [Header("Cantidad simultanea")]
    [SerializeField] private bool permitirMultiples = false;

    private float tiempoSiguiente;

    private Camera camara;
    private Vector2Int ultimaPantalla;

    private PlayerController jugador;

    private const float PlanoJuego = 0f;

    void Start()
    {
        camara = Camera.main;
        jugador = FindFirstObjectByType<PlayerController>();
        CalcularLimites();
        CalcularSiguienteAparicion();
    }

    private void CalcularLimites()
    {
        ultimaPantalla = new Vector2Int(Screen.width, Screen.height);

        if (!limitesDesdeCamara)
        {
            return;
        }

        if (camara == null)
        {
            camara = Camera.main;
        }

        float mitadAncho = LimitesCamara.MitadAncho(camara, PlanoJuego);

        if (mitadAncho <= 0f)
        {
            return;
        }

        limiteDerecho = Mathf.Max(0f, mitadAncho - radioObjeto);

        if (jugador != null && jugador.LimiteDerecho > 0f)
        {

            limiteDerecho = Mathf.Min(limiteDerecho, jugador.LimiteDerecho + toleranciaAlcance);
        }

        limiteIzquierdo = -limiteDerecho;
    }

    void Update()
    {
        if (Screen.width != ultimaPantalla.x || Screen.height != ultimaPantalla.y)
        {
            CalcularLimites();
        }

        tiempoSiguiente -= Time.deltaTime;

        if (tiempoSiguiente <= 0f)
        {
            CrearGrupoObjetos();
            CalcularSiguienteAparicion();
        }
    }

    void CrearObjetoEnPosicion(float posicionX)
    {
        Vector3 posicion = new Vector3(
            posicionX,
            transform.position.y,
            0f
        );

        GameObject objetoElegido;

        int tipoAleatorio = Random.Range(0, 2);

        if (tipoAleatorio == 0)
        {
            int indice = Random.Range(
                0,
                objetosCorrectos.Length
            );

            objetoElegido = objetosCorrectos[indice];
        }
        else
        {
            int indice = Random.Range(
                0,
                objetosIncorrectos.Length
            );

            objetoElegido = objetosIncorrectos[indice];
        }

        Instantiate(
            objetoElegido,
            posicion,
            Quaternion.identity
        );
    }

    void CrearGrupoObjetos()
    {
        int cantidad = 1;

        if (permitirMultiples)
        {
            float probabilidad = Random.value;

            if (probabilidad < 0.10f)
            {
                cantidad = 3;
            }

            else if (probabilidad < 0.40f)
            {
                cantidad = 2;
            }
        }

        if (cantidad == 1)
        {
            float posicionX = Random.Range(
                limiteIzquierdo,
                limiteDerecho
            );

            CrearObjetoEnPosicion(posicionX);
        }

        else if (cantidad == 2)
        {
            float mitad = (
                limiteIzquierdo +
                limiteDerecho
            ) / 2f;

            float posicion1 = Random.Range(
                limiteIzquierdo,
                mitad - 0.5f
            );

            float posicion2 = Random.Range(
                mitad + 0.5f,
                limiteDerecho
            );

            CrearObjetoEnPosicion(posicion1);
            CrearObjetoEnPosicion(posicion2);
        }

        else if (cantidad == 3)
        {
            float ancho =
                (limiteDerecho - limiteIzquierdo) / 3f;

            float posicion1 = Random.Range(
                limiteIzquierdo,
                limiteIzquierdo + ancho
            );

            float posicion2 = Random.Range(
                limiteIzquierdo + ancho,
                limiteIzquierdo + ancho * 2f
            );

            float posicion3 = Random.Range(
                limiteIzquierdo + ancho * 2f,
                limiteDerecho
            );

            CrearObjetoEnPosicion(posicion1);
            CrearObjetoEnPosicion(posicion2);
            CrearObjetoEnPosicion(posicion3);
        }
    }

    void CalcularSiguienteAparicion()
    {
        tiempoSiguiente = Random.Range(
            tiempoMinimo,
            tiempoMaximo
        );
    }
}
