/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

public class RiggedHand : HandModel {

  Transform GetArm() {
    return transform.Find("Root").Find("Arm");
  }

  private void RescaleHand() {
    Hand hand = GetLeapHand();

    Transform arm = GetArm();
    Transform wrist = arm.Find("Wrist");
    Finger finger = hand.Fingers[1];
    string finger_name = RiggedFinger.FINGER_NAMES[1];

    Vector3 leap_index_mcp = finger.JointPosition(Finger.FingerJoint.JOINT_MCP).ToUnityScaled();
    Vector3 leap_index_dip = finger.JointPosition(Finger.FingerJoint.JOINT_PIP).ToUnityScaled();
    Vector3 index0 = wrist.Find(finger_name + "A")
                          .Find(finger_name + "B")
                          .Find(finger_name + "C").localPosition;
    float scale = (leap_index_mcp - leap_index_dip).magnitude / index0.magnitude;
    arm.localScale = new Vector3(scale, scale, scale);
  }

  public override void InitHand(Transform deviceTransform) {
    UpdateHand(deviceTransform);
  }

  public override void UpdateHand(Transform deviceTransform) {
    RescaleHand();
    Hand hand = GetLeapHand();
    Transform arm = GetArm();

    arm.position = deviceTransform.TransformPoint(hand.PalmPosition.ToUnityScaled());
    arm.rotation = deviceTransform.rotation * hand.Basis().Rotation();

    for (int i = 0; i < fingers.Length; ++i) {
      if (fingers[i] != null)
        fingers[i].UpdateFinger(deviceTransform);
    }
  }
}
