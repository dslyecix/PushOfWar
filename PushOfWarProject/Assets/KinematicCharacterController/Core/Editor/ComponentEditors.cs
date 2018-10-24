using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KinematicCharacterController
{
    [CustomEditor(typeof(Rigidbody))]
    public class RigidbodyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Rigidbody r = (target as Rigidbody);
            if (r.gameObject.GetComponent<KinematicCharacterMotor>() || r.gameObject.GetComponent<PhysicsMover>())
            {
                GUI.enabled = false;
                DrawDefaultInspector();
                GUI.enabled = true;
            }
            else
            {
                DrawDefaultInspector();
            }
        }
    }

    [CustomEditor(typeof(CapsuleCollider))]
    public class CapsuleColliderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CapsuleCollider c = (target as CapsuleCollider);
            if (c.gameObject.GetComponent<KinematicCharacterMotor>())
            {
                GUI.enabled = false;
                DrawDefaultInspector();
                GUI.enabled = true;
            }
            else
            {
                DrawDefaultInspector();
            }
        }
    }
}