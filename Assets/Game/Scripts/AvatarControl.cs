using UnityEngine;
using Photon.Pun;

public class AvatarBtnCtrl : MonoBehaviour
{
    private int indice;
    public GameObject[] ListaAvatars;

    void Start()
    {
        // Cargar selecci�n previa (si existe)
        indice = PlayerPrefs.GetInt("PersonajeSeleccionado", 0);
        ListaAvatars[indice].SetActive(true);
    }

    public void CambiarIzquierda()
    {
        ListaAvatars[indice].SetActive(false);
        indice = (indice - 1 + ListaAvatars.Length) % ListaAvatars.Length;
        ListaAvatars[indice].SetActive(true);
    }

    public void CambiarDerecha()
    {
        ListaAvatars[indice].SetActive(false);
        indice = (indice + 1) % ListaAvatars.Length;
        ListaAvatars[indice].SetActive(true);
    }

    public void ConfirmarAvatar()
    {
        PlayerPrefs.SetInt("PersonajeSeleccionado", indice);
        // Guardar tambi�n en las propiedades de Photon para sincronizaci�n
        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new ExitGames.Client.Photon.Hashtable { { "AvatarIndex", indice } }
        );
        Debug.Log($"Avatar seleccionado: {indice} (Sincronizado en Photon)");
    }
}