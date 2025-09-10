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
    public float attackCooldown = 1f;
    private bool isAttacking = false;
    private float maxHealth = 100f;
    private SpriteRenderer spriteRenderer;
    Color originalColor;
    private Coroutine attackRoutine;


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
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // Move towards the target waypoint
            //transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            agent.SetDestination(target.position);

            if (Vector3.Distance(transform.position, target.position) < agent.stoppingDistance)
            {
                if (!isAttacking)
                {
                    attackRoutine = StartCoroutine(CloseAttack());
                }

            }
            else if (isAttacking)
            {
                isAttacking = false;
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
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);
        if (target != null)
        {
            Player player = target.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

        }
        isAttacking = false;
        
    }     
    IEnumerator FlashRed()
    {
        
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = originalColor;
    }
        
    
}
