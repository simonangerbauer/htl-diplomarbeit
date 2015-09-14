using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
    public float MaxSpeed = 10f;
	public float MaxJump = 10f;

	public Transform GroundCheck;
	public float GroundCheckRadius;
	public LayerMask  WhatIsGround;
	private bool grounded = false;
	private bool jumping = false;


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
		float horizontalMovement = Input.GetAxis ("Horizontal");

		//Setting player movement animation
		anim.SetFloat ("Speed", Mathf.Abs (horizontalMovement));

		//Player movement
		if(horizontalMovement > 0 ){
			rigidBody.velocity = new Vector2(MaxSpeed * horizontalMovement, rigidBody.velocity.y);
		}

		if(horizontalMovement < 0){
			rigidBody.velocity = new Vector2(MaxSpeed * horizontalMovement, rigidBody.velocity.y);
		}

		//Check if player grounded
		grounded = Physics2D.OverlapCircle (GroundCheck.position, GroundCheckRadius, WhatIsGround);
		anim.SetBool ("Grounded", grounded);

		//Player jump
		if(Input.GetAxis ("Jump") > 0 && grounded) {
			rigidBody.AddForce (new Vector2(0,MaxJump));
			anim.SetTrigger("OnJump");
			jumping = true;
		}


    }

    // Update is called once per frame
    void Update () {
        
	}
}
