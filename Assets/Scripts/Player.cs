using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{

    private InputSystem_Actions inputActions;
    Vector2 moveInput;
    public float speed = 5f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    Vector3 direction = Vector3.right; // Default direction
    public float health = 100f;
    private Slider healthBar;
    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }
    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Attack.performed += Attack;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions = new InputSystem_Actions();
        healthBar = GameObject.Find("Canvas/PlayerHealth").GetComponent<Slider>();
        healthBar.maxValue = health;
        healthBar.value = health;
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

        if (moveInput.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(moveInput.x), transform.localScale.y, transform.localScale.z);
            direction = new Vector3(Mathf.Sign(moveInput.x), 0, 0);
        }
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);

    }

    void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack!");
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction, Vector3.up));

    }
    public void TakeDamage(float damage)
    {
        StartCoroutine(FlashRed());
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Player Died!");
    }

    IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = originalColor;
    }
    
}
