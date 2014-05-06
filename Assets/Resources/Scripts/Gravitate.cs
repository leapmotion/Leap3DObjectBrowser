using UnityEngine;
using System.Collections;

public class Gravitate : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
		// find closest body
		GameObject closest = ClosestPlanet();
		// initial velocity is perpendicular to it
		Vector3 vel = transform.position - closest.transform.position;
		float tmp = vel.x;
		vel.x = -vel.y;
		vel.y = tmp;
		rigidbody.velocity = vel.normalized * 5.0f;
	}

	GameObject ClosestPlanet() {
		GameObject [] planets = GameObject.FindGameObjectsWithTag("Planet");
		GameObject candidate = null;
		float minDist = float.MaxValue;
		for(int i = 0; i < planets.Length; ++i) {
			float d = Vector3.Distance(transform.position, planets[i].transform.position);
			if (d < minDist) {
				minDist = d;
				candidate = planets[i];
			}
		}
		return candidate;
	}
	
	// Update is called once per frame
	void Update () {
		GameObject closest = ClosestPlanet();
		Vector3 meToClosest = closest.transform.position - transform.position;
		rigidbody.velocity += meToClosest * 0.01f;
	}
}
