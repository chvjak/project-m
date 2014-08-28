/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Control the Game
/*--------------------------------------------------------------*/
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum GameType
{
	//Real Time Strategy 
	RTS,
	//Turn Based Strategy
	TBS
}

public class GameManager : MonoBehaviour
{
	//The current player, one for machine
	public  Player currentPlayer = null;

	[HideInInspector]
	public UnitControl attacker = null;

	//List of all players in game
	[HideInInspector]
	public List<Player> players;
	//The type of Game, by default is TBS
	public static GameType gameType = GameType.TBS;
	//Time control
	private System.DateTime time;
	private System.DateTime temp;
	//Public Internal-------------
	//The current turn of the game
	[HideInInspector]
	public static int CurrentTurn = 0;
	[HideInInspector]
	public bool turnProcessed = false;
	[HideInInspector]
	public bool changeTurnOrder = false;
	[HideInInspector]
	public static GUIText GUITurnEnter;
	[HideInInspector]
	public static GUIText GUITurnCount;	
	[HideInInspector]
	public static int deploy = -2;
	//----------------------------
	//Get the formatted text of played time
	public string GameTime {
		get {
			System.TimeSpan ts = ((System.TimeSpan)(System.DateTime.Now - time));
			return ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds;
		}
	}	
	// Use this for initialization
	void Start ()
	{
		time = System.DateTime.Now;
		temp = System.DateTime.Now;
		GUITurnCount = (GUIText)GameObject.Find ("GUI_TurnCount").GetComponent<GUIText> ();
		GUITurnEnter = (GUIText)GameObject.Find ("GUI_TurnEnter").GetComponent<GUIText> ();
		GUITurnEnter.enabled = true;
		if (GameManager.gameType == GameType.RTS) {
			GUITurnCount.enabled = false;
			GUITurnEnter.enabled = false;
		}
		if (currentPlayer == null)
			throw new System.Exception ("You must have current player in \"Current Player\" attribute at GameManager class");
	}	
	//Aux the Update Method
	bool one = false;
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Debug.Log("esc");
			Application.Quit();
		}

		if (!one) {
			FixUnitPositions ();
			CurrentTurn++;
			TurnProcessed ();
			one = !one;
		}
		//If is Turn-Based
		if (GameManager.gameType == GameType.TBS) {
			if (((System.TimeSpan)(System.DateTime.Now - temp)).TotalMilliseconds > 5) {
				if (IsTurnOver ()) {
					GUITurnEnter.enabled = true;  // for some reason sometimes obj ref is not set error occurs.
					
					if (Input.GetKeyDown (KeyCode.Return)) {
						if (IsTurnOver ()) {
							turnProcessed = true;
							CurrentTurn++;
							TurnProcessed ();							
						}
					}
				} else
					GUITurnEnter.enabled = false;
				temp = System.DateTime.Now;	
			}
		}
	}
	//Checks if the turn is over
	private bool IsTurnOver ()
	{
		bool aux = true;
		foreach (Player item in Player.AllPlayers) {
			if (!item.IsTurnProcessed ()) {
				aux = false;
				break;
			}
		}
		return aux;
	}
	//Fix the units positions in an hexagon
	public void FixUnitPositions ()
	{
		Debug.Log("FixUnitPositions ()");

		Vector2 vNode, vUnit, vNodeUnit;
		float resAtual, resTest;
		foreach (Node node in MeshNodes.AllNodes) {
			node.gameObject.renderer.enabled = false;
			vNode = new Vector2 (node.transform.position.x, node.transform.position.z);
			UnitControl uc = null;
			foreach (UnitControl unit in UnitControl.AllUnits) {			
				if (unit.node == null) {
					Debug.Log("unit.node == null");
					unit.node = node;
					uc = unit;
				} else {
					Debug.Log("unit.node != null");

					vUnit = new Vector2 (unit.transform.position.x, unit.transform.position.z);
					vNodeUnit = new Vector2 (unit.node.transform.position.x, unit.node.transform.position.z);
					resAtual = (vUnit - vNodeUnit).magnitude;
					resTest = (vUnit - vNode).magnitude;
					if (resTest < resAtual) {
						unit.node = node;
						uc = unit;
					}
				}				
			}



			foreach (Node i in MeshNodes.AllNodes.Where(n => n.unitControl == uc))
				i.unitControl = null;

			if (uc != null)
			{
				node.unitControl = uc;
				//uc.transform.position = node.transform.position;
			}
		}
		foreach (UnitControl unit in UnitControl.AllUnits) {
			if (unit.player == null)
				throw new System.Exception ("One unit (" + unit.name + ") does not have a player set");						
			if (Player.AllPlayers == null || Player.AllPlayers.Count == 0)
				throw new System.Exception ("You need at last one player in the scene");
			if (unit.player == currentPlayer) {
				TileControl.EnterNodeExternal (unit.node);
				unit.renderer.enabled = true;
			} else
				unit.renderer.enabled = false;
			unit.transform.position = new Vector3 (unit.transform.position.x, 1.89f, unit.transform.position.z);
			//Rigidbody rb = (Rigidbody)unit.GetComponent<Rigidbody> ();
			//rb.isKinematic = true;
			Player player = null;
			switch (unit.player.color) {
			case Cor.BLUE:				
				player = Player.AllPlayers.FirstOrDefault (n => n.color == Cor.BLUE);				
				break;
			case Cor.GREEN:
				player = Player.AllPlayers.FirstOrDefault (n => n.color == Cor.GREEN);
				break;
			case Cor.RED:
				player = Player.AllPlayers.FirstOrDefault (n => n.color == Cor.RED);
				break;
			case Cor.WHITE:
				player = Player.AllPlayers.FirstOrDefault (n => n.color == Cor.WHITE);
				break;
			}
			if (player != null)
				player.units.Add (unit);
		}
	}
	//Called when the turn is over
	public void TurnProcessed ()
	{
		changeTurnOrder = true;
		turnProcessed = false;
		GUITurnCount.text = "Turn: " + CurrentTurn.ToString ();
		GUITurnEnter.enabled = false;
		//Processes the turn of all players
		foreach (Player item in Player.AllPlayers) {			
			item.TurnProcessed ();
		}
	}
	//Set some prefabs in the game
	public static void SetPrefabs (GameObject gameManager, GameObject guiTurnEnter, GameObject guiTurnCount)
	{
		GameObject gm1 = (GameObject)Instantiate (gameManager);
		gm1.name = "GameManager";
		GameObject gm2 = (GameObject)Instantiate (guiTurnEnter);
		gm2.name = "GUI_TurnEnter";
		GameObject gm3 = (GameObject)Instantiate (guiTurnCount);
		gm3.name = "GUI_TurnCount";
	}
	void OnGUI() {
		int w = 150;
		int h = 20;

		int x = 60;
		int y = 60;
		int y_delta = 5;

		const int max_tank = 6;
		string[] tanks = new string[max_tank]{"Future Tank", "Pz", "Cruiser", "Armoured Inf.", "New Armoured Inf.", "Flak Battery"};

		if(deploy == -2)
		{
			if (GUI.Button(new Rect(x, y  + y_delta, w, h), "Build"))
			{
				deploy = -1;
			}
		}
		else
		{
			for(int i = 0; i < max_tank; i++)
			{
				if (GUI.Button(new Rect(x, y + h*i + y_delta, w, h), tanks[i]))
				{
					deploy = i;

					Debug.Log(deploy);
				}		

			}
		}
	}

}
