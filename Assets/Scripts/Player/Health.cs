using UnityEngine;

namespace HomeByMarch
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private FloatEventChannel playerHealthChannel;
        [SerializeField] private GameObject deathPanel; // Reference to the death panel



        public delegate void DamageTaken();
        public event DamageTaken OnDamageTaken;

        public int currentHealth;

        public int CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = value;
        }
        public int MaxHealth => maxHealth;
        public bool IsDead => currentHealth <= 0;

        void Awake()
        {
            currentHealth = maxHealth;
        }

        void Start()
        {
            PublishHealthPercentage();
        }

        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            PublishHealthPercentage();
            OnDamageTaken?.Invoke();

            if (IsDead)
            {
                Debug.Log("Player is dead!");
                ShowDeathPanel(); // Show the death panel when the player dies
            }
        }

        public void Heal(int amount)
        {
            if (IsDead) return;

            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            PublishHealthPercentage();
        }

        private void PublishHealthPercentage()
        {
            if (playerHealthChannel != null)
            {
                playerHealthChannel.Invoke(currentHealth / (float)maxHealth);
            }
            else
            {
                Debug.LogWarning("PlayerHealthChannel is not assigned.");
            }
        }

        // Method to show the death panel
        public void ShowDeathPanel()
        {
            if (deathPanel != null)
            {
                deathPanel.SetActive(true); // Show the death panel
            }
            else
            {
                Debug.LogWarning("Death panel is not assigned.");
            }
        }

        // Optional method to hide the death panel if you need to reset or respawn
        public void HideDeathPanel()
        {
            if (deathPanel != null)
            {
                deathPanel.SetActive(false); // Hide the death panel
            }
        }
    }
}
