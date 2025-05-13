using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isSecondaryPlayer;
    [SerializeField] private float speed = 5f;

    private CharacterController controller;
    private Vector3 moveDirection;

    private InputHandler inputHandler;
    [SerializeField] private int health = 100;
    [SerializeField] private Slider healthBar;
    public int CurrentHealth;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float gravity = -9.81f;
    private float yVelocity;
    [SerializeField] private Transform groundCheck;
    [Space(5f)]
    [SerializeField] private Animator animator;
    [SerializeField] private string idleAnimationName, walkAnimationName, attackAnimationName, dieAnimationName, hitAnimationName;
    private int idleAnimationHash, walkAnimationHash, attackAnimationHash, dieAnimationHash, hitAnimationHash;
    [Space(5f)]
    [SerializeField] private GameObject attackHitBox;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] public AudioSource attackSound; // Reference to the AudioSource component for playing sound effects
    private float nextAttackTime = 0f;
    private bool canAttack = true;
    private bool isAttacking = false;

    [Header("Knockback")]
    private Vector3 knockbackVelocity;
    [SerializeField] private float knockbackStrength = 10f; // Higher value for more sudden knockback
    [SerializeField] private float knockbackDuration = 0.5f; // Duration of knockback in seconds
    private float knockbackTime = 0f; // Tracks knockback time
    private bool isInvincible = false;
    [SerializeField] private GameObject blood;
    public string winnerText = "Player 1"; // Default winner text
    public GamePlayManager gamePlayManager; // Reference to the GamePlayManager script

    void Start()
    {
        inputHandler = FindAnyObjectByType<InputHandler>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        idleAnimationHash = Animator.StringToHash(idleAnimationName);
        walkAnimationHash = Animator.StringToHash(walkAnimationName);
        attackAnimationHash = Animator.StringToHash(attackAnimationName);
        dieAnimationHash = Animator.StringToHash(dieAnimationName);
        hitAnimationHash = Animator.StringToHash(hitAnimationName);
        CurrentHealth = health;
        healthBar.maxValue = health;
        healthBar.value = CurrentHealth;
        if (isSecondaryPlayer)
        {
            inputHandler.OnAttackPressed_2 += Attack;
        }
        else
        {
            inputHandler.OnAttackPressed_1 += Attack;
        }
    }

    private void Update()
    {
        if (knockbackTime > 0f) // If knockback is active, disable controls
        {
            knockbackTime -= Time.deltaTime;
            controller.Move(knockbackVelocity * Time.deltaTime); // Apply knockback force

            animator.Play(hitAnimationHash); // Play hit animation during knockback
            return; // Skip other movement and attack logic
        }

        if (controller.isGrounded)
        {
            yVelocity = -2f; // Slight downward force to keep the character grounded
        }

        if (nextAttackTime <= 0)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
            nextAttackTime -= Time.deltaTime;
        }

        yVelocity += gravity * Time.deltaTime;
        controller.Move(Time.deltaTime * yVelocity * Vector3.up);

        if (isAttacking)
        {
            return; // Don't allow movement during attack
        }

        // Input handling for movement
        if (isSecondaryPlayer)
        {
            moveDirection = new Vector3(inputHandler.PlayerMovementInput_2.x, 0, inputHandler.PlayerMovementInput_2.y).normalized;
        }
        else
        {
            moveDirection = new Vector3(inputHandler.PlayerMovementInput_1.x, 0, inputHandler.PlayerMovementInput_1.y).normalized;
        }

        if (moveDirection.sqrMagnitude > 0.1f)
        {
            // Apply movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720 * Time.deltaTime);

            // Apply normal movement
            controller.Move(Time.deltaTime * moveDirection * speed);

            animator.Play(walkAnimationHash);
        }
        else
        {
            animator.Play(idleAnimationHash);
        }

        if (CurrentHealth <= 0)
        {
            animator.Play(dieAnimationHash);
            gamePlayManager.GameOver(winnerText);
            enabled = false; // Disable the script when the player dies
        }
    }

    public void AttackFinished()
    {
        isAttacking = false;
        animator.Play(idleAnimationHash);
    }

    void Attack()
    {
        if (canAttack)
        {
            isAttacking = true;
            animator.Play(attackAnimationHash);
            nextAttackTime = attackCooldown;

            // Start the attack hit detection after attack animation duration

        }
    }

    public void Damage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackHitBox.transform.position, attackHitBox.transform.localScale.x / 2, enemyLayerMask);
        foreach (var hit in hitEnemies)
        {
            if (hit.TryGetComponent<PlayerController>(out PlayerController enemy))
            {
                if (enemy == this || enemy.isInvincible) continue; // Ignore self or invincible enemies

                Debug.Log("Hit " + enemy.gameObject.name);
                enemy.CurrentHealth -= attackDamage;
                enemy.healthBar.value = enemy.CurrentHealth;
                enemy.attackSound.Play();
                enemy.SpawnBlood(enemy.transform);
                Vector3 direction = (enemy.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(direction * knockbackStrength);
            }
        }
    }

    public void SpawnBlood(Transform transform)
    {
        Instantiate(blood, transform.position, Quaternion.identity);
    }
    public void ApplyKnockback(Vector3 force)
    {
        knockbackVelocity = force;
        knockbackTime = knockbackDuration;
        isInvincible = true;
        Invoke(nameof(ResetInvincibility), knockbackDuration); // Automatically remove invincibility
    }
    private void ResetInvincibility()
    {
        isInvincible = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackHitBox.transform.position, attackHitBox.transform.localScale.x / 2);
    }
}
