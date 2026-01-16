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
    public string id = null;
    /// <summary>
    /// ノードの位置
    /// </summary>
    public Vector3 pos;
    /// <summary>
    /// 移動できるノードリスト
    /// </summary>
    public List<string> nextNodeIds = new List<string>();

    public Node(string _id, Vector3 _pos)
    {
        this.role = NodeRole.NORMAL;
        this.exitDir = NodeExitDir.NONE;
        this.id = _id;
        this.pos = _pos;
    }
    public Node(NodeRole _role, string _id, Vector3 _pos)
    {
        this.role = _role;
        this.exitDir = NodeExitDir.NONE;
        this.id = _id;
        this.pos = _pos;
    }
    public Node(NodeRole _role, NodeExitDir _exitDir, string _id, Vector3 _pos)
    {
        this.role = _role;
        this.exitDir = NodeExitDir.NONE;
        this.exitDir = _exitDir;
        this.id = _id;
        this.pos = _pos;
    }
}
