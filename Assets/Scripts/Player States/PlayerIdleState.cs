using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerController player)//Method used on start, when changed player state
    {
        player.AccelerationSpeed = 0f;//Set zero acceleration
        player.SpeedAnimation(0f, 0f);//Set idle speed
    }

    public override void HandleInput(PlayerController player)//Method used, when player input buttons for change player state
    {
        player.IdleToRun();
        player.ToRepley();
    }

    public override void LogicUpdate(PlayerController player)//Update methods on current player state
    {
        player.RecordReplay();
        player.HungryCountDown();
        player.Navigation();
    }

    public override void OnTriggerEnter(PlayerController player, Collider collider)
    {
        if (collider.CompareTag("Wall"))
        {
            player.ResetPlayerTransform();
        }
    }
}
