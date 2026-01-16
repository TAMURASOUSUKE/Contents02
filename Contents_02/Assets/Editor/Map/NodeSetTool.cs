using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class NodeSetTool: EditorWindow
{
    /// <summary>
    /// ノードを定義するオブジェクト
    /// </summary>
    GameObject rootPrefab;
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
    /// 設置するノードの役割
    /// </summary>
    NodeRole plantNodeRole;

    /// <summary>
    /// 設置するノードが出口だった時の出口方向
    /// </summary>
    NodeExitDir plantNodeExitDir;

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

        GUILayout.Label("オブジェクト欄にプレファブを入れて、\nSO欄に対応した、SOを入れて設置してください");

        // ノードを定義するオブジェクトの代入用インスペクター
        rootPrefab = (GameObject)EditorGUILayout.ObjectField("ノードを定義するオブジェクト", rootPrefab, typeof(GameObject), false);
        if(rootPrefab != null)
        {
            PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(rootPrefab));
        }

        // SOの代入用インスペクター
        so = (SO_Nodes)EditorGUILayout.ObjectField("ノードを追加したいSO_Nodes", so, typeof(SO_Nodes), false);

        // ノードサイズの変更用スライダー
        nodeSize = EditorGUILayout.Slider("ノードサイズ", nodeSize, 0.1f, 10.0f);

        // ノードの役割
        plantNodeRole = (NodeRole)EditorGUILayout.EnumPopup("設置ノードの役割", plantNodeRole);

        // 設置ノードが出口ノードだった場合
        if(plantNodeRole == NodeRole.EXIT)
        {
            GUILayout.Label("NORTHがZ方向にプラス、EASTがX方向にプラスです。");
            // 出口方向を決める
            plantNodeExitDir = (NodeExitDir)EditorGUILayout.EnumPopup("出口方向", plantNodeExitDir);
        }

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
        if (so == null || rootPrefab == null)
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
            // 位置はオブジェクトの相対座標で取る
            Node node = new Node(so.nodes.Count, hitinfo.point - rootPrefab.transform.position);
            // 生成ノードの役割が出口なら、追加情報
            if (plantNodeRole == NodeRole.EXIT)
            {
                node.role = NodeRole.EXIT;
                node.exitDir = plantNodeExitDir;
            }

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
            
            // すべてのノードの移動できるノードリストから除外
            foreach (Node node in so.nodes)
            {
                node.nextNodeIds.Remove(select.id);
                node.nextNodeIds.RemoveAll(n => n == null);
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
    /// <param name="_node01">
    /// 一つ目のノード
    /// </param>
    /// /// <param name="_node02">
    /// 二つ目のノード
    /// </param>
    void ConnecteNode(Node _node01, Node _node02)
    {
        // nullチェック
        if (so == null)
        {
            return;
        }

        Undo.RecordObject(so, "connecte node");
        
        // 移動できるノードリストにないなら追加
        if (_node01.nextNodeIds.Contains(_node02.id) == false)
        {
            _node01.nextNodeIds.Add(_node02.id);
        }

        if (_node02.nextNodeIds.Contains(_node01.id) == false)
        {
            _node02.nextNodeIds.Add(_node01.id);
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
        if (so == null || rootPrefab == null)
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
            if (HandleUtility.DistanceToCircle(node.pos + rootPrefab.transform.position, nodeSize) <= 0f)
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
        if (so == null || rootPrefab == null)
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

            if (node.role == NodeRole.NORMAL)
            {
                Handles.SphereHandleCap(0, node.pos + rootPrefab.transform.position, Quaternion.identity, nodeSize, EventType.Repaint);

            }
            else
            {
                Quaternion rot = Quaternion.LookRotation(node.exitDir.ToVector3());
                Handles.ArrowHandleCap(0, node.pos + rootPrefab.transform.position, rot, nodeSize, EventType.Repaint);
            }

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
        if(so == null || rootPrefab == null)
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
            if(node.nextNodeIds.Count > 0)
            {
                foreach (int nextId in node.nextNodeIds)
                {
                    int min = node.id < nextId ? node.id : nextId;
                    int max = min == node.id ? nextId : node.id;
                    // 描画済みノードに追加できるかどうか
                    if (!drawnNodes.Add((min, max)))
                    {
                        continue;
                    }

                    Node nextNode = so.nodes.Find(n => n.id == nextId);

                    Handles.DrawLine(node.pos + rootPrefab.transform.position, nextNode.pos + rootPrefab.transform.position);
                }
            }
        }
    }
}
