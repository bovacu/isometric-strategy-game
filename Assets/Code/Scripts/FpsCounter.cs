using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI fpsText;
    private float deltaTime;
 
    void Update () {
        deltaTime += Time.deltaTime;

        if (deltaTime >= 1f) {
            var fps = 1 / Time.unscaledDeltaTime;
            fpsText.text = $"FPS: {fps}";

            deltaTime = 0;
        }
        
        
    }
}
