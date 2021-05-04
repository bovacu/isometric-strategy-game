using System.Collections;

public abstract class AIState {

   protected AI ai;

   protected AIState(AI _ai) {
      ai = _ai;
   }

   protected AIState getStateByAction(NextAction _nextAction) {
      switch (_nextAction) {
         case NextAction.IDLE: return new IdleState(ai);
         case NextAction.MOVE: return new MoveState(ai);
         case NextAction.MELEE: return new MeleeState(ai);
         case NextAction.RANGE: return null;
         case NextAction.DEFENSE: return null;
         case NextAction.SPECIAL: return null;
      }
      
      return null;
   }
   
   protected abstract IEnumerator loadNextMove(RoomManager _roomManager);
   public abstract IEnumerator execute(RoomManager _roomManager);
}