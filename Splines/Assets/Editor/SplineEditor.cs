using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PlasticGui;

[CustomEditor(typeof(BezierSpline))]
public class SplineEditor : Editor
{
    private BezierSpline spline;

    private void OnEnable()
    {
        spline = (BezierSpline)target;
    }

    //Scene View

    private void OnSceneGUI()
    {
        if (spline.knots == null)
        {
            return;
        }

        for (int i = 0; i < spline.knots.Count; i++)
        {
            BezierKnot knot = spline.knots[i];
            if (knot == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();

            Vector3 pos = knot.transform.position;
            Quaternion rot = knot.transform.rotation;
            Vector3 scale = knot.transform.localScale;

            switch (Tools.current)
            {
                case Tool.Move:
                    pos = Handles.PositionHandle(pos, rot);
                    break;
                case Tool.Rotate:
                    rot = Handles.RotationHandle(rot, pos);
                    break;
                case Tool.Scale:
                    scale = Handles.ScaleHandle(scale, pos, rot);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(knot.transform, "Edit Knot!");
                knot.transform.position = pos;
                knot.transform.rotation = rot;
                knot.transform.localScale = scale;

                EditorUtility.SetDirty(spline);
            }

            Handles.Label(knot.transform.position + Vector3.up * 0.3f, $"Knot {i}");
        }
    }

    //Inspector

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(4);

        if (GUILayout.Button("Add Knot", GUILayout.Height(28)))
        {
            AddKnot();
        }

        GUI.enabled = spline.knots.Count > 2;
        if (GUILayout.Button("Remove Last Knot", GUILayout.Height(28)))
        {
            RemoveLastKnot();
        }
        GUI.enabled = true;

        EditorGUILayout.Space(8);

        DrawDefaultInspector();
    }

    //Knot Management
    private void AddKnot()
    {
        Undo.RecordObject(spline, "Add Knot");

        Vector3 spawnPos = Vector3.right * 3f;
        if (spline.knots.Count > 0)
        {
            BezierKnot last = spline.knots[spline.knots.Count - 1];
            if (last != null)
            {
                spawnPos = last.transform.position + last.transform.right * 3f;
            }
        }

        //Create New Knot
        GameObject obj = new GameObject($"Knot_{spline.knots.Count}");
        obj.transform.position = spawnPos;
        obj.transform.SetParent(spline.transform);

        Undo.RegisterCreatedObjectUndo(obj, "Add Knot");

        BezierKnot knot = obj.AddComponent<BezierKnot>();
        spline.knots.Add(knot);

        EditorUtility.SetDirty(spline);
    }

    private void RemoveLastKnot()
    {
        Undo.RecordObject(spline, "Remove Knot");

        BezierKnot last = spline.knots[spline.knots.Count - 1];
        spline.knots.RemoveAt(spline.knots.Count - 1);

        if (last != null)
        {
            Undo.DestroyObjectImmediate(last.gameObject);
        }

        EditorUtility.SetDirty(spline);
    }
}
