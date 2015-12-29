using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraBehaviour : MonoBehaviour {

    public Transform target;
    private float speed = 3f;
    private float minSpeed;
    private Vector2 offset;
    private float minx;
    private float halfCameraWidth;

    public Transform gameControllerTransform;
    private GameController gameController;


	void Start ()
    {
        gameController = gameControllerTransform.GetComponent<GameController>();

        Camera camera = GetComponent<Camera>();
        halfCameraWidth = camera.orthographicSize * camera.aspect;
        offset = new Vector2(halfCameraWidth-2.2f, -0.5f);
	}

    
    void Update()
    {
        float step = Mathf.Max(speed * Time.deltaTime * (target.position.x+offset.x-transform.position.x),3f*Time.deltaTime);
        Vector2 tgt = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(Mathf.Max(target.position.x + offset.x,minx),Mathf.Round(target.position.y)+offset.y), step);
        transform.position = new Vector3(tgt.x, tgt.y, -10);

        if (gameController.Started)
            minx = transform.position.x + Time.deltaTime * 0.2f;

        if (target.position.x < minx - halfCameraWidth)
            gameController.Restart();
        //if (target.position.y < -10)
        //    gameController.Restart();
	}

    

}
