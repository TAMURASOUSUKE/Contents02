using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    /// <summary>
    /// 役割
    /// </summary>
    public NodeRole role;
    /// <summary>
    /// 出口方向(役職が出口ノードの時のみ使用)
    /// </summary>
    public NodeExitDir exitDir;
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
        this.role = NodeRole.NORMAL;
        this.exitDir = NodeExitDir.NONE;
        this.id = _id;
        this.pos = _pos;
        nextNodes = new List<Node>();
    }
    public Node(NodeRole _role, int _id, Vector3 _pos)
    {
        this.role = _role;
        this.exitDir = NodeExitDir.NONE;
        this.id = _id;
        this.pos = _pos;
        nextNodes = new List<Node>();
    }
    public Node(NodeRole _role, NodeExitDir _exitDir, int _id, Vector3 _pos)
    {
        this.role = _role;
        this.exitDir = NodeExitDir.NONE;
        this.exitDir = _exitDir;
        this.id = _id;
        this.pos = _pos;
        nextNodes = new List<Node>();
    }
}
