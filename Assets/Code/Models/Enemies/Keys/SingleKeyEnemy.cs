using System;
using System.Linq;
using UnityEngine;

public class SingleKeyEnemy : BaseKey {
    
    public override void moveAnim(Vector2 _finalPos, bool _inmediate = false) {
        var _mapCell = Map.MapInfo.mapCellPrefabs.First(_c =>
            (int) _c.mapCellJson.pos.x == (int) _finalPos.x &&
            (int) _c.mapCellJson.pos.y == (int) _finalPos.y);
        LeanTween.moveLocal(gameObject, _mapCell.gameObject.GetComponent<RectTransform>().anchoredPosition, _inmediate ? 0.0f : 0.25f).setEase(LeanTweenType.easeSpring);
    }

    public override void meleeAnim(Vector2 _finalPos, bool _inmediate = false) {
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