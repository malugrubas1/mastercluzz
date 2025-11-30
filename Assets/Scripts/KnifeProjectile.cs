using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class KnifeProjectile : MonoBehaviour
{
    public float damage = 9999f;
    public float maxLifetime = 3f;
    [SerializeField] private GameObject bloodParticle;

    // 🔥 Add this:
    public float spinSpeed = 720f;  // how fast the knife spins while flying

    private float life;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        // Make the knife spin
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        // Lifetime
        life += Time.deltaTime;
        if (life >= maxLifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Instantiate(bloodParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
