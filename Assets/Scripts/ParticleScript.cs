using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour

{
    public Transform target;
    public float ime = 0.5f;
    public float speed = 5.3f;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Destroy(gameObject, ime);

    }
    // Update is called once per frame
    void Update()
    {

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, 5.3f * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
        Vector2 direction = target.position - transform.position;
        direction.Normalize();
    }
}
