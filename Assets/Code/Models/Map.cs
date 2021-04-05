using System.Collections.Generic;

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
    public List<TileInfo> tiles;
    public MapInfo mapInfo;
}