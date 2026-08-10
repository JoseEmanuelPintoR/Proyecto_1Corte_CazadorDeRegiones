using UnityEngine;
using UnityEngine.EventSystems;

public class BotonMovimiento : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Direccion
    {
        Izquierda,
        Derecha
    }

    [SerializeField] private Direccion direccion;
    [SerializeField] private PlayerController jugador;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (direccion == Direccion.Izquierda)
        {
            jugador.MoverIzquierda(true);
        }
        else
        {
            jugador.MoverDerecha(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (direccion == Direccion.Izquierda)
        {
            jugador.MoverIzquierda(false);
        }
        else
        {
            jugador.MoverDerecha(false);
        }
    }
}