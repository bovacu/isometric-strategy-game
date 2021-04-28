using System;
using System.Linq;
using UnityEngine;

public class SingleKeyEnemy : BaseKey {
    private void Start() {
        enemyInfoJson = new EnemyInfoJson {
            baseAttack = 1,
            baseDefense = 1,
            baseEnergy = 3,
            baseHealth = 4
        };

        currentAttack = enemyInfoJson.baseAttack;
        currentDefense = enemyInfoJson.baseDefense;
        currentEnergy = enemyInfoJson.baseEnergy;
        currentHealth = enemyInfoJson.baseHealth;
    }

    public override void moveAnim(Vector2 _finalPos, bool _inmediate = false) {
        var _mapCell = Map.MapInfo.mapCellPrefabs.First(_c =>
            (int) _c.mapCellJson.pos.x == (int) _finalPos.x &&
            (int) _c.mapCellJson.pos.y == (int) _finalPos.y);
        LeanTween.moveLocal(gameObject, _mapCell.gameObject.GetComponent<RectTransform>().anchoredPosition, _inmediate ? 0.0f : 0.25f).setEase(LeanTweenType.easeSpring);
    }

    public override void meleeAnim(Vector2 _finalPos, bool _inmediate = false) {
        throw new System.NotImplementedException();
    }

    public override void rangeAnim(Vector2 _finalPos, bool _inmediate = false) {
        throw new System.NotImplementedException();
    }

    public override void defenseAnim(Vector2 _finalPos, bool _inmediate = false) {
        throw new System.NotImplementedException();
    }

    public override void updateHealthStatus() {
        // throw new System.NotImplementedException();
    }
}