using UnityEngine;
using System.Collections.Generic;

public class BarbedWireTrap : MonoBehaviour
{
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private int maxTouches = 3; // number of enemies before destruction

    private HashSet<GameObject> enemiesTouched = new HashSet<GameObject>();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyController ec = other.GetComponent<EnemyController>();
        EnemySprinterController esc = other.GetComponent<EnemySprinterController>();

        if (ec != null)
        {
            ec.SetSlowed(slowMultiplier);
            ec.SendMessage("TakeDamage", damagePerSecond * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
        }

        if (esc != null)
        {
            esc.SetSlowed(slowMultiplier);
            esc.SendMessage("TakeDamage", damagePerSecond * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
        }

        // Count each unique enemy only once
        if (!enemiesTouched.Contains(other.gameObject))
        {
            enemiesTouched.Add(other.gameObject);

            if (enemiesTouched.Count >= maxTouches)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyController ec = other.GetComponent<EnemyController>();
        EnemySprinterController esc = other.GetComponent<EnemySprinterController>();

        if (ec != null)
        {
            ec.RemoveSlow();
        }

        if (esc != null)
        {
            esc.RemoveSlow();
        }
    }
}
