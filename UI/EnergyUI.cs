
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private Player _Player = null;
    [SerializeField] private Image _EnergyBar = null;

    // Update is called once per frame
    private void Update()
    {
        _EnergyBar.fillAmount = (_Player.GetEnergyTotal() - _Player.GetEnergyUsed()) / _Player.GetEnergyTotal();
    }
}
