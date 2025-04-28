using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public static float playerSpeed = 4.0f;
    public static int playerHealth = 1;

    public bool isAlive = true;
    public SpriteRenderer spriteRenderer;

    private UIManager uiManager;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);

        // Find UIManager once at start
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ No UIManager found in scene!");
        }
    }

    void Update()
    {
        if (isAlive)
        {
            Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            transform.position += playerInput.normalized * playerSpeed * Time.deltaTime;
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
