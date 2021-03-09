using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffect : MonoBehaviour
{
    #region Field Declarations

    [SerializeField] [Range(0f, 5f)] protected float duration = 1.5f;
    [SerializeField] protected RectTransform targetTransform;    
    private  float defaultWidth = 1280f;
    private  float defaultHeight = 720f;

    #endregion

    #region Startup
    protected virtual void Awake()//Set position based on the screen size
    {
        float constX = Screen.width / defaultWidth;
        float constY = Screen.height / defaultHeight;
        targetTransform = targetTransform == null ? GetComponent<RectTransform>() : targetTransform;
        targetTransform.offsetMax = new Vector2(targetTransform.offsetMax.x * constX, targetTransform.offsetMax.y * constY);
        targetTransform.offsetMin = new Vector2(targetTransform.offsetMin.x * constX, targetTransform.offsetMin.y * constY); 
    }

    #endregion

    #region Subscribers
    protected void OnSubscribeAction(Action<TweenCallback> method)//Subscribe method on OnMenuAction
    {
        UIController.Instance.OnMenuAction += method;
    }

    protected void OnUnsubscribeAction(Action<TweenCallback> method)//Unsubscribe method on OnMenuAction
    {
        UIController.Instance.OnMenuAction -= method;
    }

    #endregion
}

