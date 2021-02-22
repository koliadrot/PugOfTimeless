using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour, IObserverable
{
    #region Field Declarations

    public event Action<int, Transform, Vector3, AudioSource, AudioClip> FoodEated = (int pointValue, Transform food, Vector3 position, AudioSource sourceAudio, AudioClip clip) => { };

    [Header("GamePlay")]
    [SerializeField] int pointValue = 1;

    [Header("Audio")]
    [SerializeField] AudioClip eatClip;
    [HideInInspector] public AudioSource source;

    #endregion

    #region Startup
    private void OnEnable()
    {
        if (!Input.GetKey(GameSceneController.Instance.replayKey))//Set last position food
        {
            GameSceneController.Instance.lastFood = gameObject.transform;
            GameSceneController.Instance.lastPositionFood = transform.position;
        }
    }

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
        if (!GameSceneController.Instance.replayOn)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                FoodEated(pointValue, transform, transform.position, source, eatClip);//Get point, set last position food, activate sound effect
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
