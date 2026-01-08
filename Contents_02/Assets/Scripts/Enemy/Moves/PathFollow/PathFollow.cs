using UnityEngine;

public class PathFollow
{
    //ルート
    private SO_Nodes routeSo;
    //ルートのインデックス
    private int routeIndes = 0;
    public PathFollow(SO_Nodes _routeSo)
    {
        routeSo = _routeSo;
    }
    public void MoveTargetCalc(EnemyBlackBoardBase _bb)
    {
        if (IsCompleteMove(_bb))
        {
            //目標地点との距離
            float targetDist = Vector3.Distance(_bb.trans.position, _bb.pos);

            //次のノードとの距離
            Node current = MapManager.Instance.GetNearNode(_bb.pos);
            Node goal = MapManager.Instance.GetNearNode(_bb.target.position);

            Node nextNode = MapManager.Instance.GetShortestPathNextNode(current, goal);

            float nextNodeDist = Vector3.Distance(_bb.trans.position, nextNode.pos);

            //距離が近いほうを移動目標に入れる
            if (nextNodeDist < targetDist)
            {
                _bb.moveTarget = nextNode.pos;
            }
            else
            {
                _bb.moveTarget = _bb.target.position;
            }
        }
    }
    public void PathMoveCalc(EnemyBlackBoardBase _bb)
    {
        if(IsCompleteMove(_bb))
        {
            // 現在の目標ノードから、移動できるノードのインデックスをランダムに選ぶ
            routeIndes = Random.Range(0, routeSo.nodes[routeIndes].nextNodes.Count);
            // 移動目標変更
            _bb.moveTarget = routeSo.nodes[routeIndes].pos;
        }
    }

    public bool IsCompleteMove(EnemyBlackBoardBase _bb)
    {
        // 移動目標がないならtrueを返す
        if(_bb.moveTarget == null)
        {
            return true;
        }

        Debug.Log("nullチャッククリア");
        // 移動目標との　距離が、停止距離より、短いならtrueを返す
        float dist = Vector3.Distance(_bb.pos, _bb.moveTarget.Value);
        Debug.Log(dist);
        Debug.Log(_bb.stopDistance);
        if (dist <= _bb.stopDistance)
        {
            return true;
        }

        // それ以外はfalse
        return false;
    }

}