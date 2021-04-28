using UnityEngine;

public interface AI : Target {
    NextAction loadNextAction(RoomManager _roomManager);
    Vector2 loadFinalCell(RoomManager _roomManager);
}