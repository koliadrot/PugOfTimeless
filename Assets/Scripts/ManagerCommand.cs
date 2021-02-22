using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagerCommand : MonoBehaviour
{
    #region Field Declarations
    
    private static ManagerCommand _instance=null;
    private static readonly object threadlock = new object();
    public static ManagerCommand Instance
    {
        get
        {
            lock (threadlock)
            {
                if (_instance == null)
                    _instance = new ManagerCommand();

                return _instance;
            }
        }
    }

    private List<List<ICommandable>> _commandBuffer = new List<List<ICommandable>>();
    private List<ICommandable> command;
    private GameSceneController gameSceneController;

    [SerializeField] private int valueFrameTime = 250;

    #endregion
    
    #region Startup
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        gameSceneController = GameSceneController.Instance;
    }

    #endregion

    #region Subject Implementation

    #region Command
    public void AddCommand(List<ICommandable> command)//Add command include player, food, enemy parameters
    {
        CommandBuffer.Add(command);
    }
    public List<List<ICommandable>> CommandBuffer//Property command list
    {
        get
        {
            if (_commandBuffer.Count <= valueFrameTime)
            {
                gameSceneController.ReplayTime_OnAction(_commandBuffer.Count / (float)valueFrameTime);//Fill "replay" bar
            }
            if (_commandBuffer.Count > valueFrameTime)//Control size command list
                _commandBuffer.RemoveAt(0);
            return _commandBuffer;
        }
    }
    #endregion

    #region Record
    public void Replay()//Start "replay" 
    {
        StartCoroutine(ReplayRoutine());
    }

    public void StopReplay()//Stop "replay" 
    {
        if (gameSceneController.replayOn)
        {
            StopAllCoroutines();
            ClearRecord();
            gameSceneController.replayOn = false;
        }
    }

    public void RecordReplay()//Record command for "replay"
    {
        SetEnemyList();
        command = new List<ICommandable>();
        command.Add(new ReplayPlayer(gameSceneController.playerTransform, gameSceneController.playerTransform.position, gameSceneController.playerTransform.rotation, gameSceneController.playerAnimator.GetFloat("Speed_f"), gameSceneController.playerAnimator, gameSceneController.Lifes, gameSceneController.TotalPoints));
        command.Add(new ReplayFood(gameSceneController.lastFood, gameSceneController.lastPositionFood));
        command.Add(new ReplayEnemy(gameSceneController.enemiesTransform, gameSceneController.enemiesPosition));
        AddCommand(command);
    }
    private void ClearRecord()//Clear "replay" list command
    {
        CommandBuffer.Clear();
    }

    private void SetEnemyList()//Record enemy command for "replay"
    {
        gameSceneController.ResetTransformAndPosition();
        foreach (GameObject objectPooler in ObjectPooler.SharedInstance.pooledObjects)
        {
            gameSceneController.SetTransformAndPosition(objectPooler.transform, objectPooler.tag);
        }
    }

    private IEnumerator ReplayRoutine()//Routine "replay"
    {
        gameSceneController.replayOn = true;
        while (gameSceneController.replayOn)//Operation when player use "replay"
        {
            var command = CommandBuffer[CommandBuffer.Count - 1];
            foreach (var _command in command)
            {
                _command.Replay();//Start "replay" each type
            }
            CommandBuffer.Remove(command);
            gameSceneController.ReplayTime_OnAction(_commandBuffer.Count / valueFrameTime);//Fill "replay" bar
            if (CommandBuffer.Count == 0)//Stop "replay"
            {
                StopReplay();
                gameSceneController.ReplayWindow_OnAction(false);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    #endregion
    
    #endregion
}
