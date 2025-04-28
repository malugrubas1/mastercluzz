using UnityEngine;

public class LogicManager : MonoBehaviour
{
    [SerializeField] private PlayerValues playerValues;

    void Start()
    {
        if (playerValues == null)
        {
            Debug.LogError("❌ PlayerValues is not assigned in LogicManager Inspector!");
        }
    }

    public void EndWave()
    {
        Debug.Log("✅ Wave ended! Awarding income...");
        if (playerValues != null)
        {
            playerValues.AddIncome();
        }
    }
}
