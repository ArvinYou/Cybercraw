
using UnityEngine;

public class EntanceMiniGame : MonoBehaviour
{
    [SerializeField] private bool _IsVisited = false;//  make sure the minigame only played once

    public Light _Light;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !_IsVisited)
        {
            _IsVisited = true;
            UIScenesManager.ShowUI<MiniGame>();
            _Light.enabled = false;
        }// load the minigame scene when player enters the trigger 
    }
}


