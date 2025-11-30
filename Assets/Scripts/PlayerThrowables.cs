using UnityEngine;

public class PlayerThrowables : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Where the knife/grenade spawns (usually gun muzzle or player center).")]
    public Transform throwPoint;
    [Tooltip("Main camera used to convert mouse position.")]
    public Camera mainCamera;

    [Header("Throwing Knife")]
    public GameObject knifePrefab;
    public KeyCode knifeKey = KeyCode.Mouse1;  // right mouse button
    public float knifeSpeed = 15f;
    public float knifeCooldown = 2f;           // time between throws
    public float knifeDamage = 9999f;          // effectively one-shot

    [Header("Grenade")]
    public GameObject grenadePrefab;
    public KeyCode grenadeKey = KeyCode.G;     // press G to throw grenade
    public float grenadeTravelSpeed = 10f;     // how fast it flies to cursor
    public float grenadeCooldown = 3f;
    public float grenadeDamage = 5f;
    public float grenadeRadius = 2.5f;

    private float knifeTimer;
    private float grenadeTimer;

    void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;
    }

    void Update()
    {
        knifeTimer -= Time.deltaTime;
        grenadeTimer -= Time.deltaTime;

        if (!mainCamera || !throwPoint) return;

        // World position of mouse
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Direction for knife
        Vector2 dir = (mouseWorld - throwPoint.position).normalized;

        // knife on right click
        if (Input.GetKeyDown(knifeKey) && knifeTimer <= 0f && knifePrefab)
        {
            ThrowKnife(dir);
        }

        // grenade goes to mouse position
        if (Input.GetKeyDown(grenadeKey) && grenadeTimer <= 0f && grenadePrefab)
        {
            ThrowGrenade(mouseWorld);
        }
    }

    void ThrowKnife(Vector2 dir)
    {
        knifeTimer = knifeCooldown;

        GameObject obj = Instantiate(knifePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb) rb.velocity = dir * knifeSpeed;

        KnifeProjectile kp = obj.GetComponent<KnifeProjectile>();
        if (kp) kp.damage = knifeDamage;
    }

    void ThrowGrenade(Vector3 targetPos)
    {
        grenadeTimer = grenadeCooldown;

        GameObject obj = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);

        // we let the grenade script move it, so no velocity needed
        GrenadeProjectile gp = obj.GetComponent<GrenadeProjectile>();
        if (gp)
        {
            gp.damage = grenadeDamage;
            gp.radius = grenadeRadius;
            gp.targetPosition = targetPos;
            gp.travelSpeed = grenadeTravelSpeed;
        }
    }
}
