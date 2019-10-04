
using UnityEngine;
using System.Collections;

public class TransparentObject : MonoBehaviour
{
    [SerializeField] private Material _OriginMat;
    [SerializeField] private Material _TransMat;

    

    public void ToTrans()
    {
        GetComponent<Renderer>().material = _TransMat;
        // Debug.Log("ToTrans() called");
    }

    public void ToOrigin()
    {
        // Debug.Log("ToOrigin() called");
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetComponent<Renderer>().material = _OriginMat;
    }
}
