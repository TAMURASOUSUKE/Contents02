using UnityEngine;

public class Cell
{
    /// <summary>
    /// セル座標
    /// </summary>
    public Vector2Int cellPos;

    /// <summary>
    /// ワールド座標
    /// </summary>
    public Vector3 pos;

    /// <summary>
    /// ノード自体の移動コスト
    /// </summary>
    public int cost;

    /// <summary>
    /// セル内にある、ノード
    /// </summary>
    public SO_Nodes so_nodes;

    public Cell(Vector2Int _cellPos, int _cost)
    {
        this.cellPos = _cellPos;
        this.pos = Vector3.zero;
        this.cost = _cost;
        this.so_nodes = null;
    }

    //オクタイル用8方向定数
    public static readonly Vector2Int[] DIR_8 =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),  
    };
}
