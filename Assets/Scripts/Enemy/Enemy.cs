using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using System.Collections;

namespace HomeByMarch
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity
    {

        int currentHealth;
        [SerializeField] public int maxHealth;
        [SerializeField] PlayerDetector playerDetector;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] float attackDelay = 0.8f; 
        [SerializeField] float OnHitDelay = 0.5f; // Delay before the damage is applied
        bool isHit;
        [SerializeField] float wanderRadius = 10f;
        [SerializeField] private GameObject healthBarPrefab;

        private StateMachine stateMachine;
        public Transform Player { get; private set; }
        public Health PlayerHealth { get; private set; }

        CountdownTimer attackTimer;
        CountdownTimer onHitTimer;

        void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            PlayerHealth = Player.GetComponent<Health>(); // Correctly reference the player's health
            currentHealth = maxHealth;
        }

        void Start()
        {
            attackTimer = new CountdownTimer(timeBetweenAttack);
            onHitTimer = new CountdownTimer(OnHitDelay); // Initialize on-hit timer

            stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            var deathState = new EnemyDeathState(this, animator, agent);
            var onHitState = new EnemyOnHitState(this, animator, agent); // Add on-hit state

            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            At(attackState, onHitState, new FuncPredicate(() => isHit));
            At(onHitState, attackState, new FuncPredicate(() => !isHit && !onHitTimer.IsRunning)); // Transition back to attack state if not hit and on-hit timer is not running
            Any(deathState, new FuncPredicate(() => currentHealth <= 0));

            stateMachine.SetState(wanderState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Update()
        {
            if (playerDetector.CanDetectPlayer())
            {
                healthBarPrefab.SetActive(true);
            }
            else
            {
                healthBarPrefab.SetActive(false);
            }
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
            onHitTimer.Tick(Time.deltaTime); // Tick on-hit timer
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if (attackTimer.IsRunning) return;

            attackTimer.Start();
            StartCoroutine(DelayedAttack()); // Start the delayed attack coroutine
        }

        public void OnHit()
        {
            if (onHitTimer.IsRunning) return;

            isHit = true; // Set isHit to true when the enemy is hit
            onHitTimer.Start(); // Start the on-hit timer
            StartCoroutine(OnHitDelayed()); // Start the delayed hit coroutine
        }

        private IEnumerator OnHitDelayed()
        {
            yield return new WaitForSeconds(OnHitDelay);
            isHit = false; // Reset the isHit flag after the delay
        }

        private IEnumerator DelayedAttack()
        {
            yield return new WaitForSeconds(attackDelay); // Wait for the specified delay
            if (playerDetector.CanAttackPlayer()) // Ensure player is still in range
            {
                PlayerHealth.TakeDamage(10); // Apply damage after delay
            }
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                stateMachine.SetState(new EnemyDeathState(this, animator, agent)); // Set the death state
            }
            else
            {
                OnHit(); // Call OnHit method to trigger the hit state
            }
        }

        void Death()
        {
            // This is now managed by the EnemyDeathState, so this method can be left empty
        }
    }
}
