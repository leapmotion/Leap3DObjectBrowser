using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {
	void Update () {
		transform.LookAt(Camera.main.transform.position);
	}
}
