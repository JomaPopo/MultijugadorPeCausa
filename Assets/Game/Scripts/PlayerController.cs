using Cinemachine;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private TMP_Text playerNameText; // Asignar en el inspector

    private CharacterController controller;
    private PhotonView pv;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pv = GetComponent<PhotonView>();

        // Configurar el nombre y visibilidad del texto
        if (playerNameText != null)
        {
            playerNameText.text = pv.Owner.NickName;
            playerNameText.gameObject.SetActive(!pv.IsMine); // Ocultar solo para el jugador local
        }

        if (!pv.IsMine)
        {
            Destroy(GetComponentInChildren<CinemachineVirtualCamera>().gameObject);
            Destroy(controller); // Mejor que Destroy(GetComponent<CharacterController>())
        }
        else
        {
            Cursor.visible = false;
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (!pv.IsMine) return;

        // Lógica de movimiento (tu código actual)
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = InputManager.Instance.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (InputManager.Instance.playerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}