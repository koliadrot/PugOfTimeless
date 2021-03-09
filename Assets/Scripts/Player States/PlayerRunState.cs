using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public override void EnterState(PlayerController player)//Method used on start, when changed player state
    {

    }

    public override void HandleInput(PlayerController player)//Method used, when player input buttons for change player state
    {
        player.RunToIdle();
        player.ToRepley();
    }

    public override void LogicUpdate(PlayerController player)//Update methods on current player state
    {
        player.Move();
        player.SpeedAnimation(player.Horizontal, player.Vertical);
        player.BoundsMap();
        player.Acceleration();
        player.RecordReplay();
        player.HungryCountDown();
    }
}
