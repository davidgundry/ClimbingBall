using UnityEngine;
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
    public Transform bgPrefab;
    public Transform groundPrefab;

    public Text scoreText;
    public Text positionText;
    public GameObject buttonPanel;

    private int score = 0;
    private int position = 0;

    private bool started = false;
    public bool Started
    {
        get
        {
            return started;
        }
    }

    private Vector2 levelHead;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	    if ((int)(player.position.x/1.5f) > position)
        {
            position = (int)(player.position.x/1.5f);
            positionText.text = position.ToString();
        }

        if (player.position.x + 5 > levelHead.x)
        {
            LevelComponent[,] levelGrid = LevelGenerator.Generate();
            InstantiateLevelGrid(levelGrid, levelHead);
            levelHead += LevelGenerator.LevelHead();
        }
	}

    private void InstantiateLevelGrid(LevelComponent[,] levelGrid, Vector2 levelHead)
    {
        for (int i = 0; i < levelGrid.GetLength(0); i++)
        {
            int highest = -1;
            for (int j = 0; j < levelGrid.GetLength(1); j++)
                if (levelGrid[i, j] != null)
                {
                    if (levelGrid[i, j].Atom == LevelAtom.Ground)
                        highest = j;
                    Transform t = Instantiate(GetTransform(levelGrid[i, j].Atom));
                    t.position += new Vector3((i + levelHead.x - 1) * 1.5f, j + levelHead.y, 0);
                }

            if (highest > -1)
            {

                Transform bg = Instantiate(bgPrefab);
                MeshFilter mf = bg.GetComponent<MeshFilter>();

                Vector3[] verts = new Vector3[6];

                float x = (i + levelHead.x - 1) * 1.5f - 0.75f;
                float y = highest + levelHead.y + 0.5f;

                float minusLargeNumer = -1000000;

                verts[0] = new Vector3(x, y, 0);
                verts[1] = new Vector3(x + 1.5f, y, 0);
                verts[2] = new Vector3(x, minusLargeNumer, 0);
                verts[3] = new Vector3(x, minusLargeNumer);
                verts[4] = new Vector3(x + 1.5f, y, 0);
                verts[5] = new Vector3(x + 1.5f, minusLargeNumer, 0);

                mf.mesh.vertices = verts;

                int[] newTriangles = new int[mf.mesh.vertices.Length];
                for (int k = 0; k < mf.mesh.vertices.Length; k++)
                    newTriangles[k] = k;// ((i / 3) + 1) * 3 - 1 - i % 3;

                mf.mesh.triangles = newTriangles;
                mf.mesh.RecalculateNormals();
            }
        }
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
