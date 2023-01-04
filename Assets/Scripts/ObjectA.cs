using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectA : MonoBehaviour, IHasActive
{
    Coroutine disableCoroutine;
    public bool isActive { get; set; } = false;


    public void Init()
    {
        isActive = true;
        disableCoroutine = StartCoroutine(WaitDisable());
        this.GetComponent<SpriteRenderer>().enabled = true;
    }


    private void Disable()
    {
        isActive = false;
        StopCoroutine(disableCoroutine);
        this.GetComponent<SpriteRenderer>().enabled = false;

    }


    private IEnumerator WaitDisable()
    {
        yield return new WaitForSeconds(5);
        Disable();
    }
}
