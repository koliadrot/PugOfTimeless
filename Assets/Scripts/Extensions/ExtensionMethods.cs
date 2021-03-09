using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    #region Trasform
    /// <summary>
    /// Change transform parameters for any situation
    /// </summary>
    /// <param name="transform"></param>
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

    #region List
    /// <summary>
    /// Random shuffle elements into list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        System.Random rand = new System.Random();

        for (int i = list.Count - 1; i >= 1; i--)
        {
            int j = rand.Next(i + 1);

            T tmp = list[j];
            list[j] = list[i];
            list[i] = tmp;
        }
    }
    #endregion
}
