using UnityEngine;

public class SpawnerObjetos : MonoBehaviour
{
    [Header("Objetos")]
    [SerializeField] private GameObject objetoCorrectoPrefab;
    [SerializeField] private GameObject objetoIncorrectoPrefab;

    [Header("Tiempo de aparicion")]
    [SerializeField] private float tiempoMinimo = 1f;
    [SerializeField] private float tiempoMaximo = 2f;

    [Header("Limites horizontales")]
    [SerializeField] private float limiteIzquierdo = -4f;
    [SerializeField] private float limiteDerecho = 4f;

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
            CrearObjeto();
            CalcularSiguienteAparicion();
        }
    }

    void CrearObjeto()
    {
        float posicionX = Random.Range(
            limiteIzquierdo,
            limiteDerecho
        );

        Vector3 posicion = new Vector3(
            posicionX,
            transform.position.y,
            0f
        );

        GameObject objetoElegido;

        int tipoAleatorio = Random.Range(0, 2);

        if (tipoAleatorio == 0)
        {
            objetoElegido = objetoCorrectoPrefab;
        }
        else
        {
            objetoElegido = objetoIncorrectoPrefab;
        }

        Instantiate(
            objetoElegido,
            posicion,
            Quaternion.identity
        );
    }

    void CalcularSiguienteAparicion()
    {
        tiempoSiguiente = Random.Range(
            tiempoMinimo,
            tiempoMaximo
        );
    }
}