using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float life = 0.6f;
    [SerializeField] private float riseSpeed = 1.5f;
    [SerializeField] private Vector2 jitter = new Vector2(0.25f, 0.1f);

    private float t;
    private TextMeshPro tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
    }

    public void Init(float dmg)
    {
        transform.position += (Vector3)new Vector2(Random.Range(-jitter.x, jitter.x), Random.Range(0f, jitter.y));
        if (tmp) tmp.text = Mathf.RoundToInt(dmg).ToString();
    }

    void Update()
    {
        t += Time.deltaTime;
        transform.position += Vector3.up * (riseSpeed * Time.deltaTime);
        if (tmp) tmp.alpha = Mathf.Lerp(1f, 0f, t / life);
        if (t >= life) Destroy(gameObject);
    }
}
