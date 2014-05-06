/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// The model for our skeletal hand made out of various polyhedra.
public class SkeletalHand : HandModel {

  protected const float PALM_CENTER_OFFSET = 21.0f;

  public GameObject palm;

  void Start() {
    IgnoreCollisionsWithSelf();
  }

  protected Vector3 GetPalmCenter() {
    Hand leap_hand = GetLeapHand();
    return (leap_hand.PalmPosition.ToUnityScaled() -
            leap_hand.Direction.ToUnityScaled() * PALM_CENTER_OFFSET);
  }

  private void SetPositions(Transform deviceTransform) {
    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null)
        fingers[f].InitFinger(deviceTransform);
    }

    if (palm != null) {
      Vector3 palm_center = deviceTransform.TransformPoint(GetPalmCenter());
      palm.transform.position = palm_center;

      Hand leap_hand = GetLeapHand();
      palm.transform.rotation = deviceTransform.rotation * leap_hand.Basis().Rotation();
    }
  }

  public override void InitHand(Transform deviceTransform) {
    SetPositions(deviceTransform);
  }

  public override void UpdateHand(Transform deviceTransform) {
    SetPositions(deviceTransform);
  }
}
