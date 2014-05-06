/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// The finger model for our skeletal hand made out of various polyhedra.
public class SkeletalFinger : FingerModel {

  public Transform[] bones = new Transform[NUM_BONES];

  private void SetPositions(Transform deviceTransform) {
    for (int i = 0; i < bones.Length; ++i) {
      if (bones[i] != null) {
        // Set position.
        bones[i].transform.position = deviceTransform.TransformPoint(GetBonePosition(i));
        
        // Set rotation.
        bones[i].transform.rotation = deviceTransform.rotation * 
                                      GetLeapFinger().Bone((Bone.BoneType)(i)).Basis.Rotation();
      }
    }
  }

  public override void InitFinger(Transform deviceTransform) {
    SetPositions(deviceTransform);
  }

  public override void UpdateFinger(Transform deviceTransform) {
    SetPositions(deviceTransform);
  }
}
