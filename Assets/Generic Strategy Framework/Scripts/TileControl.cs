/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Control the Tiles
/*--------------------------------------------------------------*/
using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//The class to set the arrows
public class Arrow: MonoBehaviour
{
}

public class TileControl : MonoBehaviour
{
	public class posNear
	{
		public float x;
		public Node node;
		public posNear dad;
	}
	public GameObject arrowAux = null;
	public static GameObject arrow = null;
	static GameObject unit = null;
	public Node nodeLinks = null;
	public Node node = null;
	public static Node nodeOrigin = null;
	static Node nodeDestination = null;
	static private bool selectNode = false;
	static bool selectDestination = false;
	//Trees
	static List<TreeInstance> treesIns = null;
	//Instanciate the node
	void Start ()
	{
		//Get the same node
		nodeLinks = (Node)transform.GetComponent (typeof(Node));
		//Get the Arrow style and set it
		try {
			if (arrow == null) {
				arrow = ((TileControl)(GameObject.Find ("MeshNodes").transform.FindChild ("0;0").GetComponent<TileControl> ())).arrowAux;
			}
		} catch {
			//safe
		}		
	}

	void SetNode (GameObject unidade)
	{
		if (!selectNode) {
			selectNode = true;
			nodeOrigin = nodeLinks;	
			unit = unidade;
		}
	}
	//Set the pathfinding, show the Arrow (Red Ball) showing the way
	public virtual void OnMouseDown ()
	{
		if (selectNode && nodeOrigin != nodeLinks) {
			selectNode = false;
			selectDestination = true;
			nodeDestination = nodeLinks;
			int count = 0;
			bool noWay = false;
			List<posNear> open = new List<posNear> ();
			List<posNear> closed = new List<posNear> ();
			posNear currentNode = new posNear ();
			currentNode.x = Vector3.Distance (nodeOrigin.transform.position, nodeDestination.transform.position);
			currentNode.node = nodeOrigin;
			posNear origin = currentNode;			
			open.Add (currentNode);
			if (nodeOrigin.GetNodes ().Count () > 0) {
				while (currentNode.node != nodeDestination || noWay) {
					count++;
					if (count > 10000)
						break;
					foreach (Node item in currentNode.node.GetNodes()) {
						if (item != null)	
						if (!closed.Exists (n => n.node == item))
						if ((item.nodeType != ENodeType.OCCUPIED && !item.hasUnit) && item.nodeType != ENodeType.UNAVAILABLE && item.nodeType != ENodeType.MOUNTAIN) {
							posNear aux = new posNear ();							
							aux.x = Vector3.Distance (item.transform.position, nodeDestination.transform.position) + currentNode.x;
							aux.node = item;
							aux.dad = currentNode;
							if (!open.Exists (n => n.node == item))
								open.Add (aux);
						}
					}
					try {
						float posNearest = open.Min (n => n.x);
						posNear nearest = open.FirstOrDefault (n => n.x == posNearest);
						closed.Add (currentNode);
						open.Remove (currentNode);
						currentNode = nearest;
					} catch {
						noWay = true;
					}
				}
				if (!noWay) {
					List<Node> way = new List<Node> ();
					while (currentNode != origin) {
						currentNode = currentNode.dad;
						way.Add (currentNode.node);
					}
					way.Reverse ();
					way.Add (nodeDestination);
					if (unit != null) {
						unit.SendMessage ("SetWay", way);
					}
					if (selectNode && selectDestination) {
						if (nodeLinks.GetType () == typeof(Node)) {
							Vector3 vec = new Vector3 (1f, 1f, 1f);
							GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
							obj.transform.position = vec;
						}
					}
					selectNode = !selectNode;
					selectNode = false;
					nodeOrigin = null;
				}				
			}
		} else {			
			List<Object> setas = (List<Object>)GameObject.FindSceneObjectsOfType (typeof(Arrow)).ToList ();			
			if (setas != null && setas.Count > 0) {
				foreach (Arrow item in setas) {
					if (item.name == "SetaWay")
						item.gameObject.renderer.enabled = false;
				}
			}

			if(GameManager.deploy >= 0)
			{
				Vector3 position = this.GetComponent<Transform>().position;
				position.y = 1;

				const int max_tank = 6;

				// to be store in separate obj e.g UnitMgr alomg with
				string[] tank_prefabs = new string[max_tank]{"FutTankPrefab",
					"pz",
					"kreiser",
					"motopexota",
				    "inf2",
				    "FlakBattery"};

				//GameObject TankPrefab = (GameObject)AssetDatabase.LoadAssetAtPath (tank_prefabs[GameManager.deploy], typeof(GameObject));
				GameObject TankPrefab = (GameObject)Resources.Load (tank_prefabs[GameManager.deploy], typeof(GameObject));
				
				
				GameObject tank = Instantiate(TankPrefab, position,  Quaternion.identity) as GameObject;
				Infantry unit  = tank.GetComponent<Infantry> ();
				unit.player = ((GameManager)GameObject.Find ("GameManager").GetComponent<GameManager> ()).currentPlayer;
				unit.node = this.GetComponent<Node>();


				GameManager.deploy = -2;
			}
		}
	}
	//Set the pathfinding, show the Arrow (Red Ball) showing the way
	void OnMouseEnter ()
	{
		if (nodeOrigin != null && selectNode) {
			if (nodeLinks.nodeType == ENodeType.AVAILABLE) {
				foreach (Node item in MeshNodes.AllNodes) {
					foreach (Transform child in item.gameObject.transform) {
						if (child.name == "arrow") {
							Destroy (child.gameObject);
						}
					}
				}
				selectDestination = true;
				nodeDestination = nodeLinks;
				int count = 0;
				bool noWay = false;
				List<posNear> open = new List<posNear> ();
				List<posNear> closed = new List<posNear> ();
				posNear currentNode = new posNear ();
				currentNode.x = Vector3.Distance (nodeOrigin.transform.position, nodeDestination.transform.position);
				currentNode.node = nodeOrigin;
				posNear origin = currentNode;			
				open.Add (currentNode);
				if (nodeOrigin.GetNodes ().Count () > 0) {
					while (nodeDestination != null && (currentNode.node != nodeDestination || nodeLinks != nodeDestination)) {
						count++;
						if (count > 10000)
							break;
						foreach (Node item in currentNode.node.GetNodes()) {
							if (item != null)	
							if (!closed.Exists (n => n.node == item))
							if ((item.nodeType != ENodeType.OCCUPIED && !item.hasUnit) && item.nodeType != ENodeType.UNAVAILABLE && item.nodeType != ENodeType.MOUNTAIN) {
								posNear aux = new posNear ();							
								aux.x = Vector3.Distance (item.transform.position, nodeDestination.transform.position) + currentNode.x;
								aux.node = item;
								aux.dad = currentNode;
								if (!open.Exists (n => n.node == item)) {
									open.Add (aux);
								}
							}
						}
						if (open.Count > 0) {
							float posNearest = open.Min (n => n.x);
							posNear nearest = open.FirstOrDefault (n => n.x == posNearest);
							closed.Add (currentNode);
							open.Remove (currentNode);
							currentNode = nearest;
						} else {
							noWay = true;
						}
					}
					if (nodeDestination == null)
						noWay = true;
					if (!noWay) {
						List<Node> way = new List<Node> ();
						while (currentNode != origin) {
							currentNode = currentNode.dad;
							way.Add (currentNode.node);
						}
						way.Add (nodeDestination);
						way.Reverse ();
						for (int i = 0; i < way.Count; i++) {
							if (i + 1 < way.Count) {
								for (float j = 0.25f; j <= 1.0f; j+=0.25f) {
									GameObject goArrow = (GameObject)GameObject.Instantiate (arrow);
									goArrow.AddComponent<Arrow> ();
									goArrow.name = "arrow";
									goArrow.transform.position = new Vector3 (way [i].gameObject.transform.position.x, 18.094069e-07f, way [i].gameObject.transform.position.z);
									Vector2 v1 = new Vector2 (way [i + 1].gameObject.transform.position.x, way [i + 1].gameObject.transform.position.z);
									Vector2 v2 = new Vector2 (way [i].gameObject.transform.position.x, way [i].gameObject.transform.position.z);
									Vector2 meio = v1 + j * (v2 - v1);
									goArrow.transform.position = new Vector3 (meio.x, 18.094069e-07f, meio.y);
									Quaternion targetRotation = Quaternion.LookRotation (way [i + 1].gameObject.transform.position - goArrow.transform.position, Vector3.up);
									goArrow.transform.rotation = targetRotation;
									goArrow.transform.parent = way [i].gameObject.transform;
									foreach (Transform child in goArrow.transform)
										GameObject.DestroyImmediate (child.gameObject);
								}
							}
						}		
						for (float j = 0.25f; j <= 1.0f; j+=0.25f) {
							GameObject goArrowLast = (GameObject)GameObject.Instantiate (arrow);
							goArrowLast.AddComponent<Arrow> ();
							goArrowLast.name = "arrow";
							goArrowLast.transform.position = new Vector3 (way [way.Count - 1].gameObject.transform.position.x, 18.094069e-07f, way [way.Count - 1].gameObject.transform.position.z);
							Vector2 v11 = new Vector2 (nodeDestination.gameObject.transform.position.x, nodeDestination.gameObject.transform.position.z);
							Vector2 v22 = new Vector2 (way [way.Count - 1].gameObject.transform.position.x, way [way.Count - 1].gameObject.transform.position.z);
							Vector2 meio2 = v11 + j * (v22 - v11);
							goArrowLast.transform.position = new Vector3 (meio2.x, 18.094069e-07f, meio2.y);
							foreach (Transform child in nodeDestination.gameObject.transform) {
								if (child.name == "arrow")
									Destroy (child.gameObject);
							}
							//Rotaciona o vetor do meio
							Quaternion targetRotationDestino = Quaternion.LookRotation (nodeDestination.gameObject.transform.position - goArrowLast.transform.position, Vector3.up);
							goArrowLast.transform.rotation = targetRotationDestino;
							goArrowLast.transform.parent = way [way.Count - 1].gameObject.transform;
							foreach (Transform child in goArrowLast.transform)
								GameObject.DestroyImmediate (child.gameObject);
						}
					}
				}
			} else {
				NoWay ();
			}
		}
	}
	//Hide all arrows in the scene
	public static void NoWay ()
	{
		nodeDestination = null;
		List<UnityEngine.Object> setasWay = (List<UnityEngine.Object>)GameObject.FindSceneObjectsOfType (typeof(Arrow)).ToList ();			
		if (setasWay != null && setasWay.Count > 0) {
			foreach (Arrow item in setasWay) {
				if (item.name == "arrow")
					item.gameObject.renderer.enabled = false;
			}
		}
	}
	//When a unit enters the node
	public static void EnterNodeExternal (Node node)
	{
		if (node != null) {			
			TerrainFoW terr = (TerrainFoW)GameObject.Find ("Terrain").GetComponent<TerrainFoW> ();
			foreach (Node item in GetNodes(terr, node)) {				
				if (item != null) {
					if (item.unitControl != null) {
						if (item.unitControl.player != ((GameManager)GameObject.Find ("GameManager").GetComponent<GameManager> ()).currentPlayer) {
							item.unitControl.renderer.enabled = true;
						}
					}
					if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
						treesIns = new List<TreeInstance> (Terrain.activeTerrain.terrainData.treeInstances);
						List<TreeInstance> treesInsparente = new List<TreeInstance> ();			
						for (int i = 0; i < treesIns.Count; i++) {
							TreeInstance ti = treesIns [i];
							if (item.trees.Exists (n => n.Index == i))
								foreach (TreeAux item2 in item.trees) {
									if (i == item2.Index)
										ti.prototypeIndex = item2.Prototype;
								}						
							treesInsparente.Add (ti);
						}
						Terrain.activeTerrain.terrainData.treeInstances = treesInsparente.ToArray ();						
					}
				}
			}
			try {
				if (terr != null) 
					terr.WhitenTerrain (node.transform.position, terr.ExplorationSize);
			} catch {
				//safe
			}
		}
	}	
	//Call the recursive method to show the trees, you can adjust the values
	public static List<Node> GetNodes (TerrainFoW terr, Node node)
	{
		if (terr != null) {
			if (terr.ExplorationSize < 40)
				return new List<Node> (){ node };
			else {
				int res = Mathf.RoundToInt (terr.ExplorationSize / 50);
				if (res == 0) 
					res = 1;		
				return node.GetNodes (res, node);
			}
		} else {
			return node.GetNodes (2, node);
		}
	}
}


