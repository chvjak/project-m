/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Create the Node Class
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Enum with node states
public enum ENodeType
{
	AVAILABLE,
	UNAVAILABLE,
	OCCUPIED,
	MOUNTAIN
}
//Help to work with tree
public class TreeAux
{
	public int Index;
	public int Prototype;
}
//Manager the nodes
public class Node : MonoBehaviour
{	
	//The neighbors of this node, available after stats
	private Node[] nodeLinks = new Node[6];
	//The state of the node
	public ENodeType nodeType = ENodeType.AVAILABLE;	
	//Has unit?
	public bool hasUnit = false;
	//The current unit in this node
	[HideInInspector]
	public UnitControl unitControl = null;
	//The tree instances in this node
	[HideInInspector]
	public List<TreeAux> trees = new List<TreeAux> ();
	public int x;
	public int y;
	//Set a neighbor
	public void SetLink (int pos, Node node)
	{
		nodeLinks [pos] = node;
	}
	//Get the neighbors
	public Node[] GetNodes ()
	{
		return nodeLinks;
	}
	//Recursive method to get the neighbors
	public List<Node> GetNodes (int level, Node node)
	{
		List<Node> nodes = GetNodes ().ToList ();
		if (level > 0) {
			level--;
			List<Node> l = node.GetNodes ().ToList ();
			if (l.Count (n => n != null) > 0) {
				foreach (Node item in node.GetNodes()) {
					if (item != null)
						nodes.AddRange (item.GetNodes (level, item));
				}
			} else
				return nodes;
		}
		return nodes;
	}
}
