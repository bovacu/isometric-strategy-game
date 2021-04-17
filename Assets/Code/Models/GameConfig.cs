using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

// ------------ ENUMS -----------

public enum NextAction { IDLE = 0, MOVE = 1, MELEE = 2, RANGE = 3, DEFENSE = 4, SPECIAL = 5 }
public enum RangeType { CROSS = 0, INDIVIDUAL = 1, CIRCLE = 2, DIAGONAL = 3, ALL_DIRECTION = 4, ALL_ROOM = 5, NONE = 6 }

// ------------ ENUMS -----------

// ------------ STATUS ----------

public class StatsModification {
    public float damageTaken;
    public float damageDone;
}

public class Status {
    public string name;
    public float probabilityToAffect;
    public float damagePerMovement;
    public float damagerPerTurn;
    public int turnsDuration;
    public bool paralized;
    public StatsModification statsModification;
}

// ------------ STATUS ----------

// ------------ MOVEMENTS ----------

public class ApplyStatusJson {
    public int statusId;
    public float probability;
}

public class MovementJson {
    public string name;
    public RangeType rangeType;
    public int range;
    public int cost;
    public float damage;
    public float damageReduction;
    public float accuracy;
    public ApplyStatusJson applyStatus;
}

// ------------ MOVEMENTS ----------


public class GameConfig {
    public static Dictionary<int, MovementJson> basicMovements;
    public static Dictionary<int, Status> status;

    public static void initGameData() {
        initBasicMovements();
        initStatus();
    }

    private static void initBasicMovements() {
        var _mapUrl = $"{Application.dataPath}/Data/gameData/basicMovements.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        basicMovements = JsonConvert.DeserializeObject<Dictionary<int, MovementJson>>(_json);
        Debug.Log("Loaded basic movements");
    }

    private static void initStatus() {
        var _mapUrl = $"{Application.dataPath}/Data/gameData/status.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        status = JsonConvert.DeserializeObject<Dictionary<int, Status>>(_json);
        Debug.Log("Loaded status");
    }
}