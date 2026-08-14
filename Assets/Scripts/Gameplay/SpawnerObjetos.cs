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
    [SerializeField] private float limiteIzquierdo = -4f;
    [SerializeField] private float limiteDerecho = 4f;

    [Header("Cantidad simultanea")]
    [SerializeField] private bool permitirMultiples = false;

    private float tiempoSiguiente;

    void Start()
    {
        CalcularSiguienteAparicion();
    }

    void Update()
    {
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

            // 10% de probabilidad de 3 objetos
            if (probabilidad < 0.10f)
            {
                cantidad = 3;
            }
            // 30% de probabilidad de 2 objetos
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