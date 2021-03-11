using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameSceneController : Singleton<GameSceneController>
{
    #region Field Declarations
    public event Action<int> ScoreUpdateOnEat = (int score) => { };
    public event Action<float> LifeControl = (float life) => { };
    public event Action<float> ReplayTimeControl = (float time) => { };
    public event Action<bool> FoodControl = (bool foodStatus) => { };
    public event Action<bool> ReplayControl = (bool replayStatus) => { };
    public event Action<bool> DamageControl = (bool damageStatus) => { };

    [Header("Environment")]
    [SerializeField] private Transform ground;
    public float GroundX => Mathf.Pow(ground.localScale.x, 2f) * 0.9f;
    public float GroundZ => Mathf.Pow(ground.localScale.z, 2f) * 0.9f;

    [Header("Food")]
    [SerializeField] private string foodTag = "Food";
    [SerializeField] private float foodHealth = 0.25f;
    private bool food;

    [Header("Enemy")]
    [SerializeField] private float startDelayRespawn = 2;
    [SerializeField] private float delayRespawn = 1.5f;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Player")]
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private float timeReplay = 4;
    public KeyCode replayKey = KeyCode.R;
    public bool ReplayOn { get; set; }
    public float Difficulty { get; set; }
    public CinemachineBasicMultiChannelPerlin MainCamera => mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    private List<IObserverable> endGameObservers = new List<IObserverable>();
    private List<List<ICommandable>> commandBuffer = new List<List<ICommandable>>();
    private List<ICommandable> command = new List<ICommandable>();
    private float valueFrameTime;
    private int totalPoints;
    private float lifes = 1f;

    [Header("Pool Objects")]
    [SerializeField] private List<Pools> pools = new List<Pools>();
    [SerializeField] private Dictionary<string, List<GameObject>> pooledObjects = new Dictionary<string, List<GameObject>>();

    [Header("Record Targets")]
    public Transform playerTransform;//--->It need manual fill field for observe object on per frame
    public Animator playerAnimator;//<---
    private List<Vector3> enemiesPosition = new List<Vector3>();//--->It auto fill field after spawn object for observe object on per frame
    private List<Transform> enemiesTransform = new List<Transform>();//<---
    private Transform foodTransform;//It auto fill field after spawn object for observe object on per frame
    public Transform FoodTransform => foodTransform;

    public int TotalPoints //Update full total poits and Health Bar
    {
        get => totalPoints;
        set
        {
            totalPoints = value;
            ScoreUpdateOnEat(TotalPoints);//Update total points
            LifeUp(foodHealth);//Add health and update health bar
        }
    }

    public float Lifes//Control player life
    {
        get => lifes;
        set
        {
            lifes = value;
            LifeControl(Lifes);//Event of update health bar
            Death();//When player has not life`s anymore
        }
    }

    [System.Serializable]
    private class Pools //Pools Objects divided on tags
    {
        public string tag;
        public List<Pool> pool;

        [System.Serializable]
        public class Pool
        {
            public string name;
            public GameObject prefab;
            public int size;
        }
    }
    #endregion

    #region Startup
    protected override void Awake()
    {
        base.Awake();
        DamageControl += CameraNoiseEffect;
    }
    private void Start()
    {
        CreatePool();
        StartCoroutine(SpawnRandomFood());
        StartCoroutine(SpawnRandomEnemy());
        valueFrameTime = 60 * timeReplay;//Set max list size for record replay. 1 second = 60 frame.
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
            if (commandBuffer.Count <= valueFrameTime)
            {
                ReplayTime_OnAction(commandBuffer.Count / valueFrameTime);//Fill "replay" bar
            }
            if (commandBuffer.Count > valueFrameTime)//Control size command list
                commandBuffer.RemoveAt(0);
            return commandBuffer;
        }
    }
    #endregion

    #region Observer
    public void AddObserver(IObserverable observer)//Add observable subscriber
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IObserverable observer)//Remove observable subscriber
    {
        endGameObservers.Remove(observer);
    }

    private void NotifyObservers()//Send notification about start action observables subscribers
    {
        foreach (IObserverable observer in endGameObservers)
        {
            observer.Notify();
        }
    }
    #endregion

    #region CallBack OnAction

    public void ReplayWindow_OnAction(bool actionStatus)//Wrapped event of replay window
    {
        ReplayControl(actionStatus);
    }

    public void Damage_OnAction(bool actionStatus)//Wrapped event of damage control
    {
        DamageControl(actionStatus);
    }
    public void ReplayTime_OnAction(float time)//Wrapped event of replay time bar
    {
        ReplayTimeControl(time);
    }
    #endregion

    #region Player

    public void HungryCountDown()//Countdown player health per seconds
    {
        if (Lifes > 0 && !ReplayOn)
            Lifes -= Time.deltaTime / Difficulty;
    }
    private void LifeUp(float foodHealthValue)//Add health point to player
    {
        if (ReplayOn)//Check Replay State 
            return;
        Lifes += foodHealthValue;
        Lifes = Lifes > 1f ? 1f : Lifes;
        LifeControl(Lifes);
    }
    private void Death()//Check amount player life for end game
    {
        if (Lifes < 0f)
        {
            StopAllCoroutines();
            ReplayWindow_OnAction(false);
            Damage_OnAction(false);
            NotifyObservers();
        }
    }
    public void CollisionPlayer(float damage)//Damage of player when collision with enemy
    {
        Lifes -= damage;
        StartCoroutine(DamageEffect());
    }
    private IEnumerator DamageEffect()//Visual effect damage player
    {
        Damage_OnAction(true);
        yield return new WaitForSeconds(0.2f);
        Damage_OnAction(false);

    }
    private void CameraNoiseEffect(bool statusNoise)//Visual effect camera noise when player get damage with use cinemachine
    {
        if (statusNoise)
        {
            MainCamera.m_AmplitudeGain = 2f;
            MainCamera.m_FrequencyGain = 6f;
        }
        else
        {
            MainCamera.m_AmplitudeGain = 1f;
            MainCamera.m_FrequencyGain = 1f;
        }
    }
    #endregion

    #region Food
    private void PizzaEated(int pointValue)//Add point when player eat food
    {
        FoodControl(true);
        TotalPoints += pointValue;
        food = true;
    }
    #endregion

    #region Enemy
    public void SetTransformAndPosition(Transform enemy, string tag)//Fill enemy list  position`s
    {
        if (enemy.CompareTag(tag))
        {
            enemiesTransform.Add(enemy);
            enemiesPosition.Add(enemy.position);
        }
    }
    public void ResetTransformAndPosition()//Clear enemy list  position`s
    {
        enemiesTransform.Clear();
        enemiesPosition.Clear();
    }


    #endregion

    #region Spawn Objects
    private IEnumerator SpawnRandomFood()//Routine spawn food in random position
    {
        while (true)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-GroundX, GroundX), 0f, UnityEngine.Random.Range(-GroundZ, GroundZ));
            PooledSpawnObject(spawnPosition, foodTag);
            food = false;
            yield return new WaitUntil(() => food);
        }
    }

    private IEnumerator SpawnRandomEnemy()//Routine spawn enemy in random position
    {
        yield return new WaitForSeconds(startDelayRespawn);
        while (true)
        {
            yield return new WaitWhile(() => ReplayOn);
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-GroundX, GroundX), 0f, GroundZ);
            PooledSpawnObject(spawnPosition, enemyTag);
            yield return new WaitForSeconds(delayRespawn);
        }
    }


    private void PooledSpawnObject(Vector3 spawnPosition, string tag)//Main methods, where routine get necessary prefab and set property for it
    {
        // Get an object object from the pool
        GameObject pooledObject = GetPooledObject(tag);
        if (pooledObject != null)
        {
            pooledObject.transform.position = spawnPosition;
            pooledObject.SetActive(true);
            if (pooledObject.CompareTag(foodTag))//Check prefab used own tag
            {
                FoodController foodObject = pooledObject.GetComponent<FoodController>();//Set necessary property and events
                foodObject.FoodEated += PizzaEated;
                //foodObject.Source = sourceAuxiliary;
                AddObserver(foodObject);//Subscribe on observer
                foodTransform = pooledObject.transform;
            }
            else if (pooledObject.CompareTag(enemyTag))
            {
                EnemyController enemyObject = pooledObject.GetComponent<EnemyController>();
                enemyObject.DamageEffect += CollisionPlayer;
                AddObserver(enemyObject);//Subscribe on observer
            }

        }
    }

    public GameObject GetPooledObject(string tag)// Loop through list of pooled objects,deactivating them and adding them to the list 
    {
        foreach (GameObject poolToObject in pooledObjects[tag])
        {
            if (!poolToObject.activeSelf)
            {
                return poolToObject;
            }
        }
        return null;
    }
    private void CreatePool()//Create pool spawn objects
    {
        foreach (var pool in pools)
        {
            foreach (var objectToPool in pool.pool)
            {
                for (int i = 0; i < objectToPool.size; i++)
                {
                    GameObject obj = Instantiate(objectToPool.prefab);
                    if (!pooledObjects.ContainsKey(pool.tag))
                    {
                        pooledObjects.Add(pool.tag, new List<GameObject>());
                    }
                    pooledObjects[pool.tag].Add(obj);
                    obj.SetActive(false);
                    obj.transform.SetParent(this.transform); // set as children of Spawn Manager
                }
                pooledObjects[pool.tag].Shuffle();
            }
        }
    }
    #endregion

    #region Record Command
    public void StartReplay()//Start "replay" 
    {
        StartCoroutine("ReplayRoutine");
    }
    public void StopReplay()//Stop "replay" 
    {
        if (ReplayOn)
        {
            StopCoroutine("ReplayRoutine");
            ClearRecord();
            ReplayOn = false;
        }
    }
    private void ClearRecord()//Clear "replay" list command
    {
        CommandBuffer.Clear();
    }
    public void RecordReplay()//Record command for "replay"
    {
        SetEnemyList(enemyTag);
        command = new List<ICommandable>();
        command.Add(new ReplayPlayer(playerTransform, playerTransform.position, playerTransform.rotation, playerAnimator.GetFloat("Speed_f"), playerAnimator, Lifes, TotalPoints));
        command.Add(new ReplayFood(foodTransform, foodTransform.position));
        command.Add(new ReplayEnemy(enemiesTransform, enemiesPosition));
        AddCommand(command);
    }
    private void SetEnemyList(string tag)//Record enemy command for "replay"
    {
        ResetTransformAndPosition();
        foreach (GameObject objectPooler in pooledObjects[tag])
        {
            SetTransformAndPosition(objectPooler.transform, objectPooler.tag);
        }
    }
    private IEnumerator ReplayRoutine()//Routine "replay"
    {
        ReplayOn = true;
        while (ReplayOn)//Operation when player use "replay"
        {
            var command = CommandBuffer[CommandBuffer.Count - 1];
            foreach (var _command in command)
            {
                _command.Replay();//Start "replay" each type
            }
            CommandBuffer.Remove(command);
            ReplayTime_OnAction(commandBuffer.Count / valueFrameTime);//Fill "replay" bar
            if (CommandBuffer.Count == 0)//Stop "replay"
            {
                StopReplay();
                ReplayWindow_OnAction(false);

            }
            yield return new WaitForEndOfFrame();
        }
    }

    #endregion

    #endregion
}
