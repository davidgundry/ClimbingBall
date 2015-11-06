using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

    private Rigidbody2D rb;
    private DistanceJoint2D dj;

    public GameObject hook = null;

    public Transform gameControllerTransform;
    private GameController gameController;
    private CameraBehaviour cameraBehaviour;
    private bool noTouch = true;

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
        OnGroundJumpReady,
        Jumping
    }

    public PlayerState playerState = PlayerState.OnHook;
    private bool jumpingUp;

    //public static event Action<SwipeDirection> Swipe;
    private bool swiping = false;
    private bool eventSent = false;
    private Vector2 lastPosition;
    private float groundTargetX;
    private Collider2D groundCollider;


	// Use this for initialization
	void Start () {
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
        if (rb.velocity.y <= 0)
            jumpingUp = false;

        if ((playerState == PlayerState.OnHook) && (rb.velocity.y > 0))
            rb.drag = 200;
        else
            rb.drag = 0.5f;

        switch (playerState)
        {
            case PlayerState.OnHook:
                ManageKeyInput();
                ManageTouchInput();
                break;
            case PlayerState.Jumping:
                if (hook != null)
                    if ((rb.velocity.y > 0) && (transform.position.y > hook.transform.position.y - 0.25f))
                        DisconnectHook();
                break;
            case PlayerState.OnGround:
                if (transform.position.x < groundTargetX)
                {
                    if (rb.velocity.x <= 0)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        transform.position = new Vector3(Mathf.Min(transform.position.x + 1*Time.deltaTime, groundTargetX), transform.position.y, transform.position.z);
                    }
                }
                else if (transform.position.x > groundTargetX)
                {
                    if (rb.velocity.x >= 0)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        transform.position = new Vector3(Mathf.Max(transform.position.x - 1 * Time.deltaTime, groundTargetX), transform.position.y, transform.position.z);
                    }
                }
                else
                    playerState = PlayerState.OnGroundJumpReady;
                break;
            case PlayerState.OnGroundJumpReady:
                ManageKeyInput();
                ManageTouchInput();
                break;
        }
	}

    public void SetHook(GameObject hook)
    {
        playerState = PlayerState.OnHook;
        this.hook = hook;
        dj.connectedAnchor = hook.transform.position;
        dj.enabled = true;
    }

    public void DisconnectHook()
    {
        this.hook = null;
        dj.enabled = false;
        rb.drag = 0.5f;
        playerState = PlayerState.InAir;
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
            else if (other.gameObject.tag == "Ground")
            {
                playerState = PlayerState.OnGround;
                groundCollider = other.GetComponents<Collider2D>()[0];
                groundCollider.enabled = true;
                groundTargetX = other.transform.position.x;
            }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (playerState != PlayerState.Jumping)
        {
            if (other.gameObject.tag == "Hook")
                SetHook(other.gameObject);
        }
        else if (other.gameObject.tag == "Ground")
        {
            groundCollider.enabled = false;
            playerState = PlayerState.InAir;
        }
        
    }

    void GetCoin(GameObject coin)
    {
        GameObject.Destroy(coin);
        gameController.AddScore(1);
    }

    void Death()
    {
        gameController.Restart();
    }

    void MoveUp()
    {
        if (playerState == PlayerState.OnGroundJumpReady)
        {
            playerState = PlayerState.InAir;
        }
        else if (playerState == PlayerState.OnHook)
        {
            if (hook != null)
                rb.position = new Vector2(hook.transform.position.x, hook.transform.position.y - 0.4f);
            rb.drag = 0.5f;
            playerState = PlayerState.Jumping;
        }
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(new Vector2(0, 14), ForceMode2D.Impulse);
        gameController.StartGame();
        

        //DisconnectHook();
    }

    void MoveRight()
    {
        if (playerState == PlayerState.OnGroundJumpReady)
        {
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(new Vector2(9, 5), ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }
        else if (playerState == PlayerState.OnHook)
        {
            rb.position = new Vector2(hook.transform.position.x, hook.transform.position.y - 0.4f);
            rb.velocity = new Vector2(0, 0);
            rb.drag = 0.5f;
            rb.AddForce(new Vector2(11, 0), ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }

        gameController.StartGame();
    }

    void MoveLeft()
    {
        if (playerState == PlayerState.OnGroundJumpReady)
        {
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(new Vector2(-9, 5), ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }
        else if (playerState == PlayerState.OnHook)
        {
            rb.position = new Vector2(hook.transform.position.x, hook.transform.position.y - 0.4f);
            rb.velocity = new Vector2(0, 0);
            rb.drag = 0.5f;
            rb.AddForce(new Vector2(-11, 0), ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }

        gameController.StartGame();
    }

    void MoveDown()
    {
        if (playerState == PlayerState.OnGroundJumpReady)
        {
            if (groundCollider != null)
                groundCollider.enabled = false;
            playerState = PlayerState.InAir;
        }
        else if (playerState == PlayerState.OnHook)
        {
            rb.position = new Vector2(hook.transform.position.x, hook.transform.position.y - 0.4f);
            rb.velocity = new Vector2(0, 0);
            rb.drag = 0.5f;
            DisconnectHook();
        }

        gameController.StartGame();
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
