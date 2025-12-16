using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private int pointValue = 10;
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;

    [Header("Movement (Optional)")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveRange = 5f;
    [SerializeField] private bool randomMovement = false;

    private Vector3 startPosition;
    private float moveTimer;
    private Vector3 moveDirection;

    [Header("Effects")]
    [SerializeField] private GameObject destructionEffect;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip destroySound;

    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();

        if (randomMovement)
        {
            moveDirection = Random.insideUnitSphere;
            moveDirection.y = 0;
            moveDirection.Normalize();
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveTarget();
        }
    }

    void MoveTarget()
    {
        if (randomMovement)
        {
            // Random movement pattern
            moveTimer += Time.deltaTime;
            if (moveTimer >= 2f)
            {
                moveDirection = Random.insideUnitSphere;
                moveDirection.y = Mathf.Clamp(moveDirection.y, -0.5f, 0.5f);
                moveDirection.Normalize();
                moveTimer = 0;
            }

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Keep within range
            if (Vector3.Distance(transform.position, startPosition) > moveRange)
            {
                moveDirection = (startPosition - transform.position).normalized;
            }
        }
        else
        {
            // Simple back and forth movement
            float offset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
            transform.position = startPosition + transform.right * offset;
        }
    }

    public void TakeDamage()
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Play hit sound
            PlaySound(hitSound);
        }
    }

    void Die()
    {
        // Add points to game manager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(pointValue);
        }

        // Spawn floating points text
        if (floatingTextPrefab != null)
        {
            GameObject floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText textComponent = floatingText.GetComponent<FloatingText>();
            if (textComponent != null)
            {
                textComponent.SetText("+" + pointValue);
                textComponent.SetColor(Color.yellow);
            }
        }

        // Play destroy sound
        PlaySound(destroySound);

        // Spawn destruction effect
        if (destructionEffect != null)
        {
            GameObject effect = Instantiate(destructionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }

        // Destroy target
        Destroy(gameObject);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if hit by bullet projectile
        BulletProjectile bullet = collision.gameObject.GetComponent<BulletProjectile>();
        if (bullet != null)
        {
            TakeDamage();
        }
    }
}
