using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionNivelesController : MonoBehaviour
{
    public void AbrirAndina()
    {
        SceneManager.LoadScene("Nivel1_Andina");
    }

    public void AbrirCaribe()
    {
        SceneManager.LoadScene("Nivel2_Caribe");
    }

    public void AbrirPacifica()
    {
        SceneManager.LoadScene("Nivel3_Pacifica");
    }

    public void AbrirOrinoquia()
    {
        SceneManager.LoadScene("Nivel4_Orinoquia");
    }

    public void AbrirAmazonia()
    {
        SceneManager.LoadScene("Nivel5_Amazonia");
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}