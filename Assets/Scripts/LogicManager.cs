using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicManager : MonoBehaviour
{

    public PlayerValues PlayerVal;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EndWave()
    {
        PlayerVal.AddIncome();
    }
}

