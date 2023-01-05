using System;
using System.Collections;
using UnityEngine;
using UniRx;
using ReusableObjectManagement;

public class ObjectA : MonoBehaviour, IHasActive
{
    Coroutine disableCoroutine;
    public BoolReactiveProperty isActive { get; set; } = new BoolReactiveProperty(false);
    public IObservable<bool> isActiveObserver => isActive;
    public bool canReuse { get; private set; } = false;


    public void Init()
    {
        isActive.Value = true;
        canReuse = true;
        disableCoroutine = StartCoroutine(WaitDisable());
        this.GetComponent<SpriteRenderer>().enabled = true;
    }


    private void Disable()
    {
        isActive.Value = false;
        canReuse = false;
        StopCoroutine(disableCoroutine);
        this.GetComponent<SpriteRenderer>().enabled = false;

    }


    private IEnumerator WaitDisable()
    {
        yield return new WaitForSeconds(5);
        Disable();
    }
}
