using System.Collections;
using UnityEngine;

public interface AI : Target {
    NextAction loadNextAction(RoomManager _roomManager);
    Vector2 loadFinalCell(RoomManager _roomManager);

    IEnumerator startStateMachine(RoomManager _roomManager);
    AIState CurrentAIState { get; set; }
    
    GameObject GameObject { get; }
}