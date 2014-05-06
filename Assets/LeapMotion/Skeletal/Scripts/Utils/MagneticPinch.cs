/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Leap Motion hand script that detects pinches and grabs the
// closest rigidbody with a spring force if it's within a given range.
public class MagneticPinch : MonoBehaviour {

  const float TRIGGER_DISTANCE_RATIO = 0.7f;

  public float forceSpringConstant = 100.0f;
  public float magnetDistance = 2.0f;

  private bool pinching_;
  private Collider grabbed_;

  void Start() {
    pinching_ = false;
    grabbed_ = null;
  }

  void OnPinch(Vector3 pinch_position) {
    pinching_ = true;

    // Check if we pinched a movable object and grab the closest one that's not part of the hand.
    Collider[] close_things = Physics.OverlapSphere(pinch_position, magnetDistance);
    Vector3 distance = new Vector3(magnetDistance, 0.0f, 0.0f);

    for (int j = 0; j < close_things.Length; ++j) {
      Vector3 new_distance = pinch_position - close_things[j].transform.position;
      if (close_things[j].rigidbody != null && new_distance.magnitude < distance.magnitude &&
          !close_things[j].transform.IsChildOf(transform)) {
        grabbed_ = close_things[j];
        distance = new_distance;
      }
    }
  }

  void OnRelease() {
    grabbed_ = null;
    pinching_ = false;
  }

  void Update() {
    bool trigger_pinch = false;
    Hand hand = GetComponent<HandModel>().GetLeapHand();

    // Thumb tip is the pinch position.
    Vector3 thumb_tip = hand.Fingers[0].TipPosition.ToUnityScaled();

    // Scale trigger distance by thumb length
    Vector3 thumb_direction = thumb_tip - hand.Fingers[0].JointPosition(Finger.FingerJoint.JOINT_PIP).ToUnityScaled();
    float trigger_distance = thumb_direction.magnitude * TRIGGER_DISTANCE_RATIO;

    // Check thumb tip distance to joints on all other fingers.
    // If it's close enough, start pinching.
    for (int i = 1; i < HandModel.NUM_FINGERS && !trigger_pinch; ++i) {
      Finger finger = hand.Fingers[i];

      for (int j = 0; j < FingerModel.NUM_JOINTS && !trigger_pinch; ++j) {
        Vector3 joint_position = finger.JointPosition((Finger.FingerJoint)(j)).ToUnityScaled();
        Vector3 distance = thumb_tip - joint_position;
        if (distance.magnitude < trigger_distance)
          trigger_pinch = true;
      }
    }

    Vector3 pinch_position = transform.TransformPoint(thumb_tip);

    // Only change state if it's different.
    if (trigger_pinch && !pinching_)
      OnPinch(pinch_position);
    else if (!trigger_pinch && pinching_)
      OnRelease();

    // Accelerate what we are grabbing toward the pinch.
    if (grabbed_ != null) {
      Vector3 distance = pinch_position - grabbed_.transform.position;
      grabbed_.rigidbody.AddForce(forceSpringConstant * distance);
    }
  }
}
