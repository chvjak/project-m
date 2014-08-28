/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Insert auxiliary methods here
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;

public class Util
{
	// generates new pos between nodes identified by input vectors. in our case y0=y1 and y2=y3, stage is specified by t from 0..1
	public static Vector2 CubicInterpolate (Vector2 y0, Vector2 y1, Vector2 y2, Vector2 y3, float t)
	{
		Vector2 a0, a1, a2, a3;
		float t2;
		t2 = t * t;
		a0 = y3 - y2 - y0 + y1; // = 0
		a1 = y0 - y1 - a0;      // = 0
		a2 = y2 - y0;
		a3 = y1;
		return(a0 * t * t2 + a1 * t2 + a2 * t + a3);
	}	
}
