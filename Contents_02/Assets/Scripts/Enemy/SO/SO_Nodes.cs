using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Nodes", menuName = "Scriptable Objects/SO_Nodes")]
public class SO_Nodes : ScriptableObject
{
    public List<Node> nodes;

}

[CreateAssetMenu(fileName = "SO_NodesLibrary", menuName = "Scriptable Objects/SO_NodesLibrary")]
public class SO_NodesLibrary : ScriptableObject
{
    public List<SO_Nodes> nodesLibrary;
}
