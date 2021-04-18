using System;
using System.Linq;
using UnityEngine;



public class PlayerData : MonoBehaviour, Target {

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

    [SerializeField] public StatusType healthState;
    
    [SerializeField] public Vector2 currentCell;

    [Header("Health things")]
    [SerializeField] private HealthBarManager healthBarManager;
    [SerializeField] private EnergyBarManager energyBarManager;
    [SerializeField] private StatusManager statusManager;
    
    private void Start() {
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentEnergy = baseEnergy;
        currentHealth = baseHealth;
        energyBarManager.MaxEnergy = currentEnergy;
    }

    public void testDefenseTween() {
        
    }

    public void setHealth(int _health) {
        currentHealth = IsoMath.ClampI(_health, 0, baseHealth);
        healthBarManager.update(currentHealth);
    }

    public void setEnergy(int _energy) {
        currentEnergy = IsoMath.ClampI(_energy, 0, baseEnergy);
        energyBarManager.update(currentEnergy);
    }

    public void setStatus(StatusType _status) {
        healthState = _status;
        statusManager.addStatus(_status);
    }

    public void setCell(Vector2 _cell) {
        currentCell = _cell;
    }

    public Vector2 getCell() {
        return currentCell;
    }

    public int getHealth() {
        return currentHealth;
    }

    public int getEnergy() {
        return currentEnergy;
    }

    public StatusType getStatus() {
        return statusManager.currentAppliedStatus;
    }

    public void moveAnim(Vector2 _finalPos, bool _inmediate = false) {
        var _mapCell = Map.MapInfo.mapCellPrefabs.First(_c =>
            (int) _c.mapCellJson.pos.x == (int) _finalPos.x &&
            (int) _c.mapCellJson.pos.y == (int) _finalPos.y);
        LeanTween.moveLocal(gameObject, _mapCell.gameObject.GetComponent<RectTransform>().anchoredPosition, _inmediate ? 0.0f : 0.25f).setEase(LeanTweenType.easeSpring);
    }

    public void meleeAnim(Vector2 _finalPos, bool _inmediate = false) {
        var _initalPosIso = new Vector2(currentCell.x, currentCell.y);
        var _mapCell = Map.MapInfo.mapCellPrefabs.First(_c => _c.mapCellJson.pos.Equals(_finalPos)).gameObject.GetComponent<RectTransform>().anchoredPosition;
        var _currentPosCanvas = gameObject.GetComponent<RectTransform>().anchoredPosition;
        var _finalPoint = new Vector2((_mapCell.x + _currentPosCanvas.x) / 2f, (_mapCell.y + _currentPosCanvas.y) / 2f);
        
        LeanTween.moveLocal(gameObject, _finalPoint, 0.25f)
            .setEase(LeanTweenType.easeSpring).setOnComplete(() => {
                LeanTween.moveLocal(gameObject, Map.MapInfo.mapCellPrefabs.First(_c => _c.mapCellJson.pos.Equals(_initalPosIso)).
                    gameObject.GetComponent<RectTransform>().anchoredPosition, 0.25f).setEase(LeanTweenType.easeSpring);
            });
    }

    public void rangeAnim(Vector2 _finalPos, bool _inmediate = false) {
        
    }

    public void defenseAnim(Vector2 _finalPos, bool _inmediate = false) {
        LeanTween.scale(gameObject, new Vector3(1.15f, 1.1f, 1.0f), 0.25f)
            .setOnComplete(() => LeanTween.scale(gameObject, new Vector3(1f, 1f, 1.0f), 0.25f));
    }

    public void updateHealthStatus() {
        statusManager.update();
    }
}