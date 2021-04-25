using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class StatusManager : MonoBehaviour {

    [SerializeField] private List<GameObject> statuses;
    [SerializeField] private GameObject container;
    public StatusType currentAppliedStatus;

    private Dictionary<StatusType, StatusView> statusTurnCounter = new Dictionary<StatusType, StatusView>();

    public static bool hasStatus(StatusType _statuses, StatusType _specificStatus) {
        return (_statuses & _specificStatus) == _specificStatus;
    }

    public void update() {
        foreach (var _status in statusTurnCounter)
            _status.Value.update(() => { Debug.Log($"Status to remove: {_status.Value.status}"); removeStatusFlag(_status.Value.status); });
    }
    
    public void addStatus(StatusType _status) {
        if (container.transform.childCount == 4 || hasStatusFlag(_status))
            return;
        
        addStatusFlag(_status);
        Debug.Log($"Applied status: {_status}({(int)_status})");
    }

    private bool hasStatusFlag(StatusType _status) {
        return (currentAppliedStatus & _status) == _status;
    }

    private void addStatusFlag(StatusType _status) {
        currentAppliedStatus |= _status;
        var _statusConfig = GameConfig.status[(int) Math.Log((int) _status, 2) + 1];
        statusTurnCounter[_status] = Instantiate(statuses[(int) Math.Log((int) _status, 2)], container.transform).GetComponent<StatusView>();
        statusTurnCounter[_status].Turns = IsoMath.randomInt(_statusConfig.duration.min, _statusConfig.duration.max);
        statusTurnCounter[_status].setup();
    }
    
    private void removeStatusFlag(StatusType _status) {
        currentAppliedStatus ^= _status;
    }
}