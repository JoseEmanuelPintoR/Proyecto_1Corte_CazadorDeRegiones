using UnityEngine;
using System.Collections;

public class ObstaculoHojas : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panelHojas;

    [Header("Tiempos")]
    [SerializeField] private float tiempoMinimo = 6f;
    [SerializeField] private float tiempoMaximo = 10f;
    [SerializeField] private float duracion = 2f;

    void Start()
    {
        StartCoroutine(CicloHojas());
    }

    private IEnumerator CicloHojas()
    {
        while (true)
        {
            float espera = Random.Range(
                tiempoMinimo,
                tiempoMaximo
            );

            yield return new WaitForSeconds(espera);

            panelHojas.SetActive(true);

            yield return new WaitForSeconds(duracion);

            panelHojas.SetActive(false);
        }
    }
}