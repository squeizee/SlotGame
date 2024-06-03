using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SlotItemsData", menuName = "ScriptableObjects/SlotItemsData")]
    public class SlotItemsData : ScriptableObject
    {
        public SlotItemData[] slotItems;
        
        public SlotItemData GetRandomSlotItem(bool isScatterAllowed = true, bool isMultiplierAllowed = false)
        {
            float totalProbability = 0;
            foreach (var slotItem in slotItems)
            {
                if (!isScatterAllowed && slotItem.itemType == ItemTypes.Scatter || !isMultiplierAllowed && slotItem.itemType == ItemTypes.Multiplier)
                {
                    continue;
                }
                totalProbability += slotItem.probability;
            }

            float randomValue = Random.Range(0, totalProbability);
            float currentProbability = 0;
            foreach (var slotItem in slotItems)
            {
                if (!isScatterAllowed && slotItem.itemType == ItemTypes.Scatter || !isMultiplierAllowed && slotItem.itemType == ItemTypes.Multiplier)
                {
                    continue;
                }
                currentProbability += slotItem.probability;
                if (randomValue <= currentProbability)
                {
                    return slotItem;
                }
            }

            return null;
        }


        public int GetPayoutValue(ItemTypes itemKey)
        {
            foreach (var slotItem in slotItems)
            {
                if (slotItem.itemType == itemKey)
                {
                    return slotItem.payoutValue;
                }
            }

            return 0;
        }
    }
    
    [System.Serializable]
    public class SlotItemData
    {
        public ItemTypes itemType;
        public Sprite sprite;
        public int payoutValue;
        [Range(0f, 1f)] public float probability;
        public int multiplierValue;
    }
    
    public enum ItemTypes
    {
        Cannon,
        Compass,
        Hook,
        ManPirate,
        Map,
        Parrot,
        Rum,
        Skull,
        WomanPirate,
        Scatter,
        Multiplier,
    }
}