using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Strong_bot : MonoBehaviour
{
    //strong bot can move right and left
    void Start()
    {
        //10 secondes before start moving
        StartCoroutine(tween_bot());
    }

    IEnumerator tween_bot()
    {
        yield return new WaitForSeconds(10);
        transform.DOMoveX(-4.5f, 5).OnComplete(() => transform.DOMoveX(4.5f, 5));
        StartCoroutine(tween_bot());
    }
}
