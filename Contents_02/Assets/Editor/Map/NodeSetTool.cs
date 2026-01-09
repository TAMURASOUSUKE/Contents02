using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeSetTool: EditorWindow
{
    /// <summary>
    /// 変更するSO
    /// </summary>
    SO_Nodes so;
    /// <summary>
    /// 選択されたノード
    /// </summary>
    Node select;

    /// <summary>
    /// ノードの表示サイズ
    /// </summary>
    float nodeSize = 1.0f;

    /// <summary>
    /// 接続モード
    /// </summary>
    bool isPlant = true;
    [MenuItem("Tools/MapNodePlanter")]
    static void Open()
    {
        GetWindow<NodeSetTool>("MapNodePlanter");
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI()
    {
        GUILayout.Label("マップ上にノードを配置するツール");

        // SOの代入用インスペクター
        so = (SO_Nodes)EditorGUILayout.ObjectField("ノードを追加したいSO_Nodes", so, typeof(SO_Nodes), false);

        // ノードサイズの変更用スライダー
        nodeSize = EditorGUILayout.Slider("ノードサイズ", nodeSize, 0.1f, 10.0f);

        // 設置モード切り替え
        GUILayout.Label("設置モード切り替え\nチェックが入っている間ノードの設置ができます。");
        isPlant = EditorGUILayout.Toggle( isPlant);
    }
    
    void OnSceneGUI(SceneView _sceneView)
    {
        Event e = Event.current;

        // nullチェック
        if (so == null)
        {
            return;
        }

        PlantNode(e);
        DeleteNode(e);

        DrawNode(e);
        SelectNode(e);
        DrawConnection(e);
    }

    /// <summary>
    /// ノード設置関数
    /// </summary>
    /// <param name="_e">
    /// 現在のイベント
    /// </param>
    void PlantNode(Event _e)
    {
        // nullチェック
        if (so == null)
        {
            return;
        }

        // イベントチェック
        if (_e.type != EventType.MouseDown)
        {
            return;
        }

        // フラグチェック
        if(!isPlant)
        {
            return;
        }

        // マウスからのレイ
        Ray ray = HandleUtility.GUIPointToWorldRay(_e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitinfo) == false)
        {
            return;
        }

        if (_e.type == EventType.MouseDown && _e.button == 0)
        {
            // Undo作成
            Undo.RecordObject(so, "add node");
            // ノード作成
            // ノードIDは0から、追加された順
            Node node = new Node(so.nodes.Count, hitinfo.point);
            // リストに追加
            so.nodes.Add(node);
            // 通知
            EditorUtility.SetDirty(so);

            // イベント使用
            _e.Use();
        }
    }

    /// <summary>
    /// ノード削除関数
    /// </summary>
    /// <param name="_e">
    /// 現在のイベント
    /// </param>
    void DeleteNode(Event _e)
    {
        // nullチェック
        if (so == null || select == null)
        {
            return;
        }

        // フラグチェック
        if (isPlant)
        {
            return;
        }

        // deleteボタンが押されたら、削除
        if (_e.type == EventType.KeyDown && _e.keyCode == KeyCode.Delete)
        {
            // Undo作成
            Undo.RecordObject(so, "delete node");
            // SOのリストから除外
            so.nodes.Remove(select);
            // IDの修正
            for(int id = 0; id < so.nodes.Count; id++)
            {
                if(so.nodes[id].id != id)
                {
                    so.nodes[id].id = id;
                }
            }
            // すべてのノードの移動できるノードリストから除外
            foreach (Node node in so.nodes)
            {
                node.nextNodes.Remove(select);
            }

            // 選択されたノードをnullに戻す
            select = null;

            // 通知
            EditorUtility.SetDirty(so);

            // イベント使用
            _e.Use();
        }
    }

    /// <summary>
    /// ノード接続関数
    /// </summary>
    /// <param name="node01_">
    /// 一つ目のノード
    /// </param>
    /// /// <param name="node02_">
    /// 二つ目のノード
    /// </param>
    void ConnecteNode(Node node01_, Node node02_)
    {
        // nullチェック
        if (so == null)
        {
            return;
        }

        Undo.RecordObject(so, "connecte node");
        
        // 移動できるノードリストにないなら変更
        if (node01_.nextNodes.Contains(node02_) == false)
        {
            node01_.nextNodes.Add(node02_);
        }

        if (node02_.nextNodes.Contains(node01_) == false)
        {
            node02_.nextNodes.Add(node01_);
        }

        EditorUtility.SetDirty(so);
    }

    /// <summary>
    /// ノード選択関数
    /// </summary>
    /// <param name="_e">
    /// 現在のイベント
    /// </param>
    void SelectNode(Event _e)
    {
        // nullチェック
        if (so == null)
        {
            return;
        }

        // イベントチェック
        if (_e.type != EventType.MouseDown)
        {
            return;
        }

        // フラグチェック
        if (isPlant)
        {
            return;
        }

        foreach (Node node in so.nodes)
        {
            //距離判定
            if (HandleUtility.DistanceToCircle(node.pos, nodeSize) <= 0f)
            {
                // 選択されたノードが埋まってるかどうか
                if (select == null)
                {
                    // 埋まっていないなら、埋める
                    select = node;
                }
                else
                {
                    // 同じノードなら選択から外す
                    if (select.id == node.id)
                    {
                        select = null;
                    }
                    // そうでないなら接続
                    else
                    {
                        // 埋まっているなら、接続
                        ConnecteNode(select, node);
                        // 接続時の親ノードをnullに戻す
                        select = null;
                    }
                }

                // イベント使用
                _e.Use();

                break;
            }
        }
    }

    /// <summary>
    /// 現在設置されているノード描画関数
    /// </summary>
    /// /// <param name="_e">
    /// 現在のイベント
    /// </param>
    void DrawNode(Event _e)
    {
        // nullチェック
        if (so == null)
        {
            return;
        }

        // イベントチェック
        if(_e.type != EventType.Repaint)
        {
            return;
        }

        foreach(var node in so.nodes)
        {
            //色が変わった時元に戻すよう
            Color prev = Handles.color;

            // nullチェック
            if (select != null)
            {
                // 選択されたノードなら、色を変える
                if (select.id == node.id)
                {
                    Handles.color = Color.red;
                }
            }

            Handles.SphereHandleCap(0, node.pos, Quaternion.identity, nodeSize, EventType.Repaint);

            // 色を元に戻す
            Handles.color = prev;
        }
    }

    /// <summary>
    /// ノード間の接続の描画関数
    /// </summary>
    /// /// <param name="_e">
    /// 現在のイベント
    /// </param>
    void DrawConnection(Event _e)
    {
        // nullチェック
        if(so == null)
        {
            return;
        }

        if (_e.type != EventType.Repaint)
        {
            return;
        }

        // 描画済み関数
        HashSet<(int, int)> drawnNodes = new HashSet<(int, int)>();

        // すべてのノード
        foreach(Node node in so.nodes)
        {
            // 移動できるノードリストが0より大きいなら
            if(node.nextNodes.Count > 0)
            {
                foreach (Node next in node.nextNodes)
                {
                    int min = Mathf.Min(node.id , next.id);
                    int max = Mathf.Max(node.id , next.id);
                    // 描画済みノードに追加できるかどうか
                    if (!drawnNodes.Add((min, max)))
                    {
                        continue;
                    }

                    Handles.DrawLine(node.pos, next.pos);
                }
            }
        }
    }
}
