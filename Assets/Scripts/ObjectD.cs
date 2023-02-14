using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ObjectD : MonoBehaviour
{
    [SerializeField] CallBackExecuter callBackExecuter;
    void Start()
    {
        callBackExecuter.triggerObserver
            .Where(flag => flag == true)
            .Subscribe(nouse =>
            {
                Debug.Log("Test");
            })
            .AddTo(this);
    }
}
