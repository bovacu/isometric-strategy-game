using System;
using System.Linq;
using UnityEngine;

public enum State { NONE, BURNT, FROZEN, PARALIZED, POISONED, CONFUSED, TAUNTED }

public class PlayerData : MonoBehaviour {

    [SerializeField] public int baseEnergy;
    [NonSerialized]  public int currentEnergy;
    
    [SerializeField] public int baseHealth;
    [NonSerialized]  public int currentHealth;

    [SerializeField] public float baseAttack;
    [NonSerialized]  public float currentAttack;
    
    [SerializeField] public float baseDefense;
    [NonSerialized]  public float currentDefense;
    
    [SerializeField] public float baseSpeed;
    [NonSerialized]  public float currentSpeed;

    [SerializeField] public State healthState;
    
    [SerializeField] public Vector2 currentCell;

    [SerializeField] private HealthBarManager healthBarManager;
    [SerializeField] private EnergyBarManager energyBarManager;
    
    private void Start() {
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentEnergy = baseEnergy;
        currentHealth = baseHealth;
        energyBarManager.MaxEnergy = currentEnergy;
    }

    public void updateHealth(int _health) {
        currentHealth = Mathf.Clamp(_health, 0, baseHealth);
        healthBarManager.update(currentHealth);
    }

    public void updateEnergy(int _energy) {
        currentEnergy = Mathf.Clamp(_energy, 0, baseEnergy);
        energyBarManager.update(_energy);
    }

    public void updatePosToCellPos(Vector2 _cellPosIso, bool _inmediate = false) {
        var _mapCell = Map.MapInfo.mapTiles.First(_c =>
            (int) _c.gridPosition.x == (int) _cellPosIso.x &&
            (int) _c.gridPosition.y == (int) _cellPosIso.y);
        LeanTween.moveLocal(gameObject, _mapCell.gameObject.GetComponent<RectTransform>().anchoredPosition, _inmediate ? 0.0f : 0.25f).setEase(LeanTweenType.easeSpring);
    }
    
    public void updatePosToCellPosThenGoBack(Vector2 _cellPosIso) {
        var _initalPosIso = new Vector2(currentCell.x, currentCell.y);
        var _mapCell = Map.MapInfo.mapTiles.First(_c => _c.gridPosition.Equals(_cellPosIso)).gameObject.GetComponent<RectTransform>().anchoredPosition;
        var _currentPosCanvas = gameObject.GetComponent<RectTransform>().anchoredPosition;
        var _finalPoint = new Vector2((_mapCell.x + _currentPosCanvas.x) / 2f, (_mapCell.y + _currentPosCanvas.y) / 2f);
        
        LeanTween.moveLocal(gameObject, _finalPoint, 0.25f)
            .setEase(LeanTweenType.easeSpring).setOnComplete(() => {
                LeanTween.moveLocal(gameObject, Map.MapInfo.mapTiles.First(_c => _c.gridPosition.Equals(_initalPosIso)).
                    gameObject.GetComponent<RectTransform>().anchoredPosition, 0.25f).setEase(LeanTweenType.easeSpring);
            });
    }

    public void testDefenseTween() {
        LeanTween.scale(gameObject, new Vector3(1.15f, 1.1f, 1.0f), 0.25f)
            .setOnComplete(() => LeanTween.scale(gameObject, new Vector3(1f, 1f, 1.0f), 0.25f));
    }

    public bool canDoAction(int _nextActionCost) {
        return currentEnergy - _nextActionCost >= 0;
    }
}