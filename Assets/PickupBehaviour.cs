using UnityEngine;
using System.Collections;

public class PickupBehaviour : MonoBehaviour {

    private float idleTimer;
    private const float idleTimeout = 0.2f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            if (rb.velocity.magnitude < 0.01f)
                rb.AddForce(new Vector2(3, 0), ForceMode2D.Impulse);
            idleTimer = idleTimeout;
        }
    }

    public void SetHook(Transform hook)
    {
        GetComponent<DistanceJoint2D>().connectedAnchor = new Vector2(hook.transform.position.x, hook.transform.position.y);
    }
}
