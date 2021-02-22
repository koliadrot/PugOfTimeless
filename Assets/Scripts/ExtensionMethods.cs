using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    #region Trasform
    public static void ResetTransformation(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public static void ResetTransformation(this Transform transform, Vector3 NewPositon)
    {
        transform.position = NewPositon;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public static void ResetTransformation(this Transform transform, Vector3 NewPositon,Quaternion NewRotation)
    {
        transform.position = NewPositon;
        transform.rotation = NewRotation;
        transform.localScale = Vector3.one;
    }
    public static void ResetTransformation(this Transform transform, Vector3 NewPositon, Quaternion NewRotation, float NewRatioLocalScale)
    {
        transform.position = NewPositon;
        transform.rotation = NewRotation;
        transform.localScale = Vector3.one * NewRatioLocalScale;
    }
    public static void ResetTransformation(this Transform transform, Quaternion NewRotation)
    {
        transform.position = Vector3.zero;
        transform.rotation = NewRotation;
        transform.localScale = Vector3.one;
    }
    public static void ResetTransformation(this Transform transform, Quaternion NewRotation, float NewRatioLocalScale)
    {
        transform.position = Vector3.zero;
        transform.rotation = NewRotation;
        transform.localScale = Vector3.one * NewRatioLocalScale;
    }
    public static void ResetTransformation(this Transform transform, float NewRatioLocalScale)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * NewRatioLocalScale;
    }
    public static void ResetTransformation(this Transform transform, Vector3 NewPositon, float NewRatioLocalScale)
    {
        transform.position = NewPositon;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * NewRatioLocalScale;
    }
    #endregion
}
