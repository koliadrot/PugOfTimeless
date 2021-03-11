using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerController player);//Method used on start, when changed player state

    public abstract void LogicUpdate(PlayerController player);//Method used, when player input buttons for change player state

    public abstract void HandleInput(PlayerController player);//Update methods on current player state
    
    public abstract void OnTriggerEnter(PlayerController player, Collider collider);//Method used, when player collider collision with other colliders 

}
