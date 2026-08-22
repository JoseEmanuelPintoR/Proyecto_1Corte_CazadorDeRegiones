using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionNivelesController : MonoBehaviour
{

    public void AbrirAndina()
    {
        AbrirInstrucciones(0);
    }

    public void AbrirCaribe()
    {
        AbrirInstrucciones(1);
    }

    public void AbrirPacifica()
    {
        AbrirInstrucciones(2);
    }

    public void AbrirOrinoquia()
    {
        AbrirInstrucciones(3);
    }

    public void AbrirAmazonia()
    {
        AbrirInstrucciones(4);
    }

    private void AbrirInstrucciones(int region)
    {
        PlayerPrefs.SetInt(InstruccionesController.ClaveRegion, region);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Instrucciones");
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
