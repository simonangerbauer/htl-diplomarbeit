using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour {
    public float MaxSpeed = 1f;
	public float MaxJump = 1f;

	public Transform GroundCheck;
	public float GroundCheckRadius;
	public LayerMask  WhatIsGround;
    public bool runActivated;
    private List<GameObject> bullets = new List<GameObject>();

    private bool grounded = false;
	private bool JumpPressed = false;

    public static float DEFAULT_MOVEMENT_RIGHT = 1f;

    private bool shoot = true;
    public GameObject bullet;

    public float speed = 150f;


    bool facingRight = true;


    Animator anim;
	Rigidbody2D rigidBody;
    GameObject rifle;
    GameObject bulletSpawn;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
		rigidBody=GetComponent<Rigidbody2D>();
        rifle = GameObject.Find("LeftArm");
        bulletSpawn = GameObject.Find("BulletSpawnPoint");
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
        if (shoot)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }
    }

    public void Shoot()
    {
        if ((Input.mousePosition.x > Screen.width / 5 || Input.mousePosition.y > Screen.height / 2.5) && (Input.mousePosition.x > 60 || Input.mousePosition.y < Screen.height - 60))
        {
            //shooting animation on character
            //anim.CrossFade("Shooting", 0f);

            Vector3 target = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            target = Camera.main.ScreenToWorldPoint(target);
            target.Set(target.x, target.y, gameObject.transform.position.z);

            //rifle movement
            Quaternion rotation = Quaternion.LookRotation(target - transform.position, transform.TransformDirection(Vector3.up));
            rifle.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w); ;

            //bullet spawning
            GameObject obj = bullets.FirstOrDefault(x => !x.activeInHierarchy);
            if (obj == null)
            {
                obj = (GameObject)Instantiate(bullet);
                bullets.Add(obj);
            }

            obj.SetActive(true);
            obj.transform.position = new Vector3(bulletSpawn.transform.position.x,bulletSpawn.transform.position.y,0);
            obj.transform.rotation = bulletSpawn.transform.rotation;
            obj.GetComponent<Rigidbody2D>().velocity = (target - bulletSpawn.transform.position).normalized * speed;
            shoot = false;
            Invoke("CanShootAgain", 1f);
        }
    }

    private void CanShootAgain()
    {
        shoot = true;
    }
}
