using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pug : Animal, IObserverable
{
    #region Field Declarations

    public event Action GamePlay = () => { };

    private List<ICommandable> command;
    private GameSceneController gameSceneController;

    [SerializeField] Animator animator;

    [Header("Bounds Range")]
    [SerializeField] float leftBound = -25f;
    [SerializeField] float rightBound = 25f;
    [SerializeField] float upBound = 25f;
    [SerializeField] float downBound = -25f;

    #endregion

    #region Startup
    private void Start()
    {
        gameSceneController = GameSceneController.Instance;
        gameSceneController.AddObserver(this);
        gameSceneController.GamePlaySubscribe += SubscribeOnEvent;
    }
    private void SubscribeOnEvent()
    {
        GamePlay += Move;
        GamePlay += Acceleration;
        GamePlay += ReplayPerson;
        GamePlay += gameSceneController.HungryCountDown;
    }
    private void UnsubscribeOnEvent()
    {
        GamePlay -= Move;
        GamePlay -= Acceleration;
        GamePlay -= ReplayPerson;
        GamePlay -= gameSceneController.HungryCountDown;
    }

    #endregion

    #region Subject Implementation

    private void Update()
    {
        GamePlay();
    }

    #region Movement
    public override void Move()//Overriding method "Move" from animal
    {
        if (!gameSceneController.replayOn)
        {
            base.Move();
            ChangeAnimation(horizontal, vertical);
        }
        Bounds();
    }

    private void ChangeAnimation(float Horizontal, float Vertical)//Change animation speed on dependency move player
    {
        if (Mathf.Abs(Horizontal) > Mathf.Abs(Vertical))
        {
            animator.SetFloat("Speed_f", Mathf.Abs(Horizontal * 0.85f));
        }
        else
        {
            animator.SetFloat("Speed_f", Mathf.Abs(Vertical));
        }
    }

    private void Bounds()//Set bounds for player, when he cross it reset position
    {
        if (target.transform.position.z > upBound || target.transform.position.z < downBound || target.transform.position.x > rightBound || target.transform.position.x < leftBound)
            target.ResetTransformation(15f);
    }

    #endregion

    #region Replay Player
    private void ReplayPerson()//Method "Replay". Returning position and state game objects back out to some time
    {
        if (Input.GetKeyDown(gameSceneController.replayKey))//Start "Replay"
        {
            if (!gameSceneController.replayOn)
            {
                ManagerCommand.Instance.Replay();
                gameSceneController.ReplayWindow_OnAction(true);
            }
        }
        else if (Input.GetKeyUp(gameSceneController.replayKey))//Stop "Replay"
        {
            if (gameSceneController.replayOn)
            {
                ManagerCommand.Instance.StopReplay();
                gameSceneController.ReplayWindow_OnAction(false);
            }
        }
        else if (!Input.GetKey(gameSceneController.replayKey))//Record "Replay" when player don't use it
        {
            ManagerCommand.Instance.RecordReplay();
        }
    }

    #endregion

    #region Observer Action
    public void Notify()
    {
        GameOver();
    }

    private void GameOver()//Activate when player has zero life
    {
        UnsubscribeOnEvent();
        ChangeAnimation(0f, 0f);
        animator.SetBool("Sit_b", true);
    }

    #endregion

    #endregion
}
