

using UnityEngine;

public class TransTest : MonoBehaviour
{
    public GameObject tmp;
    private bool _IsActive = false;

    private void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            ChangeTrans();
        }
    }

    private void ChangeTrans()
    {
        if (!_IsActive)
        {
            tmp.SetActive(false);
        }
        else
        {
            tmp.SetActive(true);
        }
        _IsActive = !_IsActive;

    }
}
