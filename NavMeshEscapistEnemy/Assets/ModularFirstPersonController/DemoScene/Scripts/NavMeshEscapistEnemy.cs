using UnityEngine;
using UnityEngine.AI;

public class NavMeshEscapistEnemy : MonoBehaviour
{
    public Transform player;
    public GameObject projectilePrefab;
    public float detectionRadius = 10f;
    public float fleeDistance = 5f;
    public float activeDuration = 5f;
    public float tiredDuration = 3f;
    public float shootingInterval = 1.5f;

    private NavMeshAgent agent;
    private bool isTired = false;
    private float currentTiredTimer;
    private float currentActiveTimer;
    private float shootTimer;
    private bool hasLineOfSight;
    private Renderer enemyRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>(); // Asegúrate de que el enemigo tenga un Renderer
        currentTiredTimer = tiredDuration;
        currentActiveTimer = activeDuration;
        shootTimer = shootingInterval;
    }

    void Update()
    {
        if (isTired)
        {
            HandleTiredState();
        }
        else
        {
            HandleActiveState();
        }
    }

    private void HandleTiredState()
    {
        agent.isStopped = true;
        enemyRenderer.material.color = Color.blue; // Visual para estado cansado

        currentTiredTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
        {
            ShootAtPlayer();
            shootTimer = shootingInterval;
        }

        if (currentTiredTimer <= 0)
        {
            isTired = false;
            currentTiredTimer = tiredDuration;
            enemyRenderer.material.color = Color.red; // Cambia al color normal
        }
    }

    private void HandleActiveState()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit hit;
        hasLineOfSight = Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius) && hit.transform == player;

        if (hasLineOfSight)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        currentActiveTimer -= Time.deltaTime;
        if (currentActiveTimer <= 0)
        {
            currentActiveTimer = activeDuration;
            Vector3 fleeDirection = -directionToPlayer;
            Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(fleeTarget, out navHit, fleeDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }
            else
            {
                agent.SetDestination(player.position);
            }

            Debug.DrawLine(transform.position, fleeTarget, Color.red, 1f);
            isTired = true;
        }
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity);
            projectile.transform.LookAt(player);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = projectile.transform.forward * 10f; // Ajusta la velocidad según lo necesites
            }
        }
        else
        {
            Debug.LogWarning("Projectile prefab not assigned.");
        }
    }
}
