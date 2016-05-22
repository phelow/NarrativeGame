using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextBlurb : MonoBehaviour {

	/// <summary>
	/// Whether or not this is a "Driving" text blurb. If it's a driver it will kick off the chain of all following
	/// text objects.
	/// </summary>
	[SerializeField]private bool m_isDriver = false;

	[SerializeField]private string m_string = "";
	[SerializeField]private Text m_text;

	//todo: unserialize
	[SerializeField]private string [] m_words;

	[SerializeField]private TextBlurb [] m_nextNodes;

	private Queue<GhostWord> m_ghosts;

	private int m_curWordTyping = 0;
	private int m_curLetterTyping = 0;
	private float c_textCharWidth = 10.0f;

	private float m_XStart = 0.0f;
	private int m_ghostsSpawned = 0;
	private TextBlurb m_parent;


	private bool m_completed = false;

	[SerializeField]private GameObject m_ghostSpawnPoint;

	int curGhosts = 0;


	//These two variables determine how many words to show that have not yet been typed.
	[SerializeField]private float m_minWordsRevealed;
	[SerializeField]private float m_maxWordsRevealed;

	//A word that prompts the player to type it
	[SerializeField]private GameObject m_ghostWord;

	/// <summary>
	/// If the next key in the string has been pressed
	/// </summary>
	private bool m_nextKeyPressed = false;


	void Start () {
		m_ghosts = new Queue<GhostWord> ();
		m_XStart = 0;
		m_words = ParseText (m_string);
		if (m_isDriver == true) {
			StartCoroutine (WriteWordsByInput());
		}
	}

	/// <summary>
	/// Parses the text, takes the text string and converts it into an array of words.
	/// </summary>
	/// <returns>List of the words in strings.</returns>
	/// <param name="inputText">The input string text that is to be parsed.</param>
	private static string [] ParseText (string inputText){
		char[] delimiters = { ' ' };
		return inputText.Split (delimiters);
	}

	void Update(){
		//Check to see if our next key is pressed
		if (m_completed == false) {
			m_nextKeyPressed = Input.GetKeyDown (NextKeyCode ());
		}
	}

	private void InstantiateGhosts(){
		//Instantiate the first x words
		StartCoroutine (KickoffGhosts ());

	}

	private IEnumerator KickoffGhosts(){

		int wordsToReveal = (int) Random.Range(m_minWordsRevealed, m_maxWordsRevealed) - curGhosts;
		float opacity = 1.0f;
		for (int i = m_ghostsSpawned; i < m_ghostsSpawned + wordsToReveal && i < m_words.Length; i++) {
			Vector3 newPos = new Vector3 (m_XStart, transform.position.y,transform.position.z);
			GameObject gw = (GameObject) GameObject.Instantiate (m_ghostWord,newPos,transform.rotation);
			yield return new WaitForSeconds (.1f);
			gw.GetComponent<GhostWord>().SetPosition (newPos);
			yield return new WaitForEndOfFrame ();
			m_XStart += m_words [i].Length * c_textCharWidth;

			m_ghosts.Enqueue (gw.GetComponent<GhostWord>());
			gw.GetComponent<GhostWord>().SetWord (m_words [i],opacity);
			curGhosts++;
			opacity *= Random.Range (0.4f, 0.6f);
		}
		m_ghostsSpawned += wordsToReveal;
	}
	/// <summary>
	/// Main driving coroutine for this prefab. Called when this blurb can be activated. Listens to input text and shows
	/// text as it unfolds.
	/// </summary>
	private IEnumerator WriteWordsByInput(){

		//instantiate the following words
		InstantiateGhosts();
		m_completed = false;
		while (m_completed == false) {
			yield return new WaitForEndOfFrame ();

			//If next char in the word is currently pressed
			if (m_nextKeyPressed && m_curWordTyping < m_words.Length && m_completed == false) {

				//write that char in the word	
				m_text.text += "" + m_words[m_curWordTyping][m_curLetterTyping];
				//move your position forward
				m_curLetterTyping++;

				if (m_curLetterTyping >= m_words [m_curWordTyping].Length) {
					m_curLetterTyping = 0;
					m_curWordTyping++;
					GhostWord gw = m_ghosts.Dequeue ();
					curGhosts--;
					gw.FadeOut (this);

					InstantiateGhosts ();
					m_text.text += " ";
					//move onto the next word
				}

				if (m_curWordTyping >= m_words.Length) {
					ActivateNextNode ();

				}
			}

		}
	}

	private bool HasKeys(){
		return true;
	}

	public void Enable(TextBlurb parent){
		m_parent = parent;
		if (HasKeys()) {
			StartCoroutine (WriteWordsByInput ());
		}
	}

	public void DisableChildren(){
		//disable each one one by one
		foreach (TextBlurb child in m_nextNodes) {
			child.DisableSelf ();
		}
	}

	public void DisableSelf(){
		//kill all ghosts
		while(m_ghosts.Count != 0){
			GhostWord gw = m_ghosts.Dequeue ();

			Destroy (gw.gameObject);
		}

		//stop listening for input
		m_completed = true;
	}

	/// <summary>
	/// Activates the next set of nodes and disables this one.
	/// </summary>
	private void ActivateNextNode(){
		foreach (TextBlurb tb in m_nextNodes) {
			tb.Enable (this);
		}
		if (m_isDriver == false) {
			m_parent.DisableChildren ();
		}
		m_completed = true;
	}

	/// <summary>
	/// Gets and returns the KeyCode for the next character in the string
	/// </summary>
	/// <returns>The key code for the next character in the string.</returns>
	private string NextKeyCode(){
		//Determine the next character
		string nextChar = ""+ m_words[m_curWordTyping][m_curLetterTyping];

		//Compare the next character to all available keycodes, return that character

		return (string)nextChar.ToLower();
	}

}
