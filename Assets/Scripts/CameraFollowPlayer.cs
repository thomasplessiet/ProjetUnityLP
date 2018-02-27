using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour {
	Transform cameraTransform;

	void Awake () {
		cameraTransform = GameObject.FindGameObjectWithTag ("MainCamera").transform;
	}

	void Update () {
		cameraTransform.position = new Vector3 (transform.position.x, transform.position.y, -10f); 
	}
}
