using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Portal : Gadgets
{

    [SerializeField] private GameObject _Portal = null;
    [SerializeField] private GameObject _Exit = null;
    [SerializeField] private AudioClip _portalExit;
    [SerializeField] private int _NumOfTimeCanBeUsed = 2;
    [SerializeField] private TextMeshProUGUI _Counter;
    [SerializeField] private Image _Image;

    private Vector3 _PlayerInitialLocation;
    private Vector3 _PlayerExitLocation;
    private bool _HasInnitial = false;
    private GameObject _ActivePortal;
    private GameObject _ExitPortal;

    private void SetInitialLocation()
    {
        _PlayerExitLocation = p.transform.position;
        _PlayerInitialLocation = new Vector3(p.transform.position.x, p.transform.position.y + .5f, p.transform.position.z + .5f);
    }

    public void OnPortalButtonClicked()
    {
        if (_NumOfTimeCanBeUsed > 0)
        {
            if (!_HasInnitial)
            {
                SetInitialLocation();
                SpawnPortal();
                _HasInnitial = true;
            }
            else if (_HasInnitial)
            {
                p.transform.position = _PlayerExitLocation;
                Destroy(_ActivePortal);
                DespawnPortal();
                p.UseActionPoint(_ActionPointNeeded);
                _HasInnitial = false;
                _NumOfTimeCanBeUsed--;
                _Counter.text = "" + _NumOfTimeCanBeUsed;
                if (_NumOfTimeCanBeUsed == 0)
                {
                    _Image.color = Color.red;
                }
            }
            p.UseEnergy(_EnergyNeeded);
        }
    }

    private void SpawnPortal()
    {
        _sfxGadgetSource.clip = _sfxGadgetClip;
        _sfxGadgetSource.Play();
        _ActivePortal = (GameObject)Instantiate(_Portal, _PlayerInitialLocation, Quaternion.identity);
        Debug.Log("Spawning");
    }

    private void DespawnPortal()
    {
        _sfxGadgetSource.clip = _portalExit;
        _sfxGadgetSource.Play();
        _ExitPortal = (GameObject)Instantiate(_Exit, _PlayerInitialLocation, Quaternion.identity);

    }

}
