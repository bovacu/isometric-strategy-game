using UnityEngine;
using Random = System.Random;

public class IsoMath {

    private static Random random = new Random();

    /// <summary>
    /// Includes both sides [min, max]
    /// </summary>
    /// <returns></returns>
    public static int randomInt(int _min, int _max) {
        return random.Next(_min, _max + 1);
    }
    
    public static void initIsoMathWithSeed(int _seed) {
        random = new Random(_seed);
    }
    
    public static int ClampI(int _value, int _min, int _max) {
        return (_value >= _min && _value <= _max  ? _value : (_value < _min ? _min : _max));
    } 
    
    public static float ClampF(float _value, float _min, float _max) {
        return (_value >= _min && _value <= _max  ? _value : (_value < _min ? _min : _max));
    }

    public static bool probability(float _probToHappen) {
        _probToHappen = ClampF(_probToHappen, 0, 1);
        var _randomProb = random.NextDouble();
        // Debug.Log($"OnLeftProb: {_probToHappen}, onRightProb: {1 - _probToHappen}, randomProb: {_randomProb}, happened: {_randomProb <= _probToHappen}");
        return _randomProb < _probToHappen;
    }
}