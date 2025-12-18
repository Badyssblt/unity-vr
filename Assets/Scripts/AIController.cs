using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthSystem))]
public class AIController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private AIState startingState = AIState.Patrol;
    private AIState currentState;

    public enum AIState
    {
        Idle,       // Ne fait rien
        Patrol,     // Patrouille
        Chase,      // Poursuit le joueur
        Attack,     // Attaque le joueur
        Dead        // Mort
    }

    [Header("Detection Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float fieldOfView = 90f; // Angle de vision
    [SerializeField] private LayerMask obstacleMask; // Pour vérifier si un obstacle bloque la vue
    [SerializeField] private bool autoFindPlayer = true;

    [Header("Movement Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float patrolSpeed = 2f;
    private float patrolTimer;
    private Vector3 patrolTarget;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool showDebugLogs = false;

    // Composants
    private NavMeshAgent agent;
    private HealthSystem healthSystem;
    private AIWeaponHandler weaponHandler;

    public Transform Player => player;
    public AIState CurrentState => currentState;
    public bool CanSeePlayer { get; private set; }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthSystem = GetComponent<HealthSystem>();
        weaponHandler = GetComponent<AIWeaponHandler>();

        // Auto-trouver le joueur
        if (autoFindPlayer && player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"✅ {gameObject.name} a trouvé le joueur: {player.name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ {gameObject.name} : Aucun joueur trouvé avec le tag 'Player'!");
            }
        }

        // S'abonner à l'événement de mort
        if (healthSystem != null)
        {
            healthSystem.onDeath.AddListener(OnDeath);
        }
    }

    void Start()
    {
        currentState = startingState;
        agent.speed = patrolSpeed;
        SetNewPatrolTarget();
    }

    void Update()
    {
        if (currentState == AIState.Dead) return;

        // Ne pas bouger tant que le jeu n'a pas commencé
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive())
        {
            agent.isStopped = true;
            return;
        }

        // Vérifier si on peut voir le joueur
        CanSeePlayer = CheckPlayerInSight();

        // Machine à états
        switch (currentState)
        {
            case AIState.Idle:
                UpdateIdle();
                break;
            case AIState.Patrol:
                UpdatePatrol();
                break;
            case AIState.Chase:
                UpdateChase();
                break;
            case AIState.Attack:
                UpdateAttack();
                break;
        }
    }

    void UpdateIdle()
    {
        // Détecter le joueur
        if (CanSeePlayer)
        {
            ChangeState(AIState.Chase);
        }
    }

    void UpdatePatrol()
    {
        // Réactiver le mouvement
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        // Vérifier si on est arrivé au point de patrouille
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                SetNewPatrolTarget();
                patrolTimer = 0f;
            }
        }

        // Détecter le joueur
        if (CanSeePlayer)
        {
            ChangeState(AIState.Chase);
        }
    }

    void UpdateChase()
    {
        if (player == null) return;

        // Réactiver le mouvement
        agent.isStopped = false;
        agent.speed = chaseSpeed;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si le joueur est à portée d'attaque ET visible
        if (distanceToPlayer <= attackRange && CanSeePlayer)
        {
            ChangeState(AIState.Attack);
        }
        // Si le joueur est trop loin ou hors de vue, retourner en patrouille
        else if (distanceToPlayer > detectionRange || !CanSeePlayer)
        {
            ChangeState(AIState.Patrol);
        }
        else
        {
            // Poursuivre le joueur
            agent.SetDestination(player.position);
        }
    }

    void UpdateAttack()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // VRAIMENT arrêter de bouger
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Regarder le joueur
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Garder l'IA horizontale
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Tirer si le cooldown est écoulé ET que l'IA voit le joueur
        if (Time.time >= lastAttackTime + attackCooldown && CanSeePlayer)
        {
            Attack();
            lastAttackTime = Time.time;
        }

        // Si le joueur est trop loin, le poursuivre
        if (distanceToPlayer > attackRange)
        {
            agent.isStopped = false; // Réactiver le mouvement
            ChangeState(AIState.Chase);
        }
        // Si le joueur est hors de vue, chercher
        else if (!CanSeePlayer)
        {
            agent.isStopped = false; // Réactiver le mouvement
            ChangeState(AIState.Chase);
        }
    }

    void Attack()
    {
        if (showDebugLogs)
            Debug.Log($"🔫 {gameObject.name} attaque le joueur!");

        // Déclencher le tir via AIWeaponHandler
        if (weaponHandler != null)
        {
            weaponHandler.Shoot();
        }
    }

    bool CheckPlayerInSight()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Vérifier la distance
        if (distanceToPlayer > detectionRange)
        {
            if (showDebugLogs) Debug.Log($"🔍 {gameObject.name}: Joueur trop loin ({distanceToPlayer:F1}m > {detectionRange}m)");
            return false;
        }

        // Vérifier l'angle de vision
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfView / 2f)
        {
            if (showDebugLogs) Debug.Log($"🔍 {gameObject.name}: Joueur hors du champ de vision ({angleToPlayer:F1}° > {fieldOfView / 2f}°)");
            return false;
        }

        // Vérifier si un obstacle bloque la vue (seulement si obstacleMask est configuré)
        if (obstacleMask != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out hit, distanceToPlayer, obstacleMask))
            {
                if (showDebugLogs) Debug.Log($"🔍 {gameObject.name}: Obstacle bloque la vue ({hit.collider.name})");
                return false;
            }
        }

        return true;
    }

    void SetNewPatrolTarget()
    {
        // Choisir un point aléatoire dans le rayon de patrouille
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = navHit.position;
            agent.SetDestination(patrolTarget);

            if (showDebugLogs)
                Debug.Log($"🚶 {gameObject.name} patrouille vers {patrolTarget}");
        }
    }

    void ChangeState(AIState newState)
    {
        if (currentState == newState) return;

        if (showDebugLogs)
            Debug.Log($"🔄 {gameObject.name} : {currentState} → {newState}");

        currentState = newState;

        // Actions lors du changement d'état
        switch (newState)
        {
            case AIState.Patrol:
                SetNewPatrolTarget();
                break;
        }
    }

    void OnDeath()
    {
        currentState = AIState.Dead;
        agent.enabled = false;

        if (showDebugLogs)
            Debug.Log($"💀 {gameObject.name} est mort!");

        // Désactiver les composants
        if (weaponHandler != null)
            weaponHandler.enabled = false;

        // Désactiver l'objet après 5 secondes (ou le détruire)
        Destroy(gameObject, 5f);
    }

    // Visualisation dans la Scene view
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // Rayon de détection (bleu)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Rayon d'attaque (rouge)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Rayon de patrouille (vert)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        // Champ de vision (jaune)
        if (Application.isPlaying)
        {
            Gizmos.color = CanSeePlayer ? Color.red : Color.yellow;
            Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward * detectionRange;
            Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward * detectionRange;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        }

        // Ligne vers le joueur si visible
        if (Application.isPlaying && player != null && CanSeePlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up, player.position);
        }
    }
}
