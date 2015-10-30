using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

    private Rigidbody2D rb;
    private DistanceJoint2D dj;

    public GameObject hook = null;

    public bool madeMove = false;
    private bool jumpingUp = false;

    public Transform camera;
    private CameraBehaviour cameraBehaviour;
    private bool noTouch = true;

    public enum SwipeDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    //public static event Action<SwipeDirection> Swipe;
    private bool swiping = false;
    private bool eventSent = false;
    private Vector2 lastPosition;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        dj = GetComponent<DistanceJoint2D>();
        cameraBehaviour = camera.GetComponent<CameraBehaviour>();
        dj.enabled = false;

	}

    void Swipe(SwipeDirection d)
    {
        switch (d)
        {
            case SwipeDirection.Up:
                MoveUp();
                break;
            case SwipeDirection.Right:
                MoveRight();
                break;
            case SwipeDirection.Left:
                MoveLeft();
                break;
            case SwipeDirection.Down:
                MoveDown();
                break;
        }
    }

    void MoveUp()
    {
        rb.position = new Vector2(hook.transform.position.x, hook.transform.position.y - 0.4f);
        rb.velocity = new Vector2(0, 0);
        rb.drag = 0.5f;
        rb.AddForce(new Vector2(0, 14), ForceMode2D.Impulse);
        madeMove = true;
        cameraBehaviour.Started();
        jumpingUp = true;
        //DisconnectHook();
    }

    void MoveRight()
    {
        rb.position = new Vector2(hook.transform.position.x, hook.transform.position.y - 0.4f);
        rb.velocity = new Vector2(0, 0);
        rb.drag = 0.5f;
        //if (rb.velocity.x == 0)
            rb.AddForce(new Vector2(11, 0), ForceMode2D.Impulse);
       // else
       //     rb.AddForce(new Vector2(20, 0), ForceMode2D.Impulse);
                
        madeMove = true;
        cameraBehaviour.Started();
     }

    void MoveLeft()
    {
        rb.position = new Vector2(hook.transform.position.x, hook.transform.position.y - 0.4f);
        rb.velocity = new Vector2(0, 0);
        rb.drag = 0.5f;
       // if (rb.velocity.x == 0)
            rb.AddForce(new Vector2(-11, 0), ForceMode2D.Impulse);
        //else
       //     rb.AddForce(new Vector2(-20, 0), ForceMode2D.Impulse);
        madeMove = true;
        cameraBehaviour.Started();
    }

    void MoveDown()
    {
        rb.position = new Vector2(hook.transform.position.x, hook.transform.position.y - 0.4f);
        rb.velocity = new Vector2(0, 0);
        rb.drag = 0.5f;
        DisconnectHook();
        madeMove = true;
        cameraBehaviour.Started();
    }


	// Update is called once per frame
	void Update ()
    {

        if (rb.velocity.y <= 0)
            jumpingUp = false;

        if ((hook != null) && (!madeMove) && (rb.velocity.y > 0))
            rb.drag = 200;
        else
            rb.drag = 0.5f;

        if ((!madeMove) && (hook != null))
        {
            if (Input.GetKeyDown("up"))
                MoveUp();
            else if ((Input.GetKeyDown("right")))// && (rb.velocity.x >= 0))
                MoveRight();
            else if ((Input.GetKeyDown("left")))// && (rb.velocity.x <= 0))
                MoveLeft();
            if (Input.GetKeyDown("down"))
                MoveDown();
        }

        if (hook != null)
        {
            if ((rb.velocity.y > 0) && (transform.position.y > hook.transform.position.y - 0.25f) && (madeMove))
                DisconnectHook();
        }


        if (Input.touchCount == 0)
            noTouch = true;
        else if (Input.GetTouch(0).deltaPosition.sqrMagnitude != 0)
        {
            if (noTouch)
            {
                if (swiping == false)
                {
                    swiping = true;
                    lastPosition = Input.GetTouch(0).position;
                }
                else
                {
                    if (!eventSent)
                    {
                            Vector2 direction = Input.GetTouch(0).position - lastPosition;

                            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                            {
                                if (direction.x > 0)
                                    Swipe(SwipeDirection.Right);
                                else
                                    Swipe(SwipeDirection.Left);
                            }
                            else
                            {
                                if (direction.y > 0)
                                    Swipe(SwipeDirection.Up);
                                else
                                    Swipe(SwipeDirection.Down);
                            }

                            eventSent = true;
                            noTouch = false;
                            swiping = false;
                    }
                 }
             }
        }
        else
        {
            swiping = false;
            eventSent = false;
        }
	}

    public void SetHook(GameObject hook)
    {
        this.hook = hook;
        dj.connectedAnchor = hook.transform.position;
        dj.enabled = true;
    }

    public void DisconnectHook()
    {
        this.hook = null;
        dj.enabled = false;
        madeMove = false;
        rb.drag = 0.5f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Blade")
            Death();
        else if (!jumpingUp)
            if (other.gameObject.tag == "Hook")
                SetHook(other.gameObject);
            else if (other.gameObject.tag == "Pickup")
                GetCoin(other.gameObject);
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((!madeMove) && (!jumpingUp))
        {
            if (other.gameObject.tag == "Hook")
                SetHook(other.gameObject);
        }
        madeMove = false;
    }

    void GetCoin(GameObject coin)
    {
        GameObject.Destroy(coin);
        cameraBehaviour.AddScore(1);
    }

    void Death()
    {
        cameraBehaviour.Restart();
    }
}
