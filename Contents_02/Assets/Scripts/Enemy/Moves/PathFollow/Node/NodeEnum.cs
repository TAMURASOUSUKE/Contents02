using UnityEngine;

/// <summary>
/// ノードの役割
/// </summary>
public enum NodeRole
{
    NORMAL,
    EXIT
}

/// <summary>
/// ノードの出入口方向
/// </summary>
public enum NodeExitDir
{
    /// <summary>
    /// 役割が、NORMAL用
    /// </summary>
    NONE,
    /// <summary>
    /// ワールド座標だとZ方向にプラス
    /// </summary>
    NORTH,
    SOUTH,
    /// <summary>
    /// ワールド座標だとX方向にプラス
    /// </summary>
    EAST,
    SOUTH_EAST,
    NORTH_EAST,
    WEST,
    SOUTH_WEST,
    NORTH_WEST,
}
