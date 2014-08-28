using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Cor
{
	BLUE,
	GREEN,
	RED,
	WHITE
}

public class Player : MonoBehaviour
{
	//Static-------------------
	//All players of game
	public static List<Player> allPlayers;

	public static List<Player> AllPlayers {
		get {
			if (allPlayers != null)
				return allPlayers;
			else {
				allPlayers = new List<Player> ();
				foreach (Player player in GameObject.FindSceneObjectsOfType(typeof(Player))) {
					allPlayers.Add (player);
				}
				return allPlayers;
			}
		}
	}
	//--------------------------	
	//The Player name
	public string name;
	//The Player color
	public Cor color;
	//The Player ip
	public string ip;
	//The Player units
	[HideInInspector]
	public List<UnitControl> units;
	//Public Internal-------------
	[HideInInspector]
	public bool turnProcessed = false;
	//----------------------------
	//Time Control
	private System.DateTime temp;	
	
	// Use this for initialization
	void Start ()
	{
		temp = System.DateTime.Now;
	}	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (GameManager.gameType == GameType.TBS) {
			if (((System.TimeSpan)(System.DateTime.Now - temp)).TotalMilliseconds > 5) {
				turnProcessed = IsTurnOver ();
				if(turnProcessed)
				{
					//do something
				}
				temp = System.DateTime.Now;
			}
		}
	}
	//Checks if the turn is over
	private bool IsTurnOver ()
	{
		bool aux = true;
		foreach (UnitControl unit in units) {
			if (!unit.IsTurnProcessed ()) {
				aux = false;
				break;
			}
		}
		return aux;
	}
	
	public bool IsTurnProcessed ()
	{
		return IsTurnOver ();
	}
	
	//Called when the turn is over
	public void TurnProcessed ()
	{
		turnProcessed = false;
		foreach (UnitControl unit in units) {
			unit.TurnProcessed ();
		}
	}
}
