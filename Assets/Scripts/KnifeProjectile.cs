using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class KnifeProjectile : MonoBehaviour
{
    public float damage = 1f;
    public float maxLifetime = 3f;   // how long the knife lives if it hits nothing
    [SerializeField] private GameObject bloodParticle;
    private float life;

    void Awake()
    {
        // Make sure collider is trigger so it passes through and just detects hits.
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        life += Time.deltaTime;
        if (life >= maxLifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Let enemies handle damage however they want
            other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Instantiate(bloodParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            // hit wall/floor/whatever
            Destroy(gameObject);
        }
    }
}
