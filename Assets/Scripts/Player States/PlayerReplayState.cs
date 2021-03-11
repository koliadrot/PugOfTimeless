using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReplayState : PlayerBaseState
{
    public override void EnterState(PlayerController player)//Method used on start, when changed player state
    {
        player.AccelerationSpeed = 0f;//Set zero acceleration
    }

    public override void HandleInput(PlayerController player)//Method used, when player input buttons for change player state
    {
        player.ReplayToIdle();
    }

    public override void LogicUpdate(PlayerController player)//Update methods on current player state
    {
        player.Navigation();
    }
    public override void OnTriggerEnter(PlayerController player, Collider collider)
    {

    }
}
