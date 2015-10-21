using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
    public float MaxSpeed = 1f;
	public float MaxJump = 1f;

	public Transform GroundCheck;
	public float GroundCheckRadius;
	public LayerMask  WhatIsGround;
    public bool runActivated;

	private bool grounded = false;
	private bool JumpPressed = false;

    public static float DEFAULT_MOVEMENT_RIGHT = 1f;


    bool facingRight = true;
    Animator anim;
	Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
		rigidBody=GetComponent<Rigidbody2D>();
	}

    void FixedUpdate()
    {
        //Reading player movement
        float horizontalMovement = (!runActivated) ? Input.GetAxis("Horizontal") : 1;

        //Check if player grounded
        grounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, WhatIsGround);
        anim.SetBool("Grounded", grounded);

        //Setting player movement animation
        anim.SetFloat("Speed", Mathf.Abs(horizontalMovement));

        //Player movement
        if (runActivated)
        {
            Move(CharacterController.DEFAULT_MOVEMENT_RIGHT);
        }
        else
        {
            Move(horizontalMovement);
        }

        //Player jump
        if ((Input.GetAxis("Jump") > 0 || JumpPressed) && grounded )
        {
            rigidBody.AddForce(new Vector2(0, MaxJump));
            anim.SetTrigger("OnJump");
            JumpPressed = false;
        }
    }

    private void Move(float movement)
    {
        rigidBody.velocity = new Vector2(MaxSpeed * movement, rigidBody.velocity.y);
    }

    public void Jump()
    {
        JumpPressed = true;
    }

    // Update is called once per frame
    void Update () {
        
	}
}
