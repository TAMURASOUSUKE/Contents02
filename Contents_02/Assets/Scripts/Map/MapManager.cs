using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    SO_Nodes nodesSo;
    [SerializeField]
    SO_FieldData fieldSo;

    Dictionary<Vector2Int,Cell> mapCell = new Dictionary<Vector2Int,Cell>();

    public static MapManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        //SetUp();
    }

    public void SetUp()
    {
        for (int i = 0; i < fieldSo.width; i++)
        {
            for (int j = 0; j < fieldSo.depth; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                //mapCell[pos] = new Cell(pos,2);
                
            }
        }
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
        Node nearNode = nodesSo.nodes[0];
        //一番近いノードの距離
        float nearDist = Vector3.Distance(pos, nearNode.pos);
        for (int i = 1; i < nodesSo.nodes.Count; i++)
        {
            // 距離
            float dist = Vector3.Distance(pos, nodesSo.nodes[i].pos);
            // 一番近いノードの距離の更新
            if(nearDist > dist)
            {
                nearNode = nodesSo.nodes[i];
                nearDist = dist;
            }
        }

        return nearNode;
    }
}