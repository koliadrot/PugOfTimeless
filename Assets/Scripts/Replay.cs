using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayer : ICommandable
{
    private Transform player;
    private Vector3 position;
    private Quaternion rotation;
    private Animator animator = null;
    private float speedAnimation;
    private float lifes = -999;//999 - Protect from don't need request
    private int points = -999;//999 - Protect from don't need request

    //Structure only: position
    public ReplayPlayer(Transform player, Vector3 position, Quaternion rotation)
    {
        this.player = player;
        this.position = position;
        this.rotation = rotation;
    }
    //Structure only: position,animation
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float speedAnimation, Animator animator) : this(target, position, rotation)
    {
        this.speedAnimation = speedAnimation;
        this.animator = animator;
    }
    //Structure only: position,lifes
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float lifes) : this(target, position, rotation)
    {
        this.lifes = lifes;
    }
    //Structure only: position,points
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, int points) : this(target, position, rotation)
    {
        this.points = points;
    }
    //Structure only: position,animation,lifes
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float speedAnimation, Animator animator, float lifes) : this(target, position, rotation, speedAnimation, animator)
    {
        this.lifes = lifes;
    }
    //Structure only: position,animation,points
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float speedAnimation, Animator animator, int points) : this(target, position, rotation, speedAnimation, animator)
    {
        this.points = points;
    }
    //Structure only: position,lifes,points
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float lifes, int points) : this(target, position, rotation, lifes)
    {
        this.points = points;
    }
    //Structure only: position,animation,lifes,points
    public ReplayPlayer(Transform target, Vector3 position, Quaternion rotation, float speedAnimation, Animator animator, float lifes, int points) : this(target, position, rotation, speedAnimation, animator, lifes)
    {
        this.points = points;
    }

    public void Replay()//Replay position,rotation,speed animation, lifes and points player's
    {
        player.position = position;
        player.rotation = rotation;
        if (animator != null) animator.SetFloat("Speed_f", speedAnimation);
        if (lifes != -999) GameSceneController.Instance.Lifes = lifes;
        if (points != -999) GameSceneController.Instance.TotalPoints = points;
    }

}
public class ReplayFood : ICommandable
{
    private Transform food;
    private Vector3 position;

    public ReplayFood(Transform food, Vector3 position)
    {
        this.food = food;
        this.position = position;
    }

    public void Replay()//Replay position food
    {
        food.position = position;
    }

}
public class ReplayEnemy : ICommandable
{
    private List<Transform> enemies = new List<Transform>();
    private List<Vector3> positions = new List<Vector3>();
    private List<bool> states = new List<bool>();
    public ReplayEnemy(List<Transform> enemies, List<Vector3> positions)
    {
        foreach (Transform enemy in enemies)
        {
            this.enemies.Add(enemy);
        }
        foreach (Vector3 position in positions)
        {
            this.positions.Add(position);
        }
        foreach (var state in enemies)
        {
            states.Add(state.gameObject.activeSelf);
        }
    }

    public void Replay()//Replay position enemies
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].position = positions[i];
            enemies[i].gameObject.SetActive(states[i]);
        }
    }
}
