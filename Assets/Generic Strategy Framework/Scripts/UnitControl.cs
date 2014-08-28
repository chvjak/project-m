/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Control the Units
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class UnitControl : MonoBehaviour
{
	[HideInInspector]
	public UnitControl attacker = null;
	[HideInInspector]
	public UnitControl target = null;
	
	//All units from game
	public static List<UnitControl> allUnits = null;

	public static List<UnitControl> AllUnits {
		get {
			if (allUnits == null) {
				allUnits = GetUnits ();
				return allUnits;
			} else
				return allUnits;
		}
		set {
		}
	}	
	//The player that controls the unit
	public Player player = null;
	//The curretn node/hexagon
	public Node currentNode = null;

	public Node node {
		set {
			currentNode = value;
			//If the unit is not movel like a turrent you can do something
			if (!movel) {
				nodesMonitored.Clear ();
				nodesMonitored.Add (currentNode);
				CirculoList (currentNode.GetNodes ().ToList (), coveredArea - 1);
				foreach (var item in nodesMonitored) {
					//do something
				}
			}
		}
		get {
			return currentNode;
		}
	}
	//The area that this unit covered when its a immobile like a turrent
	public int coveredArea = 3;
	//For immobile units the covered nodes
	public List<Node> nodesMonitored = null;
	//If the unit is moving
	[HideInInspector]
	public bool moving = false;
	[HideInInspector]
	public List<GameObject> arrows = null;
	private List<Node> movNode = null;
	public bool movel = true;
	[HideInInspector]
	private bool active = false;
	[HideInInspector]
	private bool firstTurn = true;	
	//Trees
	private static List<TreeInstance> treesIns;	
	//Moving
	List<Vector2> vNodes;
	//Sets the path that the unit must follow
	public void SetWay (List<Node> moves)
	{
		movNode = new List<Node> ();
		vNodes = new List<Vector2> ();
		arrows = new List<GameObject> ();
		indice = 0;
		turnProcess = false;
		executingTurn = true;
		foreach (Node item in moves) {
			movNode.Add (item);
			vNodes.Add (new Vector2 (item.transform.position.x, item.transform.position.z));
			foreach (Transform i2 in item.transform) {
				if (i2.name == "arrow") {
					i2.name = "ArrowWay";
					arrows.Add (i2.gameObject);
				}
			}
		}
		
		moving = true;
		audio.Play();
	}
	//Get all unit from scene
	public static List<UnitControl> GetUnits ()
	{
		List<UnitControl> aux = new List<UnitControl> ();
		GameObject[] units = GameObject.FindGameObjectsWithTag("unit");

		if (units.Length == 0) return aux;

		foreach (GameObject unit in units.ToList())
			aux.Add (unit.GetComponent<UnitControl> ());
		return aux;
	}
	//If the current turn..
	[HideInInspector]
	public bool turnProcess = false;
	[HideInInspector]
	public bool executingTurn = false;
	
	//Start the script
	void Start ()
	{
		tag = "unit";
		if (!AllUnits.Exists (n => n == this))
			allUnits.Add (this);
		
		movNode = new List<Node> ();
		nodesMonitored = new List<Node> ();
	}
	//Sets the current node for this unit
	public void SetNode ()
	{
		if (currentNode != null) {
			this.transform.position = currentNode.transform.position;
			if (!movel) {
				nodesMonitored.Clear ();
				nodesMonitored.Add (currentNode);
				CirculoList (currentNode.GetNodes ().ToList (), coveredArea - 1);
				foreach (var item in nodesMonitored) {
					item.renderer.enabled = true;					
				}				
			}			
		}
	}
	
	//These variables assist in FixedUpdate
	private float normalPos = 0;
	private int indice = 0;
	Vector2 currPos;
	int turnLastMove;
	//The unit will be follow the path sets on SetWay method
	void FixedUpdate ()
	{		


		if(target != null){ // debug
			Vector3 from = this.transform.position;
			Vector3 to = target.transform.position;

			Debug.DrawLine(to, from, Color.red);
		}

		if (GameManager.gameType == GameType.RTS) {
			if ((movNode != null) && moving) {
				normalPos += 0.01f;
				currPos = Util.CubicInterpolate (vNodes [indice], vNodes [indice], vNodes [indice + 1], vNodes [indice + 1], normalPos);
				gameObject.transform.position = new Vector3 (currPos.x, gameObject.transform.position.y, currPos.y);
				if (normalPos >= 1.0f) {				
					indice++;
					node = movNode [indice];
					node.unitControl = this;
					if (player == ((GameManager)GameObject.Find ("GameManager").GetComponent<GameManager> ()).currentPlayer)
						TileControl.EnterNodeExternal (node);
					normalPos = 0;
					node.unitControl = this;					
					if (indice + 1 >= movNode.Count) {
						moving = false;
						foreach (GameObject item in arrows) {
							Destroy (item.gameObject);
						}				
						node = movNode.Last ();
						node.unitControl = this;				
						arrows.Clear ();
						movNode = null;
						moving = false;
						if (player == ((GameManager)GameObject.Find ("GameManager").GetComponent<GameManager> ()).currentPlayer)
							TileControl.EnterNodeExternal (node);						
					} else {					
						node = movNode [indice - 1];					
					}
				}
			}
		} else {
			if (executingTurn || firstTurn) {
				Debug.Log ("executingTurn=true");
				if ((movNode != null) && moving) {
					Vector2 nextTilePosition2d = Util.CubicInterpolate (vNodes [indice], vNodes [indice], vNodes [indice + 1], vNodes [indice + 1], 1);
					Vector3 nextTilePosition = new Vector3 (nextTilePosition2d.x, gameObject.transform.position.y, nextTilePosition2d.y);

					if(NeedRotation(nextTilePosition))
					{
						UpdateRotation (nextTilePosition);
					}
					else
					{
						normalPos += 0.01f; // percent of the way between tiles to cover
						currPos = Util.CubicInterpolate (vNodes [indice], vNodes [indice], vNodes [indice + 1], vNodes [indice + 1], normalPos);
						Vector3 newPosition = new Vector3 (currPos.x, gameObject.transform.position.y, currPos.y);
						
						
						gameObject.transform.position = newPosition;
                        Debug.Log ("normalPos="+normalPos);
						if (Math.Abs(normalPos - 1.0f) <= 0.01f) { // reached next tile in sequence
                            Debug.Log ("normalPos >= 1.0f");
							indice++;
							node = movNode [indice];
							//node.unitControl = this;
							if (player == ((GameManager)GameObject.Find ("GameManager").GetComponent<GameManager> ()).currentPlayer)
								TileControl.EnterNodeExternal (node);
							normalPos = 0;
							node.unitControl = this;
                            audio.Stop();

							turnProcess = true;
							executingTurn = false;
							firstTurn = false;

							if (indice + 1 >= movNode.Count) { //reached dest tile
								moving = false;
								foreach (GameObject item in arrows) {
									Destroy (item.gameObject);
								}				
								node = movNode.Last ();
								node.unitControl = this;				
								arrows.Clear ();
								movNode = null;
								
								if (player == ((GameManager)GameObject.Find ("GameManager").GetComponent<GameManager> ()).currentPlayer)
									TileControl.EnterNodeExternal (node);
							} else {					
								node = movNode [indice - 1];					
							}

						}

					}
				}
				else
				{

					if(target != null){
						Debug.Log("Not moving. Going to fire");
						Vector3 from = this.transform.position;
						Vector3 targetPos = target.transform.position;
						
						if(NeedRotation(targetPos))
						{

							UpdateRotation (targetPos);
						}
						else
						{
                            audio.Stop();
							//fire
							//flash light
							//play shot sound
							//mark target as hit
							//make target explode
							//remove target
							Infantry inf = (Infantry) this;
							inf.Fire(); // Plays fire sound. no much sense to keep it in Infantry

							Explode e = target.gameObject.GetComponent<Explode>();
							e.Set();

							moving = false;
							executingTurn = false; // not sure

						}
						
					}
				}
			}
		}
	}	
	bool NeedRotation(Vector3 targetPos) {
		var targetDir = targetPos - gameObject.transform.position;
		return (Math.Abs(Vector3.Angle(targetDir, gameObject.transform.forward)) > 1.0);
	}

	void UpdateRotation(Vector3 targetPos) {
		Vector3 targetDir = targetPos - gameObject.transform.position;
		float speed = 1;
		float step = speed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetDir, step, 0.0F);
		//Debug.DrawRay(transform.position, newDir, Color.red);
		gameObject.transform.rotation = Quaternion.LookRotation(newDir);
	}

	//Hide all arrows in the scene
	private void HideArrows ()
	{
		List<UnityEngine.Object> setasWay = (List<UnityEngine.Object>)GameObject.FindSceneObjectsOfType (typeof(Arrow)).ToList ();			
		if (setasWay != null && setasWay.Count > 0) {
			foreach (Arrow item in setasWay) {
				if (item.name == "ArrowWay")
					item.gameObject.renderer.enabled = false;
			}
		}
	}	

	public void  OnMouseOver () {
		//Input.GetMouseButton: 0 - LB, 1 - RB, 2 - MB
		GameManager gm = (GameManager)GameObject.Find ("GameManager").GetComponent<GameManager> ();
		// BUG: target automatically becomes next attacker;
		if (Input.GetMouseButton(1)) {
			if(gm.attacker == null)
			{
				gm.attacker = this;
				Debug.Log("ATTACK: attacker assigned");
			}
			else
			{
				if(this != gm.attacker)
				{
					this.attacker = gm.attacker;
					this.attacker.target = this;
					gm.attacker = null;

					Debug.Log("ATTACK: target assigned");

					this.attacker.moving = true; // or better this.attacker.attacking = true;
					this.attacker.executingTurn = true;
                    this.attacker.audio.Play();
   


				}
			}
		}

		if (Input.GetMouseButton(0)) {
			if(gm.attacker != null) gm.attacker = null;
		}

	}

	//When the players clicks on the unit
	public virtual void OnMouseDown ()
	{
		Debug.Log("Clicked");
		if (((GameManager)GameObject.Find ("GameManager").GetComponent<GameManager> ()).currentPlayer == player) {
			HideArrows ();
			active = !active;
			if (!moving) {
				if (movel && node != null) {
					node.SendMessage ("SetNode", this.gameObject);			
				}	
			} else {			
				if (arrows != null && arrows.Count > 0) {
					foreach (GameObject item in arrows) {
						if (item != null)
							item.renderer.enabled = true;
					}
				}
			}
		} else
			print ("You can control only your units");
	}
	//This unit no longer belongs to this node
	void OnCollisionExit (Collision collision)
	{
		if (node != null) {
			node.hasUnit = false;
		}
	}
	//When the unit is immobile, do this or else
	public void CirculoList (List<Node> nodes, int tam)
	{
		if (tam > 0) {
			foreach (var item in nodes) {
	
				if (!nodesMonitored.Exists (n => n == item))
					nodesMonitored.Add (item);
				CirculoList (item.GetNodes ().ToList (), tam - 1);
			}
		}
	}
	
	public bool IsTurnProcessed ()
	{
		if (executingTurn)
			return false;
		if (moving && !executingTurn && turnProcess && !firstTurn)
			return true;
		else if (!moving && !firstTurn) {
			return true;
		} else if (moving && turnProcess && firstTurn)
			return true;
		return false;
	}
	
	public void TurnProcessed ()
	{
		if (moving && turnProcess && !firstTurn)
			executingTurn = true;
		firstTurn = false;
		turnProcess = false;
	}

}
