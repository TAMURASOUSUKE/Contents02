using System.Collections.Generic;
using UnityEngine;

public class PathFollow
{
    //ルート
    private SO_Nodes routeSo;
    //ルートのノード
    private Node next;
    public PathFollow(SO_Nodes _routeSo)
    {
        routeSo = _routeSo;
        next = routeSo.nodes[0];
    }

    /// <summary>
    /// ゴールまでの移動を計算し、移動目標を設定します
    /// </summary>
    /// <param name="_bb">
    /// ブラックボード
    /// </param>
    public void MoveTargetCalc(EnemyBlackBoardBase _bb)
    {
        if (IsCompleteMove(_bb))
        {
            //目標地点との距離
            float targetDist = Vector3.Distance(_bb.trans.position, _bb.pos);

            // ターゲットに近づくノードの探索
            Node nextNode = MapManager.Instance.GetShortestPathNextNode(_bb.pos, _bb.target.position);

            // ノードとの距離
            float nextNodeDist = Vector3.Distance(_bb.trans.position, nextNode.pos);

            // 距離が近いほうを移動目標に入れる
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

    /// <summary>
    /// ノードによる移動をランダムでします
    /// </summary>
    /// <param name="_bb">
    /// ブラックボード
    /// </param>
    public void PathRandMoveCalc(EnemyBlackBoardBase _bb)
    {
        if(IsCompleteMove(_bb))
        {
            // 現在のノードからけるノードリスト
            List<int> nextNodes = next.nextNodeIds;
            // 現在の目標ノードから、移動できるノードのインデックスをランダムに選ぶ
            int index = Random.Range(0, nextNodes.Count);
            // 移動目標変更
            int nextId = nextNodes[index];
            next = routeSo.nodes.Find(n => n.id == nextId);
            _bb.moveTarget = next.pos;
        }
    }

    /// <summary>
    /// 現在の移動目標に到達したかどうか
    /// </summary>
    /// <param name="_bb">
    /// ブラックボード
    /// </param>
    /// <returns>
    /// 到達したならtrue、してない間はfalse
    /// </returns>
    private bool IsCompleteMove(EnemyBlackBoardBase _bb)
    {
        // 移動目標がないならtrueを返す
        if(_bb.moveTarget == null)
        {
            return true;
        }

        // 移動目標との　距離が、停止距離より、短いならtrueを返す
        float dist = Vector3.Distance(_bb.pos, _bb.moveTarget.Value);
        if (dist <= _bb.stopDistance)
        {
            return true;
        }

        // それ以外はfalse
        return false;
    }

}