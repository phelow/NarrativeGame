using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	[SerializeField]private float m_forceMultiplier = 10.0f;
	[SerializeField]private Rigidbody2D m_rb;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.LeftArrow)) {
			m_rb.AddForce (Vector2.left * m_forceMultiplier);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			m_rb.AddForce (Vector2.right * m_forceMultiplier);

		}

		if (Input.GetKey (KeyCode.UpArrow)) {
			m_rb.AddForce (Vector2.up * m_forceMultiplier);

		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			m_rb.AddForce (Vector2.down * m_forceMultiplier);

		}
	}
}
