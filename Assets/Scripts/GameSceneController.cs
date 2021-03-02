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
    public event Action<bool> ReplayControl = (bool replayStatus) => { };
    public event Action<bool> DamageControl = (bool damageStatus) => { };
    public event Action GamePlaySubscribe = () => { };

    [Header("Food Spawn")]
    [SerializeField] private float xFoodRange = 23f;
    [SerializeField] private float zFoodRange = 23f;
    [SerializeField] private float startFoodDelay = 2;
    [SerializeField] private string foodTag = "Food";
    [SerializeField] private float foodHealth = 0.25f;
    private bool _foodStatus;

    [Header("Enemy Spawn")]
    [SerializeField] private float xEnemyRange = 23f;
    [SerializeField] private float zEnemyRange = 23f;
    [SerializeField] private float startEnemyDelay = 2;
    [SerializeField] private float delayRespawn = 1.5f;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("GamePlay")]
    [SerializeField] private CinemachineVirtualCamera cinemachine;
    [HideInInspector] public bool replayOn = false;
    [HideInInspector] public float difficulty = 10f;
    public KeyCode replayKey = KeyCode.R;
    private List<IObserverable> endGameObservers;
    private int _totalPoints;
    private float _lifes = 1f;
    private bool _startGame = false;

    [Header("Record Targets")]
    public Transform playerTransform;
    public Animator playerAnimator;
    [HideInInspector] public Transform lastFood;
    [HideInInspector] public Vector3 lastPositionFood;
    [HideInInspector] public List<Vector3> enemiesPosition;
    [HideInInspector] public List<Transform> enemiesTransform;

    [Header("Audio")]
    [SerializeField] private AudioSource sourceAuxiliary;
    [SerializeField] private AudioSource sourceMainTheme;

    public int TotalPoints //Update full total poits and Health Bar
    {
        get => _totalPoints;
        set
        {
            _totalPoints = value;
            ScoreUpdateOnEat(TotalPoints);//Update total points
            LifeUp(foodHealth);//Add health and update health bar
        }
    }

    public float Lifes//Control player life
    {
        get => _lifes;
        set
        {
            _lifes = value;
            LifeControl(Lifes);//Event of update health bar
            Death();//When player has not life`s anymore
        }
    }
    public bool Food { get => _foodStatus; set => _foodStatus = value; }
    public bool StartGame//Subscribe methods on event "GamePlay" of Pug class
    {
        get => _startGame;
        set
        {
            _startGame = value;
            if (StartGame) GamePlaySubscribe();
        }
    }

    #endregion

    #region Startup
    protected override void Awake()
    {
        base.Awake();
        endGameObservers = new List<IObserverable>();
        DamageControl += CinemachineNoiseDamage;
        ReplayControl += TimeSound;
    }
    private void Start()
    {
        StartCoroutine(SpawnRandomFood());
        StartCoroutine(SpawnRandomEnemy());
    }
    public void CheckOnStartGame()//Check bool for start game
    {
        if (!StartGame) return;
    }
    #endregion

    #region Subject Implementation

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

    #region Player Life

    public void HungryCountDown()//Countdown player health per seconds
    {
        if (Lifes > 0 && !Input.GetKey(replayKey))
            Lifes -= Time.deltaTime / difficulty;
    }
    private void LifeUp(float foodHealthValue)//Add health point to player
    {
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
            sourceMainTheme.Stop();
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
    private void CinemachineNoiseDamage(bool statusNoise)//Visual effect camera noise when player get damage
    {
        if (statusNoise)
        {
            cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2f;
            cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 6f;
        }
        else
        {
            cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 1f;
            cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 1f;
        }

    }
    #endregion

    #region Food
    private void PizzaEated(int pointValue, Transform food, Vector3 position, AudioSource source, AudioClip clip)//Add point when player eat food
    {
        TotalPoints += pointValue;
        Food = true;
    }
    private void PizzaPosition(int pointValue, Transform food, Vector3 position, AudioSource source, AudioClip clip)//Set last position food when player eat food
    {
        lastFood = food;
        lastPositionFood = position;
    }
    private void PizzaSound(int pointValue, Transform food, Vector3 position, AudioSource source, AudioClip clip)// Sound effect when player eat food
    {
        source.PlayOneShot(clip);
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

    #region Audio
    private void TimeSound(bool sound)//Sound effect when activate replay
    {
        if (sound)
        {
            sourceAuxiliary.Play();
            sourceMainTheme.Pause();
        }
        else
        {
            sourceAuxiliary.Stop();
            sourceMainTheme.Play();
        }

    }
    #endregion

    #region Spawning
    private IEnumerator SpawnRandomFood()//Routine spawn food in random position
    {
        yield return new WaitUntil(() => StartGame);//waiting, when user choice type of difficulty
        yield return new WaitForSeconds(startFoodDelay);
        while (true)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-xFoodRange, xFoodRange), 0f, UnityEngine.Random.Range(-zFoodRange, zFoodRange));
            PooledSpawnObject(spawnPosition, foodTag);
            Food = false;
            yield return new WaitUntil(() => Food);
        }
    }
    private IEnumerator SpawnRandomEnemy()//Routine spawn enemy in random position
    {
        yield return new WaitUntil(() => StartGame);//waiting, when user choice type of difficulty
        yield return new WaitForSeconds(startEnemyDelay);
        while (true)
        {
            yield return new WaitWhile(() => replayOn);
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-xEnemyRange, xEnemyRange), 0, zEnemyRange);
            PooledSpawnObject(spawnPosition, enemyTag);
            yield return new WaitForSeconds(delayRespawn);
        }
    }

    private void PooledSpawnObject(Vector3 spawnPosition, string tag)//Main methods, where routine get necessary prefab and set property for it
    {
        // Get an object object from the pool
        GameObject pooledObject = ObjectPooler.Instance.GetPooledObject(tag);
        if (pooledObject != null)
        {
            pooledObject.transform.position = spawnPosition;
            pooledObject.SetActive(true);
            if (pooledObject.CompareTag(foodTag))//Check prefab used own tag
            {
                FoodController foodObject = pooledObject.GetComponent<FoodController>();//Set necessary property and events
                foodObject.FoodEated += PizzaEated;
                foodObject.FoodEated += PizzaPosition;
                foodObject.FoodEated += PizzaSound;
                foodObject.source = sourceAuxiliary;
                AddObserver(foodObject);// Subscribe on observer
            }
            else if (pooledObject.CompareTag(enemyTag))
            {
                EnemyController enemyObject = pooledObject.GetComponent<EnemyController>();
                enemyObject.DamageEffect += CollisionPlayer;
                enemyObject.DamageEffect += enemyObject.SoundDamage;
                AddObserver(enemyObject);
            }

        }
    }

    #endregion

    #endregion
}
