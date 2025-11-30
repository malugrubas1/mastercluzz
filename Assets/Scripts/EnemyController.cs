using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2.0f;
    private float currentSpeed;
    public Transform target;               // Hive

    [Header("Health Scaling")]
    public float baseHealth = 5f;          // HP on waves 1–wavesPerStep
    public float extraHealthPerStep = 2f;  // extra HP every step
    public int wavesPerStep = 10;          // every X waves they get tankier

    [Header("References")]
    public GameObject enemy;               // usually this gameObject
    public WaveSpawner WS;                 // set via tag "WaveLogic" if left empty
    public GameObject damageNumberPrefab;  // floating damage text prefab

    private float health;
    private bool isDead = false;

    void Start()
    {
        if (!enemy) enemy = gameObject;

        // Find WaveSpawner
        if (!WS)
        {
            GameObject w = GameObject.FindGameObjectWithTag("WaveLogic");
            if (w) WS = w.GetComponent<WaveSpawner>();
        }

        // Find hive target
        if (!target)
        {
            GameObject hive = GameObject.FindGameObjectWithTag("Hive");
            if (hive) target = hive.transform;
        }

        currentSpeed = speed;

        // ---- HEALTH SCALING ----
        int currentWave = (WS != null) ? WS.currWave : 1;
        int step = Mathf.Max(0, (currentWave - 1) / Mathf.Max(1, wavesPerStep));
        health = baseHealth + step * extraHealthPerStep;
        // Debug.Log($"Normal enemy wave {currentWave}, HP = {health}");
    }

    void Update()
    {
        if (isDead || !target) return;

        // Move toward hive
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );

        // keep z constant
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
    }

    // ---------- COLLISION ----------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            CharacterMovement.playerHealth--;
            Die();
        }
        else if (other.CompareTag("Bullet"))
        {
            // default bullet dmg = 1
            float dmg = 1f;

            // Try to read a public float "damage" from BulletScript, if it exists
            var bs = other.GetComponent<BulletScript>();
            if (bs != null)
            {
                try
                {
                    var field = bs.GetType().GetField("damage");
                    if (field != null && field.FieldType == typeof(float))
                    {
                        dmg = (float)field.GetValue(bs);
                    }
                }
                catch { }
            }

            TakeDamage(dmg);

            // optional: destroy bullet on hit
            Destroy(other.gameObject);
        }
    }

    // Called by traps / grenade / barbed wire via SendMessage("TakeDamage", float)
    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        health -= dmg;

        // Floating damage number
        if (damageNumberPrefab)
        {
            GameObject go = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
            var dn = go.GetComponent<DamageNumber>();
            if (dn != null) dn.Init(dmg);
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // notify spawner
        if (WS != null)
        {
            WS.EnterNameHere(gameObject);
        }

        Destroy(enemy != null ? enemy : gameObject);
    }

    // ---------- SLOW FOR BARBED WIRE ----------
    public void SetSlowed(float slowMultiplier)
    {
        currentSpeed = speed * slowMultiplier;
    }

    public void RemoveSlow()
    {
        currentSpeed = speed;
    }
}
