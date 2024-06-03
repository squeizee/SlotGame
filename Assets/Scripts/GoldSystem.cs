using ScriptableObjects;
using UnityEngine;

public class GoldSystem : MonoBehaviour
{
    [SerializeField] private SlotGameData slotGameData;
    
    public int startingGold = 100;

    private int _currentGold;
    private const string PlayerPrefsKey = "GoldAmount";

    private void Start()
    {
        _currentGold = PlayerPrefs.GetInt(PlayerPrefsKey, startingGold);
        UpdateGold();
    }
    
    public void DeductSpinCost()
    {
        _currentGold -= slotGameData.betAmount;
        SaveGoldAmount();
        UpdateGold();
    }
    
    public void AddGold(int amount)
    {
        _currentGold += amount;
        SaveGoldAmount();
        UpdateGold();
    }
    
    private void SaveGoldAmount()
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, _currentGold);
        PlayerPrefs.Save(); 
    }
    
    private void UpdateGold()
    {
        slotGameData.goldAmount = _currentGold;
    }
}