/*--------------------------------------------------------------*/
//Generic Strategy Framework
//Created by Rafael Batista
//Control the Fog of War
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TerrainFoW : MonoBehaviour
{
	//The exploration size to units
	public float ExplorationSize = 60.0f;
	//Show all hexagons at runtime
	public bool viewExagons = false;
	private List<TreeInstance> treeDefault = new List<TreeInstance> ();
	private List<TreePrototype> treePrototypes = new List<TreePrototype> ();
	private List<DetailPrototype> detailPrototype = new List<DetailPrototype> ();
	private Terrain terr;
	protected int hmWidth;
	protected int hmHeight;
	protected int alphaMapWidth;
	protected int alphaMapHeight;
	protected int numOfAlphaLayers;
	protected const float DEPTH_METER_CONVERT = 0.05f;
	protected const float TEXTURE_SIZE_MULTIPLIER = 1.25f;
	private float[,] heightMapBackup;
	private float[, ,] alphaMapBackup;
	private bool changedVar = false;
	float[, ,] alphasBkp = null;
	private List<int[,]> detailBkp = null;
	private List<int[,]> detailAux = null;
	//Restores the terrain
	void OnApplicationQuit ()
	{
		if (Debug.isDebugBuild) {
			if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
				Terrain.activeTerrain.terrainData.treeInstances = treeDefault.ToArray ();
				Terrain.activeTerrain.terrainData.treePrototypes = treePrototypes.ToArray ();			
				int nLayers = Terrain.activeTerrain.terrainData.detailPrototypes.Length;			
				for (int i = 0; i < nLayers; i++) {
					Terrain.activeTerrain.terrainData.SetDetailLayer (0, 0, i, detailBkp [i]);
				}
			}
			if (terr != null && terr.terrainData != null) {
				terr.terrainData.SetHeights (0, 0, heightMapBackup);
				terr.terrainData.SetAlphamaps (0, 0, alphaMapBackup);
			}
		}
	}
	//Hide the trees
	void Start ()
	{	
		detailBkp = new List<int[,]> ();
		detailAux = new List<int[,]> ();
		//TD------------------------------
		terr = this.GetComponent<Terrain> ();
		if (terr != null && terr.terrainData != null) {
			hmWidth = terr.terrainData.heightmapWidth;
			hmHeight = terr.terrainData.heightmapHeight;
			alphaMapWidth = terr.terrainData.alphamapWidth;
			alphaMapHeight = terr.terrainData.alphamapHeight;
			numOfAlphaLayers = terr.terrainData.alphamapLayers;
			
			if (Debug.isDebugBuild) {
				heightMapBackup = terr.terrainData.GetHeights (0, 0, hmWidth, hmHeight);
				alphaMapBackup = terr.terrainData.GetAlphamaps (0, 0, alphaMapWidth, alphaMapHeight);   
			} 
			BlackTerrain ();			
		}
		//--------------------------------		
		if (Debug.isDebugBuild) {
			if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
				treeDefault = Terrain.activeTerrain.terrainData.treeInstances.ToList ();
				treePrototypes = Terrain.activeTerrain.terrainData.treePrototypes.ToList ();
				detailPrototype = Terrain.activeTerrain.terrainData.detailPrototypes.ToList ();
				int nLayers = Terrain.activeTerrain.terrainData.detailPrototypes.Length;			
				for (int i = 0; i < nLayers; i++) {
					detailBkp.Add (Terrain.activeTerrain.terrainData.GetDetailLayer (0, 0, terr.terrainData.detailWidth, terr.terrainData.detailHeight, i));
				}	
			}
		}
		if (!Application.isPlaying) {
		} else {
			if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
				int nLayers = Terrain.activeTerrain.terrainData.detailPrototypes.Length;
				int[,] hds = new int[terr.terrainData.detailWidth, terr.terrainData.detailHeight];
				for (int i = 0; i < nLayers; i++) {			
					detailAux.Add (terr.terrainData.GetDetailLayer (0, 0, terr.terrainData.detailWidth, terr.terrainData.detailHeight, i));
				}				
				for (int y = 0; y < terr.terrainData.detailWidth; y++)
					for (int x = 0; x < terr.terrainData.detailHeight; x++) {
						hds [x, y] = 0;
					}			
				for (int i = 0; i < nLayers; i++) {
					Terrain.activeTerrain.terrainData.SetDetailLayer (0, 0, i, hds);
				}
				List<TreePrototype> trees = new List<TreePrototype> (Terrain.activeTerrain.terrainData.treePrototypes);
				List<TreePrototype> prototypes = new List<TreePrototype> ();		
				TreePrototype tp = new TreePrototype ();
				tp.prefab = (GameObject)Resources.Load ("EmptyTree");
				prototypes.Add (tp);	
				foreach (TreePrototype item in trees) {			
					prototypes.Add (item);			
				}				
				Terrain.activeTerrain.terrainData.treePrototypes = prototypes.ToArray ();
				List<TreeInstance> treesIns = new List<TreeInstance> (Terrain.activeTerrain.terrainData.treeInstances);
				MeshNodes mn = (MeshNodes)GameObject.Find ("MeshNodes").GetComponent<MeshNodes> ();		
				try {			
					for (int i = 0; i < treesIns.Count; i++) {
						TreeInstance ti = treesIns [i];				
						TreeInstance currentTree = ti;
						Vector3 currentTreeWorldPosition = Vector3.Scale (currentTree.position, Terrain.activeTerrain.terrainData.size) + Terrain.activeTerrain.transform.position;
						float fMenor = 10000000;
						Node menor = null;
						foreach (Node item in mn.GetNodes()) {
				
							if (menor == null) {
								menor = item;
							} else {
								float aux = Vector3.Distance (currentTreeWorldPosition, item.transform.position);
								if (aux < fMenor) {
									fMenor = aux;
									menor = item;	
								}
							}				
						}
						TreeAux ta = new TreeAux ();
						ta.Index = i;
						ta.Prototype = ti.prototypeIndex + 1;				
						menor.trees.Add (ta);
					}
				} catch {
					//safe
				}
				try {
					List<TreeInstance> tis = new List<TreeInstance> ();
					for (int i = 0; i < Terrain.activeTerrain.terrainData.treeInstances.Length; i++) {
						TreeInstance tiii = Terrain.activeTerrain.terrainData.treeInstances [i];
						tiii.prototypeIndex = 0;
						tis.Add (tiii);
					}
					Terrain.activeTerrain.terrainData.treeInstances = tis.ToArray ();
				} catch {	
					//safe
				}
			}
		}
	}
	
	void OnMouseEnter ()
	{
		TileControl.NoWay ();
	}
	
	//Darkens the terrain
	protected void BlackTerrain ()
	{
		float[, ,] alphas = terr.terrainData.GetAlphamaps (0, 0, terr.terrainData.alphamapResolution, terr.terrainData.heightmapResolution - 1);
		alphasBkp = new float[terr.terrainData.alphamapResolution, terr.terrainData.heightmapResolution, numOfAlphaLayers];		
		for (int i = 0; i < terr.terrainData.alphamapResolution; i++) {
			for (int j = 0; j < terr.terrainData.heightmapResolution - 1; j++) {
				for (int layerCount = 0; layerCount < numOfAlphaLayers; layerCount++) {
					try {			
						alphasBkp [i, j, layerCount] = alphas [i, j, layerCount];
						alphas [i, j, layerCount] = -0.1f;
					} catch {
						//safe
					}	
				}
			}			
		}
		terr.terrainData.SetAlphamaps (0, 0, alphas);
	}
	//Lightens the terrain
	public void WhitenTerrain (Vector3 pos, float whiteArea)
	{
		Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPos (pos, terr, alphaMapWidth, alphaMapHeight);
		int alphaMapCraterWidth = (int)(whiteArea * (alphaMapWidth / terr.terrainData.size.x));
		int alphaMapCraterLength = (int)(whiteArea * (alphaMapHeight / terr.terrainData.size.z));
		int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - (alphaMapCraterWidth / 2));
		int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - (alphaMapCraterLength / 2));
		try {			
			float[, ,] alphas = terr.terrainData.GetAlphamaps (alphaMapStartPosX, alphaMapStartPosZ, alphaMapCraterWidth, alphaMapCraterLength);
			for (int i = 0; i < alphaMapCraterLength; i++) {
				for (int j = 0; j < alphaMapCraterWidth; j++) {
					for (int layerCount = 0; layerCount < numOfAlphaLayers; layerCount++) {
						alphas [i, j, layerCount] = alphasBkp [alphaMapStartPosZ + i, alphaMapStartPosX + j, layerCount];
					}
				}
			}
			terr.terrainData.SetAlphamaps (alphaMapStartPosX, alphaMapStartPosZ, alphas);
		} catch {
			//safe
		}
		int nLayers = Terrain.activeTerrain.terrainData.detailPrototypes.Length;
		for (int i = 0; i < nLayers; i++) {		
			int[,] hds = new int[terr.terrainData.detailWidth, terr.terrainData.detailHeight];
			for (int y = 0; y < alphaMapCraterLength; y++)
				for (int x = 0; x < alphaMapCraterWidth; x++) {
					hds [x, y] = detailAux [i] [alphaMapStartPosZ + x, alphaMapStartPosX + y];
				}		
			Terrain.activeTerrain.terrainData.SetDetailLayer (alphaMapStartPosX, alphaMapStartPosZ, i, hds);			
		}
	}

	protected Vector3 GetNormalizedPositionRelativeToTerrain (Vector3 pos, Terrain terrain)
	{
		Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
		Vector3 coord;
		coord.x = tempCoord.x / terr.terrainData.size.x;
		coord.y = tempCoord.y / terr.terrainData.size.y;
		coord.z = tempCoord.z / terr.terrainData.size.z;
		return coord;
	}

	protected Vector3 GetRelativeTerrainPositionFromPos (Vector3 pos, Terrain terrain, int mapWidth, int mapHeight)
	{
		Vector3 coord = GetNormalizedPositionRelativeToTerrain (pos, terrain);
		return new Vector3 ((coord.x * mapWidth), 0, (coord.z * mapHeight));
	}
	
	void Update ()
	{
		if (!changedVar) {
			if (viewExagons) {
				foreach (Node item in MeshNodes.AllNodes) {
					item.renderer.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
					item.renderer.material.mainTextureScale = new Vector2 (1, 1);
					item.renderer.material.shader = Shader.Find ("Transparent/Diffuse");
					item.renderer.enabled = true;
				}	
				changedVar = true;
			}
		} else {
			if (!viewExagons) {
				if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
					foreach (Node item in MeshNodes.AllNodes) {
						if (!viewExagons) {
							item.renderer.material.color = new Color (1, 1, 1, 0);
							item.renderer.material.mainTextureScale = new Vector2 (1, 1);
							item.renderer.material.shader = Shader.Find ("Transparent/Diffuse");
							item.renderer.enabled = false;
						}					
						if (Terrain.activeTerrain.SampleHeight (item.transform.position) > 0) {
							item.nodeType = ENodeType.MOUNTAIN;
						}					
					}
				}
				changedVar = false;
			}
		}
	}
}
