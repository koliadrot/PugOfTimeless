using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RailWay : MonoBehaviour
{
    #region Field Declaration

    [Header("Movement")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform pizza;
    private const float speedMove = 0.2f;
    private const float speedRotate = 0.03f;//Choosed optimal speed rotation, which it will consider with speed movement target object
    private List<Transform> pointsDestination = new List<Transform>();

    [Header("Animation")]
    [SerializeField] private Animator targetAnimator;

    #endregion

    #region Startup
    private void OnEnable()
    {
        pointsDestination = transform.GetComponentsInChildren<Transform>().ToList();//Fill list of destination points due to children objects.Default added in list<Transform> with position = Vector3(0f,0f,0f)

        StartCoroutine(RailWayMovement());//Started
    }
    #endregion
   
    #region Subject Implementation

    IEnumerator RailWayMovement()//Move target object of type "railway"
    {
        while (true)//Circle railway movement
        {
            pointsDestination.Shuffle();//Random shuffle list
            foreach (Transform point in pointsDestination)
            {
                pizza.position = point.position;//Change positon pizza
                targetAnimator.SetFloat("Speed_f", 0.7f);//Speed rotation animation

                Quaternion newRotation = Quaternion.LookRotation(point.position - target.position);//Set angle rotate between target and destination points
                while (Mathf.Abs(Mathf.Round(target.rotation.eulerAngles.y)) != Mathf.Abs(Mathf.Round(newRotation.eulerAngles.y)))//Circle rotation between target and destination points
                {
                    target.rotation = Quaternion.Slerp(target.rotation, newRotation, speedRotate);//Rotation. For smoothly rotation, necessary multiply "Time.timeDelta" on "speedRotation"
                    yield return new WaitForFixedUpdate();//Update circle
                }
                targetAnimator.SetFloat("Speed_f", 1f);//Speed movement animation

                while (point.localPosition != target.position)//Circle movement from start point to next point destination
                {
                    target.position = Vector3.MoveTowards(target.position, point.localPosition, speedMove);//Movement
                    yield return new WaitForFixedUpdate();//Update circle
                }
            }
        }
    }
    #endregion
}
