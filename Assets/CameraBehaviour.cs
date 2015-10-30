using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraBehaviour : MonoBehaviour {

    public Transform target;
    private float speed = 3f;
    private Vector2 offset;

    private float minx;

    public Transform hookPrefab;
    public Transform coinPrefab;
    public Transform wallPrefab;
    public Transform floorPrefab;
    public Transform bladePrefab;

    public Text scoreText;
    public Text positionText;
    public GameObject buttonPanel;

    private int score = 0;
    private int position = 0;

    private bool started = false;

    private Vector2 levelHead;

    LevelComponent lc;
    LevelComponent lc2;
    LevelComponent lc3;
    LevelComponent lc4;
    LevelComponent lc5;
    LevelComponent lc6;
    LevelComponent lc7;

	void Start ()
    {
        offset = new Vector2(2f, -0.5f);

        levelHead = new Vector2(0, 0);
        Transform a = Instantiate(hookPrefab);
        a.position = levelHead;
        GenerateLevel();

/*        for (int i = 0; i < 100; i++)
        {
            Transform hook = Instantiate(hookPrefab);
            hook.position = new Vector2(i * 1.5f, 0);
            if ((i % 8 == 0) && (i >0))
            {
                Transform coin = Instantiate(coinPrefab);
                coin.position = new Vector2(i * 1.5f, -0.4f);
            }
        }*/
	}

    private void GenerateLevel()
    {
        LevelAtom[] a = new LevelAtom[2];
        Vector2[] v = new Vector2[2];
        a[0] = LevelAtom.Hook;
        a[1] = LevelAtom.Hook;
        v[0] = new Vector2(1.5f, 0);
        v[1] = new Vector2(3, 0);
        lc = new LevelComponent(a,v,new Vector2(3,0));

        LevelAtom[] a2 = new LevelAtom[2];
        Vector2[] v2 = new Vector2[2];
        a2[0] = LevelAtom.Hook;
        a2[1] = LevelAtom.Hook;
        v2[0] = new Vector2(1.5f, 0);
        v2[1] = new Vector2(3, -1);
        lc2 = new LevelComponent(a2, v2, new Vector2(3, -1));

        LevelAtom[] a3 = new LevelAtom[3];
        Vector2[] v3 = new Vector2[3];
        a3[0] = LevelAtom.Hook;
        a3[1] = LevelAtom.Hook;
        a3[2] = LevelAtom.Wall;
        v3[0] = new Vector2(0, 1f);
        v3[1] = new Vector2(1.5f, 0);
        v3[2] = new Vector2(0.75f, 0);
        lc3 = new LevelComponent(a3, v3, new Vector2(1.5f, 0));

        LevelAtom[] a4 = new LevelAtom[2];
        Vector2[] v4 = new Vector2[2];
        a4[0] = LevelAtom.Floor;
        a4[1] = LevelAtom.Hook;
        v4[0] = new Vector2(1.5f, -1);
        v4[1] = new Vector2(3, 0);
        lc4 = new LevelComponent(a4, v4, new Vector2(3, 0));

        LevelAtom[] a5 = new LevelAtom[1];
        Vector2[] v5 = new Vector2[1];
        a5[0] = LevelAtom.Hook;
        v5[0] = new Vector2(0, -2);
        lc5 = new LevelComponent(a5, v5, new Vector2(0, -2));

        LevelAtom[] a6 = new LevelAtom[2];
        Vector2[] v6 = new Vector2[2];
        a6[0] = LevelAtom.Hook;
        a6[1] = LevelAtom.Hook;
        v6[0] = new Vector2(0, 1);
        v6[1] = new Vector2(0, 2);
        lc6 = new LevelComponent(a6, v6, new Vector2(0, 2));

        LevelAtom[] a7 = new LevelAtom[1];
        Vector2[] v7 = new Vector2[1];
        a7[0] = LevelAtom.Blade;
        v7[0] = new Vector2(0.75f, 0);
        lc7 = new LevelComponent(a7, v7, new Vector2(0, 0));

    }

    private void AddComponent(LevelComponent component)
    {
        for (int i = 0; i < component.atoms.Length; i++)
        {
            Transform a = Instantiate(GetTransform(component.atoms[i]));
            a.position = levelHead+component.positions[i];
        }
        levelHead += component.endPosition;
    }

    public Transform GetTransform(LevelAtom a)
    {
        switch (a)
        {
            case LevelAtom.Hook:
                return hookPrefab;
                break;
            case LevelAtom.Wall:
                return wallPrefab;
                break;
            case LevelAtom.Floor:
                return floorPrefab;
                break;
            case LevelAtom.Blade:
                return bladePrefab;
                break;
        }
        return null;
    }

    void Update()
    {
        float step = Mathf.Max(speed * Time.deltaTime * (target.position.x+offset.x-transform.position.x),3f*Time.deltaTime);
        Vector2 tgt = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(Mathf.Max(target.position.x + offset.x,minx),Mathf.Round(target.position.y)+offset.y), step);
        transform.position = new Vector3(tgt.x, tgt.y, -10);

        if (started)
            minx = transform.position.x + Time.deltaTime * 0.2f;

        if (target.position.x < minx-2.8)
            Restart();
        if (target.position.y < -100)
            Restart();

        if ((int)(target.position.x/1.5f) > position)
        {
            position = (int)(target.position.x/1.5f);
            positionText.text = position.ToString();
        }

        if (target.position.x + 5 > levelHead.x)
        {
            int r = Random.Range(0, 7);
            switch (r)
            {
                case 0:
                    AddComponent(lc);
                    break;
                case 1:
                    AddComponent(lc2);
                    break;
                case 2:
                    AddComponent(lc3);
                    break;
                case 3:
                    AddComponent(lc4);
                    break;
                case 4:
                    AddComponent(lc5);
                    break;
                case 5:
                    AddComponent(lc6);
                    break;
                case 6:
                    AddComponent(lc7);
                    break;
            }
        }
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

    public void Started()
    {
        started = true;
        buttonPanel.SetActive(false);
    }

}
