using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SlotGameData", menuName = "ScriptableObjects/SlotGameData")]
    public class SlotGameData : ScriptableObject
    {
        public GameStates currentGameState;
       
        public int rowCount = 5;
        public int colCount = 6;
        public int betAmount;
        public int betChangeAmount;
        public int goldAmount;
        public int lastPayout;
        
        public bool isAutoSpin;
        public bool isFreeSpin;
        
        
        private ItemTypes[,] _slotItems = new ItemTypes[5, 6];
        

        public void SetSlotItem(ItemTypes item, int row, int col)
        {
            _slotItems[row, col] = item;
        }

        public void SetGameState(GameStates gameState)
        {
            currentGameState = gameState;
            EventManager.Instance.GameStateChanged();
        }
        
        public Dictionary<ItemTypes,int> GetCountSlotItemsGroupByType()
        {
            var slotItemsGroupByType = _slotItems.Cast<ItemTypes>().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            
            return slotItemsGroupByType;
        }
        
        public bool HasMultiplierSymbol()
        {
            var slotItemsGroupByType = GetCountSlotItemsGroupByType();
            var multipliers = slotItemsGroupByType.Where(x => x.Key == ItemTypes.Multiplier).ToDictionary(x => x.Key, x => x.Value);

            return multipliers.Count >= 3;
        }
        
    }
    public enum GameStates
    {
        Idle = 0,
        Spinning = 1,
        Stopped = 2,
        Finished = 3,
    }
}