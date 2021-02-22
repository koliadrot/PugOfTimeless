using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour, IMoveable
{
    #region Field Declarations

    [Header("Object Parameters")]
    [SerializeField] protected Transform target;
    [SerializeField] protected float _speedTranslation;
    [SerializeField] protected float _speedRotation;
    [SerializeField] protected AccelerationType accelerationType = AccelerationType.Fast;
    [SerializeField] protected KeyCode accelerationKey = KeyCode.LeftShift;
    protected float accelerationRatio;
    protected float horizontal;
    protected float vertical;

    #endregion

    #region Statup
    protected virtual void Awake()
    {
        ChooseAcceleration();
    }

    #endregion

    #region Movement
    public virtual void Move()//Method for overriding for child class
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        target.Rotate(Vector3.up * horizontal * _speedRotation * Time.deltaTime);
        target.Translate(Vector3.forward * vertical * _speedTranslation * Time.deltaTime);
    }

    public virtual void Acceleration()//Acceleration player
    {
        if (Input.GetKeyDown(accelerationKey))
            _speedTranslation *= accelerationRatio;
        if (Input.GetKeyUp(accelerationKey))
            _speedTranslation /= accelerationRatio;
    }

    protected void ChooseAcceleration()//Set ration for change acceleration
    {
        if (accelerationType == AccelerationType.Slow)
        {
            accelerationRatio = 1.25f;
        }
        if (accelerationType == AccelerationType.Medium)
        {
            accelerationRatio = 1.5f;
        }
        if (accelerationType == AccelerationType.Fast)
        {
            accelerationRatio = 2f;
        }
        if (accelerationType == AccelerationType.VeryFast)
        {
            accelerationRatio = 2.25f;
        }
    }

    #endregion
}
public enum AccelerationType
{
    Slow,
    Medium,
    Fast,
    VeryFast
};
