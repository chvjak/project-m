using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {
	public GameObject debrisPrefab; // drag the debris prefab here
	public GameObject explosionPrefab; 
	public GameObject smokePrefab; 


	public float force = 50;
	public float radius = 15; // explosion force decreases to zero at this distance


	public void Set()
	{
		Debug.Log("force="+force);

		Instantiate(explosionPrefab, transform.position, transform.rotation);

		// create replacement pieces:
		GameObject debrisPrefabInstance = Instantiate(debrisPrefab, transform.position, transform.rotation) as GameObject;

		Vector3 pos = transform.position; // get hit point
		// get a list with all rigidbodies in the broken brick object:
		Component[] debris = debrisPrefabInstance.GetComponentsInChildren<Rigidbody>();

		// add explosion force to them according to their positions:
		foreach (Rigidbody rb in debris){ 
			GameObject smokeInstance = Instantiate(smokePrefab, rb.position, rb.rotation) as GameObject;
			smokeInstance.transform.parent = rb.transform;
			rb.AddExplosionForce(force, pos, radius);
		}
		Destroy(gameObject); // destroy original brick
	}    
}