/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Control the MeshNodes
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class MeshNodes: MonoBehaviour
{
	[HideInInspector]
	public GameObject Hexagon;
	[HideInInspector]
	public GameObject Arrow;
	[HideInInspector]
	public static int Xsize;
	[HideInInspector]
	public static int Ysize;
	[HideInInspector]
	public static List<Node> allNodes = null;
	
	public static List<Node> AllNodes {
		get {
			if (allNodes != null)
				return allNodes;
			else {
				MeshNodes mn = (MeshNodes)GameObject.FindObjectOfType(typeof(MeshNodes));
				return mn.GetNodes();
			}
		}
		set {
			allNodes = value;
		}
	}
	[HideInInspector]
	public static Node[][] nodes = null;
	private float hw;
	private float hh;
	[HideInInspector]
	public MeshNodes meshNodes = null;
	
	public List<Node> GetNodes ()
	{
		List<Node> nodes = new List<Node> ();
		foreach (Transform child in gameObject.transform) {
			if (child.GetComponent<Node> () != null)
				nodes.Add ((Node)child.GetComponent<Node> ());
		}
		return nodes;
	}
	
	void Start ()
	{
		Node last = gameObject.transform.GetChild (gameObject.transform.childCount - 1).GetComponent<Node> ();
		
		string[] sLast = last.name.Split (';');	
		Xsize = Convert.ToInt32 (sLast [0]) + 1;
		Ysize = Convert.ToInt32 (sLast [1]) + 1;		
		
		nodes = new Node[Xsize][];
		for (int i = 0; i < Xsize; i++) {
			nodes [i] = new Node[Ysize];
		}
		
		allNodes = new List<Node>();
		foreach (Transform child in gameObject.transform) {
			if (child.GetComponent<Node> () != null) {
				Node n = (Node)child.GetComponent<Node> ();				
				nodes [n.x] [n.y] = n;				
				allNodes.Add (n);
			}
		}
		//Sets all neighbors
		bool atoffs = true;
		for (int x = 0; x < Xsize; x++) {		
			for (int y = 0; y < Ysize; y++) {
				//right
				if ((x - 1) >= 0) 
					nodes [x] [y].SetLink (0, nodes [x - 1] [y]);
				//left
				if ((x + 1) < Xsize) 
					nodes [x] [y].SetLink (1, nodes [x + 1] [y]);				
				if (atoffs) {
					//top left
					if ((x >= 0) && ((y - 1) >= 0)) 
						nodes [x] [y].SetLink (2, nodes [x] [y - 1]);				
					//top right
					if (((y - 1) >= 0) && ((x + 1) < Xsize)) 
						nodes [x] [y].SetLink (3, nodes [x + 1] [y - 1]);				
					//down left
					if ((x >= 0) && ((y + 1) < Ysize)) 
						nodes [x] [y].SetLink (4, nodes [x] [y + 1]);				
					//down right
					if (((y + 1) < Ysize) && ((x + 1) < Xsize)) 
						nodes [x] [y].SetLink (5, nodes [x + 1] [y + 1]);
				} else {
					//top left
					if (((x - 1) >= 0) && ((y - 1) >= 0)) 
						nodes [x] [y].SetLink (2, nodes [x - 1] [y - 1]);
					//top right
					if ((y - 1) >= 0) 
						nodes [x] [y].SetLink (3, nodes [x] [y - 1]);
					//down left
					if (((x - 1) >= 0) && ((y + 1) < Ysize)) 
						nodes [x] [y].SetLink (4, nodes [x - 1] [y + 1]);
					//down right
					if ((y + 1) < Ysize) 
						nodes [x] [y].SetLink (5, nodes [x] [y + 1]);
					
				}
				atoffs = !atoffs;
			}			
		}
		//Clean the vector-----
		for (int i = 0; i < Xsize; i++) {
			nodes [i] = null;
		}
		nodes = null;
		//---------------------
		//Paint in Black the nodes and search for mountains
		foreach (Node item in AllNodes) {
			item.renderer.material.color = new Color (1, 1, 1, 0);
			item.renderer.material.mainTextureScale = new Vector2 (1, 1);
			item.renderer.material.shader = Shader.Find ("Transparent/Diffuse");
			if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
				if (Terrain.activeTerrain.SampleHeight (item.transform.position) > 0) {
					item.nodeType = ENodeType.MOUNTAIN;
				}
			}
		}
	}
	//Get the first position for tha hexagons
	private Vector3 GetFirstPosition ()
	{
		Vector3 initPos;
		initPos = new Vector3 (-hw * Xsize / 2f + hw / 2, 0,
		                       Ysize / 2f * hh - hh / 2);
		
		return initPos;
	}
	//Get the position of the hexagon
	private Vector3 GetPosition (Vector2 gridPos)
	{
		Vector3 initPos = GetFirstPosition ();
		float offset = 0;
		if (gridPos.y % 2 != 0)
			offset = hw / 2;
		float x = initPos.x - offset + gridPos.x * hw;
		float y = initPos.y - gridPos.y * hh * 0.75f;
		return new Vector3 (x, 0, y);
	}
	//Methos extern to generate nodes
	public void GenerateNodes (int pXSize, int pYSize, GameObject Hexagon, GameObject Arrow)
	{
		Xsize = pXSize;
		Ysize = pYSize;
		this.Hexagon = Hexagon;
		this.Arrow = Arrow;
		GenerateNodes (pXSize, pYSize);	
		foreach (Node item in AllNodes) {
			item.gameObject.AddComponent<MeshCollider> ();
		}
	}
	//Generate all nodes
	private void GenerateNodes (int Xsize, int Ysize)
	{
		nodes = new Node[Xsize][];
		for (int i = 0; i < Xsize; i++) {
			nodes [i] = new Node[Ysize];
		}
		AllNodes = new List<Node> ();		
		hw = Hexagon.renderer.bounds.size.x;
		hh = Hexagon.renderer.bounds.size.y;		
		GameObject meshNodes = new GameObject ("MeshNodes");
		meshNodes.AddComponent<MeshNodes>();
		for (int x = 0; x < Xsize; x++) {
			for (int y = 0; y < Ysize; y++) {			
				GameObject hex = (GameObject)Instantiate (Hexagon);						
				TileControl mt = (TileControl)hex.AddComponent<TileControl> ();	
				if (y == 0 && x == 0)
					mt.arrowAux = Arrow;				
				Node h = (Node)hex.AddComponent<Node> ();
				h.nodeType = ENodeType.AVAILABLE;
				hex.name = (x).ToString () + ";" + (y).ToString ();
				Vector2 gridPos = new Vector2 (x, y);
				hex.transform.position = GetPosition (gridPos);
				hex.transform.parent = meshNodes.transform;
				hex.transform.Rotate (-270.0f, 0, 0);
				h.x = x;
				h.y = y;
				nodes [x] [y] = h;
				AllNodes.Add (h);
			}
		}	
		meshNodes.transform.localScale = new Vector3 (2000.0f, 1500.0f, 2000.0f);		
		meshNodes.transform.position = new Vector3 (0, 0.01f, 0);
	}
}