using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarBatteryController : MonoBehaviour {
    [SerializeField] private List<GameObject> batteryLevels;
    [SerializeField] private int containerPosition;

    public int ContainerPosition {
        get => containerPosition;
        set => containerPosition = value;
    }

    public int NumberOfLevels => batteryLevels.Count;

    public void update(int _currentPlayerLife) {
        for (var _i = batteryLevels.Count - 1; _i >= 0; _i--) {
            batteryLevels[_i].SetActive(_i >= batteryLevels.Count - _currentPlayerLife);
        }
    }
}
