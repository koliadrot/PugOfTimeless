using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour, IObserverable
{
    #region Field Declarations

    public event Action<int> FoodEated = (int pointValue) => { };

    [Header("GamePlay")]
    [SerializeField] private int pointValue = 1;

    #endregion

    #region Subject Implementation

    #region GamePlay
    private void Death()//Active when player eat food
    {
        GameSceneController.Instance.RemoveObserver(this);
        FoodEated = null;
        gameObject.SetActive(false);
    }

    #endregion

    #region Collisons
    private void OnTriggerEnter(Collider other)//Activate on trigger enter
    {
        if (!GameSceneController.Instance.ReplayOn)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                FoodEated(pointValue);//Get point, activate sound effect
                Death();
            }
        }
    }

    #endregion

    #region Observer Action
    public void Notify()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #endregion
}
