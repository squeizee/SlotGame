using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using ScriptableObjects;
using UnityEngine;

namespace SlotMachine
{
    public class SlotMachineUI : MonoBehaviour
    {
        private SlotGameData _slotGameData;
        private SlotItemsData _slotItemsData;
        [SerializeField] private List<Reel> reelsList;

        public void Initialize(SlotGameData slotGameData, SlotItemsData slotItemsData)
        {
            _slotGameData = slotGameData;
            _slotItemsData = slotItemsData;

            foreach (var reel in reelsList)
            {
                reel.Initialize(_slotGameData, _slotItemsData);
                reel.CreateSlotItems(slotGameData.rowCount);
            }

            SubscribeEvents();
        }


        private void SubscribeEvents()
        {
        }

        public void RollTheReels()
        {
            List<Sequence> spinSequences = new List<Sequence>();

            foreach (var reel in reelsList)
            {
                spinSequences.Add(reel.Spin());
            }

            spinSequences[^1].OnComplete(() => { EventManager.Instance.SpinFinished(); });
        }

        public void HighlightWinningItems(List<ItemTypes> winningTypes)
        {
            foreach (var reel in reelsList)
            {
                reel.HighlightWinningItems(winningTypes);
            }
        }

        public void DropSlotItems()
        {
            List<Sequence> dropSequences = new List<Sequence>();

            foreach (var reel in reelsList)
            {
                dropSequences.Add(reel.DropItems());
            }
        }
        
        public int GetMultiplierValue()
        {
            List<int> multiplierValues = new List<int>();

            foreach (var reel in reelsList)
            {
                multiplierValues.Add(reel.GetMultiplierValue());
            }

            return multiplierValues.Sum();
        }
    }
}