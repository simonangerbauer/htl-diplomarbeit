using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
		public GameObject bullet;
		public float speed = 500f;
		public GameObject aim;

        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
		private List<GameObject> bullets = new List<GameObject>();

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }

		public void Jump()
		{
			if (!m_Jump)
			{
				// Read the jump input in Update so button presses aren't missed.
				m_Jump = true;
			}
		}
		public void Shoot()
		{
			GameObject obj = bullets.FirstOrDefault(x => !x.activeInHierarchy);
			if (obj == null) 
			{
				obj = (GameObject)Instantiate(bullet);
				bullets.Add(obj);
			}
			obj.SetActive (true);
			obj.transform.position = transform.position;
			obj.transform.rotation = transform.rotation;
			obj.GetComponent<Rigidbody2D>().velocity = (aim.transform.position - transform.position).normalized * speed; 

		}
        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            // bool crouch = Input.GetKey(KeyCode.LeftControl);
            // float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(1, false, m_Jump);
            m_Jump = false;
        }
    }
}
