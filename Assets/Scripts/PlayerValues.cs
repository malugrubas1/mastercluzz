using UnityEngine;
using UnityEngine.UI;

public class Hive
{
    public int HiveTBPCount;
    public int HivePCount;
    public int Income;

    public int IncomePerWave(int HivesPlaced, int IncomePerHive)
    {
        int income = HivesPlaced * IncomePerHive;
        return income;
    }
}

public class PlayerValues : MonoBehaviour
{
    [SerializeField] public int Honey = 0;
    [SerializeField] public Text HoneyCounter;

    [SerializeField] public Text Tier1hiveCounter;

    public Hive Tier1Hive = new Hive();
    public Hive Tier2Hive = new Hive();
    public Hive Tier3Hive = new Hive();

    void Start()
    {
        Tier1Hive.Income = 100;
        Tier2Hive.Income = 200;
        Tier3Hive.Income = 300;

        Tier1Hive.HiveTBPCount = 3;
        Tier1Hive.HivePCount = 0;
    }

    void Update()
    { 
        HoneyCounter.text = Honey.ToString();
        Tier1hiveCounter.text = Tier1Hive.HiveTBPCount.ToString();
    }

    public void AddIncome()
    {
        int income = 
            Tier1Hive.IncomePerWave(Tier1Hive.HivePCount, Tier1Hive.Income) +
            Tier2Hive.IncomePerWave(Tier2Hive.HivePCount, Tier2Hive.Income) +
            Tier3Hive.IncomePerWave(Tier3Hive.HivePCount, Tier3Hive.Income);

        Honey += income;
    }

    
}


