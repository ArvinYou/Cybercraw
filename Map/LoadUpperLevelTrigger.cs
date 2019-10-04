using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadUpperLevelTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _UpperLevel;
    private Renderer[] _ChildGameRenderer;
    private Collider[] _ChildGameCollider;

    private void OnTriggerEnter(Collider other)
    {
        HideUpperLevel();
    }

    private void OnTriggerExit(Collider other)
    {
        ShowUpperLevel();
    }

    private void Awake()
    {
        _ChildGameRenderer = _UpperLevel.GetComponentsInChildren<Renderer>();
        _ChildGameCollider = _UpperLevel.GetComponentsInChildren<Collider>();
        // Debug.Log("_ChildMesh.length: " + _ChildMesh.Length);
        // HideUpperLevel();
    }

    public void ShowUpperLevel()
    {
        foreach (Renderer rd in _ChildGameRenderer)
        {
            rd.enabled = true;
        }
        foreach (Collider cd in _ChildGameCollider)
        {
            cd.enabled = true;
        }
        // _UpperLevel.SetActive(true);
    }
    public void HideUpperLevel()
    {
        // Debug.Log("hidding");
        foreach (Collider cd in _ChildGameCollider)
        {
            cd.enabled = false;
        }
        foreach (Renderer rd in _ChildGameRenderer)
        {
            rd.enabled = false;
        }
        
        // _UpperLevel.SetActive(false);
    }
}
