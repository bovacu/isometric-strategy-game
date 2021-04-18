using System;
using TMPro;
using UnityEngine;

public class StatusView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI turnsTxt;
    [SerializeField] public StatusType status;
    [NonSerialized]  public int Turns;

    public void setup() {
        turnsTxt.text = $"{Turns}";
    }
    
    public void update(Action onEnd) {
        Turns--;
        turnsTxt.text = $"{Turns}";
        
        if(Turns == 0) {
            onEnd();
            Destroy(gameObject);
        }
    }
}