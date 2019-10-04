using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerLoad : MonoBehaviour
{
    [SerializeField] private GameObject _Trigger;

    private MeshRenderer[] _ChildMesh;

    private void Awake()
    {
        _ChildMesh = GetComponentsInChildren<MeshRenderer>();
        HideUpperLevel();

    }

    public void ShowUpperLevel()
    {
        foreach (MeshRenderer mr in _ChildMesh)
        {
            mr.enabled = false;
        }
    }
    public void HideUpperLevel()
    {
        Debug.Log("Hidding!");
        foreach (MeshRenderer mr in _ChildMesh)
        {
            mr.enabled = true;
        }
    }
}
