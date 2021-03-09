using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIFadeText : UIEffect
{
    #region Field Declaration

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] [Range(0f, 1f)] private float minAlpha = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float maxAlpha = 1f;

    #endregion

    #region Startup
    private void Start()
    {
        TextFadeOff();
    }
    #endregion

    #region Subject Implementation

    private void TextFadeOff()//Decreases transparency over time
    {
        text.DOFade(minAlpha, duration).OnComplete(TextFadeOn);
    }
    private void TextFadeOn()//Increases transparency over time
    {
        text.DOFade(maxAlpha, duration).OnComplete(TextFadeOff);
    }

    #endregion
}
