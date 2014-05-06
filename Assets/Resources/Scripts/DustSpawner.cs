using UnityEngine;
using System.Collections;

public class DustSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 300; ++i) {
			GameObject dust = Instantiate(Resources.Load("Prefabs/Dust")) as GameObject;
			dust.transform.position = new Vector3(Random.Range(-50, 50), Random.Range(-30, 30), Random.Range(-50, 50));
		}
	}
}
