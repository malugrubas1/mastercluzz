using UnityEngine;

public class EnemySprinterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6.0f;
    private float currentSpeed;
    public Transform target;

    [Header("Health Scaling")]
    public float baseHealth = 3f;
    public float extraHealthPerStep = 1.5f;
    public int wavesPerStep = 10;

    [Header("References")]
    public GameObject enemy;
    public WaveSpawner WS;
    public GameObject damageNumberPrefab;

    private float health;
    private bool isDead = false;

    void Start()
    {
        if (!enemy) enemy = gameObject;

        // Wave spawner
        if (!WS)
        {
            GameObject w = GameObject.FindGameObjectWithTag("WaveLogic");
            if (w) WS = w.GetComponent<WaveSpawner>();
        }

        // Target hive
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
        // Debug.Log($"Sprinter wave {currentWave}, HP = {health}");
    }

    void Update()
    {
        if (!target)
        {
            GameObject hive = GameObject.FindGameObjectWithTag("Hive");
            if (hive) target = hive.transform;
        }

        if (isDead || !target) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );

        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Hive"))
        {
            SetSlowed(0.4f);
            Invoke("RemoveSlow", 0.5f);
            var hiver = GameObject.FindGameObjectWithTag("Hive").GetComponent<HiveUpgrade>();
            hiver.ZniszczNigger();

        }
        if (other.CompareTag("Player"))
        {
            CharacterMovement.playerHealth--;
            Die();
        }
        else if (other.CompareTag("Bullet"))
        {
            float dmg = 1f;

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
            Destroy(other.gameObject);
        }
    }

    // Called by traps / grenade / barbed wire via SendMessage("TakeDamage", float)
    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        health -= dmg;

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
