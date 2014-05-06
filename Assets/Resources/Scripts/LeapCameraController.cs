using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class LeapCameraController : MonoBehaviour {

	Controller m_leapController;
	bool m_twoHandGrabLastFrame = false;
	Vector3 m_lastRightPos;
	Vector3 m_lastLeftPos;
	Vector3 m_offsetEuler;
	Vector3 m_pivotPt;
	public Vector3 m_offset;
  public float scaleMovement = 20.0f;
	
	void Start () {
		m_leapController = new Controller();
	}
	
	Hand GetLeftMostHand(Frame f) {
		float xComp = float.MaxValue;
		Hand candidate = null;
		for(int i = 0; i < f.Hands.Count; ++i) {
			if (f.Hands[i].PalmPosition.ToUnityScaled().x < xComp) {
				candidate = f.Hands[i];
				xComp = f.Hands[i].PalmPosition.ToUnityScaled().x;
			}
		}	
		return candidate;
	}

	Hand GetRightMostHand(Frame f) {
		float xComp = -float.MaxValue;
		Hand candidate = null;
		for(int i = 0; i < f.Hands.Count; ++i) {
			if (f.Hands[i].PalmPosition.ToUnityScaled().x > xComp) {
				candidate = f.Hands[i];
				xComp = f.Hands[i].PalmPosition.ToUnityScaled().x;
			}
		}
		return candidate;
	}
	
	bool Pinching(Hand h) {
		if (h == null) return false;
		return h.PinchStrength > 0.7f;
	}
	
	bool Grabbing(Hand h) {
		if (h == null) return false;
		return h.GrabStrength > 0.45f;
	}

  void ProcessTranslate(Hand hand) {
    // did the hand exist last frame?
    transform.position -= transform.rotation * (scaleMovement * hand.PalmVelocity.ToUnityScaled());
  }

	void ApplyWorldScale(float scaleFactor) {
		GameObject [] moons = GameObject.FindGameObjectsWithTag("Moon");
		GameObject [] planets = GameObject.FindGameObjectsWithTag("Planet");
		GameObject [] dusts = GameObject.FindGameObjectsWithTag("Dust");
		for (int i = 0; i < moons.Length; ++i) {
			moons[i].transform.localScale *= scaleFactor;
			Vector3 pvtToObj = moons[i].transform.position - m_pivotPt;
			moons[i].transform.position = m_pivotPt + pvtToObj * scaleFactor;
			moons[i].rigidbody.velocity *= scaleFactor;
		}
		for (int i = 0; i < planets.Length; ++i) {
			planets[i].transform.localScale *= scaleFactor;
			Vector3 pvtToObj = planets[i].transform.position - m_pivotPt;
			planets[i].transform.position = m_pivotPt + pvtToObj * scaleFactor;
			planets[i].rigidbody.velocity *= scaleFactor;
		}
		for (int i = 0; i < dusts.Length; ++i) {
			dusts[i].transform.localScale *= scaleFactor;
			Vector3 pvtToObj = dusts[i].transform.position - m_pivotPt;
			dusts[i].transform.position = m_pivotPt + pvtToObj * scaleFactor;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		Frame f = m_leapController.Frame();
				
		// Get Front most hand
		Hand left_hand = GetLeftMostHand(f);
		// Get second front most hand
		Hand right_hand = GetRightMostHand(f);

		bool twoHandGrab = Grabbing(left_hand) && Grabbing(right_hand) && f.Hands.Count > 1;

		// rotation gets priority over translation
		if (twoHandGrab) {
			if (m_twoHandGrabLastFrame) {
				
				Vector3 currentLeftToRight = right_hand.PalmPosition.ToUnityScaled() - left_hand.PalmPosition.ToUnityScaled();
				Vector3 lastLeftToRight = m_lastRightPos - m_lastLeftPos;
				m_offsetEuler = Quaternion.FromToRotation(lastLeftToRight, currentLeftToRight).eulerAngles;

				Vector3 righthandPos = transform.TransformPoint(m_offset + right_hand.PalmPosition.ToUnityScaled());
				Vector3 leftHandPos = transform.TransformPoint(m_offset + left_hand.PalmPosition.ToUnityScaled());

				m_pivotPt = (righthandPos + leftHandPos) * 0.5f;
				float scaleDifference = 1.0f + 0.3f * (currentLeftToRight.magnitude - lastLeftToRight.magnitude);
				ApplyWorldScale(scaleDifference);
				//GameObject.Find("DebugCube").transform.position = m_pivotPt;

				transform.RotateAround(m_pivotPt, transform.up, -m_offsetEuler.y);
				transform.RotateAround(m_pivotPt, transform.right, -m_offsetEuler.x);
				transform.RotateAround(m_pivotPt, transform.forward, -m_offsetEuler.z);

			}

			m_lastRightPos = right_hand.PalmPosition.ToUnityScaled();
			m_lastLeftPos = left_hand.PalmPosition.ToUnityScaled();
		}

		if (!twoHandGrab) {
			for (int i = 0; i < f.Hands.Count; ++i) {
				if (Grabbing(f.Hands[i])) ProcessTranslate(f.Hands[i]);
			}
		}

		m_twoHandGrabLastFrame = twoHandGrab;
	}
}
