using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public int damage = 1;
	public float restartLevelDelay = 1f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int food;
	private bool puddleBool;


	protected override void Start () {
		animator = GetComponent <Animator> ();
		food = GameManager.instance.playerFoodPoints;
		foodText = FindObjectOfType <Text> ();

		base.Start ();
		UpdateFoodText (0);
	}

	private void OnDisable () {
		GameManager.instance.playerFoodPoints = food;
	}


	void Update () {
		
		if (food < 0) {
			food = 0;
		}

		if (!GameManager.instance.playersTurn) {
			return;
		}


		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0) {
			vertical = 0;
		}

		if (horizontal != 0 || vertical != 0) {
			AttemptMove <Wall> (horizontal, vertical);
		}
	}

	protected override void AttemptMove <T> (int xDir, int yDir) {
		UpdateFoodText (0);

		base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit;
		if (Move (xDir, yDir, out hit, true)) {
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}
			

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
		puddleBool = true;
	}

	private void OnTriggerEnter2D (Collider2D other)
	{


		if (other.tag == "Exit")
		{
			Invoke ("Restart", restartLevelDelay);

			enabled = false;

		
		} else if(other.tag == "Food") {
			food += pointsPerFood;
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
			other.gameObject.SetActive (false);
			UpdateFoodText (pointsPerFood);
		
		} else if(other.tag == "Soda") {
			food += pointsPerSoda;
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
			UpdateFoodText (pointsPerSoda);
		
		} else if(other.tag == "DangerousCollidable" && !GameManager.instance.playersTurn && puddleBool) {
			
			food -= 10;
			puddleBool = false;

			UpdateFoodText (-10);
		}
	}

	protected override void OnCantMove <T> (T component) {
		
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}

	protected override void EnemyAttack <T> (T component) {
		
		Enemy baddie = component as Enemy;
		animator.SetTrigger ("playerChop");
		baddie.TakeDamage (damage);

		if (baddie.selected == Enemy.EnemyType.Spawner) {
			LoseFood (5);
		}
	}


	private void Restart () {
		Application.LoadLevel (Application.loadedLevel);
	}


	public void LoseFood (int loss) {
		animator.SetTrigger ("playerHit");
		food -= loss;
		UpdateFoodText (-(loss));
		CheckIfGameOver ();
	}


	public void UpdateFoodText (int foodDelta) {
		if (foodDelta > 0) {
			foodText.text = "Santé + " + foodDelta.ToString() + ": " + food.ToString();
		} else if (foodDelta == 0) {
			foodText.text = "Santé: " + food.ToString();
		} else {
			foodText.text = "Santé " + foodDelta.ToString() + ": " + food.ToString();
		}
	}


	private void CheckIfGameOver () {
		if (food <= 0) {
			SoundManager.instance.PlaySingle (gameOverSound);
			SoundManager.instance.musicSource.Stop ();
			GameManager.instance.GameOver ();
		}
	}


	protected override void EnemyHitWall <T> (T component) {
	}

}
