using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IObserverable
{
    #region Field Declarations

    [Header("Player Parameters")]
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private float speedMovement;
    [SerializeField] private float speedRotation;
    [SerializeField] private AccelerationType accelerationType = AccelerationType.Fast;
    [SerializeField] private KeyCode accelerationKey = KeyCode.LeftShift;
    [SerializeField] private Animator animator;
    [SerializeField] private float defaultScale = 10f;
    [SerializeField] private Bounds bounds = new Bounds();
    private float accelerationSpeed = 0f;
    public float AccelerationSpeed
    {
        get => accelerationSpeed;
        set
        {
            if (value >= 0f && value <= speedMovement)//Protect acceleration value from not right value
                accelerationSpeed = value;
        }
    }
    private float test;
    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    public float Test { get => test; set => test = value; }

    [System.Serializable]
    private class Bounds
    {
        public float leftBound = -25f;
        public float rightBound = 25f;
        public float upBound = 25f;
        public float downBound = -25f;
    }
    private enum AccelerationType
    {
        Slow,
        Medium,
        Fast,
    }

    private List<ICommandable> command;
    private GameSceneController gameSceneController;

    private PlayerBaseState currentState;

    public readonly PlayerIdleState IdleState = new PlayerIdleState();
    public readonly PlayerRunState RunState = new PlayerRunState();
    public readonly PlayerReplayState ReplayState = new PlayerReplayState();
    public readonly PlayerGameOverState GameOverState = new PlayerGameOverState();


    #endregion

    #region Statup
    private void Start()
    {
        gameSceneController = GameSceneController.Instance;
        gameSceneController.AddObserver(this);
        TransitionToState(IdleState);
    }

    #endregion

    #region Logic Update
    private void Update()
    {
        currentState.HandleInput(this);//Update methods on input buttons for change player state
        currentState.LogicUpdate(this);//Update methods on current player state
    }

    #region GamePlay
    public void TransitionToState(PlayerBaseState state)//Changed player state
    {
        currentState = state;
        currentState.EnterState(this);
    }
    public void GameOver()//Activate when player has zero life
    {
        SpeedAnimation(0f, 0f);
        animator.SetBool("Sit_b", true);
    }
    public void BoundsMap()//Set bounds for player, when he cross it reset position
    {
        if (targetPlayer.transform.position.z > bounds.upBound || targetPlayer.transform.position.z < bounds.downBound || targetPlayer.transform.position.x > bounds.rightBound || targetPlayer.transform.position.x < bounds.leftBound)
            targetPlayer.ResetTransformation(defaultScale);
    }
    public void HungryCountDown()//Countdown player lives
    {
        gameSceneController.HungryCountDown();
    }
    #endregion

    #region Movement
    public void Move()//Method movement player
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        targetPlayer.Rotate(Vector3.up * Horizontal * speedRotation * Time.deltaTime);
        targetPlayer.Translate(Vector3.forward * Vertical * (speedMovement + AccelerationSpeed) * Time.deltaTime);

    }
    public void SpeedAnimation(float Horizontal, float Vertical)//Change animation speed on dependency movement player
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

    public void Acceleration()//Acceleration player
    {
        if (Input.GetKeyDown(accelerationKey))
            AccelerationSpeed = CulculationAcceleration();
        else if (Input.GetKeyUp(accelerationKey))
            AccelerationSpeed = 0f;
    }

    private float CulculationAcceleration()//Set acceleration speed
    {
        if (accelerationType == AccelerationType.Slow)
        {
            AccelerationSpeed = 0.25f * speedMovement;
        }
        if (accelerationType == AccelerationType.Medium)
        {
            AccelerationSpeed = 0.5f * speedMovement;
        }
        if (accelerationType == AccelerationType.Fast)
        {
            AccelerationSpeed = speedMovement;
        }
        return AccelerationSpeed;
    }

    #endregion

    #region Replay Player
    public void RecordReplay()//Method "Replay". Returning position and state game objects back out to some time
    {
        gameSceneController.RecordReplay();
    }

    #endregion

    #endregion

    #region Handle Input
    public void RunToIdle()//Change run on idle state
    {
        if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f)
        {
            TransitionToState(IdleState);
        }
    }
    public void IdleToRun()//Change idle on run state
    {
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            TransitionToState(RunState);
        }
    }

    public void ToRepley()//Change any state on replay state
    {
        if (Input.GetKeyDown(gameSceneController.replayKey))
        {
            TransitionToState(ReplayState);
            gameSceneController.StartReplay();
            gameSceneController.ReplayWindow_OnAction(true);
        }
    }
    public void ReplayToIdle()//Change replay on idle state
    {
        if (Input.GetKeyUp(gameSceneController.replayKey))//Stop "Replay"
        {
            TransitionToState(IdleState);
            gameSceneController.StopReplay();
            gameSceneController.ReplayWindow_OnAction(false);
        }
        else if (!gameSceneController.ReplayOn)
        {
            TransitionToState(IdleState);
        }
    }

    #endregion

    #region Observer Action
    public void Notify()
    {
        TransitionToState(GameOverState);//Change player state
    }
    #endregion
}
