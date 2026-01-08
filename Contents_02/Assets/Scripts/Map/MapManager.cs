using UnityEngine;

public class MapManager : MonoBehaviour
{
    SO_Nodes so;

    public static MapManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    public Node GetShortestPathNextNode(Node _current, Node _goal)
    {
        Node[] astarResult = AStar.Calc(_current, _goal);

        return astarResult[0];
    }

    /// <summary>
    /// 一番近いノードを取得
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>
    /// 現在のワールド座標
    /// </returns>
    public Node GetNearNode(Vector3 pos)
    {
        //一番近いノードが入る
        Node nearNode = so.nodes[0];
        //一番近いノードの距離
        float nearDist = Vector3.Distance(pos, nearNode.pos);
        for (int i = 1; i < so.nodes.Count; i++)
        {
            // 距離
            float dist = Vector3.Distance(pos, so.nodes[i].pos);
            // 一番近いノードの距離の更新
            if(nearDist > dist)
            {
                nearNode = so.nodes[i];
                nearDist = dist;
            }
        }

        return nearNode;
    }
}
