using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicManager : MonoBehaviour
{

    public PlayerValues PlayerVal;
    public int WaveCounter = 0;
    public Text WaveCounterT;
    void Start()
    {
        
    }

    void Update()
    {
        WaveCounterT.text = WaveCounter.ToString();
    }

    public void StartWave()
    {
        WaveCounter += 1;
    }
    public void EndWave()
    {
        PlayerVal.AddIncome();
    }
}

