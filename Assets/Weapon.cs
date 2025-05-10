using UnityEngine;
using Photon.Pun;


public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int damage = 10;
    public float fireRate = 1f;
    [SerializeField] private Camera playerCamera; // Serializado para asignación en Inspector

    [Header("VFX")]
    public GameObject muzzleVFX;
    public GameObject hitVFX;
    public GameObject bloodVFX;

    [Header("Muzzle Position")]
    [SerializeField] private Transform muzzlePosition; // Punto de origen de disparos

    private float nextFire;

    void Update()
    {
        // Control de cadencia de disparo
        if (nextFire > 0)
            nextFire -= Time.deltaTime;

        if (Input.GetButton("Fire1") && nextFire <= 0 && PhotonNetwork.IsConnected)
        {
            nextFire = 1 / fireRate;
            Fire();
        }
    }

    void Fire()
    {
        // Efecto de disparo desde la posición del cañón
        if (muzzleVFX != null && muzzlePosition != null)
        {
            PhotonNetwork.Instantiate(muzzleVFX.name,
                                    muzzlePosition.position,
                                    muzzlePosition.rotation);
        }

        // Lógica de raycast
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            SpawnImpactVFX(hit);
            ApplyDamage(hit);
        }
    }

    private void SpawnImpactVFX(RaycastHit hit)
    {
        Health targetHealth = hit.transform.GetComponent<Health>();
        bool isEnemy = targetHealth != null && targetHealth.isEnemy;

        GameObject impactVFX = isEnemy ? bloodVFX : hitVFX;

        if (impactVFX != null)
        {
            PhotonNetwork.Instantiate(impactVFX.name,
                                    hit.point,
                                    Quaternion.LookRotation(hit.normal));
        }
    }

    private void ApplyDamage(RaycastHit hit)
    {
        PhotonView targetPhotonView = hit.transform.GetComponent<PhotonView>();
        Health targetHealth = hit.transform.GetComponent<Health>();

        if (targetPhotonView != null && targetHealth != null)
        {
            targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
        }
    }
}