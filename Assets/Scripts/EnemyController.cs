using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IObserverable
{
    #region Field Declarations

    public event Action<float> DamageEffect = (float damageValue) => { };
    private GameSceneController gameSceneController;

    [Header("Spawn")]
    [SerializeField] private float xRange = 23f;
    [SerializeField] private float zRange = 23f;

    [Header("Movement")]
    [SerializeField] private float speed = 40f;

    [Header("GamePlay")]
    [SerializeField] [Range(0, 1)] private float damageToPlayer = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip collisionSound;

    #endregion

    #region Startup
    private void Start()
    {
        gameSceneController = GameSceneController.Instance;
    }
    #endregion

    #region Subject Implementation

    #region GamePlay
    // Update is called once per frame
    private void Update()
    {
        ForwardMove();
    }
    #endregion

    #region Movement
    void ForwardMove()//Moving enemy, when player don't activity "replay"
    {
        if (!gameSceneController.ReplayOn)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
    #endregion
   
    #region Audio
    public void SoundDamage(float nonValue)//Sound effect, when player collision with player
    {
        source.PlayOneShot(collisionSound);
    }
    #endregion

    #region Collosions
    private void OnTriggerEnter(Collider other)//Activate on trigger enter
    {
        if (!gameSceneController.ReplayOn)
        {
            if (other.gameObject.CompareTag("Wall"))//Respawning enemy without disable on start line
            {
                Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-xRange, xRange), 0, zRange);
                transform.position = spawnPosition;
            }
            if (other.gameObject.CompareTag("Player"))//Damage to player
            {
                DamageEffect(damageToPlayer);
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
