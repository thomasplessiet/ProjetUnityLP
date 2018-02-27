using UnityEngine;
using System.Collections;

public class Puddle : MonoBehaviour {

	public int turnLife;
	public Enemy enemyCreator;
	public GameManager gameManager;

	void Start () {
		turnLife = (int)(Random.Range (3, 4));
	}

	void Update () {
		if (turnLife <= 0) {
			DestroyObject (gameObject);
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "DangerousCollidable") {
			if (GetInstanceID() > other.gameObject.GetInstanceID()) {
				DestroyObject(gameObject);
			}
		}
		if (other.gameObject.tag == "Wall") {
			DestroyObject(gameObject);
		}
	}
		
}
