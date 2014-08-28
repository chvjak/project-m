/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Create the Setup Window
/*--------------------------------------------------------------*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Xml;

public class MeshNodeConfig: EditorWindow
{
	[MenuItem ("GSF/Create MeshNodes")]	
	static void  Init ()
	{
		MeshNodeConfig window = (MeshNodeConfig)EditorWindow.GetWindow (typeof(MeshNodeConfig));
		window.title = "Config";
		window.maxSize = new Vector2 (240, 50);
		window.minSize = new Vector2 (240, 50);
		window.Show ();
	}
	
	private string valueX = "20", valueY = "20";
	private StringBuilder meshX = null, meshY = null;
	
	public void OnGUI ()
	{        
		GUIStyle generic_style = new GUIStyle ();		
		GUILayout.BeginVertical ("");	
		
		GUILayout.BeginHorizontal ("", generic_style);		
		GUILayout.Label ("X nodes", GUILayout.Width (50));		
		meshX = new StringBuilder ();
		foreach (char c in valueX) {
			if (char.IsDigit (c)) {
				meshX.Append (c);
			}
		}
		valueX = UnityEngine.GUILayout.TextField (meshX.ToString (), 4).ToString ();
		
		GUILayout.Label ("Y nodes", GUILayout.Width (50));		
		GUILayout.Space (10);
		meshY = new StringBuilder ();
		foreach (char c in valueY) {
			if (char.IsDigit (c)) {
				meshY.Append (c);
			}
		}
		valueY = UnityEngine.GUILayout.TextField (meshY.ToString (), 4);		
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal ("", generic_style);		 
		if (GUILayout.Button ("Create", GUILayout.Width (80))) {
			MeshNodesCreate mn = new MeshNodesCreate ();
			mn.GenerateNodes (Convert.ToInt32 (valueX), Convert.ToInt32 (valueY));
		}
		GUILayout.EndHorizontal ();
		
		GUILayout.EndVertical ();
	}
}