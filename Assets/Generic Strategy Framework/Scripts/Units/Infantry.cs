/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Control the infantry units
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;

public class Infantry : Unit {
	public AudioClip shotSound;
	public void Fire() {

		audio.PlayOneShot(shotSound, 2.7F);
	}

	#region implemented abstract members of Unit
	public override void Start ()
	{
		//Implement It!
	}

	public override void Update ()
	{
		//Implement It!
	}

	public override void ApplyDamage (float value)
	{
		//Implement It!
	}

	 void  OnMouseOver1 () {
		if (Input.GetMouseButton(0)){
			//attacker
			Debug.Log("Pressed left click.");
		}
		
		if (Input.GetMouseButton(1))
		{
			//target
			Debug.Log("Pressed right click.");
		}
		

	
	}



	#endregion
	
}
