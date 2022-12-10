using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Attack AttackValue;
    public CardPlayer player;
    // public Transform atkPosRef;
    public Vector2 OriginalPosition;
    Vector2 originalScale;
    Color originalColor;
    bool isClickable = true;
    private void Start()
    {
        OriginalPosition = this.transform.position;
        originalScale = this.transform.localScale;
        originalColor = GetComponent<Image>().color;

        //infinite
        // transform.DOMove(atkPosRef.position, 5).SetLoops(-1, LoopType.Yoyo);

        // sequen
        // var seq = DOTween.Sequence();
        // seq.Append(transform.DOMove(atkPosRef.position, 1));
        // seq.Append(transform.DOMove(startPosition, 1));
    }

    public void OnClick()
    {
        if (isClickable)
        {
            OriginalPosition = this.transform.position;
            player.SetChosenCard(this);
        }

    }



    internal void Reset()
    {
        transform.position = OriginalPosition;
        transform.localScale = originalScale;
        GetComponent<Image>().color = originalColor;

    }
    public void SetClickable(bool value)
    {
        isClickable = value;
    }

















    // float timer = 0;
    // private void Update()
    // {
    //     if (timer <= 5)
    //     {
    //         timer += Time.deltaTime;
    //     }
    //     else
    //     {
    //         this.transform = atkPosRef.transform;
    //     }
    // }

}

