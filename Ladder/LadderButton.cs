
using UnityEngine;

public class LadderButton : MonoBehaviour
{
    [SerializeField] private Ladder _LadderAttached = null;

    private void OnMouseDown()
    {
        _LadderAttached.ButtonClicked();
    }
}
