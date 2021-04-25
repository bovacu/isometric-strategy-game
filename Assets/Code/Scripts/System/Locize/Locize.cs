using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

// --------------- CELL VISUALIZER ----------
public class CellVisualizerInfoJson {
    public string header;
    public string mechanic;
}

public class VisualizerConfigJson {
    public string header;
    public string damage;
    public string heal;
    public string mechanic;
    public string effect;
}

public class CellVisualizerJson {
    public Dictionary<string, CellVisualizerInfoJson> cells;
    public VisualizerConfigJson visualizerConfig;
}
// -------------- CELL VISUALIZER -------------


// --------------- STATUS ----------
public class StatusInfoJson {
    public string id;
    public string header;
    public string effect;
    public string curedBy;
}

public class TooltipConfigStatusJson {
    public string effect;
    public string curedBy;
}

public class StatusJson {
    public List<StatusInfoJson> types;
    public TooltipConfigStatusJson tooltipConfig;
}
// -------------- STATUS -------------

public class Locize {
    
    public static StatusJson status;
    public static Dictionary<string, string> locized;
    public static CellVisualizerJson cellVisualizer;

    public static void initLocized() {
        Debug.Log(Application.streamingAssetsPath);
        initStatus();
        initEn();
        initCells();
    }

    private static void initEn() {
        var _mapUrl = $"{Application.streamingAssetsPath}/Data/locized/en.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        locized = JsonConvert.DeserializeObject<Dictionary<string, string>>(_json);
    }

    private static void initCells() {
        var _mapUrl = $"{Application.streamingAssetsPath}/Data/locized/cellsLocized.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        cellVisualizer = JsonConvert.DeserializeObject<CellVisualizerJson>(_json);
    }
    
    private static void initStatus() {
        var _mapUrl = $"{Application.streamingAssetsPath}/Data/locized/status.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        status = JsonConvert.DeserializeObject<StatusJson>(_json);
    }

    public static string translateHashtags(string _data, bool _ignoreHexNumbers = true) {
        var _tags = Regex.Matches(_data, @"#(\w+)").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

        for (var index = 0; index < _tags.Count; index++) {
            var _tag = _tags[index];
            if(!isHexNumber(_tag) || !_ignoreHexNumbers)
                _data = _data.Replace($"#{_tag}", locized[_tag.TrimStart('#')]);
        }

        return _data;
    }

    private static bool isHexNumber(string _possibleHex) {
        long output;
        return long.TryParse(_possibleHex, System.Globalization.NumberStyles.HexNumber, null, out output);
    }

    /// <summary>
    /// For data like "This is my {id}"
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="_newId"></param>
    /// <returns></returns>
    public static string substituteId(string _data, string _newId) {
        var _tags = Regex.Matches(_data, @"{(\w+)}").Cast<Match>().Select(m => m.Groups[1].Value).ToList();
        if (_tags.Count > 0) {
            var _tag = _tags[0];
            _data = _data.Replace($"{'{'}{_tag}{'}'}", _newId);
        }

        return _data;
    }

}