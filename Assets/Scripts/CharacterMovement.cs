using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public static float playerSpeed = 4.0f;
    public static int playerHealth = 1;

    public bool isAlive = true;
    public SpriteRenderer spriteRenderer;

    [SerializeField] private Animator _animator;

    private UIManager uiManager;

    void Start()
    {
        playerHealth = 1;
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);

        // Find UIManager once at start
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ No UIManager fouSLEZAK CWELU KURWOnd in scene!");
        }
    }

    void Update()
    {
        if (isAlive)
        {
            Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            transform.position += playerInput.normalized * playerSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.W))
            {
                _animator.SetBool("runningW", true);
            }
            else
            {
                _animator.SetBool("runningW", false);
            }
            if (Input.GetKey(KeyCode.S))
            {
                _animator.SetBool("runningS", true);
            }
            else
            {
                _animator.SetBool("runningS", false);
            }
            if (Input.GetKey(KeyCode.A))
            {
                _animator.SetBool("runningA", true);
            }
            else
            {
                _animator.SetBool("runningA", false);
            }
            if (Input.GetKey(KeyCode.D))
            {
                _animator.SetBool("runningD", true);
            }
            else
            {
                _animator.SetBool("runningD", false);
            }
        }

        if (playerHealth <= 0 && isAlive)
        {
            Die();
        }

    }

    private void Die()
    {
        Debug.Log("💀 Player has died.");
        isAlive = false;

        if (uiManager != null)
        {
            uiManager.ShowDeathScreen();
        }
        else
        {
            Debug.LogError("❌ uiManager reference missing during death!");
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    // Call this function to deal damage
    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        Debug.Log("💥 Player took " + amount + " damage. Health: " + playerHealth);
    }
}
