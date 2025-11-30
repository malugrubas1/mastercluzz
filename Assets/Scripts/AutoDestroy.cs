using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifetime = 0.3f;

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            Destroy(gameObject);
    }
}
