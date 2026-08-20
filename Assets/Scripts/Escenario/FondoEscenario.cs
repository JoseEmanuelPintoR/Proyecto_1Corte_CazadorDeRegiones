using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class FondoEscenario : MonoBehaviour
{
    [Header("Camara")]
    [Tooltip("Si se deja vacio se usa Camera.main.")]
    public Camera camaraObjetivo;

    [Tooltip("Profundidad del fondo desde la camara. El Piso llega hasta z=10, asi que 22 lo deja bien detras.")]
    public float distancia = 22f;

    [Header("Encuadre")]
    [Tooltip("Zoom adicional sobre el ajuste 'cover'. 1 = justo lo necesario para cubrir la pantalla.")]
    public float zoomExtra = 1f;

    [Header("Linea de suelo del arte")]
    [Tooltip("Altura normalizada del suelo pintado dentro del PNG. 0 = borde inferior, 1 = borde superior. " +
             "Subirla acerca la camara al suelo; bajarla la aleja.")]
    [Range(0f, 1f)]
    public float lineaSueloNormalizada = 0.16f;

    [Tooltip("Altura del suelo del mundo donde el jugador apoya los pies.")]
    public float alturaSueloMundo = 0f;

    [Tooltip("Z del plano donde ocurre el juego (el jugador y los objetos).")]
    public float planoJugadorZ = 0f;

    [Header("Orden de dibujado")]
    public int ordenEnCapa = -100;

    private SpriteRenderer sprite;

    private void OnEnable()
    {
        sprite = GetComponent<SpriteRenderer>();
        Colocar();
    }

    private void LateUpdate()
    {
        Colocar();
    }

    public void Colocar()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        if (sprite == null || sprite.sprite == null)
        {
            return;
        }

        Camera camara = CamaraEnUso();

        if (camara == null)
        {
            return;
        }

        sprite.sortingOrder = ordenEnCapa;

        Transform camaraTransform = camara.transform;

        transform.rotation = camaraTransform.rotation;

        float alturaFrustum = AlturaFrustum(camara);
        float anchoFrustum = alturaFrustum * camara.aspect;

        Vector3 tamanoSprite = sprite.sprite.bounds.size;

        if (tamanoSprite.x <= 0f || tamanoSprite.y <= 0f)
        {
            return;
        }

        float escala = Mathf.Max(anchoFrustum / tamanoSprite.x, alturaFrustum / tamanoSprite.y) * zoomExtra;
        transform.localScale = new Vector3(escala, escala, 1f);

        float centroSpriteY = sprite.sprite.bounds.center.y * escala;
        float desplazamientoY = -centroSpriteY;
        float desplazamientoX = -sprite.sprite.bounds.center.x * escala;

        transform.position = camaraTransform.position
            + camaraTransform.forward * distancia
            + camaraTransform.up * desplazamientoY
            + camaraTransform.right * desplazamientoX;
    }

    public float AlturaCamaraParaLineaDeSuelo()
    {
        Camera camara = CamaraEnUso();

        if (camara == null)
        {
            return 0f;
        }

        float objetivoNormalizado = lineaSueloNormalizada * 2f - 1f;

        if (camara.orthographic)
        {
            return alturaSueloMundo - objetivoNormalizado * camara.orthographicSize;
        }

        float profundidad = planoJugadorZ - camara.transform.position.z;

        if (profundidad <= 0f)
        {
            return camara.transform.position.y;
        }

        float tangente = Mathf.Tan(camara.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return alturaSueloMundo - objetivoNormalizado * profundidad * tangente;
    }

    private Camera CamaraEnUso()
    {
        return camaraObjetivo != null ? camaraObjetivo : Camera.main;
    }

    private float AlturaFrustum(Camera camara)
    {
        if (camara.orthographic)
        {
            return camara.orthographicSize * 2f;
        }

        return 2f * distancia * Mathf.Tan(camara.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }
}
