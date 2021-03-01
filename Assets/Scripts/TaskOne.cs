using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskOne : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GameInfo gameOne = new GameInfo(100, 1, 10);
        gameOne.IncreaseCoinsCount(10);
        gameOne.IncreaseCoinsCount(10);
        gameOne.IncreaseCoinsCount(10);
        gameOne.IncreaseCoinsCount(10);
        gameOne.DecreaseCoinsCount(20);
        gameOne.GetRewards();
        gameOne.NewLevelStart();
        gameOne.GetRewards();
        gameOne.GetRewards();
        gameOne.DecreaseCoinsCount(20);
        gameOne.NewLevelStart();
        gameOne.events.ShowEvents();
        gameOne.events.TrackEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
