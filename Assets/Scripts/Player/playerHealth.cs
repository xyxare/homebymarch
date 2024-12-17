using UnityEngine;
using UnityEngine.UI;

namespace HomeByMarch
{
    public class PlayerHealth : MonoBehaviour
    {
        public float health;
        public float maxHealth;
        public Image healthBar;

        void Start()
        {
            health = maxHealth;
        }

        void Update()
        {
            // Update the health bar based on the current health
            healthBar.fillAmount = Mathf.Clamp(health / maxHealth, 0.0f, 1.0f);
            Debug.Log("Current Health: " + health);
        }

        // Method to take damage
        public void TakeDamage(float damageAmount)
        {
            health -= damageAmount;

            // Ensure health doesn't drop below zero
            health = Mathf.Clamp(health, 0.0f, maxHealth);

            // Check if the player is dead
            if (health <= 0)
            {
               Debug.Log("enemy died");
            }
        }

        // Method to handle player's death
        private void Die()
        {
            Debug.Log("Player has died.");
            // Additional logic for when the player dies (e.g., respawn, game over screen)
        }
    }
}
