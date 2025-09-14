using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

    private InputSystem_Actions inputActions;
    Vector2 moveInput;
    public GameObject projectilePrefab;
    public Transform firePoint;
    Vector3 direction = Vector3.right;
    private Slider healthBar;
    private SpriteRenderer spriteRenderer;
    Color originalColor;
    bool iframeIsActive = false;
    Animator animator;
    State currentState = State.Idle;
    bool canMove = true;
    public bool attack1CanHit = false;
    public List<GameObject> targets;
    [Header("Character Configs")]
    public float attack1Duration = 0.5f;
    public float attack2Duration = 0.5f;
    public float takingDamageDuration = 0.3f;
    public float iframeDuration = 1f;
    public int blinkRate = 5;
    [Header("Character Stats")]
    public float health = 100f;
    public float speed = 5f;
    public float closeAttackDamage = 20f;
    public float rangedAttackDamage = 10f;



    // Karakter durumları
    enum State
    {
        Idle,
        Moving,
        Attacking_1,
        Attacking_2,
        TakingDamage,
        Dead
    }
    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }
    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += Move;
        inputActions.Player.Move.canceled += MoveCanceled;
        inputActions.Player.Attack.performed += Attack_1;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions = new InputSystem_Actions();
        healthBar = GameObject.Find("Canvas/PlayerHealth").GetComponent<Slider>();
        healthBar.maxValue = health;
        healthBar.value = health;
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        animator = transform.Find("Sprite").GetComponent<Animator>();
        targets = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {

        if (canMove)
        {
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }

        if (moveInput.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(moveInput.x), transform.localScale.y, transform.localScale.z);
            direction = new Vector3(Mathf.Sign(moveInput.x), 0, 0);
        }

        if (moveInput.magnitude > 0)
        {
            StartCoroutine(ChangeState(State.Moving));
        }
        else
        {
            StartCoroutine(ChangeState(State.Idle));
        }
        if (targets.Count > 0)
        {
            Attack_1();    
        }
    }
    //Karakter saldırı fonksiyonu
    public void Attack_1()
    {
        if (currentState == State.Attacking_1) return;
        StartCoroutine(ChangeState(State.Attacking_1));
        Debug.Log("Attack 1!");
    }
    void Attack_1(InputAction.CallbackContext context)
    {
        Attack_1();
    }
    public void Attack_2()
    {
        if (currentState == State.Attacking_2) return;
        StartCoroutine(ChangeState(State.Attacking_2));
    }

    //Karakter hareketi için InputAction'daki okunan değeri moveInput değişkenine atar
    void Move(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        moveInput = context.ReadValue<Vector2>();
    }
    //Karakter hareketi durduğunda moveInput değişkenini sıfırlar
    void MoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    //Karakter hasar alma fonksiyonu
    public void TakeDamage(float damage)
    {
        if (iframeIsActive) return;
        StartCoroutine(ChangeState(State.TakingDamage));
        StartCoroutine(ActivateIFrames());
        StartCoroutine(FlashRed());
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            Die();
        }
    }
    //Karakter ölme fonksiyonu
    void Die()
    {
        Debug.Log("Player Died!");
    }

    //Karakter hasar aldığında kısa süreliğine kırmızı yanıp sönme efekti
    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = originalColor;
    }
    //Karakterin hasar aldıktan sonra kısa süreliğine dokunulmaz olma fonksiyonu
    IEnumerator ActivateIFrames()
    {
        iframeIsActive = true;
        StartCoroutine(BlinkEffect());
        yield return new WaitForSeconds(iframeDuration);
        iframeIsActive = false;
    }
    //Karakterin i-frame durumu boyunca yanıp sönme efekti
    IEnumerator BlinkEffect()
    {
        int blinkCount = (int)(iframeDuration * blinkRate);
        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f);
            yield return new WaitForSeconds(iframeDuration / blinkCount / 2);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(iframeDuration / blinkCount / 2);
        }

    }

    //Karakterin durumunu değiştiren fonksiyon
    IEnumerator ChangeState(State newState)
    {
        Debug.Log($"state: {currentState}");
        if (currentState == newState) yield break;
        if (currentState == State.Dead) yield break;
        if (currentState == State.Attacking_1) yield break;
        if (currentState == State.Attacking_2) yield break;
        if (currentState == State.TakingDamage) yield break;
        if (newState == State.Attacking_1)
        {
            SetState(State.Attacking_1);
            yield return new WaitForSeconds(attack1Duration/2f);
            if (targets.Count > 0)
            {
                targets.RemoveAll(t => t == null);

                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        target.GetComponent<Enemy>()?.TakeDamage(closeAttackDamage);
                    }
                }
            }
            yield return new WaitForSeconds(attack1Duration);
            SetState(State.Idle);
        }
        else if (newState == State.Attacking_2)
        {
            SetState(State.Attacking_2);
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction, Vector3.up));
            rangedAttackDamage = projectile.GetComponent<Projectile>().damage;
            yield return new WaitForSeconds(attack2Duration);
            SetState(State.Idle);
        }
        else if (newState == State.TakingDamage)
        {
            SetState(State.TakingDamage);
            yield return new WaitForSeconds(takingDamageDuration);
            SetState(State.Idle);
        }
        else
        {
            SetState(newState);
        }


    }

    //Animatör ve hareket kontrolü için karakterin durumunu ayarlar
    void SetState(State state)
    {
        animator.SetInteger("State", (int)state);
        currentState = state;
        if (state == State.TakingDamage) //state == State.Attacking || 
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }

    }


}
