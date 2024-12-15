using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;

namespace HomeByMarch
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity
    {

        private DungeonGameController gameController;
        float currentHealth;
        [SerializeField] public int maxHealth;
        [SerializeField] PlayerDetector playerDetector;
        [SerializeField] private UnityEngine.AI.NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] float attackDelay = 0.8f;
        [SerializeField] float OnHitDelay = 0.5f; // Delay before the damage is applied
        bool isHit;
        [SerializeField] float wanderRadius = 10f;
        [SerializeField] private GameObject healthBarPrefab;

        [SerializeField] public float attackDistance = 30f;
        [SerializeField] public int attackDamage = 10;
        public LayerMask attackLayer;

        private StateMachine stateMachine;
        public Transform Player { get; private set; }
        public Health PlayerHealth { get; private set; }
        CountdownTimer attackTimer;
        CountdownTimer onHitTimer;

        public Image healthBar;

        void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            PlayerHealth = Player.GetComponent<Health>(); // Correctly reference the player's health
           
            currentHealth = maxHealth;
        }

        void Start()
        {

            gameController = FindObjectOfType<DungeonGameController>();
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
            if (currentHealth < maxHealth || playerDetector.CanDetectPlayer())
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

            healthBar.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0.0f, 1.0f);
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if (attackTimer.IsRunning) return;

            attackTimer.Start();
            agent.isStopped = true; // Stop movement during the attack
            StartCoroutine(DelayedAttack()); // Start the delayed attack coroutine
        }

        private IEnumerator DelayedAttack()
        {
            yield return new WaitForSeconds(attackDelay); // Wait for the specified delay
            if (playerDetector.CanAttackPlayer()) // Ensure player is still in range
            {
                AttackRayCast(); // Call the attack raycast method
            }
            agent.isStopped = false; // Resume movement after the attack
        }

        public void OnHit()
        {
           
            onHitTimer.Start(); // Start the on-hit timer
            StartCoroutine(OnHitDelayed()); // Start the delayed hit coroutine
        }

        private IEnumerator OnHitDelayed()
        {
            yield return new WaitForSeconds(OnHitDelay);
            isHit = false; // Reset the isHit flag after the delay
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0.0f, maxHealth);

            if (currentHealth <= 0)
            {

                if (gameController != null)
                {
                    gameController.OnEnemyDefeated();  // Notify the DungeonGameController that the enemy is defeated
                }
                stateMachine.SetState(new EnemyDeathState(this, animator, agent)); // Set the death state
            }
            else
            {
                OnHit(); // Call OnHit method to trigger the hit state
            }
        }

        void AttackRayCast()
        {
            Debug.Log("AttackRayCast initiated");

            // Adjust the ray origin to slightly above the enemy to avoid collision with the ground
            Vector3 rayOrigin = transform.position + Vector3.up * 1f; // Adjust for height if necessary

            // Define ray directions (straight, diagonals, etc.)
            Vector3[] rayDirections = new Vector3[10]
            {
        transform.forward,                      // Straight ahead
        transform.forward + transform.right,     // Slightly to the right
        transform.forward - transform.right,     // Slightly to the left
        transform.forward + transform.up,        // Slightly upwards
        transform.forward - transform.up,        // Slightly downwards
        transform.forward + transform.right + transform.up, // Diagonal up-right
        transform.forward + transform.right - transform.up, // Diagonal down-right
        transform.forward - transform.right + transform.up, // Diagonal up-left
        transform.forward - transform.right - transform.up, // Diagonal down-left
        transform.forward + transform.up * 2     // More upwards (for vertical range)
            };

            // Perform the raycasts and log the result
            foreach (var direction in rayDirections)
            {
                // Calculate the end point of the ray by adding the direction * distance to the ray origin
                Vector3 rayEnd = rayOrigin + direction * attackDistance;

                // Visualize the ray using Debug.DrawLine
                Debug.DrawLine(rayOrigin, rayEnd, Color.red, 0.1f);

                if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, attackDistance))
                {
                    Debug.Log($"Hit object: {hit.transform.name} at position: {hit.point}");

                    // Check if the hit object has the "Player" tag
                    if (hit.transform.CompareTag("Player"))
                    {
                        Debug.Log("Player hit! Applying damage.");
                        PlayerHealth.TakeDamage(attackDamage);  // Apply damage to the player
                    }
                    else
                    {
                        Debug.LogWarning("Hit object is not the player.");
                    }
                }
                else
                {
                    Debug.LogWarning("Raycast did not hit any objects.");
                }
            }
        }


    }
}
