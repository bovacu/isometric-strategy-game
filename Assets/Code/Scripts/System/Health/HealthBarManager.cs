using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthBarManager : MonoBehaviour {

    private const int MAX_CONTAINERS = 5;
    [SerializeField] private List<HealthBarBatteryController> containers;

    // This is the amount of red blocks the battery has
    private int batteryContainerLevels = 3;
    
    private void Start() {
        if (containers.Any())
            batteryContainerLevels = containers[0].NumberOfLevels;
    }

    public void update(int _playerHealth) {
        shake();
        var _container = _playerHealth / batteryContainerLevels - (_playerHealth % batteryContainerLevels == 0 && _playerHealth != 0 ? 1 : 0);
        var _currentContainerLevels = _playerHealth - (batteryContainerLevels * _container);
        
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

    private void shake() {
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