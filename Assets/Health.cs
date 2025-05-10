using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Health : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;
    public bool isEnemy = false;

    [Header("UI")]
    public TextMeshProUGUI healthText;
    public Image healthFillImage; // Cambiamos Slider por Image

    private void Start()
    {
        health = maxHealth;
        UpdateHealthUI();
    }

    [PunRPC]
    public void TakeDamage(int _damage)
    {
        health -= _damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = health.ToString();

        if (healthFillImage != null)
        {
            // Configuramos el fill amount basado en la salud actual
            healthFillImage.fillAmount = (float)health / maxHealth;
        }
    }

    // Método opcional para curar
    [PunRPC]
    public void Heal(int _amount)
    {
        health += _amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }
}