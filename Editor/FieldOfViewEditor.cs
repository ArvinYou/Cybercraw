using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fov = target as FieldOfView;
        if(fov.ShowGUI == false) return;
        
        var vAngle = fov.ViewAngle / 2f;
        Vector3 vieAngleA = fov.DirFromAngle(-vAngle, false);
        Vector3 vieAngleB = fov.DirFromAngle(vAngle, false);

        Handles.color = Color.magenta;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, vieAngleA, fov.ViewAngle, fov.ViewRadius);

        Handles.DrawLine(fov.transform.position, fov.transform.position + vieAngleA * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + vieAngleB * fov.ViewRadius);

        Handles.color = Color.red;
        foreach (var item in fov.FindVisibleTargets())
        {
            Handles.DrawLine(fov.transform.position, item.position);
        }
    }
}
