/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

public class RiggedFinger : FingerModel {

  public static readonly string[] FINGER_NAMES = {"Thumb", "Index", "Middle", "Ring", "Pinky"};
  
  public override void InitFinger(Transform deviceTransform) {
    UpdateFinger(deviceTransform);
  }

  public override void UpdateFinger(Transform deviceTransform) {
    Transform mcp = transform.Find(FINGER_NAMES[(int)fingerType] + "B");
    Transform pip = mcp.Find(FINGER_NAMES[(int)fingerType] + "C");
    Transform dip = pip.Find(FINGER_NAMES[(int)fingerType] + "D");

    mcp.rotation = deviceTransform.rotation *
                   GetBoneRotation((int)Bone.BoneType.TYPE_PROXIMAL);
    pip.rotation = deviceTransform.rotation *
                   GetBoneRotation((int)Bone.BoneType.TYPE_INTERMEDIATE);
    dip.rotation = deviceTransform.rotation *
                   GetBoneRotation((int)Bone.BoneType.TYPE_DISTAL);
  }
}
