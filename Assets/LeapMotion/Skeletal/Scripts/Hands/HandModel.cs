/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Interface for all hands.
public abstract class HandModel : MonoBehaviour {

  public const int NUM_FINGERS = 5;

  public FingerModel[] fingers = new FingerModel[NUM_FINGERS];

  private Hand hand_ = null;

  public void SetLeapHand(Hand hand) {
    hand_ = hand;
    for (int i = 0; i < fingers.Length; ++i) {
      if (fingers[i] != null)
        fingers[i].SetLeapHand(hand_);
    }
  }

  public Hand GetLeapHand() { return hand_; }

  public abstract void InitHand(Transform deviceTransform);

  public abstract void UpdateHand(Transform deviceTransform);

  protected void IgnoreCollisionsWithSelf() {
    Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
    for (int i = 0; i < colliders.Length; ++i)
      for (int j = i + 1; j < colliders.Length; ++j)
        Physics.IgnoreCollision(colliders[i], colliders[j]);
  }
}
