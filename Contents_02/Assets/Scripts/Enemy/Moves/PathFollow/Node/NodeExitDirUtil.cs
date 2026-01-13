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

    public static Vector3 ToVector3(this NodeExitDir dir)
    {
        return DirectionVectors[dir];
    }
}
