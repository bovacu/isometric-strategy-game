using System.Collections.Generic;
using Newtonsoft.Json;

public class MapInfo {
    public int width;
    public int height;
    public int top;
    public int bottom;
    public int left;
    public int right;
}

public class Position {
    public int x;
    public int y;
}

public class Tile {
    public string basePath;
    public string leftPath;
    public string rightPath;
}

public class TileInfo {
    public Position pos;
    public string underlayTile;
}

public class Map {

    public List<TileInfo> jsonTiles;
    public MapInfo info;
    
    [JsonIgnore] private static Map map;
    [JsonIgnore] public static Map MapInfo {
        get => map;
        set => map = value;
    }
    [JsonIgnore] public ValidArea validArea = new ValidArea();
    [JsonIgnore] public List<Cell> mapTiles = new List<Cell>();
}