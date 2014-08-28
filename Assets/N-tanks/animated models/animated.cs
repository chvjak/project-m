using UnityEngine;
using System.Collections;

public class animated : MonoBehaviour {
	

	// Update is called once per frame
	void Update () {
		Animator a = GetComponent<Animator>();

		if (Input.GetKey("up")){
			Debug.Log("driving");
			a.Play("driving");
		}
		if (Input.GetKey("space"))
		{
			Debug.Log("firing");
			a.Play("firing");
		}
		if (Input.GetKey("d"))
			a.Play("destroyed");

	}
}
