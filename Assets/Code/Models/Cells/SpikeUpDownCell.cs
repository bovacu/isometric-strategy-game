using UnityEngine;

public class SpikeUpDownCell : Cell {
    [SerializeField] private GameObject spikes;

    public override void update(RoomManager _roomManager) {
        spikes.SetActive(_roomManager.Turn % 2 == 0);
        foreach (var _target in _roomManager.RoomTargets) {
            if(_target.getCell().Equals(base.mapCellJson.pos))
                interact(_target);
        }
    }

    public override void interact(Target _target) {
        if(spikes.activeInHierarchy) {
            var _cellConfig = GameConfig.cellsInfo[mapCellJson.id];
            var _damage = _cellConfig.damage;
            _target.setHealth(_target.getHealth() - _damage - (StatusManager.hasStatus(_target.getStatus(), (StatusType) _cellConfig.applyStatus.statusId) ? 1 : 0));
            if(IsoMath.probability(_cellConfig.applyStatus.probability) && _cellConfig.applyStatus.statusId != -1)
                _target.setStatus((StatusType) _cellConfig.applyStatus.statusId);
        }
    }

    protected override void setLayerForInnerComponents(int _layer) {
        spikes.GetComponent<SpriteRenderer>().sortingOrder = _layer;
    }
}