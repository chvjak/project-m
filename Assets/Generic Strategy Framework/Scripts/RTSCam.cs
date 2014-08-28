/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Control a RTS Camera
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;

public class RTSCam : MonoBehaviour
{	
	enum DirHor
	{
		NULL,
		LEFT,
		RIGHT
	}
	
	enum DirVert
	{
		NULL,
		UP,
		DOWN
	}
	
	public float XZ_speed = 10.0f;
	public float Y_speed = 20.0f;
	
	void Update ()
	{		
		float pW_R = (90 * Screen.width) / 100;
		float pW_L = (10 * Screen.width) / 100;
		float pH_D = (90 * Screen.height) / 100;
		float pH_U = (10 * Screen.height) / 100;
		
		if (Input.mousePosition.x > pW_R && Input.mousePosition.x < Screen.width) {
			transform.position = new Vector3 (transform.position.x + (0.1f * XZ_speed), transform.position.y, transform.position.z);
		}
		if (Input.mousePosition.x < pW_L && Input.mousePosition.x > 0) {
			transform.position = new Vector3 (transform.position.x - (0.1f * XZ_speed), transform.position.y, transform.position.z);
		}
		if (Input.mousePosition.y > pH_D && Input.mousePosition.y < Screen.height) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + (0.1f * XZ_speed));
		}
		if (Input.mousePosition.y < pH_U && Input.mousePosition.y > 0) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - (0.1f * XZ_speed));
		}		
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			transform.transform.position = new Vector3 (transform.position.x, transform.position.y + (0.5f * Y_speed), transform.position.z);
		}
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			if (transform.position.y - (0.5f * Y_speed) > 1.0f)
				transform.transform.position = new Vector3 (transform.position.x, transform.position.y - (0.5f * Y_speed), transform.position.z);
			else
				transform.position = new Vector3 (transform.position.x, 20.0f, transform.position.z);
		}
	}
}
