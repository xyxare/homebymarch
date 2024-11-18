using UnityEngine;

namespace HomeByMarch
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private FloatEventChannel playerHealthChannel;

        public delegate void DamageTaken();
        public event DamageTaken OnDamageTaken;

        private int currentHealth;

        public int CurrentHealth => currentHealth;
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
    }
}
