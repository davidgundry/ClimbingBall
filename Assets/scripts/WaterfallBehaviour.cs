using UnityEngine;
using System.Collections;

public class WaterfallBehaviour : MonoBehaviour {

    public bool waterActive;
    private MeshFilter waterMeshFilter;

    public float activeTime;
    public float delayTime;
    private float timer;

	// Use this for initialization
	void Start () {
        waterMeshFilter = GetComponent<MeshFilter>();

        waterMeshFilter.mesh.vertices = GameController.RectMeshVerts(-0.75f, 0.5f, 1, -0.2f);

        int[] newTriangles = new int[waterMeshFilter.mesh.vertices.Length];
        for (int k = 0; k < waterMeshFilter.mesh.vertices.Length; k++)
            newTriangles[k] = k;
        waterMeshFilter.mesh.triangles = newTriangles;
        waterMeshFilter.mesh.RecalculateNormals();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if ((timer <= 0) && (!waterActive))
            StartWaterfall();
        else if (timer <= 0)
            StopWaterfall();
        else
            timer -= Time.deltaTime;
	}

    void StartWaterfall()
    {
        waterMeshFilter.mesh.vertices = GameController.RectMeshVerts(-0.5f, 0.5f, 1, -10000000); ; ;
        timer = activeTime;
        waterActive = true;
    }

    void StopWaterfall()
    {
        waterMeshFilter.mesh.vertices = GameController.RectMeshVerts(-0.5f, 0.5f, 1, -0.2f);
        timer = delayTime;
        waterActive = false;
    }
}
