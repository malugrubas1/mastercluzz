using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float maxHealth = 5f;

    [Header("FX")]
    [Tooltip("Assign your DamageNumber (TMP) prefab here")]
    public GameObject damageNumberPrefab;

    [Header("Refs (auto-wired)")]
    public WaveSpawner WS;               // found via tag "WaveLogic"
    private Transform target;            // hive (tag "Hive")

    private float currentSpeed;
    private float health;
    private bool isDead;
    private bool spawnerNotified;

    void Awake()
    {
        // Rigidbody setup so trigger events fire reliably
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Ensure collider is trigger (barbed wire & bullets rely on trigger)
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        health = maxHealth;
        currentSpeed = speed;

        var wsObj = GameObject.FindGameObjectWithTag("WaveLogic");
        if (wsObj) WS = wsObj.GetComponent<WaveSpawner>();
        else Debug.LogWarning("[EnemyController] No WaveSpawner with tag 'WaveLogic' found.");

        var hive = GameObject.FindGameObjectWithTag("Hive");
        if (hive) target = hive.transform;
        else Debug.LogWarning("[EnemyController] No object with tag 'Hive' found.");
    }

    void Update()
    {
        // reacquire target if it was created later
        if (!target)
        {
            var hive = GameObject.FindGameObjectWithTag("Hive");
            if (hive) target = hive.transform;
            if (!target) return;
        }

        // move toward hive
        transform.position = Vector2.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);

        if (!isDead && health <= 0f)
            Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMovement.playerHealth--;
        }
        else if (other.CompareTag("Bullet"))
        {
            float dmg = 1f;
            var bs = other.GetComponent<BulletScript>();
            if (bs != null && bs.GetType().GetField("damage") != null)
            {
                // If BulletScript has a public float damage
                try { dmg = (float)bs.GetType().GetField("damage").GetValue(bs); } catch { }
            }
            TakeDamage(dmg);
        }
    }

    // Called by traps via SendMessage("TakeDamage", float)
    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        health -= dmg;

        // Floating damage number
        if (damageNumberPrefab)
        {
            var go = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
            var dn = go.GetComponent<DamageNumber>();
            if (dn) dn.Init(dmg);
        }

        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        NotifySpawnerOnce();
        Destroy(gameObject);
    }

    // Slow / Unslow (barbed wire uses these)
    public void SetSlowed(float slowMultiplier)
    {
        currentSpeed = speed * Mathf.Clamp(slowMultiplier, 0.05f, 1f);
    }

    public void RemoveSlow()
    {
        currentSpeed = speed;
    }

    private void OnDestroy()
    {
        // In case something else destroyed us (cleanup, scene change), still notify once
        NotifySpawnerOnce();
    }

    private void NotifySpawnerOnce()
    {
        if (spawnerNotified) return;
        spawnerNotified = true;
        if (WS) WS.EnterNameHere(gameObject);
    }
}
