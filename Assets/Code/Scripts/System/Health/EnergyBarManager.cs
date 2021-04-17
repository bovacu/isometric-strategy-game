using TMPro;
using UnityEngine;

public class EnergyBarManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI energyText;

    public int MaxEnergy {
        get;
        set;
    }
    
    public void update(int _playerEnergy) {
        energyText.text = $"{_playerEnergy}/{MaxEnergy}";
        if(_playerEnergy > MaxEnergy / 2f)
            energyText.color = Color.white;
        else if(_playerEnergy <= MaxEnergy / 2f && _playerEnergy > MaxEnergy / 4f)
            energyText.color = Color.yellow;
        else if(_playerEnergy <= MaxEnergy / 4f)
            energyText.color = Color.red;
    }
}