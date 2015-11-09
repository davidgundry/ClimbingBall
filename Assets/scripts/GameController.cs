﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

    public Transform player;
    public Transform camera;

    public Transform hookPrefab;
    public Transform coinPrefab;
    public Transform wallPrefab;
    public Transform floorPrefab;
    public Transform bladePrefab;
    public Transform groundPrefab;

    public Material groundMaterial;

    public Text scoreText;
    public Text positionText;
    public GameObject buttonPanel;

    private int score = 0;
    private int position = 0;

    private int levelChunk = 0;
    private float cameraWidth;

    private bool started = false;
    public bool Started
    {
        get
        {
            return started;
        }
    }

    private Vector2 levelHead;

	void Start ()
    {
        cameraWidth = camera.GetComponent<Camera>().orthographicSize * camera.GetComponent<Camera>().aspect * 2;
	}
	
	void Update ()
    {
	    if ((int)(player.position.x/1.5f) > position)
        {
            position = (int)(player.position.x/1.5f);
            positionText.text = position.ToString();
        }

        if (player.position.x + cameraWidth > levelHead.x * 1.5f)
            ExtendLevel();
	}

    private void ExtendLevel()
    {
        LevelComponent[,] levelGrid = LevelGenerator.Generate();
        InstantiateLevelGrid(levelGrid, levelHead, levelChunk);
        levelHead += LevelGenerator.LevelHead();
        levelChunk++;

        if (levelChunk >= 3)
            DestroyLevelChunk(levelChunk - 3);
    }

    private void DestroyLevelChunk(int chunkID)
    {
        GameObject chunk = GameObject.Find("LevelChunk" + chunkID);
        Destroy(chunk);
    }

    private void InstantiateLevelGrid(LevelComponent[,] levelGrid, Vector2 levelHead, int chunkID)
    {
        GameObject chunk = new GameObject();
        chunk.name = "LevelChunk"+chunkID;

        for (int i = 0; i < levelGrid.GetLength(0); i++)
        {
            for (int j = 0; j < levelGrid.GetLength(1); j++)
                if (levelGrid[i, j] != null)
                {
                    Transform t = Instantiate(GetTransform(levelGrid[i, j].Atom));
                    t.position += new Vector3((i + levelHead.x - 1) * 1.5f, j + levelHead.y, 0);
                    t.parent = chunk.transform;
                }
        }

        GameObject bg = AddBackgroundGround(GetHighestArray(levelGrid, LevelAtom.Ground));
        bg.transform.parent = chunk.transform;
    }

    private int[] GetHighestArray(LevelComponent[,] levelGrid, LevelAtom atom)
    {
        int[] highestGrounds = new int[levelGrid.GetLength(0)];

        for (int i = 0; i < levelGrid.GetLength(0); i++)
            highestGrounds[i] = LevelGenerator.GetHighestAtom(i, atom, levelGrid);

        return highestGrounds;
    }

    private GameObject AddBackgroundGround(int[] highestGrounds)
    {
        GameObject bg = new GameObject();
        bg.name = "BackgroundMesh";
        bg.AddComponent<MeshFilter>();
        bg.AddComponent<MeshRenderer>();
        MeshFilter mf = bg.GetComponent<MeshFilter>();

        Vector3[] verts = new Vector3[highestGrounds.Length * 6];

        for (int i = 0; i < highestGrounds.Length; i++)
            if (highestGrounds[i] > -1)
            {
                Vector3[] newVerts = RectMeshVerts((i + levelHead.x - 1) * 1.5f - 0.75f, highestGrounds[i] + levelHead.y-1 + 0.5f, 1.5f, -10000000); ;
                for (int j = 0; j < 6; j++)
                    verts[i*6+j] = newVerts[j];
            }

        mf.mesh.vertices = verts;

        int[] newTriangles = new int[mf.mesh.vertices.Length];
        for (int k = 0; k < mf.mesh.vertices.Length; k++)
            newTriangles[k] = k;

        mf.mesh.triangles = newTriangles;
        mf.mesh.RecalculateNormals();
        bg.GetComponent<Renderer>().material = groundMaterial;

        return bg;
    }

    private Vector3[] RectMeshVerts(float x, float y, float w, float h)
    {
         Vector3[] verts = new Vector3[6];

        verts[0] = new Vector3(x, y, 0);
        verts[1] = new Vector3(x + w, y, 0);
        verts[2] = new Vector3(x, y + h, 0);
        verts[3] = new Vector3(x, y + h, 0);
        verts[4] = new Vector3(x + w, y, 0);
        verts[5] = new Vector3(x + w, y+h, 0);

        return verts;
    }

    public Transform GetTransform(LevelAtom a)
    {
        switch (a)
        {
            case LevelAtom.Hook:
                return hookPrefab;
            case LevelAtom.Wall:
                return wallPrefab;
            case LevelAtom.Floor:
                return floorPrefab;
            case LevelAtom.Blade:
                return bladePrefab;
            case LevelAtom.Ground:
                return groundPrefab;
        }
        return null;
    }

    public void Restart()
    {
        Application.LoadLevel(0);
    }

    public void AddScore(int add)
    {
        score += add;
        scoreText.text = score.ToString();
    }

    public void StartGame()
    {
        started = true;
        buttonPanel.SetActive(false);
    }
}