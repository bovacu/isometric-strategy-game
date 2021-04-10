using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Locize {

    public static string translate(string _data) {
        var _tags = Regex.Matches(_data, @"#(\w+)").Cast<Match>().Select(m => m.Groups[1].Value).ToList();
        for (var index = 0; index < _tags.Count; index++) {
            var _tag = _tags[index];
            _data = _data.Replace($"#{_tag}", GameConfig.locized[_tag.TrimStart('#')]);
        }

        return _data;
    }
    
}