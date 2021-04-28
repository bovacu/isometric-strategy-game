using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour {
    private static RoomManager roomManager;
    
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject statusContainer;
    [SerializeField] private TextMeshProUGUI nextActionTxt;
    [SerializeField] private TextMeshProUGUI turnTxt;
    [SerializeField] private Button finishTurnBtn;
    
    [Header("DebugButtons")]
    [SerializeField] private Button moveBtn;
    [SerializeField] private Button meleeBtn;
    [SerializeField] private Button rangeBtn;
    [SerializeField] private Button defenseBtn;

    [SerializeField] private Transform enemyParent;
    
    private NextAction nextAction;
    public List<Vector2> availableCells = new List<Vector2>();
    private DoActionManager doActionManager = new DoActionManager();
    private LoadActionManager loadActionManager = new LoadActionManager();

    public Target UserTarget;
    public List<Target> RoomTargets;
    public List<AI> RoomAIs;
    
    public int Turn { get; set; } = 1;

    private void Awake() {
        roomManager = this;
        playerData.gameObject.SetActive(true);
        nextAction = NextAction.IDLE;
        nextActionTxt.text = $"Next Action: {nextAction}";
        turnTxt.text = $"Turn: {Turn}";
        
        loadActionManager.initLoadActionManager();
        doActionManager.initDoActionManager();
    }

    void spawnEnemy() {
        var _enemy = Resources.Load("Prefabs/Enemies/SingleKeyEnemy") as GameObject;
        var _cell = Map.MapInfo.mapCellPrefabs.First(_c => _c.mapCellJson.pos.Equals(new Vector2(0, 2)));
        var _ske = Instantiate(_enemy, enemyParent);
        _ske.GetComponent<RectTransform>().anchoredPosition = _cell.gameObject.GetComponent<RectTransform>().anchoredPosition;
        var _add = _ske.GetComponent<SingleKeyEnemy>();
        _add.setCell(_cell.mapCellJson.pos);
        RoomAIs.Add(_add);
    }

    private void Start() {
        playerData.moveAnim(playerData.currentCell, true);
        finishTurnBtn.onClick.AddListener(() => {
            StartCoroutine(changeTurnAndUpdateTilesAndEnemies());
        });

        RoomTargets = new List<Target>();
        RoomAIs = new List<AI>();

        moveBtn.onClick.AddListener(() => {
            UserTarget = playerData;
            SetNextAction(NextAction.MOVE);
        });
        
        meleeBtn.onClick.AddListener(() => {
            UserTarget = playerData;
            SetNextAction(NextAction.MELEE);
        });
        
        rangeBtn.onClick.AddListener(() => {
            UserTarget = playerData;
            SetNextAction(NextAction.RANGE);
        });
        
        defenseBtn.onClick.AddListener(() => {
            UserTarget = playerData;
            SetNextAction(NextAction.DEFENSE);
        });
        
        spawnEnemy();
    }

    private void enableButtons(bool _enable) {
        finishTurnBtn.interactable = _enable;
        meleeBtn.interactable = _enable;
        moveBtn.interactable = _enable;
        rangeBtn.interactable = _enable;
        defenseBtn.interactable = _enable;

        var _color = _enable ? Color.white : new Color(160f / 255f, 160f / 255f, 160f / 255f, 1);
        finishTurnBtn.image.color = _color;
        meleeBtn.image.color = _color;
        moveBtn.image.color = _color;
        rangeBtn.image.color = _color;
        defenseBtn.image.color = _color;

        finishTurnBtn.transform.GetChild(0).gameObject.GetComponent<Image>().color = _color;
        meleeBtn.transform.GetChild(0).gameObject.GetComponent<Image>().color = _color;
        moveBtn.transform.GetChild(0).gameObject.GetComponent<Image>().color = _color;
        rangeBtn.transform.GetChild(0).gameObject.GetComponent<Image>().color = _color;
        defenseBtn.transform.GetChild(0).gameObject.GetComponent<Image>().color = _color;
    }
    
    IEnumerator changeTurnAndUpdateTilesAndEnemies() {
        enableButtons(false);
        
        RoomTargets.Clear();
        RoomTargets.AddRange(RoomAIs);
        RoomTargets.Add(playerData);
        
        Turn++;
        // Enemy update
        foreach (var _enemy in RoomAIs) {
            UserTarget = _enemy;
            
            roomManager.nextAction = _enemy.loadNextAction(this);
            var _range = GameConfig.basicMovements[(int)roomManager.nextAction].range;
            var _rangeType = GameConfig.basicMovements[(int) roomManager.nextAction].rangeType;
            loadActionManager.loadAction(roomManager, _range, _rangeType, (int)roomManager.nextAction);
            
            var _finalCell = _enemy.loadFinalCell(this);
            if(availableCells.Any())
                roomManager.doActionManager.doAction(roomManager, _finalCell, roomManager.nextAction);
                
            _enemy.setEnergy(3);
        }

        // Cell update
        foreach (var _cell in Map.MapInfo.mapCellPrefabs)
            _cell.update(this);

        // Status update
        foreach (var _target in RoomTargets) 
            _target.updateHealthStatus();

        playerData.setEnergy(playerData.baseEnergy);
        SetNextAction(NextAction.IDLE);
        
        turnTxt.text = $"Turn: {Turn}";
        enableButtons(true);

        UserTarget = playerData;
        
        yield return null;
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

    public void SetNextAction(NextAction _action) {
        if (_action == NextAction.IDLE) {
            clearTurn();
            nextAction = _action;
        } else {
            if ( nextAction == _action && availableCells.Any()) {
                clearTurn();
                return;
            }
        
            nextAction = _action;
            nextActionTxt.text = $"Next Action: {nextAction}";
            loadAction(nextAction);
        }
    }

    private void loadAction(NextAction _action) {
        if (_action == NextAction.IDLE) return;
        
        var _intAction = (int) _action;
        var _range = GameConfig.basicMovements[_intAction].range;
        loadActionManager.loadAction(roomManager, _range, GameConfig.basicMovements[_intAction].rangeType, _intAction);
    }

    public void clearTurn(bool _full = false) {
        foreach (var _cell in availableCells) 
            Map.MapInfo.mapCellPrefabs.First(_c => _c.mapCellJson.pos.Equals(_cell)).upSide.GetComponent<SpriteRenderer>().color = Color.white;
        availableCells.Clear();
    }

    // private static string statusToString(State _state) {
    //     switch (_state) {
    //         case State.NONE: return "none";
    //         case State.BURNT: return "burnt";
    //         case State.FROZEN: return "frozen";
    //         case State.PARALIZED: return "paralized";
    //         case State.POISONED: return "poisoned";
    //         case State.CONFUSED: return "confused";
    //         case State.TAUNTED: return "taunted";
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
    //     }
    // }
}