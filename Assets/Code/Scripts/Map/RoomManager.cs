using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour {
    private static RoomManager roomManager;
    
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject statusContainer;
    [SerializeField] private TextMeshProUGUI nextActionTxt;
    [SerializeField] private TextMeshProUGUI turnTxt;

    [SerializeField] private Button finishMoveBtn;
    [SerializeField] private Button finishTurnBtn;
    
    private NextAction nextAction;
    private int turnCount = 1;
    public List<Vector2> availableCells = new List<Vector2>();
    private DoActionManager doActionManager = new DoActionManager();
    private LoadActionManager loadActionManager = new LoadActionManager();

    private void Awake() {
        roomManager = this;
        playerData.gameObject.SetActive(true);
        nextAction = NextAction.IDLE;
        nextActionTxt.text = $"Next Action: {nextAction}";
        turnTxt.text = $"Turn: {turnCount}";
        
        loadActionManager.initLoadActionManager();
        doActionManager.initDoActionManager();
    }

    private void Start() {
        finishMoveBtn.interactable = false;
        playerData.updatePosToCellPos(playerData.currentCell, true);
    }

    public PlayerData getPlayerData() {
        return roomManager.playerData;
    }

    public static void doAction(Vector2 _finalCell, Action _onEnd = null) {
        if(roomManager.nextAction == NextAction.IDLE)
            return;
        
        if(roomManager.doActionManager.doAction(roomManager, _finalCell, roomManager.nextAction))
            _onEnd?.Invoke();
    }

    public void SetNextAction(int _action) {
        if ((int) nextAction == _action && availableCells.Any()) {
            unloadAvailablePositions();
            return;
        }
        
        nextAction = (NextAction) _action;
        nextActionTxt.text = $"Next Action: {nextAction}";
        loadAction(nextAction);
    }

    private void loadAction(NextAction _action) {
        if (_action == NextAction.IDLE) return;
        
        var _intAction = (int) _action;
        var _range = GameConfig.basicMovements[_intAction].range;
        loadActionManager.loadAction(roomManager, _range, GameConfig.basicMovements[_intAction].rangeType, _intAction);
    }

    public void unloadAvailablePositions() {
        foreach (var _cell in availableCells) 
            Map.MapInfo.mapTiles.First(_c => _c.gridPosition.Equals(_cell)).upSide.GetComponent<SpriteRenderer>().color = Color.white;
        availableCells.Clear();
    }

    private static string statusToString(State _state) {
        switch (_state) {
            case State.NONE: return "none";
            case State.BURNT: return "burnt";
            case State.FROZEN: return "frozen";
            case State.PARALIZED: return "paralized";
            case State.POISONED: return "poisoned";
            case State.CONFUSED: return "confused";
            case State.TAUNTED: return "taunted";
            default:
                throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
        }
    }
}