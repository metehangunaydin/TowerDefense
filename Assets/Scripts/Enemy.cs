using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class Enemy : MonoBehaviour
{

    Transform target;
    public GameObject[] waypoints;
    //public float speed = 2f;
    //public float reachDistance = 1f;
    private NavMeshAgent agent;
    public float health = 100f;
    private GameObject healthBar;
    public float damage = 10f;
    private float maxHealth = 100f;
    private SpriteRenderer spriteRenderer;
    Color originalColor;
    private Coroutine attackRoutine;
    private Animator animator;
    private State currentState = State.Idle;
    public float attackDelay = 0.3f;
    public float attackCooldown = 1f;
    bool targetInAttackRange = false;
    float attackAnimationDuration;
    bool canChangeState = true;

    enum State
    {
        Idle,
        Moving,
        Attacking,
        TakingDamage,
        Dead
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waypoints[0] = GameObject.Find("Player");
        target = waypoints[0].transform;
        agent = GetComponent<NavMeshAgent>();
        healthBar = transform.Find("EnemyHealthBar/HealthBar").gameObject;
        maxHealth = health;
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        animator = transform.Find("Sprite").GetComponent<Animator>();

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == "Attack")
            {
                attackAnimationDuration = clip.length;
                break;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // Move towards the target waypoint
            //transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if(canChangeState)
            {
                agent.SetDestination(target.position);
            }

            //hareket ediyorsa state moving e geç
            if (agent.velocity.magnitude > 0.01f)
            {
                StartCoroutine(ChangeState(State.Moving));
                UpdateDirection();
            }
            else
            {
                StartCoroutine(ChangeState(State.Idle));
            }

            if (Vector3.Distance(transform.position, target.position) < agent.stoppingDistance) // Eğer hedef menzil içindeyse saldırı başlat
            {
                targetInAttackRange = true;
                if (currentState != State.Attacking)
                {
                    attackRoutine = StartCoroutine(CloseAttack());
                }

            }
            else if (currentState == State.Attacking) // Eğer saldırı durumundaysa ve menzil dışına çıktıysa saldırıyı durdur
            {
                targetInAttackRange = false;
                currentState = State.Idle;
                StopCoroutine(attackRoutine);
                spriteRenderer.color = originalColor;
            }

        }

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        float healthPercent = health / maxHealth;
        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
        // Sola doğru scale etmek için pozisyonu da güncelle
        float barWidth = 1f; // HealthBar'ın orijinal genişliği (scale.x = 1 iken)
        float offset = (1f - healthPercent) * barWidth * 0.5f;
        healthBar.transform.localPosition = new Vector3(-offset, healthBar.transform.localPosition.y, healthBar.transform.localPosition.z);
        StartCoroutine(FlashRed());
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }



    IEnumerator CloseAttack()
    {
        if (currentState == State.Attacking) yield break;
        StartCoroutine(ChangeState(State.Attacking));
        yield return new WaitForSeconds(attackDelay); // Saldırının gerçekleşme yani oyuncuya hasar verme süresi
        if (!targetInAttackRange) yield break; // Eğer hedef saldırı menzilinde değilse çık
        // Burada oyuncuya hasar verme işlemi yapılacak
        if (target != null)
        {
            Player player = target.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

        }


    }
    IEnumerator FlashRed()
    {

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = originalColor;
    }

    void SetState(State state)
    {
        animator.SetInteger("State", (int)state);
        currentState = state;

    }
    IEnumerator ChangeState(State newState)
    {
        if (!canChangeState) yield break;
        if (currentState == newState) yield break;
        if (currentState == State.Dead) yield break;
        if (currentState == State.Attacking) yield break;
        if (currentState == State.TakingDamage) yield break;
        if (newState == State.Attacking)
        {
            SetState(State.Attacking);
            canChangeState = false;
            yield return new WaitForSeconds(attackAnimationDuration);
            SetState(State.Idle);
            yield return new WaitForSeconds(attackCooldown);
            canChangeState = true;
        }
        else
        {
            SetState(newState);
        }
    }

    void UpdateDirection()
    {
        spriteRenderer.flipX = agent.velocity.x < 0;    
    }
        
    
}
