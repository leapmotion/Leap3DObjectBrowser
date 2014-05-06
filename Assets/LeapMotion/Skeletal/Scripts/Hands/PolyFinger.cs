/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Leap;

public class PolyFinger : FingerModel {

  const int MAX_SIDES = 30;
  const int TRIANGLE_INDICES_PER_QUAD = 6;
  const int VERTICES_PER_QUAD = 4;

  public Material material;
  public int sides = 4;
  public bool smoothNormals = false;
  public float startingAngle = 0.0f;
  public float[] widths = new float[NUM_JOINTS];
  
  private Mesh mesh_;
  private Vector3[] vertices_;
  private Vector3[] normals_;
  private Vector3[] joint_vertices_;

  private Mesh cap_mesh_;
  private Vector3[] cap_vertices_;

  void Update() {
    if (mesh_ == null)
      return;

    mesh_.vertices = vertices_;
    mesh_.RecalculateBounds();

    if (smoothNormals)
      mesh_.normals = normals_;
    else
      mesh_.RecalculateNormals();

    cap_mesh_.vertices = cap_vertices_;
    cap_mesh_.RecalculateBounds();
    cap_mesh_.RecalculateNormals();

    Graphics.DrawMesh(mesh_, Matrix4x4.identity, material, 0);
    if (widths[0] != 0 || widths[NUM_JOINTS - 1] != 0)
      Graphics.DrawMesh(cap_mesh_, Matrix4x4.identity, material, 0);
  }

  protected Quaternion GetJointRotation(int joint) {
    if (joint <= 0)
      return GetBoneRotation(joint);
    if (joint >= NUM_BONES)
      return GetBoneRotation(joint - 1);

    return Quaternion.Slerp(GetBoneRotation(joint - 1), GetBoneRotation(joint), 0.5f);
  }

  protected void InitJointVertices() {
    joint_vertices_ = new Vector3[sides];
    for (int s = 0; s < sides; ++s) {
      float angle = startingAngle + s * 360.0f / sides;
      joint_vertices_[s] = Quaternion.AngleAxis(angle, -Vector3.forward) * Vector3.up;
    }
  }

  protected void UpdateMesh(Transform deviceTransform) {
    int vertex_index = 0;

    for (int i = 0; i < NUM_BONES; ++i) {
      Vector3 joint_position = deviceTransform.TransformPoint(GetJointPosition(i));
      Vector3 next_joint_position = deviceTransform.TransformPoint(GetJointPosition(i + 1));
      Quaternion joint_rotation = deviceTransform.rotation * GetJointRotation(i);
      Quaternion next_joint_rotation = deviceTransform.rotation * GetJointRotation(i + 1);

      for (int s = 0; s < sides; ++s) {
        int next_side = (s + 1) % sides;

        if (smoothNormals) {
          Vector3 normal = joint_rotation * joint_vertices_[s];
          Vector3 next_normal = joint_rotation * joint_vertices_[next_side];

          normals_[vertex_index] = normals_[vertex_index + 2] = normal;
          normals_[vertex_index + 1] = normals_[vertex_index + 3] = next_normal;
        }

        Vector3 offset = joint_rotation * (widths[i] * joint_vertices_[s]);
        vertices_[vertex_index++] = joint_position + offset;

        offset = joint_rotation * (widths[i] * joint_vertices_[next_side]);
        vertices_[vertex_index++] = joint_position + offset;

        offset = next_joint_rotation * (widths[i + 1] * joint_vertices_[s]);
        vertices_[vertex_index++] = next_joint_position + offset;

        offset = next_joint_rotation * (widths[i + 1] * joint_vertices_[next_side]);
        vertices_[vertex_index++] = next_joint_position + offset;
      }
    }
  }

  protected void UpdateCapMesh(Transform deviceTransform) {
    Vector3 base_position = deviceTransform.TransformPoint(GetJointPosition(0));
    Vector3 tip_position = deviceTransform.TransformPoint(GetJointPosition(NUM_JOINTS - 1));
    Quaternion base_rotation = deviceTransform.rotation * GetJointRotation(0);
    Quaternion tip_rotation = deviceTransform.rotation * GetJointRotation(NUM_JOINTS - 1);

    for (int s = 0; s < sides; ++s) {
      cap_vertices_[s] = base_position + base_rotation * (widths[0] * joint_vertices_[s]);
      cap_vertices_[sides + s] = tip_position + tip_rotation *
                                 (widths[NUM_JOINTS - 1] * joint_vertices_[s]);
    }
  }

  protected void InitMesh() {
    mesh_ = new Mesh();

    int vertex_index = 0;
    vertices_ = mesh_.vertices;
    int num_vertices = VERTICES_PER_QUAD * sides * NUM_BONES;
    if (vertices_.Length != num_vertices)
      Array.Resize(ref vertices_, num_vertices);

    normals_ = mesh_.normals;
    if (normals_.Length != num_vertices)
      Array.Resize(ref normals_, num_vertices);

    Vector2[] uv = mesh_.uv;
    if (uv.Length != num_vertices)
      Array.Resize(ref uv, num_vertices);

    int triangle_index = 0;
    int[] triangles = mesh_.triangles;
    int num_triangles = TRIANGLE_INDICES_PER_QUAD * sides * NUM_BONES;
    if (triangles.Length != num_triangles)
      Array.Resize(ref triangles, num_triangles);

    for (int i = 0; i < NUM_BONES; ++i) {
      for (int s = 0; s < sides; ++s) {

        triangles[triangle_index++] = vertex_index;
        triangles[triangle_index++] = vertex_index + 1;
        triangles[triangle_index++] = vertex_index + 2;

        triangles[triangle_index++] = vertex_index + 3;
        triangles[triangle_index++] = vertex_index + 2;
        triangles[triangle_index++] = vertex_index + 1;

        uv[vertex_index] = new Vector3((1.0f * s) / sides, (1.0f * i) / NUM_BONES);
        uv[vertex_index + 1] = new Vector3((1.0f + s) / sides, (1.0f * i) / NUM_BONES);
        uv[vertex_index + 2] = new Vector3((1.0f * s) / sides, (1.0f + i) / NUM_BONES);
        uv[vertex_index + 3] = new Vector3((1.0f + s) / sides, (1.0f + i) / NUM_BONES);

        vertices_[vertex_index++] = new Vector3(0, 0, 0);
        vertices_[vertex_index++] = new Vector3(0, 0, 0);
        vertices_[vertex_index++] = new Vector3(0, 0, 0);
        vertices_[vertex_index++] = new Vector3(0, 0, 0);
      }
    }
    mesh_.vertices = vertices_;
    mesh_.normals = normals_;
    mesh_.uv = uv;
    mesh_.triangles = triangles;
  }

  protected void InitCaps() {
    cap_mesh_ = new Mesh();

    cap_vertices_ = cap_mesh_.vertices;
    int num_vertices = 2 * sides;
    if (num_vertices != cap_vertices_.Length)
      Array.Resize(ref cap_vertices_, num_vertices);

    Vector2[] uv = cap_mesh_.uv;
    if (uv.Length != num_vertices)
      Array.Resize(ref uv, num_vertices);

    int triangle_index = 0;
    int[] triangles = cap_mesh_.triangles;
    int num_triangles = 2 * 3 * (sides - 2);
    if (num_triangles != triangles.Length)
      Array.Resize(ref triangles, num_triangles);

    for (int i = 0; i < sides; ++i) {
      cap_vertices_[i] = new Vector3(0, 0, 0);
      cap_vertices_[i + sides] = new Vector3(0, 0, 0);
      uv[i] = 0.5f * joint_vertices_[i];
      uv[i] += new Vector2(0.5f, 0.5f);
      uv[i + sides] = 0.5f * joint_vertices_[i];
      uv[i + sides] += new Vector2(0.5f, 0.5f);
    }

    for (int i = 0; i < sides - 2; ++i) {
      triangles[triangle_index++] = 0;
      triangles[triangle_index++] = i + 2;
      triangles[triangle_index++] = i + 1;

      triangles[triangle_index++] = sides;
      triangles[triangle_index++] = sides + i + 1;
      triangles[triangle_index++] = sides + i + 2;
    }

    cap_mesh_.vertices = cap_vertices_;
    cap_mesh_.uv = uv;
    cap_mesh_.triangles = triangles;
  }

  public override void InitFinger(Transform deviceTransform) {
    InitJointVertices();
    InitCaps();
    InitMesh();

    UpdateFinger(deviceTransform);
  }

  public override void UpdateFinger(Transform deviceTransform) {
    UpdateMesh(deviceTransform);
    UpdateCapMesh(deviceTransform);
  }
}
