using UnityEngine;
using System.Collections;

public class BladeBehaviour : MonoBehaviour {

    private Vector2[] waypoints;
    private Vector2 origin;
    private int currentWaypoint = 0;
    private float speed = 1;

	// Use this for initialization
	void Start () {
        waypoints = new Vector2[2];
        waypoints[0] = new Vector2(0, 1);
        waypoints[1] = new Vector2(0, -1);
        origin = new Vector2(transform.position.x, transform.position.y);
	}
	
	// Update is called once per frame
	void Update () {
        if (waypoints.Length > currentWaypoint)
        {
            float step = speed * Time.deltaTime;
            Vector2 tgt = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), origin+waypoints[currentWaypoint], step);
            transform.position = new Vector3(tgt.x, tgt.y, 0);
            if (new Vector2(transform.position.x, transform.position.y) == origin + waypoints[currentWaypoint])
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
        else
            currentWaypoint = 0;
	}


}
