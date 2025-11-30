using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 targetPosition;
    public float travelSpeed = 10f;
    public float arriveThreshold = 0.1f; // how close to target before exploding

    [Header("Explosion")]
    public float radius = 2.5f;
    public float damage = 5f;
    public GameObject explosionPrefab;   // optional VFX
    public LayerMask hitMask;            // optional: set to Enemy layer, or leave 0

    [Header("Safety")]
    public float maxLifetime = 5f;       // just in case it never reaches

    private float life;

    void Update()
    {
        life += Time.deltaTime;
        if (life >= maxLifetime)
        {
            Explode();
            return;
        }

        // Move towards target
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            travelSpeed * Time.deltaTime
        );

        // If close enough, explode
        if (Vector2.Distance(transform.position, targetPosition) <= arriveThreshold)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Spawn VFX
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Damage enemies in radius
        Collider2D[] hits;
        if (hitMask.value != 0)
        {
            hits = Physics2D.OverlapCircleAll(transform.position, radius, hitMask);
        }
        else
        {
            hits = Physics2D.OverlapCircleAll(transform.position, radius);
        }

        foreach (var h in hits)
        {
            if (!h) continue;
            if (h.CompareTag("Enemy"))
            {
                h.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
