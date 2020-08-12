using System;
using UnityEngine;

public class SimpleTimer {

    public float targetTime = 60.0f;

    public void update() {
        targetTime -= Time.deltaTime;

    }

    public bool timerEnded() {
        return targetTime <= 0.0f;
    }


}
