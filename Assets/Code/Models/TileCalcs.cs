using UnityEngine;

public class TileCalcs {

    public static float tileWidth = 1.92f;
    public static float tileHeight = 0.96f;
    public static bool activateCellDebugging;
    public static Vector2 tileCenter;

    public static Vector2 toGrid(int _x, int _y) {
        var _tileX = (_x - _y ) * (tileWidth / 2f);
        var _tileY = (_x + _y) * (tileHeight / 2f);

        _tileY += -tileCenter.y / 2f * (tileHeight) - tileCenter.x / 2f * (tileHeight);
        _tileX += tileCenter.y / 2f * (tileWidth) - tileCenter.x / 2f * (tileWidth);

        if (tileCenter.x % 2 != 0 && tileCenter.y % 2 == 0) {
            _tileX += tileCenter.y / 2f * (tileWidth) - tileCenter.x / 2f * (tileWidth);
            _tileY += tileCenter.y / 2f * (tileHeight) - tileCenter.x / 2f * (tileHeight);
        }
        
        if (tileCenter.y % 2 != 0 && tileCenter.x % 2 == 0) {
            _tileX += tileCenter.y / 2f * (tileWidth) - tileCenter.x / 2f * (tileWidth);
            _tileY -= tileCenter.y / 2f * (tileHeight) - tileCenter.x / 2f * (tileHeight);
        }

        if (tileCenter.y % 2 != 0 && tileCenter.x % 2 != 0) {
            _tileY -= tileHeight / 2f;
        }
        
        return new Vector2(_tileX, _tileY);
    }
    
    public static Vector2 getRealCell(Vector2 _cellPos, Vector2 _mousePosCentered, Vector2 _worldOffset) {
        // _cellPos is the left-lower corner of the rectangle of the tile, we need to calc all four vertices of the rhombus (A, B, C, D), and then, the center (Q)
        // With that, we will calc if AB, BC, CD, DA intersects with QMosePos. If no intersection, the mouse is over the rhombus inside the rectangle, else
        //    - if AB, the mouse is over the rhombus on upper-left
        //    - if BC, the mouse is over the rhombus on upper-right
        //    - if CD, the mouse is over the rhombus on lower-right
        //    - if DA, the mouse is over the rhombus on lower-left

        var _collisionDebug = "";
        var _sideMultiplierX = _cellPos.x < 0 ? -1 : 1;
        var _sideMultiplierY = _cellPos.y < 0 ? -1 : 1;

        _cellPos.x = (int) _cellPos.x;
        _cellPos.y = (int) _cellPos.y;

        var _finalCell = new Vector2(_cellPos.x, _cellPos.y);

        _finalCell.x += _sideMultiplierX;
        _finalCell.y += _sideMultiplierY;

        _cellPos.x *= tileWidth * 100;
        _cellPos.y *= tileHeight * 100;

        var A = new Vector2(_cellPos.x, _cellPos.y + tileHeight / 2f * 100 * _sideMultiplierY);
        var B = new Vector2(_cellPos.x+ tileWidth / 2f * 100 * _sideMultiplierX, _cellPos.y + tileHeight * 100 * _sideMultiplierY);
        var C = new Vector2(_cellPos.x + tileWidth * 100 * _sideMultiplierX, _cellPos.y + tileHeight / 2f * 100 * _sideMultiplierY);
        var D = new Vector2(_cellPos.x + tileWidth / 2f * 100 * _sideMultiplierX, _cellPos.y);
        
        var Q = new Vector2(_cellPos.x + tileWidth / 2f * 100 * _sideMultiplierX, _cellPos.y + tileHeight / 2f * 100 * _sideMultiplierY);
        var P = new Vector2(_mousePosCentered.x, _mousePosCentered.y);

        var _extraRestX = 0;
        var _extraRestY = 0;
        
        fixCenter(ref _extraRestX, ref _extraRestY, _sideMultiplierX, _sideMultiplierY);  
        
        // Check intersections
        if (lineSegmentsIntersect(A, B, Q, P)) {
            _collisionDebug = "AB";
            correctValues(out A, out B, out C, out D, out Q, _cellPos, -tileWidth / 2f * 100, tileHeight / 2f * 100, _sideMultiplierX, _sideMultiplierY);
            fixFirstQuadrant("AB", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixSecondQuadrant("AB", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixThirdQuadrant("AB", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixFourthQuadrant("AB", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
        }
        
        if (lineSegmentsIntersect(B, C, Q, P)) {
            _collisionDebug = "BC";
            correctValues(out A, out B, out C, out D, out Q, _cellPos, tileWidth / 2f * 100, tileHeight / 2f * 100, _sideMultiplierX, _sideMultiplierY);
            fixFirstQuadrant("BC", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixSecondQuadrant("BC", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixThirdQuadrant("BC", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixFourthQuadrant("BC", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
        }
        
        if (lineSegmentsIntersect(C, D, Q, P)) {
            _collisionDebug = "CD";
            correctValues(out A, out B, out C, out D, out Q, _cellPos, tileWidth / 2f * 100, -tileHeight / 2f * 100, _sideMultiplierX, _sideMultiplierY);
            fixFirstQuadrant("CD", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixSecondQuadrant("CD", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixThirdQuadrant("CD", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixFourthQuadrant("CD", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
        }
        
        if (lineSegmentsIntersect(D, A, Q, P)) {
            _collisionDebug = "DA";
            correctValues(out A, out B, out C, out D, out Q, _cellPos, -tileWidth / 2f * 100, -tileHeight / 2f * 100, _sideMultiplierX, _sideMultiplierY);
            fixFirstQuadrant("DA", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixSecondQuadrant("DA", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixThirdQuadrant("DA", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
            fixFourthQuadrant("DA", _sideMultiplierX, _sideMultiplierY, ref _extraRestX, ref _extraRestY, ref _finalCell);
        }

        if (activateCellDebugging) {
            // Cell outline
            Debug.DrawLine(new Vector3(A.x / 100f, A.y / 100f), new Vector3(B.x / 100f, B.y / 100f), !_collisionDebug.Contains("AB") ? Color.green : Color.red);
            Debug.DrawLine(new Vector3(B.x / 100f, B.y / 100f), new Vector3(C.x / 100f, C.y / 100f), !_collisionDebug.Contains("BC") ? Color.green : Color.red);
            Debug.DrawLine(new Vector3(C.x / 100f, C.y / 100f), new Vector3(D.x / 100f, D.y / 100f), !_collisionDebug.Contains("CD") ? Color.green : Color.red);
            Debug.DrawLine(new Vector3(D.x / 100f, D.y / 100f), new Vector3(A.x / 100f, A.y / 100f), !_collisionDebug.Contains("DA") ? Color.green : Color.red);

            // Line from Q to mouse pos
            Debug.DrawLine(new Vector3(Q.x / 100f, Q.y / 100f), new Vector3(P.x / 100f, P.y / 100f), Color.red);
        }

        return new Vector2(_finalCell.x + _finalCell.y + _extraRestX * _sideMultiplierX, _finalCell.y - _finalCell.x - _extraRestY * _sideMultiplierY);
    }

    private static void correctValues(out Vector2 A, out Vector2 B, out Vector2 C, out Vector2 D, out Vector2 Q, Vector2 _cellInitPos, float _xCorrection, float _yCorrection, int _xSideM, int _ySideM) {
        A = new Vector2(_cellInitPos.x + _xCorrection * _xSideM, _cellInitPos.y + _yCorrection * _ySideM + tileHeight / 2f * 100 * _ySideM);
        B = new Vector2(_cellInitPos.x + _xCorrection * _xSideM + tileWidth / 2f * 100 * _xSideM, _cellInitPos.y + _yCorrection * _ySideM + tileHeight * 100 * _ySideM);
        C = new Vector2(_cellInitPos.x + _xCorrection * _xSideM + tileWidth * 100 * _xSideM, _cellInitPos.y + _yCorrection * _ySideM + tileHeight / 2f * 100 * _ySideM);
        D = new Vector2(_cellInitPos.x + _xCorrection * _xSideM + tileWidth / 2f * 100 * _xSideM, _cellInitPos.y + _yCorrection * _ySideM);
        Q = new Vector2(_cellInitPos.x + _xCorrection * _xSideM + tileWidth / 2f * 100 * _xSideM, _cellInitPos.y + _yCorrection * _ySideM + tileHeight / 2f * 100 * _ySideM);
    }

    private static void fixCenter(ref int _extraSideX, ref int _extraSideY, int _sideX, int _sideY) {
        if (_sideY > 0 && _sideX > 0) {
            _extraSideX--;
        }
        
        if (_sideY > 0 && _sideX < 0) {
            _extraSideY++;
        }
        
        if (_sideY < 0 && _sideX > 0) {
            _extraSideY++;
        }
        
        if (_sideY < 0 && _sideX < 0) {
            _extraSideX--;
        }
    }
    
    private static void fixFirstQuadrant(string _side, int _sideX, int _sideY, ref int _extraX, ref int _extraY, ref Vector2 _finalCell) {
        if (_sideX > 0 && _sideY > 0) {
            switch (_side) {
                case "AB": {
                    _finalCell.y++;
                    _extraX--;
                    break;
                }

                case "BC": {
                    _extraY++;
                    _finalCell.y++;
                    break;
                }

                case "CD": {
                    _finalCell.y--;
                    _extraX++;
                    break;
                }

                case "DA": {
                    _finalCell.x--;
                    _extraY++;
                    break;
                }
            }
        }
    }
    private static void fixSecondQuadrant(string _side, int _sideX, int _sideY, ref int _extraX, ref int _extraY, ref Vector2 _finalCell) {
        if (_sideX <= 0 && _sideY > 0) {
            switch (_side) {
                case "AB": {
                    _finalCell.x++;
                    _extraY--;
                    break;
                }

                case "BC": {
                    _finalCell.y++;
                    _extraX++;
                    break;
                }

                case "CD": {
                    _finalCell.x--;
                    _extraY++;
                    break;
                }

                case "DA": {
                    _extraX--;
                    _finalCell.y--;
                    break;
                }
            }
        }
    }
    
    private static void fixThirdQuadrant(string _side, int _sideX, int _sideY, ref int _extraX, ref int _extraY, ref Vector2 _finalCell) {
        if (_sideX <= 0 && _sideY < 0) {
            switch (_side) {
                case "AB": {
                    // _finalCell.x++;
                    _extraY--;
                    break;
                }

                case "BC": {
                    _finalCell.x--;
                    _extraY--;
                    break;
                }

                case "CD": {
                    // _finalCell.x--;
                    _extraY++;
                    break;
                }

                case "DA": {
                    _extraX--;
                    // _finalCell.y--;
                    break;
                }
            }
        }
    }
    
    private static void fixFourthQuadrant(string _side, int _sideX, int _sideY, ref int _extraX, ref int _extraY, ref Vector2 _finalCell) {
        if (_sideX > 0 && _sideY < 0) {
            switch (_side) {
                case "AB": {
                    //_finalCell.y++;
                    _extraX--;
                    break;
                }

                case "BC": {
                    _finalCell.y--;
                    _extraX++;
                    break;
                }

                case "CD": {
                    _finalCell.y++;
                    _extraY--;
                    break;
                }

                case "DA": {
                    _finalCell.y++;
                    _extraX--;
                    break;
                }
            }
        }
    }

    private static bool lineSegmentsIntersect(Vector2 lineOneA, Vector2 lineOneB, Vector2 lineTwoA, Vector2 lineTwoB) {
        return (((lineTwoB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x) > (lineTwoA.y - lineOneA.y) * (lineTwoB.x - lineOneA.x))
            != ((lineTwoB.y - lineOneB.y) * (lineTwoA.x - lineOneB.x) > (lineTwoA.y - lineOneB.y) * (lineTwoB.x - lineOneB.x))
            
            && 
            
            ((lineTwoA.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x)) 
            != ((lineTwoB.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)));
    }
}