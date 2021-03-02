using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationUI : MonoBehaviour
{
    #region Field Declarations
    private RectTransform _rect;
    [SerializeField] [Range(0f, 5f)] float duration = 1.5f;
    [SerializeField] float defaultWidth = 1280f;
    [SerializeField] float defaultHeight = 720f;
    private float constX;
    private float constY;
    private Vector2 startPosition;

    #endregion

    #region Startup
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        constX = Screen.width / defaultWidth;
        constY = Screen.height / defaultHeight;
        _rect.offsetMax = new Vector2(_rect.offsetMax.x * constX, _rect.offsetMax.y * constY);
        _rect.offsetMin = new Vector2(_rect.offsetMin.x * constX, _rect.offsetMin.y * constY);
        startPosition = _rect.anchoredPosition;
    }

    private void OnEnable()
    {
        UISliding();
    }
    #endregion

    #region Methods UI effects
    private void UISliding()//Method description function, when game start and subscribe other methods of action
    {
        _rect.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutBack).OnComplete(OnSubscribeAction);
    }
    private void UIEffect(TweenCallback method)//Method description function, which calls when player interaction with UI elements
    {
        _rect.DOAnchorPos(startPosition, duration).SetEase(Ease.InBack).OnComplete(method);

    }
    #endregion

    #region Signatures 
    private void OnDisable()
    {
        OnUnsubscribeAction();
    }
    private void OnSubscribeAction()//Subscribe method on OnMenuAction
    {
        UIManager.Instance.OnMenuAction += UIEffect;
    }

    private void OnUnsubscribeAction()//Unsubscribe method on OnMenuAction
    {
        UIManager.Instance.OnMenuAction -= UIEffect;
    }

    #endregion
}
