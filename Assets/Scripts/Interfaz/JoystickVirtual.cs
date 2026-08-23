using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickVirtual : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private PlayerController jugador;
    [SerializeField] private RectTransform mango;

    [Header("Ajustes")]
    [SerializeField] private float radio = 110f;
    [SerializeField] private float zonaMuerta = 0.25f;

    private RectTransform baseJoystick;
    private Canvas lienzo;

    void Awake()
    {
        baseJoystick = transform as RectTransform;
        lienzo = GetComponentInParent<Canvas>();
    }

    void OnDisable()
    {
        Soltar();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Arrastrar(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Arrastrar(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Soltar();
    }

    private void Arrastrar(PointerEventData eventData)
    {
        if (baseJoystick == null || radio <= 0f)
        {
            return;
        }

        bool encontrado = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            baseJoystick, eventData.position, CamaraDelLienzo(), out Vector2 local);

        if (!encontrado)
        {
            return;
        }

        Vector2 desplazamiento = Vector2.ClampMagnitude(local, radio);

        if (mango != null)
        {
            mango.anchoredPosition = desplazamiento;
        }

        Avisar(desplazamiento.x / radio);
    }

    private void Soltar()
    {
        if (mango != null)
        {
            mango.anchoredPosition = Vector2.zero;
        }

        Avisar(0f);
    }

    private Camera CamaraDelLienzo()
    {

        if (lienzo == null || lienzo.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return lienzo.worldCamera;
    }

    private void Avisar(float horizontal)
    {
        if (jugador == null)
        {
            return;
        }

        jugador.MoverIzquierda(horizontal < -zonaMuerta);
        jugador.MoverDerecha(horizontal > zonaMuerta);
    }
}
