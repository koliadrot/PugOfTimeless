using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UISlide : UIEffect
{
    #region Field Declaration

    private Vector2 startTargetPosition;

    #endregion

    #region Subject Implementation
    protected override void Awake()
    {
        base.Awake();
        startTargetPosition = targetTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        InSlide();
    }

    private void OnDisable()
    {
        OnUnsubscribeAction(OutSlide);
    }

    #endregion

    #region Methods UI effects
    public void InSlide()//Method description function, when game start and subscribe other methods of action
    {
        targetTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutBack).OnComplete(() => 
        {
            OnSubscribeAction(OutSlide);
        });
    }
    public void OutSlide(TweenCallback method)//Method description function, which calls when player interaction with UI elements
    {
        targetTransform.DOAnchorPos(startTargetPosition, duration).SetEase(Ease.InBack).OnComplete(method);
    }
    #endregion
}
