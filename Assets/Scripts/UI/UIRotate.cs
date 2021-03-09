using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIRotate : MonoBehaviour
{
    #region Field Declaration

    [SerializeField] private RectTransform targetTransform;
    [SerializeField] [Range(0f, 100f)] private float speedRotation = 50f;

    #endregion

    #region Startup
    private void Start()
    {
        targetTransform = targetTransform == null ? GetComponent<RectTransform>() : targetTransform;//Added transform if manual not filled
    }
    #endregion

    #region Subject Implementation
    private void FixedUpdate()//Update frame
    {
        Rotation();
    }
    private void Rotation()//Rotate object with current speed
    {
        targetTransform.Rotate(Vector3.forward * speedRotation * Time.fixedDeltaTime);
    }
    #endregion
}
