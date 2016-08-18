using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GhostWord : MonoBehaviour {
	[SerializeField]TextMesh m_text;

	Vector2 m_centralPosition;

	[SerializeField]private float m_minLerpTime = 1.0f;
	[SerializeField]private float m_maxLerpTime = 5.0f;
	[SerializeField]private float m_maxDistFromCenter = 5.0f;

	// Use this for initialization
	void Start () {
		m_text.color = Color.white;
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void LoseLetter(){
		m_text.text = m_text.text.Substring (1, m_text.text.Length - 1);
	}

	public void FadeOut(){
		StartCoroutine (FadeOutRoutine ());
	}

	private IEnumerator FadeOutRoutine(){
		float t = 0.0f;
		float fadeOutTime = Random.Range (m_minLerpTime, m_maxLerpTime);
		Color origColor = m_text.color;
		Color invis = new Color (0.0f, 0.0f, 0.0f, 0.0f);
		while (t < fadeOutTime) {
			t += Time.deltaTime;
			m_text.color = Color.Lerp (origColor, invis, t/fadeOutTime);
			yield return new WaitForEndOfFrame ();
		}
		Destroy (this.gameObject);
	}

	public void SetWord(string newText, float opacity){
		m_text.text = newText;

		m_text.color = new Color (1.0f, 1.0f, 1.0f, opacity);
	}

	public void SetPosition(Vector2 newPosition){
		m_centralPosition = newPosition;
		transform.position = newPosition;
		StartCoroutine (InterpolatePosition ());
	}

	private IEnumerator InterpolatePosition(){
		yield return new WaitForEndOfFrame ();

		transform.position = m_centralPosition;
		while (true){
			Vector3 origPosition = new Vector3 (transform.position.x, transform.position.y,transform.position.z);
			Vector3 newPosition = new Vector3 (m_centralPosition.x + Random.Range (-m_maxDistFromCenter, m_maxDistFromCenter), m_centralPosition.y + Random.Range (-m_maxDistFromCenter, m_maxDistFromCenter),transform.position.z);
			float t = 0.0f;

			float lerpTime = Random.Range (m_minLerpTime, m_maxLerpTime);
			while (t < lerpTime) {
				t += Time.deltaTime;
				transform.position = Vector3.Lerp (origPosition, newPosition, t/lerpTime);
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForEndOfFrame();
		}
	}
}
