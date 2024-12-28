using UnityEngine;
using TMPro;

public class HealthTextUpdater : MonoBehaviour
{
    public PlayerData playerData;
    public TextMeshProUGUI healthTextMeshPro;
    public TextMeshProUGUI attackTextMeshPro;
    public TextMeshProUGUI defenseTextMeshPro;
    public TextMeshProUGUI cooldownTextMeshPro;
    public TextMeshProUGUI movementSpeedTextMeshPro;

    void Start()
    {
        if (playerData == null || healthTextMeshPro == null || attackTextMeshPro == null || defenseTextMeshPro == null || cooldownTextMeshPro == null || movementSpeedTextMeshPro == null)
        {
            Debug.LogError("PlayerData or TextMeshProUGUI references are not assigned.");
            return;
        }
        UpdateHealthText();
        UpdateAttackText();
        UpdateDefenseText();
        UpdateCooldownText();
        UpdateMovementSpeedText();
    }

    public void UpdateHealthText()
    {
        healthTextMeshPro.text = "Health: " + playerData.currentHealth.ToString();
    }

    public void UpdateAttackText()
    {
        attackTextMeshPro.text = "Attack: " + playerData.currentAttack.ToString();
    }

    public void UpdateDefenseText()
    {
        defenseTextMeshPro.text = "Defense: " + playerData.currentDefense.ToString();
    }

    public void UpdateCooldownText()
    {
        cooldownTextMeshPro.text = "Cooldown: " + playerData.cooldown.ToString()+"%";
    }

    public void UpdateMovementSpeedText()
    {
        movementSpeedTextMeshPro.text = "Movement Speed: " + playerData.currentMovementSpeed.ToString();
    }

    void Update()
    {
        // Optionally, update the text every frame
        UpdateHealthText();
        UpdateAttackText();
        UpdateDefenseText();
        UpdateCooldownText();
        UpdateMovementSpeedText();
    }
}