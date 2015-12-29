using UnityEngine;
using System.Collections;

public class PickupBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //other.gameObject.GetComponent<PlayerBehaviour>().GetCoin(gameObject);
        Debug.Log("trigger");
    }
}
