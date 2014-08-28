using UnityEngine;
using System.Collections;

public class animated : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Animator a = GetComponent<Animator>();
		a.Play("idle");
	}
	
	// Update is called once per frame
	void Update () {
		Animator a = GetComponent<Animator>();

		if (Input.GetKey("up"))
			a.Play("driving");
		if (Input.GetKey("space"))
			a.Play("firing");
		//if (Input.GetKey("d"))
			//a.Play("destroyed");

	}
}
