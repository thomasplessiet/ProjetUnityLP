using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public float turnDelay = 0.00f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public int playerFoodPoints = 100;
	public int level = 3;
	[HideInInspector] public bool playersTurn = true;
	public List<Puddle> puddles;
	public GameObject gameOverPanel;
	public Text gameOverText;

	private List<Enemy> enemies;
	private bool enemiesMoving;


	void Awake()
	{
		gameOverPanel.SetActive(false);

		if (instance == null)


			instance = this;


		else if (instance != this)


		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();
		puddles = new List<Puddle> ();

		SceneManager.activeSceneChanged += OnSceneLoaded;
	}

	void OnSceneLoaded (Scene previousScene, Scene newScene) {

		level++;
	}

	void OnDestroy () {
		SceneManager.activeSceneChanged -= OnSceneLoaded;
	}

	void InitGame () {
		enemies.Clear ();
		boardScript.SetupScene (level);
	}

	public void GameOver () {
		gameOverPanel.SetActive(true);
		gameOverText.text = " TU AS PÉRI!";

		enabled = false;
	}

	void Update () {
		if (playersTurn || enemiesMoving) {
			return;
		}

		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList (Enemy script) {
		enemies.Add (script);
	}

	IEnumerator MoveEnemies() {
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++) {
			enemies [i].MoveEnemy ();
			yield return new WaitForSeconds (0.05f);
		}

		playersTurn = true;
		enemiesMoving = false;
		for (int i = 0; i < puddles.Count; i++) {
			puddles [i].turnLife--;
		}
	}

}
