
/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Create the MeshNodes
/*--------------------------------------------------------------*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class MeshNodesCreate: ScriptableObject
{
	private static string pathHexagon = "Assets/Generic Strategy Framework/Prefabs/Hexagon.prefab";
	public static string pathArrow = "Assets/Generic Strategy Framework/Prefabs/Arrow.prefab";
	public static string pathGameManager = "Assets/Generic Strategy Framework/Prefabs/GameManager.prefab";
	public static string pathGUITurnEnter = "Assets/Generic Strategy Framework/Prefabs/GUI/GUI_TurnEnter.prefab";
	public static string pathGUITurnCount = "Assets/Generic Strategy Framework/Prefabs/GUI/GUI_TurnCount.prefab";
	public GameObject hexagon;
	public GameObject arrow;
	public GameObject gameManager;
	public GameObject guiTurnEnter;
	public GameObject guiTurnCount;
	private float hw;
	private float hh;
	public MeshNodes meshNodes = null;
	
	public void GenerateNodes (int xSize, int ySize)
	{
		hexagon = (GameObject)AssetDatabase.LoadAssetAtPath (pathHexagon, typeof(GameObject));
		arrow = (GameObject)AssetDatabase.LoadAssetAtPath (pathArrow, typeof(GameObject));
		gameManager = (GameObject)AssetDatabase.LoadAssetAtPath (pathGameManager, typeof(GameObject));
		guiTurnEnter = (GameObject)AssetDatabase.LoadAssetAtPath (pathGUITurnEnter, typeof(GameObject));
		guiTurnCount = (GameObject)AssetDatabase.LoadAssetAtPath (pathGUITurnCount, typeof(GameObject));
		meshNodes = new MeshNodes ();
		meshNodes.Hexagon = hexagon;
		meshNodes.Arrow = arrow;
		MeshNodes mn = new MeshNodes ();
		mn.GenerateNodes (xSize, ySize, hexagon, arrow);
		GameManager.SetPrefabs(gameManager, guiTurnEnter, guiTurnCount);
	}
}