/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//The base class for all units of game
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;

public abstract class Unit : UnitControl {
	//The body value of this unit
	public float body = 100.0f;
	//The unit model
	public string unitModel;	
	// Use this for initialization
	public abstract void Start ();	
	// Update is called once per frame
	public abstract void Update ();
	//Use this to modify the body value
	public abstract void ApplyDamage(float value);
	//Create what you want....
	//...
	//...
}
