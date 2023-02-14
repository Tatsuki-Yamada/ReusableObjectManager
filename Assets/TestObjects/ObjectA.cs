using System;
using System.Collections;
using UnityEngine;
using UniRx;
using ReusableObjectManagement;

public class ObjectA : MonoBehaviour, IHasAlive
{
    Coroutine disableCoroutine;
    public BoolReactiveProperty isAlive { get; set; } = new BoolReactiveProperty(false);
    public IObservable<bool> isAliveObserver => isAlive;
    public bool canReuse { get; set; } = false;


    public void Init()
    {
        isAlive.Value = true;
        canReuse = true;
        disableCoroutine = StartCoroutine(WaitDisable());
        this.GetComponent<SpriteRenderer>().enabled = true;
    }


    private void Disable()
    {
        isAlive.Value = false;
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
