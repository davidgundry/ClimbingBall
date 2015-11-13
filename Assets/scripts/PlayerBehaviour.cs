using UnityEngine;
using System.Collections;

using Seconds = System.Single;

public class PlayerBehaviour : MonoBehaviour {

    private Rigidbody2D rb;
    private DistanceJoint2D dj;

    public GameObject platform = null;

    public Transform gameControllerTransform;
    private GameController gameController;
    private CameraBehaviour cameraBehaviour;
    private bool noTouch = true;

    private Seconds idleTimer;
    private Seconds idleTimeout = 2f;
    private bool idling = false;

    public enum SwipeDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    public enum PlayerState
    {
        InAir,
        OnHook,
        OnGround,
        Jumping
    }

    public PlayerState playerState = PlayerState.OnHook;
    private bool jumpingUp = false;

    //public static event Action<SwipeDirection> Swipe;
    private bool swiping = false;
    private bool eventSent = false;
    private Vector2 lastPosition;
    private float? groundTargetX = null;
    private Collider2D groundCollider;


	// Use this for initialization
	void Start () {
        idleTimer = idleTimeout;
        gameController = gameControllerTransform.GetComponent<GameController>();
        rb = GetComponent<Rigidbody2D>();
        dj = GetComponent<DistanceJoint2D>();
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


	void Update ()
    {

        if (!idling)
        {
            if ((playerState == PlayerState.OnHook) && (rb.velocity.y > 0))
                rb.drag = 200;
            else
                rb.drag = 0.5f;
        }

        if (rb.velocity.y <= 0)
            jumpingUp = false;

        switch (playerState)
        {
            case PlayerState.OnHook:
                ManageKeyInput();
                ManageTouchInput();
                break;
            case PlayerState.Jumping:
                if ((rb.velocity.y > 0) && (transform.position.y > platform.transform.position.y - 0.25f))
                    DisconnectHook();
                break;
            case PlayerState.OnGround:
                if (transform.position.x < groundTargetX)
                {
                    if (rb.velocity.x <= 0)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        transform.position = new Vector3(Mathf.Min(transform.position.x + 1 * Time.deltaTime, groundTargetX ?? 0), transform.position.y, transform.position.z);
                    }
                }
                else if (transform.position.x > groundTargetX)
                {
                    if (rb.velocity.x >= 0)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        transform.position = new Vector3(Mathf.Max(transform.position.x - 1 * Time.deltaTime, groundTargetX ?? 0), transform.position.y, transform.position.z);
                    }
                }

                ManageKeyInput();
                ManageTouchInput();
                break;

        }

        if (playerState == PlayerState.OnHook)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                if (rb.velocity.magnitude < 0.01f)
                {
                    rb.AddForce(new Vector2(4, 0), ForceMode2D.Impulse);
                    rb.drag = 2f;
                    idling = true;
                }
                idleTimer = idleTimeout;
            }
        }
    }

    public void SetHook(GameObject platform)
    {
        playerState = PlayerState.OnHook;
        this.platform = platform;
        dj.connectedAnchor = platform.transform.position;
        dj.enabled = true;
    }

    public void DisconnectHook()
    {
        this.platform = null;
        dj.enabled = false;
        rb.drag = 0.5f;
        playerState = PlayerState.InAir;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Pickup")
            GetCoin(other.gameObject);
        if (other.gameObject.tag == "Blade")
            Death();
        else if (!jumpingUp)
            if (other.gameObject.tag == "Hook")
                SetHook(other.gameObject);
            else if (other.gameObject.tag == "Ground")
            {
                playerState = PlayerState.OnGround;
                groundCollider = other.GetComponents<Collider2D>()[0];
                groundCollider.enabled = true;
                groundTargetX = other.transform.position.x;
                platform = other.gameObject;
            }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Waterfall")
            if (other.gameObject.GetComponent<WaterfallBehaviour>().waterActive)
                Death();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (playerState != PlayerState.Jumping)
        {
            if (!jumpingUp)
                if (other.gameObject.tag == "Hook")
                    SetHook(other.gameObject);
        }
        else if (other.gameObject.tag == "Ground")
        {
            groundCollider.enabled = false;
            playerState = PlayerState.InAir;
            platform = null;
            groundTargetX = null;
        }
        
    }

    void GetCoin(GameObject coin)
    {
        GameObject.Destroy(coin);
        gameController.AddScore(1);
    }

    public void Death()
    {
        gameController.Restart();
    }

    void MoveUp()
    {
        if (playerState == PlayerState.OnGround)
        {
            rb.position = new Vector2(platform.transform.position.x, platform.transform.position.y + 0.2f);
            playerState = PlayerState.Jumping;
        }
        else if (playerState == PlayerState.OnHook)
        {
            rb.position = new Vector2(platform.transform.position.x, platform.transform.position.y - 0.4f);
            playerState = PlayerState.Jumping;
        }
        jumpingUp = true;
        rb.velocity = new Vector2(0, 0);
        rb.drag = 0.5f;
        rb.AddForce(new Vector2(0, 14), ForceMode2D.Impulse);
        gameController.StartGame();
        idleTimer = idleTimeout;
        idling = false;
    }

    void MoveRight()
    {
        if (playerState == PlayerState.OnGround)
        {
            rb.velocity = new Vector2(0, 0);
            rb.position = new Vector2(platform.transform.position.x, platform.transform.position.y +0.2f);
            rb.AddForce(new Vector2(8f, 6f), ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }
        else if (playerState == PlayerState.OnHook)
        {
            rb.position = new Vector2(platform.transform.position.x, platform.transform.position.y - 0.4f);
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(new Vector2(11, 0), ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }
        rb.drag = 0.5f;
        gameController.StartGame();
        idleTimer = idleTimeout;
        idling = false;
    }

    void MoveLeft()
    {
        if (playerState == PlayerState.OnGround)
        {
            rb.velocity = new Vector2(0, 0);
            rb.position = new Vector2(platform.transform.position.x, platform.transform.position.y + 0.2f);
            rb.AddForce(new Vector2(-8f, 6f), ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }
        else if (playerState == PlayerState.OnHook)
        {
            rb.position = new Vector2(platform.transform.position.x, platform.transform.position.y - 0.4f);
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(new Vector2(-11, 0), ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }
        rb.drag = 0.5f;
        gameController.StartGame();
        idleTimer = idleTimeout;
        idling = false;
    }

    void MoveDown()
    {
        if (playerState == PlayerState.OnGround)
        {
            rb.position = new Vector2(platform.transform.position.x, platform.transform.position.y + 0.2f);
            rb.velocity = new Vector2(0, 0);
            if (groundCollider != null)
                groundCollider.enabled = false;
            playerState = PlayerState.InAir;
        }
        else if (playerState == PlayerState.OnHook)
        {
            rb.position = new Vector2(platform.transform.position.x, platform.transform.position.y - 0.4f);
            rb.velocity = new Vector2(0, 0);
            DisconnectHook();
        }
        rb.drag = 0.5f;
        gameController.StartGame();
        idleTimer = idleTimeout;
        idling = false;
    }

    private void ManageKeyInput()
    {
        if (Input.GetKeyDown("up"))
            MoveUp();
        else if (Input.GetKeyDown("right"))
            MoveRight();
        else if (Input.GetKeyDown("left"))
            MoveLeft();
        if (Input.GetKeyDown("down"))
            MoveDown();
    }

    private void ManageTouchInput()
    {
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
}
