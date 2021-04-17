using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

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

    public static void initLocized() {
        initStatus();
        initEn();
    }

    private static void initEn() {
        var _mapUrl = $"{Application.dataPath}/Data/locized/en.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        locized = JsonConvert.DeserializeObject<Dictionary<string, string>>(_json);
    }

    private static void initStatus() {
        var _mapUrl = $"{Application.dataPath}/Data/locized/status.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        status = JsonConvert.DeserializeObject<StatusJson>(_json);
    }

    public static string translate(string _data) {
        var _tags = Regex.Matches(_data, @"#(\w+)").Cast<Match>().Select(m => m.Groups[1].Value).ToList();
        for (var index = 0; index < _tags.Count; index++) {
            var _tag = _tags[index];
            _data = _data.Replace($"#{_tag}", locized[_tag.TrimStart('#')]);
        }

        return _data;
    }
    
}