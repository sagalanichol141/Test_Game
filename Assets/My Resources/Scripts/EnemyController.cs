using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    public Transform bulletOut;
    public Transform pooledBulletsParent;

    private SphereCollider attackTrigger;
    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;
    private float wanderTimer;
    private float fovTimer;
    private PlaySound sound;

    private enum State
    {
        Normal,
        Chasing,
        Attack
    }

    private State currentState = State.Normal;

    private void Start()
    {
        sound = new PlaySound();
        attackTrigger = GetComponent<SphereCollider>();
        TriggerOff();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        wanderTimer = enemyData.wanderDelay;
        fovTimer = 0f;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Normal:
                Wander();
                CheckForPlayer();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }

        // Set animator parameter "Magnitude" berdasarkan velocity magnitude karakter.
        animator.SetFloat("Magnitude", agent.velocity.magnitude);
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * enemyData.wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, enemyData.wanderRadius, 1);
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
            wanderTimer = enemyData.wanderDelay;
        }

        // Atur kecepatan dan rotasi NavMeshAgent berdasarkan data normal.
        agent.speed = enemyData.normalSpeed;
        agent.angularSpeed = enemyData.rotationSpeed;
    }

    private void CheckForPlayer()
    {
        fovTimer += Time.deltaTime;

        if (fovTimer >= 1f / enemyData.fovRayCount)
        {
            fovTimer = 0f;
            float halfFOV = enemyData.fovAngle / 2f;
            
            for (int i = 0; i < enemyData.fovRayCount; i++)
            {
                float angle = -halfFOV + (i * (enemyData.fovAngle / (enemyData.fovRayCount - 1)));
                Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

                if (Physics.Raycast(transform.position + Vector3.up * enemyData.fovRayHeight, direction, out RaycastHit hit, enemyData.fovRayDistance))
                {
                    if (hit.collider.CompareTag(enemyData.targetTag))
                    {
                        target = hit.collider.transform;
                        ChangeState(State.Chasing);
                        break;
                    }
                }

                // Debug ray untuk ray FOV
                Debug.DrawRay(transform.position + Vector3.up * enemyData.fovRayHeight, direction * enemyData.fovRayDistance, Color.green);
            }
        }

        // Atur kecepatan dan rotasi NavMeshAgent berdasarkan data normal.
        agent.speed = enemyData.normalSpeed;
        agent.angularSpeed = enemyData.rotationSpeed;
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
        animator.SetBool("Attack", false);
        animator.SetBool("Chasing", true);

        // Atur kecepatan dan rotasi NavMeshAgent berdasarkan data chasing.
        agent.speed = enemyData.chasingSpeed;
        agent.angularSpeed = enemyData.rotationSpeed;

        if (Vector3.Distance(transform.position, target.position) <= enemyData.stopDistance)
        {
            ChangeState(State.Attack);
        }
    }

    private void Attack()
    {
        agent.isStopped = true;
        animator.SetBool("Chasing", false);
        animator.SetBool("Attack", true);

        // Look at target saat melakukan serangan.
        Vector3 targetDirection = (target.position - transform.position).normalized;
        targetDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(targetDirection);

        if (Vector3.Distance(transform.position, target.position) >= enemyData.stopDistance + 0.25f)
        {
            ChangeState(State.Chasing);
        }
    }

    public void TriggerOn()
    {
        sound.PlaySoundAt(enemyData.slashSFX, transform.position, 1, "Slash Enemy", true);
        attackTrigger.enabled = true;
    }

    public void TriggerOff()
    {
        attackTrigger.enabled = false;
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentState == State.Attack && other.gameObject.CompareTag(enemyData.targetTag))
        {
            other.gameObject.GetComponent<Health>().TakeDamage(enemyData.attackDamage);
            TriggerOff();
        }
    }

    private void OnDisable() {
        if (agent != null)
            agent.enabled = false;
    }

    private void OnEnable() {
        if (agent != null)
            agent.enabled = true;
    }

    public void Shoot()
    {
        sound.PlaySoundAt(enemyData.shootSFX, transform.position, 1, "Shoot Enemy", true);
        foreach (Transform bullet in pooledBulletsParent)
        {
            if (!bullet.gameObject.activeSelf)
            {
                bullet.position = bulletOut.position;
                bullet.rotation = transform.rotation;
                bullet.gameObject.SetActive(true);
                bullet.gameObject.GetComponent<Bullet>().targetTag = enemyData.targetTag;
                bullet.gameObject.GetComponent<Bullet>().hitDamage = enemyData.attackDamage;
                return;
            }
        }
    }
}
