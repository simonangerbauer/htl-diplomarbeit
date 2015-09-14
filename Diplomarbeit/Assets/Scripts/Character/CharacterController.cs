using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
    public float MaxSpeed = 10f;

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
        float move = Input.GetAxis("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(move));
        rigidBody.velocity = new Vector2(move * MaxSpeed, rigidBody.velocity.y);
		rigidBody.AddForce (Vector2.right * MaxSpeed * move);

    }

    // Update is called once per frame
    void Update () {
        
	}
}
