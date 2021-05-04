using System.Collections;
using System.Linq;
using UnityEngine;

public class MoveState : AIState {

    private Vector2 finalPos;
    
    public MoveState(AI _ai) : base(_ai) { }
    
    protected override IEnumerator loadNextMove(RoomManager _roomManager) {
        var _range = GameConfig.basicMovements[(int)_roomManager.NextAction].range;
        var _rangeType = GameConfig.basicMovements[(int) _roomManager.NextAction].rangeType;
        _roomManager.LoadActionManager.loadAction(_roomManager, _range, _rangeType, (int)_roomManager.NextAction);
        _roomManager.clearCellColor(); // So AI doesn't show its available cells
        finalPos = ai.loadFinalCell(_roomManager);
        return null;
    }

    public override IEnumerator execute(RoomManager _roomManager) {
        Debug.Log("Executing Move State");
        yield return loadNextMove(_roomManager);
        var _finishedExecution = false;

        if (_roomManager.availableCells.Any())
            _roomManager.DoActionManager.doAction(_roomManager, finalPos, _roomManager.NextAction, () => { _finishedExecution = true; });
        else
            _finishedExecution = true;
        
        yield return new WaitWhile(() => !_finishedExecution);
        ai.CurrentAIState = new IdleState(ai);
        yield return ai.CurrentAIState.execute(_roomManager);
    }
}