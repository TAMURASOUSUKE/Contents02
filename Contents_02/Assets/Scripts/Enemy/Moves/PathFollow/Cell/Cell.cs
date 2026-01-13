using UnityEngine;

public struct Cell
{
    /// <summary>
    /// セル座標
    /// </summary>
    public Vector2Int pos;
    public int cost;

    public Cell(Vector2Int _pos, int _cost)
    {
        this.pos = _pos;
        this.cost = _cost;
    }

    //オクタイル用8方向定数
    public static readonly Vector2Int[] DIR_8 =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
    };
}
