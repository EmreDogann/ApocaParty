using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEvent : MonoBehaviour
{
    // Start is called before the first frame update
    private TimerRE atimer;
    void Start()
    {
        atimer = new TimerRE(TriggerRE, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        atimer.Update();
    }
    private void TriggerRE(){
        Debug.Log("randome event triggered");
    }
}
