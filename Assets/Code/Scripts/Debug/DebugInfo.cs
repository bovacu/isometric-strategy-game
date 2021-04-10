using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class DebugInfo : MonoBehaviour {

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI fps;
    // [SerializeField] private TextMeshProUGUI memory;
    [SerializeField] private TextMeshProUGUI platform;
    [SerializeField] private TextMeshProUGUI cpu;
    [SerializeField] private TextMeshProUGUI gpu;
    [SerializeField] private TextMeshProUGUI ram;

    private float secondCounter = 0.0f;
    private float updateInterval = 0.5f;

    private void Start() {
        panel.SetActive(false);
        StartCoroutine(updateFps());
    }

    private void OnEnable() {
        platform.text = $"OS: {SystemInfo.operatingSystem} [{Application.platform}]";
        cpu.text = $"CPU: {SystemInfo.processorType} [{SystemInfo.processorCount} cores]";
        gpu.text = $"GPU: {SystemInfo.graphicsDeviceName}, {SystemInfo.graphicsDeviceType}";
        ram.text = $"RAM: {SystemInfo.graphicsMemorySize}";
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            panel.SetActive(!panel.activeInHierarchy);
        }
    }

    protected IEnumerator updateFps() {
        while (true) {
            var previousUpdateTime = Time.unscaledTime;
            var previousUpdateFrames = Time.frameCount;

            while (Time.unscaledTime < previousUpdateTime + updateInterval) {
                yield return null;
            }

            var timeElapsed = Time.unscaledTime - previousUpdateTime;
            var framesChanged = Time.frameCount - previousUpdateFrames;

            var _newValue = framesChanged / timeElapsed;
            fps.text = $"{_newValue}";
        }
    }
    
}