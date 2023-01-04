using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterController : MonoBehaviour
{
    ReusableObjectManager ReusableObjectManager;

    private void Awake()
    {
        ReusableObjectManager = (ReusableObjectManager)FindObjectOfType<ReusableObjectManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ReusableObjectManager.MainAction<ObjectA>().Init();

        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            ReusableObjectManager.MainAction<ObjectB>().Init();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ReusableObjectManager.MainAction<ObjectC>().Init();
        }
    }
}
