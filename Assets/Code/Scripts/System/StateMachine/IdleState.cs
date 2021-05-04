using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class IdleState : AIState {
    public IdleState(AI _ai) : base(_ai) { }
    
    protected override IEnumerator loadNextMove(RoomManager _roomManager) {
        _roomManager.NextAction = ai.loadNextAction(_roomManager);
        yield return null;
    }

    public override IEnumerator execute(RoomManager _roomManager) {
        Debug.Log("Executing Idle State");
        _roomManager.UserTarget = ai;
        
        yield return new WaitForSeconds(0.5f);
        
        if (_roomManager.UserTarget.getEnergy() <= 0) {
            ai.setEnergy(1000);
            yield return null;
        } else {
            yield return loadNextMove(_roomManager);
            ai.CurrentAIState = getStateByAction(_roomManager.NextAction);
            _roomManager.UserTarget = ai;
            yield return ai.CurrentAIState.execute(_roomManager);
        }
    }
}