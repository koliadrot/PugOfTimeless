using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameOverState : PlayerBaseState
{
    public override void EnterState(PlayerController player)//Method used on start, when changed player state
    {
        player.GameOver();
    }

    public override void HandleInput(PlayerController player)//Method used, when player input buttons for change player state
    {

    }

    public override void LogicUpdate(PlayerController player)//Update methods on current player state
    {

    }
    public override void OnTriggerEnter(PlayerController player, Collider collider)
    {

    }
}
