using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InstruccionesController : MonoBehaviour
{

    [System.Serializable]
    public class RegionInstrucciones
    {
        public string clave;
        public string nombreMostrado;
        public string escenaNivel;

        public Sprite fondo;
        public Sprite cuadro;

        public Sprite recoge;
        public Sprite evita;
        public Sprite lineaMedio;
        public Sprite botonMenu;
        public Sprite botonEmpezar;
        public Sprite flecha;

        public Sprite[] iconosRecoge;
        public string[] nombresRecoge;

        public Sprite[] iconosEvita;
        public string[] nombresEvita;
    }

    public const string ClaveRegion = "RegionSeleccionada";

    [SerializeField] private RegionInstrucciones[] regiones;

    [Header("Pantalla")]
    [SerializeField] private Image fondo;
    [SerializeField] private Image cuadro;
    [SerializeField] private TMP_Text textoNivel;
    [SerializeField] private TMP_Text textoRegion;

    [Header("Arte que tambien cambia por region")]
    [SerializeField] private Image imagenRecoge;
    [SerializeField] private Image imagenEvita;
    [SerializeField] private Image imagenLinea;
    [SerializeField] private Image imagenBotonMenu;
    [SerializeField] private Image imagenBotonEmpezar;
    [SerializeField] private Image imagenFlechaRegresar;
    [SerializeField] private Image imagenFlechaAnterior;
    [SerializeField] private Image imagenFlechaSiguiente;

    [Header("Recoge")]
    [SerializeField] private Image[] iconosRecoge;
    [SerializeField] private TMP_Text[] textosRecoge;

    [Header("Evita")]
    [SerializeField] private Image[] iconosEvita;
    [SerializeField] private TMP_Text[] textosEvita;

    private int indice;

    void Start()
    {
        if (regiones == null || regiones.Length == 0)
        {
            Debug.LogWarning("Instrucciones sin regiones configuradas.");
            return;
        }

        indice = Mathf.Clamp(PlayerPrefs.GetInt(ClaveRegion, 0), 0, regiones.Length - 1);
        Mostrar();
    }

    public void Anterior()
    {
        Mover(-1);
    }

    public void Siguiente()
    {
        Mover(1);
    }

    private void Mover(int paso)
    {
        if (regiones == null || regiones.Length == 0)
            return;

        indice = (indice + paso + regiones.Length) % regiones.Length;
        Mostrar();
    }

    private void Mostrar()
    {
        RegionInstrucciones region = regiones[indice];

        PlayerPrefs.SetInt(ClaveRegion, indice);

        Poner(fondo, region.fondo);
        Poner(cuadro, region.cuadro);

        Poner(imagenRecoge, region.recoge);
        Poner(imagenEvita, region.evita);
        Poner(imagenLinea, region.lineaMedio);
        Poner(imagenBotonMenu, region.botonMenu);
        Poner(imagenBotonEmpezar, region.botonEmpezar);

        Poner(imagenFlechaRegresar, region.flecha);
        Poner(imagenFlechaAnterior, region.flecha);
        Poner(imagenFlechaSiguiente, region.flecha);

        if (textoNivel != null)
        {
            textoNivel.text = "Nivel " + (indice + 1) + " de " + regiones.Length;
        }

        if (textoRegion != null)
        {
            textoRegion.text = region.nombreMostrado;
        }

        LlenarLista(iconosRecoge, textosRecoge, region.iconosRecoge, region.nombresRecoge);
        LlenarLista(iconosEvita, textosEvita, region.iconosEvita, region.nombresEvita);
    }

    private static void Poner(Image destino, Sprite sprite)
    {
        if (destino == null || sprite == null)
            return;

        destino.sprite = sprite;
        destino.enabled = true;
    }

    private static void LlenarLista(Image[] iconos, TMP_Text[] textos, Sprite[] sprites, string[] nombres)
    {
        if (iconos == null || textos == null)
            return;

        for (int i = 0; i < iconos.Length; i++)
        {
            bool hay = sprites != null && i < sprites.Length && nombres != null && i < nombres.Length;

            if (iconos[i] != null)
            {
                iconos[i].sprite = hay ? sprites[i] : null;
                iconos[i].enabled = hay;
            }

            if (i < textos.Length && textos[i] != null)
            {
                textos[i].text = hay ? nombres[i] : "";
            }
        }
    }

    public void Empezar()
    {
        if (regiones == null || regiones.Length == 0)
            return;

        SceneManager.LoadScene(regiones[indice].escenaNivel);
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void Regresar()
    {
        SceneManager.LoadScene("SeleccionNiveles");
    }
}
