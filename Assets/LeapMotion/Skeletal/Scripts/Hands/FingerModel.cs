/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Interface for all fingers.
public abstract class FingerModel : MonoBehaviour {

  public const int NUM_BONES = 4;
  public const int NUM_JOINTS = 5;

  public Finger.FingerType fingerType = Finger.FingerType.TYPE_INDEX;

  private Hand hand_;
  private Finger finger_;

  // Returns the location of the given joint on the finger.
  protected Vector3 GetJointPosition(int joint) {
    if (joint >= NUM_BONES)
      return finger_.Bone((Bone.BoneType.TYPE_DISTAL)).NextJoint.ToUnityScaled();
    return finger_.Bone((Bone.BoneType)(joint)).PrevJoint.ToUnityScaled();
  }

  // Returns the center of the given bone on the finger.
  protected Vector3 GetBonePosition(int bone_type) {
    Bone bone = finger_.Bone((Bone.BoneType)(bone_type));
    return (bone.PrevJoint.ToUnityScaled() + bone.NextJoint.ToUnityScaled()) * 0.5f;
  }

  // Returns the direction the given bone is facing on the finger.
  protected Vector3 GetBoneDirection(int bone_type) {
    return finger_.Bone((Bone.BoneType)(bone_type)).Direction.ToUnity();
  }

  // Returns the rotation quaternion of the given bone.
  protected Quaternion GetBoneRotation(int bone_type) {
    return finger_.Bone((Bone.BoneType)(bone_type)).Basis.Rotation();
  }

  public void SetLeapHand(Hand hand) {
    hand_ = hand;
    finger_ = hand.Fingers[(int)fingerType];
  }

  public Hand GetLeapHand() { return hand_; }
  public Finger GetLeapFinger() { return finger_; }

  public abstract void InitFinger(Transform deviceTransform);

  public abstract void UpdateFinger(Transform deviceTransform);
}
