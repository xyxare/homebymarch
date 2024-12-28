using UnityEngine;
using TMPro;

public class HealthTextUpdater : MonoBehaviour
{
    public PlayerData playerData;
    public TextMeshProUGUI healthTextMeshPro;
    public TextMeshProUGUI attackTextMeshPro;
    public TextMeshProUGUI defenseTextMeshPro;

    void Start()
    {
        if (playerData == null || healthTextMeshPro == null || attackTextMeshPro == null || defenseTextMeshPro == null)
        {
            Debug.LogError("PlayerData or TextMeshProUGUI references are not assigned.");
            return;
        }
        UpdateHealthText();
        UpdateAttackText();
        UpdateDefenseText();
    }

    public void UpdateHealthText()
    {
        healthTextMeshPro.text = "Health: " + playerData.health.ToString();
    }

    public void UpdateAttackText()
    {
        attackTextMeshPro.text = "Attack: " + playerData.attack.ToString();
    }

    public void UpdateDefenseText()
    {
        defenseTextMeshPro.text = "Defense: " + playerData.defense.ToString();
    }

    void Update()
    {
        // Optionally, update the text every frame
        UpdateHealthText();
        UpdateAttackText();
        UpdateDefenseText();
    }
}