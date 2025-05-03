using Photon.Pun;
using UnityEngine;
using System.IO;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    private int selectedAvatarIndex;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            // Obtener el índice del avatar seleccionado desde las propiedades del jugador
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("AvatarIndex"))
            {
                selectedAvatarIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["AvatarIndex"];
            }
            CreateController();
        }
    }

    private void CreateController()
    {
        string prefabName = selectedAvatarIndex == 0 ? "PlayerController" : "PlayerController_1";
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabName), Vector3.zero, Quaternion.identity);
    }
}