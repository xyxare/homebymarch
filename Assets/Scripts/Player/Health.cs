using UnityEngine;

namespace HomeByMarch
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private FloatEventChannel playerHealthChannel;
        [SerializeField] private GameObject deathPanel; // Reference to the death panel
        [SerializeField] private BloodParticle bloodParticle; // Reference to the BloodParticle scriptable object

        public delegate void DamageTaken();
        public event DamageTaken OnDamageTaken;

        public PlayerData playerData;

        public float currentHealth;

        public float CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = value;
        }
        public float MaxHealth => playerData.currentHealth;
        public bool IsDead => currentHealth <= 0;

        void Awake()
        {
            currentHealth = MaxHealth;
        }

        void Start()
        {
            currentHealth = MaxHealth; // Set currentHealth to MaxHealth on start
            PublishHealthPercentage();
        }

        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            // Reduce damage based on player's defense percentage
            float reducedDamage = damage * (1 - playerData.defense / 100f);
            currentHealth = Mathf.Clamp(currentHealth - reducedDamage, 0, MaxHealth);
            PublishHealthPercentage();
            OnDamageTaken?.Invoke();

            // Activate BloodParticle effect
            ActivateBloodParticle();

            if (IsDead)
            {
                Debug.Log("Player is dead!");
                ShowDeathPanel(); // Show the death panel when the player dies
            }
        }

        public void Heal(int amount)
        {
            if (IsDead) return;

            currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
            PublishHealthPercentage();
        }

        private void PublishHealthPercentage()
        {
            if (playerHealthChannel != null)
            {
                playerHealthChannel.Invoke(currentHealth / (float)MaxHealth);
            }
            else
            {
                Debug.LogWarning("PlayerHealthChannel is not assigned.");
            }
        }

        // Activate the BloodParticle effect
        private void ActivateBloodParticle()
        {
            if (bloodParticle != null)
            {
                bloodParticle.CastSpell(transform); // Cast the BloodParticle spell
            }
            else
            {
                Debug.LogWarning("BloodParticle is not assigned.");
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
