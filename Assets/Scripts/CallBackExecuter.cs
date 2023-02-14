using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CallBackExecuter : MonoBehaviour
{
    private BoolReactiveProperty trigger = new BoolReactiveProperty(false);
    public IObservable<bool> triggerObserver => trigger;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            trigger.Value = !trigger.Value;
        }
    }
}
