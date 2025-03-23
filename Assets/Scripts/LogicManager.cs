using UnityEngine;

public class LogicManager : MonoBehaviour
{
    private PlayerValues playerValues;

    void Start()
    {
        playerValues = GetComponent<PlayerValues>();   
       
    }

    public void EndWave()
    {

       playerValues.AddIncome();

    }
}
