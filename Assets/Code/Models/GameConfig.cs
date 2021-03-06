using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

// ------------ ENUMS -----------
[Flags]
public enum StatusType { NONE = 0, POISONED = 1, BLEEDING = 2, BURNT = 4, CONFUSED = 8, FROZEN = 16, PARALIZED = 32, TAUNTED = 64, WET = 128 }
public enum NextAction { IDLE = 0, MOVE = 1, MELEE = 2, RANGE = 3, DEFENSE = 4, SPECIAL = 5 }
public enum RangeType { CROSS = 0, INDIVIDUAL = 1, CIRCLE = 2, DIAGONAL = 3, ALL_DIRECTION = 4, ALL_ROOM = 5, NONE = 6 }

// ------------ ENUMS -----------

// ---------- ENEMIES INFO ---------

public class EnemyInfoJson {
    public string name;
    public int baseHealth;
    public int baseEnergy;
    public int baseAttack;
    public int baseDefense;
}

// ---------- ENEMIES INFO ---------

// ---------- PROP INFO ---------

public class Movable {
    public bool canMove;
}

public class Destructible {
    public bool canDestroy;
}

public class PropInfoJson {
    public string name;
    public Destructible destructible;
    public Movable movable;
}

// ---------- PROP INFO ---------

// ---------- CELL INFO ---------

public class CellInfoJson {
    public string id;
    public string name;
    public int damage;
    public string mechanic;
    public ApplyStatusJson applyStatus;
}

// ---------- CELL INFO ---------

// ------------ STATUS ----------

public class StatsModification {
    public float damageTaken;
    public float damageDone;
}

public class Duration {
    public int min;
    public int max;
}

public class StatusJson {
    public string name;
    public float probabilityToAffect;
    public float damagerPerTurn;
    public Duration duration;
    public bool paralized;
    public StatsModification statsModification;
}

// ------------ STATUS ----------

// ------------ MOVEMENTS ----------

public class ApplyStatusJson {
    public int statusId;
    public float probability;
}

public class ActionJson {
    public string name;
    public string description;
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
    public static Dictionary<int, ActionJson> basicMovements;
    public static Dictionary<int, StatusJson> statusInfo;
    public static Dictionary<int, CellInfoJson> cellsInfo;
    public static Dictionary<int, PropInfoJson> propsInfo;
    public static Dictionary<int, EnemyInfoJson> enemiesInfo;

    public static void initGameData() {
        initBasicMovements();
        initStatus();
        initPropsInfo();
        initCellsInfo();
        initEnemiesInfo();
    }

    private static void initEnemiesInfo() {
        var _mapUrl = $"{Application.streamingAssetsPath}/Data/gameData/enemies.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        enemiesInfo = JsonConvert.DeserializeObject<Dictionary<int, EnemyInfoJson>>(_json);
        Debug.Log("Loaded enemies info");
    }
    
    private static void initPropsInfo() {
        var _mapUrl = $"{Application.streamingAssetsPath}/Data/gameData/propsInfo.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        propsInfo = JsonConvert.DeserializeObject<Dictionary<int, PropInfoJson>>(_json);
        Debug.Log("Loaded props info");
    }
    
    private static void initCellsInfo() {
        var _mapUrl = $"{Application.streamingAssetsPath}/Data/gameData/cells.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        cellsInfo = JsonConvert.DeserializeObject<Dictionary<int, CellInfoJson>>(_json);
        Debug.Log("Loaded cells info");
    }
    
    private static void initBasicMovements() {
        var _mapUrl = $"{Application.streamingAssetsPath}/Data/gameData/basicMovements.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        basicMovements = JsonConvert.DeserializeObject<Dictionary<int, ActionJson>>(_json);
        Debug.Log("Loaded basic movements");
    }

    private static void initStatus() {
        var _mapUrl = $"{Application.streamingAssetsPath}/Data/gameData/status.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        statusInfo = JsonConvert.DeserializeObject<Dictionary<int, StatusJson>>(_json);
        Debug.Log("Loaded status");
    }
}