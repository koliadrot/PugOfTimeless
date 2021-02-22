using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayer : ICommandable
{
    private Transform _player;
    private Vector3 _position;
    private Quaternion _rotation;
    private Animator _animator = null;
    private float _speedAnimation;
    private float _lifes = -100;
    private int _points = -100;

    //Structure only: position
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation)
    {
        this._player = target;
        this._position = position;
        this._rotation = rotation;
    }
    //Structure only: position,animation
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float speedAnimation, Animator animator) : this(target, position, rotation)
    {
        this._animator = animator;
        this._speedAnimation = speedAnimation;
    }
    //Structure only: position,lifes
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float lifes) : this(target, position, rotation)
    {
        this._lifes = lifes;
    }
    //Structure only: position,points
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, int points) : this(target, position, rotation)
    {
        this._points = points;
    }
    //Structure only: position,animation,lifes
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float speedAnimation, Animator animator, float lifes) : this(target, position, rotation, speedAnimation, animator)
    {
        this._lifes = lifes;
    }
    //Structure only: position,animation,points
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float speedAnimation, Animator animator, int points) : this(target, position, rotation, speedAnimation, animator)
    {
        this._points = points;
    }
    //Structure only: position,lifes,points
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float lifes, int points) : this(target, position, rotation, lifes)
    {
        this._points = points;
    }
    //Structure only: position,animation,lifes,points
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float speedAnimation, Animator animator, float lifes, int points) : this(target, position, rotation, speedAnimation, animator, lifes)
    {
        this._points = points;
    }

    public void Replay()//Replay position,rotation,speed animation, lifes and points player's
    {
        _player.position = _position;
        _player.rotation = _rotation;
        if (_animator != null) _animator.SetFloat("Speed_f", _speedAnimation);
        if (_lifes != -100) GameSceneController.Instance.Lifes = _lifes;
        if (_points != -100) GameSceneController.Instance.TotalPoints = _points;
    }

}
public class ReplayFood : ICommandable
{
    private Transform _food;
    private Vector3 _position;

    public ReplayFood(Transform food, Vector3 position)
    {
        this._food = food;
        this._position = position;
    }

    public void Replay()//Replay position food
    {
        _food.position = _position;
    }

}
public class ReplayEnemy : ICommandable
{
    private List<Transform> _enemies = new List<Transform>();
    private List<Vector3> _positions = new List<Vector3>();
    private List<bool> _states = new List<bool>();
    public ReplayEnemy(List<Transform> enemies, List<Vector3> positions)
    {
        foreach (Transform enemy in enemies)
        {
            this._enemies.Add(enemy);
        }
        foreach (Vector3 position in positions)
        {
            this._positions.Add(position);
        }
        foreach (var state in enemies)
        {
            _states.Add(state.gameObject.activeSelf);
        }
    }

    public void Replay()//Replay position enemies
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].position = _positions[i];
            _enemies[i].gameObject.SetActive(_states[i]);
        }
    }
}
