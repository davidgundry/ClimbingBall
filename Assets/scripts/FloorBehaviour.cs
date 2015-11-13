using UnityEngine;
using System.Collections;

public class FloorBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        other.transform.position = transform.position + new Vector3(0, 0.2f, 0);
        Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        rb.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        rb.AddForce(new Vector2(8f, 10f), ForceMode2D.Impulse);
    }
}
