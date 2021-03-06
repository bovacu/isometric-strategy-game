using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class MapInfo {
    public int width;
    public int height;
    public int top;
    public int bottom;
    public int left;
    public int right;
}

public class MapCellJson {
    public Vector2 pos;
    public string underlayTile;
    public int id;
}

public class MapPropJson {
    public Vector2 pos;
    public string underlayTile;
    public int id;
}

public class MapEnemiesJson {
    public Vector2 pos;
    public string underlayTile;
    public int id;
}

public class Map {

    public List<MapCellJson> jsonTiles;
    public List<MapPropJson> jsonProps;
    public List<MapEnemiesJson> jsonEnemies;
    public MapInfo info;
    
    [JsonIgnore] private static Map map;
    [JsonIgnore] public static Map MapInfo {
        get => map;
        set => map = value;
    }
    
    [JsonIgnore] public ValidArea validArea = new ValidArea();
    [JsonIgnore] public List<Prop> props = new List<Prop>();
    [JsonIgnore] public List<Cell> mapCellPrefabs = new List<Cell>();
}