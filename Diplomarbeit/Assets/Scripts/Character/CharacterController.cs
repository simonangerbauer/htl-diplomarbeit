using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
    public float MaxSpeed = 10f;

    bool facingRight = true;
    Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(move));
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(move * MaxSpeed, rigidBody.velocity.y);


    }

    // Update is called once per frame
    void Update () {
        
	}
}
