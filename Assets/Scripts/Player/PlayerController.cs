using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;

namespace HomeByMarch
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        Rigidbody rb;
        [SerializeField] FixedJoystick joystick;
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] InputReader input;
        [SerializeField] EnemyDetector enemyDetector; // Reference to EnemyDetector

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;
        [SerializeField] bool useCharacterForward = false;

        [SerializeField] float turnSpeed = 10f;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;

        [Header("Dash Settings")]
        [SerializeField] float dashForce = 10f;
        [SerializeField] float dashDuration = 1f;
        [SerializeField] float dashCooldown = 2f;

        [Header("Attack Settings")]
        [SerializeField] public float attackCooldown = 0.5f;
        [SerializeField] public float attackDistance = 10f;
        [SerializeField] public int attackDamage = 10;
        [SerializeField] public int attackDelay = 5;
        [SerializeField] public int attackSpeed = 2;

        [SerializeField] SpellStrategy[] spells;
        public LayerMask attackLayer;

        bool attacking = false;
        bool readyToAttack = true;
        int attackCount;


        const float ZeroF = 0f;

        Transform mainCam;
        Vector2 inputs;
        float currentSpeed;
        float velocity;
        float turnSpeedMultiplier;
        float jumpVelocity;
        float dashVelocity = 1f;
        float speed;
        Camera mainCamera;
        Vector3 targetDirection;
        List<Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        CountdownTimer dashTimer;
        CountdownTimer dashCooldownTimer;
        CountdownTimer attackTimer;

        StateMachine stateMachine;

        // Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");

        Transform Enemy;
        Health EnemyHealth;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mainCamera = Camera.main;
            attackLayer = LayerMask.GetMask("Enemy");
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera is not assigned or could not be found.");
            }

            SetupTimers();
            SetupStateMachine();

            Enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
            EnemyHealth = Enemy.GetComponent<Health>();
            enemyDetector = GetComponent<EnemyDetector>(); // Initialize EnemyDetector
        }

        void SetupStateMachine()
        {
            // State Machine
            stateMachine = new StateMachine();

            // Declare states
            var locomotionState = new LocomotionState(this, animator);
            var attackState = new AttackState(this, animator);

            // Define transitions
            At(locomotionState, attackState, new FuncPredicate(() => attackTimer.IsRunning));
            At(attackState, locomotionState, new FuncPredicate(() => !attackTimer.IsRunning));
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));

            // Set initial state
            stateMachine.SetState(locomotionState);
        }

        bool ReturnToLocomotionState()
        {
            return !attackTimer.IsRunning;
        }

        void SetupTimers()
        {
            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);

            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () =>
            {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };

            attackTimer = new CountdownTimer(attackCooldown);

            timers = new(5) { jumpTimer, jumpCooldownTimer, dashTimer, dashCooldownTimer, attackTimer };
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Start() => input.EnablePlayerActions();

        void OnEnable()
        {
            input.Attack += OnAttack;
            // HeadsUpDisplay.OnButtonPressed += CastSpell;
        }

        void OnDisable()
        {
            input.Attack -= OnAttack;
            // HeadsUpDisplay.OnButtonPressed -= CastSpell;
        }

        void CastSpell(int index)
        {
            
            // spells[index].CastSpell(transform);
            Debug.Log("spellcasted");
       
        }

        void OnAttack()
        {
            if (!attackTimer.IsRunning)
            {
                attackTimer.Start();
                Attack(); // Call the Attack method when the attack is performed
            }
        }

        public void Attack()
        {

            if (!readyToAttack) return; // Only proceed if we are ready to attack

            // Start the attack timer and initiate the attack logic
            readyToAttack = false;
            attacking = true;

            // Call the raycast to check for enemies and deal damage
            AttackRayCast();

            // Reset the attack state after the attack cooldown
            Invoke(nameof(ResetAttack), attackCooldown);
            SFXManager.PlaySFX(SoundTypes.SwordSwing);
            // Invoke(nameof(ResetAttack), attackSpeed);
            // Invoke(nameof(AttackRayCast), attackDelay);
            // if (rb == null || animator == null || enemyDetector == null)
            // {
            //     Debug.LogError("Required components (Rigidbody, Animator, or EnemyDetector) are missing!");
            //     return;
            // }

            // // Check if an enemy is detected and within attack range
            // if (enemyDetector.CanDetectEnemy() && enemyDetector.CanAttackEnemy())
            // {
            //     Debug.Log($"Attacking enemy: {enemyDetector.Enemy.name}");

            //     // Apply damage to the enemy
            //     enemyDetector.EnemyHealth.TakeDamage(attackDamage);
            //     Debug.Log($"Enemy {enemyDetector.Enemy.name} hit and took {attackDamage} damage.");
            // }
            // else
            // {
            //     Debug.LogWarning("No enemy detected or enemy out of attack range!");
            // }
        }

        void ResetAttack()
        {
            readyToAttack = true;
            attacking = false;
        }
        void AttackRayCast()
        {
            if(!attacking) return; // Only proceed if we are attacking
            Debug.Log("AttackRayCast initiated");

            // Adjust the ray origin and direction
            Vector3 rayOrigin = transform.position + Vector3.up * 1f; // Adjust for the player's height if necessary
            Vector3 rayDirection = transform.forward;

            // Debug the raycast by drawing it in the scene view
            Vector3 endPoint = rayOrigin + rayDirection * attackDistance;
            Debug.DrawLine(rayOrigin, endPoint, Color.blue, 6f);

            // Perform the raycast
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, attackDistance, attackLayer))
            {
                Debug.Log($"Hit object: {hit.transform.name} at position: {hit.point}");
                
                // Hit an enemy, apply damage
                //change Actor to Health Component or any component that contains take damage
                if (hit.transform.TryGetComponent<Enemy>(out Enemy enemyComponent))
                {
                    // Apply damage to the enemy
                    enemyComponent.TakeDamage(attackDamage);
                    Debug.Log($"Enemy {enemyComponent.name} took {attackDamage} damage.");
                }
                else
                {
                    Debug.LogWarning("Hit object is not an enemy.");
                }
            }
            else
            {
                Debug.LogWarning("Raycast did not hit any objects.");
            }
            ResetAttack();
        }



        void HitTarget(Vector3 pos)
        {
            Debug.Log("Hit target");
            // audioSource.pitch = 1;
            // audioSource.PlayOneShot(hitSound);

            // GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
            // Destroy(GO, 20);
        }



        void Update()
        {
            stateMachine.Update();
            HandleTimers();
            // if (input.Attack.IsPressed()) { Attack(); }
        }

        void FixedUpdate()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            inputs.x = joystick.Horizontal;
            inputs.y = joystick.Vertical;

            stateMachine.FixedUpdate();
#else
            InputSystemHelper.EnableBackendsWarningMessage();
#endif
        }

        void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public void HandleMovement()
        {
            if (useCharacterForward)
            {
                speed = Mathf.Abs(inputs.x) + inputs.y;
            }
            else
            {
                speed = Mathf.Abs(inputs.x) + Mathf.Abs(inputs.y);
            }

            speed = Mathf.Clamp(speed, 0f, 1f);
            speed = Mathf.SmoothDamp(animator.GetFloat("Speed"), speed, ref velocity, 0.1f);
            animator.SetFloat("Speed", speed);

            UpdateTargetDirection();

            Vector3 moveDirection = new Vector3(inputs.x * moveSpeed, rb.velocity.y, inputs.y * moveSpeed);
            rb.velocity = moveDirection;

            if (inputs != Vector2.zero && targetDirection.magnitude > 0.1f)
            {
                HandleRotation();
            }
        }

        public void UpdateTargetDirection()
        {
            if (!useCharacterForward)
            {
                turnSpeedMultiplier = 1f;
                Vector3 forward = mainCamera.transform.TransformDirection(Vector3.forward);
                forward.y = 0;
                Vector3 right = mainCamera.transform.TransformDirection(Vector3.right);
                targetDirection = inputs.x * right + inputs.y * forward;
            }
            else
            {
                turnSpeedMultiplier = 0.2f;
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                forward.y = 0;
                Vector3 right = transform.TransformDirection(Vector3.right);
                targetDirection = inputs.x * right + Mathf.Abs(inputs.y) * forward;
            }
        }

        void HandleRotation()
        {
            Vector3 lookDirection = targetDirection.normalized;
            Quaternion freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
            float differenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
            float eulerY = transform.eulerAngles.y;

            if (differenceRotation != 0)
            {
                eulerY = freeRotation.eulerAngles.y;
            }

            Vector3 euler = new Vector3(0, eulerY, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * turnSpeedMultiplier * Time.deltaTime);
        }
    }
}

