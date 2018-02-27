using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {


	public enum EnemyType {
		Basic, Digger, Trailer, Spawner
	};

	public EnemyType selected = new EnemyType ();
	public GameObject enemySpawned;
	public int playerDamage;
	public int hp = 4;
	public int wallDamage;
	public Puddle puddleTrail;
	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;

	private GameManager gameManage;
	private Animator animator;
	private Transform target;
	private int skipMove;

	void Awake () {
		gameManage = GameObject.Find ("GameManager").GetComponent<GameManager> ();
	}

	protected override void Start () {
		if (selected == EnemyType.Basic) {
			hp = 2;
			playerDamage = 10;
		} else if (selected == EnemyType.Digger) {
			hp = 4;
			playerDamage = 20;
			wallDamage = 2;
		} else if (selected == EnemyType.Trailer) {
			hp = 10;
			playerDamage = 20;
		} else if (selected == EnemyType.Spawner) {
			hp = 6;
			playerDamage = 0;
		}

		GameManager.instance.AddEnemyToList (this);
		animator = GetComponent <Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
		}

	protected override void AttemptMove <T> (int xDir, int yDir) {
		if (skipMove == 1) {
			skipMove = 0;
			return;
		}

		if (selected == EnemyType.Spawner) {
			SpawnEnemy ();
			return;
		}

		base.AttemptMove <T> (xDir, yDir);

		if (selected == EnemyType.Trailer) {
			GameObject trail = Instantiate (puddleTrail.gameObject, new Vector2 (transform.position.x - xDir, transform.position.y - yDir), Quaternion.identity) as GameObject;
			trail.transform.parent = transform.parent;
			Puddle trailScript = trail.GetComponent <Puddle> ();
			gameManage.puddles.Add (trailScript);

			TakeDamage (1);
			Debug.Log (hp);
		}

		skipMove = Random.Range(0, 2);
	}

	public void MoveEnemy () {

		if (hp > 0) {
			int xDir = 0;
			int yDir = 0;

			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
				yDir = target.position.y > transform.position.y ? 1 : -1;
			} else {
				xDir = target.position.x > transform.position.x ? 1 : -1; 
			}
				
			AttemptMove <Player> (xDir, yDir);
		}
	}

	void SpawnEnemy () {
		if ((int)Random.Range (0, 10) == 1) {
			Instantiate (enemySpawned, new Vector2 (transform.position.x + Random.Range (-1, 1), transform.position.y + Random.Range (-1, 1)), Quaternion.identity);
		}
	}

	protected override void OnCantMove <T> (T component) {
		if (selected == EnemyType.Spawner) {
			return;
		}

		Player hitplayer = component as Player;

		animator.SetTrigger ("enemyAttack");
		SoundManager.instance.RandomizeSfx (enemyAttack1, enemyAttack2);
		hitplayer.LoseFood (playerDamage);
		Player targetScript = target.gameObject.GetComponent <Player> ();
	}

	protected override void EnemyHitWall <T> (T component) {
		if (selected != EnemyType.Digger) {
			return;
		}
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("enemyAttack");
	}

	public void TakeDamage (int damage) {
		hp -= damage;

		if (hp <= 0) {
			DestroyObject (gameObject);
		}
	}

	protected override void EnemyAttack <T> (T component) {
	}
}