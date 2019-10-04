using UnityEngine;
using TMPro;

public class Gadgets : MonoBehaviour
{
    [Header("Progamming Stuff")]
    [SerializeField] protected TextMeshProUGUI _TMP;
    [SerializeField] protected Player p = null;
    [SerializeField] protected AudioClip _sfxGadgetClip;
    [SerializeField] protected AudioClip _sfxUIClip;
    [SerializeField] protected AudioSource _sfxGadgetSource;

    [Header("Gadget Status")]
    
    [SerializeField] protected int _EnergyNeeded = 2;
    
    protected int _ActionPointNeeded = 1;
    protected bool _IsButtonClicked = false;

    public int GetEnergyNeeded()
    {
        return _EnergyNeeded;
    }

}
