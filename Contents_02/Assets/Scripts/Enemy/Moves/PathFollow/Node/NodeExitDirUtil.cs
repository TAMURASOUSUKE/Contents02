using System.Collections.Generic;
using UnityEngine;

public static class NodeExitDirUtil
{
    public static readonly Dictionary<NodeExitDir, Vector3> DirectionVectors = new()
    {
        { NodeExitDir.NORTH,      Vector3.forward },
        { NodeExitDir.SOUTH,      Vector3.back },
        { NodeExitDir.EAST,       Vector3.right },
        { NodeExitDir.WEST,       Vector3.left },
        { NodeExitDir.NORTH_EAST, (Vector3.forward + Vector3.right).normalized },
        { NodeExitDir.NORTH_WEST, (Vector3.forward + Vector3.left).normalized },
        { NodeExitDir.SOUTH_EAST, (Vector3.back + Vector3.right).normalized },
        { NodeExitDir.SOUTH_WEST, (Vector3.back + Vector3.left).normalized },
    };

    /// <summary>
    /// 方向をワールド空間上の方向にする
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Vector3 ToVector3(this NodeExitDir dir)
    {
        return DirectionVectors[dir];
    }

    /// <summary>
    /// 逆方向を取得
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static NodeExitDir GetOppositeDir(this NodeExitDir dir)
    {
        switch (dir)
        {
            case NodeExitDir.NORTH: return NodeExitDir.SOUTH;
            case NodeExitDir.SOUTH: return NodeExitDir.NORTH;
            case NodeExitDir.EAST: return NodeExitDir.WEST;
            case NodeExitDir.WEST: return NodeExitDir.EAST;
            case NodeExitDir.NORTH_EAST: return NodeExitDir.SOUTH_WEST;
            case NodeExitDir.NORTH_WEST: return NodeExitDir.SOUTH_EAST;
            case NodeExitDir.SOUTH_EAST: return NodeExitDir.NORTH_WEST;
            case NodeExitDir.SOUTH_WEST: return NodeExitDir.NORTH_EAST;
            default: return NodeExitDir.NONE;
        }
    }

    /// <summary>
    /// 方向と位置から隣り合うセルの座標を取得
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Vector2Int GetNeighborCell(Vector2Int pos, NodeExitDir dir)
    {
        switch (dir)
        {
            case NodeExitDir.NORTH: return pos + Vector2Int.up;
            case NodeExitDir.SOUTH: return pos + Vector2Int.down;
            case NodeExitDir.EAST: return pos + Vector2Int.right;
            case NodeExitDir.WEST: return pos + Vector2Int.left;
            case NodeExitDir.NORTH_EAST: return pos + Vector2Int.up + Vector2Int.right;
            case NodeExitDir.NORTH_WEST: return pos + Vector2Int.up + Vector2Int.left;
            case NodeExitDir.SOUTH_EAST: return pos + Vector2Int.down + Vector2Int.right;
            case NodeExitDir.SOUTH_WEST: return pos + Vector2Int.down + Vector2Int.left;
            default: return pos;
        }
    }

}
