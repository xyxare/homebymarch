using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;
using Photon.Pun;
using System.Collections;


namespace HomeByMarch
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity
    {
        int currentHealth;
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
        public MaterialChanger materialChanger;
        CountdownTimer attackTimer;
        CountdownTimer onHitTimer;

        void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            PlayerHealth = Player.GetComponent<Health>(); // Correctly reference the player's health
            materialChanger = GetComponent<MaterialChanger>();
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
            if (materialChanger != null)
            {
                // Change material when hit
                materialChanger.ChangeMaterial();
            }
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

        // Raycast-based attack targeting the player
        // Raycast-based attack targeting the player
        void AttackRayCast()
        {
            Debug.Log("AttackRayCast initiated");

            // Adjust the ray origin and direction
            Vector3 rayOrigin = transform.position + Vector3.up * 1f; // Adjust for the player's height if necessary

            // Define ray directions (center, left, right, up, down, and diagonals)
            Vector3[] rayDirections = new Vector3[10]
            {
        transform.forward,                  // Straight ahead
        transform.forward + transform.right, // Slightly to the right
        transform.forward - transform.right, // Slightly to the left
        transform.forward + transform.up,    // Slightly upwards
        transform.forward - transform.up,    // Slightly downwards
        transform.forward + transform.right + transform.up, // Diagonal up-right
        transform.forward + transform.right - transform.up, // Diagonal down-right
        transform.forward - transform.right + transform.up, // Diagonal up-left
        transform.forward - transform.right - transform.up, // Diagonal down-left
        transform.forward + transform.up * 2, // More upwards
            };

            // Debug the raycasts by drawing them in the scene view
            foreach (var direction in rayDirections)
            {
                Vector3 endPoint = rayOrigin + direction * attackDistance;
                Debug.DrawLine(rayOrigin, endPoint, Color.blue, 6f);
            }

            // Perform the raycasts
            foreach (var direction in rayDirections)
            {
                if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, attackDistance))
                {
                    Debug.Log($"Hit object: {hit.transform.name} at position: {hit.point}");

                    // Check if the hit object has the "Player" tag
                    if (hit.transform.CompareTag("Player"))
                    {
                        // Apply damage to the player
                        PlayerHealth.TakeDamage(attackDamage);
                        Debug.Log($"Player took {attackDamage} damage.");
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
