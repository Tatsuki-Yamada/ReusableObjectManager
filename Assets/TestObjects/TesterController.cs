using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableObjectManagement;

public class TesterController : MonoBehaviour
{
    ReusableObjectManager reusableObjectManager;

    private void Awake()
    {
        reusableObjectManager = (ReusableObjectManager)FindObjectOfType<ReusableObjectManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            reusableObjectManager.CreateOrReuse<ObjectA>().Init();

        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            reusableObjectManager.CreateOrReuse<ObjectB>().Init();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            reusableObjectManager.CreateOrReuse<ObjectC>().Init();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (reusableObjectManager.TryGetManagementList<ObjectA>(out List<ObjectA> list))
            {
                foreach (ObjectA obj in list)
                {
                    obj.name = "Test";
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (reusableObjectManager.TryGetManagementList<ObjectC>(out List<ObjectC> list))
            {
                foreach (ObjectC obj in list)
                {
                    obj.name = "Test";
                }
            }
        }
    }
}
