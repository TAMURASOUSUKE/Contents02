using UnityEngine;

public class ACell
{
    /// <summary>
    /// どのセルか
    /// </summary>
    public Cell cell;
    /// <summary>
    /// 推定移動コスト
    /// </summary>
    public int cost;
    /// <summary>
    /// オクタイル距離のためfloat型
    /// </summary>
    public float dist;
    /// <summary>
    /// スコア
    /// </summary>
    public float score;
    /// <summary>
    /// 計算時親セル
    /// </summary>
    public ACell parent;

    public ACell(Cell _cell, int _cost, float _dist, ACell _parent)
    {
        this.cell = _cell;
        this.cost = _cost;
        this.dist = _dist;
        this.score = _cost + _dist; //スコアは、距離とコストの合計なので自動で計算
        this.parent = _parent;
    }
}
