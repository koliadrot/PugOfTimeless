using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IObserverable
{
    #region Field Declarations

    [Header("Player Parameters")]
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private Transform navigationArrow;
    [SerializeField] private float speedMovement = 10f;
    [SerializeField] private float speedRotation = 100f;
    [SerializeField] private float smoothlyNavigation = 8f;
    [SerializeField] private AccelerationType accelerationType = AccelerationType.Fast;
    [SerializeField] private KeyCode accelerationKey = KeyCode.LeftShift;
    [SerializeField] private Animator animator;
    [SerializeField] private float defaultPlayerScale = 10f;
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
    private enum AccelerationType
    {
        Slow,
        Medium,
        Fast,
    }
    public float Horizontal { get; set; }
    public float Vertical { get; set; }

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
    public void HungryCountDown()//Countdown player lives
    {
        gameSceneController.HungryCountDown();
    }
    public void Navigation()//Help player with navigation searching pizza
    {
        //navigationArrow.LookAt(gameSceneController.FoodTransform);//Navigation without smoothly movement
        navigationArrow.rotation = Quaternion.Slerp(navigationArrow.rotation, Quaternion.LookRotation(gameSceneController.FoodTransform.position - navigationArrow.position), Time.deltaTime * smoothlyNavigation);
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

    #region OnTrigger
    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(this, other);
    }
    public void ResetPlayerTransform()//Reset transform player position
    {
        targetPlayer.ResetTransformation(defaultPlayerScale);
    }
    #endregion

    #region Observer Action
    public void Notify()
    {
        TransitionToState(GameOverState);//Change player state
    }
    #endregion
}
