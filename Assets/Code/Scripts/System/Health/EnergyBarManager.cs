using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EnergyBarManager : MonoBehaviour {

    private const int MAX_CONTAINERS = 2;
    [SerializeField] private List<EnergyBarBatteryController> containers;

    // This is the amount of red blocks the battery has
    private int batteryContainerLevels = 3;
    
    private void Start() {
        if (containers.Any())
            batteryContainerLevels = containers[0].NumberOfLevels;
    }

    public void update(int _playerEnergy) {
        var _container = _playerEnergy / batteryContainerLevels - (_playerEnergy % batteryContainerLevels == 0 && _playerEnergy != 0 ? 1 : 0);
        var _currentContainerLevels = _playerEnergy - (batteryContainerLevels * _container);
        
        // Update the current container
        containers[_container].update(_currentContainerLevels);

        for (var _i = 0; _i < containers.Count; _i++) {
            // Fill all containers below
            if(_i < _container)
                containers[_i].update(batteryContainerLevels);
            
            // Empty all containers up
            else if(_i > _container)
                containers[_i].update(0);
        }
    }

    public void shake() {
        float height = Mathf.PerlinNoise(9.5f, 0f)*2.5f;
        float shakeAmt = height*0.1f; // the degrees to shake the camera
        float shakePeriodTime = 0.22f; // The period of each shake
        float dropOffTime = 0.6f; // How long it takes the shaking to settle down to nothing
        LTDescr shakeTween = LeanTween.moveLocalX( gameObject, 15, shakeAmt)
            .setEase( LeanTweenType.easeShake ) // this is a special ease that is good for shaking
            .setLoopClamp()
            .setRepeat(2);
        shakeTween.cleanup();
    }
}