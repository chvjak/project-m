/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Init the Setup Window
/*--------------------------------------------------------------*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Xml;

public class SConfig: EditorWindow
{
	static void  Init ()
	{
		SConfig window = (SConfig)EditorWindow.GetWindow (typeof(SConfig));
		window.Show ();
	}
}