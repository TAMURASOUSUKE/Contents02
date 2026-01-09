using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    /// <summary>
    /// ID
    /// </summary>
    public int id;
    /// <summary>
    /// ノードの位置
    /// </summary>
    public Vector3 pos;
    /// <summary>
    /// 移動できるノードリスト
    /// </summary>
    public List<Node> nextNodes;

    public Node(int _id, Vector3 _pos)
    {
        this.id = _id;
        this.pos = _pos;
        nextNodes = new List<Node>();
    }
}
