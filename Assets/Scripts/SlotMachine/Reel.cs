using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ScriptableObjects;
using UnityEngine;

namespace SlotMachine
{
    public class Reel : MonoBehaviour
    {
        [SerializeField] private int reelIndex;
        [SerializeField] private SlotItem slotItemPrefab;

        private SlotGameData _slotGameData;
        private SlotItemsData _slotItemsData;

        private List<SlotItem> _onHoldItems = new();
        private List<SlotItem> _slotItems = new();

        private float _yIncrement = -150;
        
        private int RowCount => _slotGameData.rowCount;

        private Sequence spinSequence;
        private Sequence dropSequence;

        public void Initialize(SlotGameData slotGameData, SlotItemsData slotItemsData)
        {
            _slotGameData = slotGameData;
            _slotItemsData = slotItemsData;
        }

        public void CreateSlotItems(int numberOfItems)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                var slotItem = Instantiate(slotItemPrefab, transform);
                SlotItemData slotItemData = _slotItemsData.GetRandomSlotItem();
                if (slotItemData.itemType == ItemTypes.Scatter)
                {
                    if (CheckScatter())
                    {
                        slotItemData = _slotItemsData.GetRandomSlotItem(false);
                    }
                }
                slotItem.Initialize(slotItemData.itemType, slotItemData.sprite, new Vector2Int(i, reelIndex),slotItemData.multiplierValue);
                _slotGameData.SetSlotItem(slotItemData.itemType, i, reelIndex);
                _slotItems.Add(slotItem);
            }

            MoveToTargetPositions();
        }
    
        public Sequence Spin()
        {
            float delay = 1.2f;

            spinSequence?.Kill();
            spinSequence = DOTween.Sequence();

            spinSequence = MoveToInitialPosition();

            spinSequence.InsertCallback(spinSequence.Duration(), RandomizeSlotItems);
            spinSequence.Insert(spinSequence.Duration() + delay, MoveToTargetPositions());

            return spinSequence;
        }

        private bool CheckScatter()
        {
            return _slotItems.Any(slotItem => slotItem.ItemType == ItemTypes.Scatter);
        }
        private Sequence MoveToInitialPosition()
        {
            float duration = 0.2f;
            float delay = 0.05f;
            float beginDelay = reelIndex * .2f;

            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(beginDelay);

            for (int i = 0; i < _slotItems.Count; i++)
            {
                // Move the slot item to the bottom of the reel
                sequence.Insert((i * delay) + beginDelay,
                    _slotItems[i].transform.DOLocalMoveY((RowCount + 1) * _yIncrement, duration));
                // Move the slot item to the top of the reel
                sequence.Insert((i * delay) + beginDelay + duration, _slotItems[i].transform.DOLocalMoveY(0, 0));
            }

            return sequence;
        }

        private Sequence MoveToTargetPositions()
        {
            float duration = 0.2f;
            float delay = 0.05f;
            float beginDelay = reelIndex * .25f;

            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(beginDelay);

            for (int i = 0; i < _slotItems.Count; i++)
            {
                sequence.Insert((i * delay) + beginDelay,
                    _slotItems[i].transform
                        .DOLocalMoveY((RowCount - _slotItems[i].Coordinate.x) * _yIncrement, duration));
            }

            return sequence;
        }

        private void RandomizeSlotItems()
        {
            for (int i = 0; i < _slotItems.Count; i++)
            {
                SlotItemData slotItemData = _slotItemsData.GetRandomSlotItem(isMultiplierAllowed:_slotGameData.isFreeSpin);
                if (slotItemData.itemType == ItemTypes.Scatter)
                {
                    if (CheckScatter())
                    {
                        slotItemData = _slotItemsData.GetRandomSlotItem(false);
                    }
                }
                _slotItems[i].Initialize(slotItemData.itemType, slotItemData.sprite, new Vector2Int(i, reelIndex),slotItemData.multiplierValue);
                _slotGameData.SetSlotItem(slotItemData.itemType, i, reelIndex);
            }
        }

        public void HighlightWinningItems(List<ItemTypes> winningTypes)
        {
            foreach (var slotItem in _slotItems)
            {
                if (winningTypes.Contains(slotItem.ItemType))
                {
                    slotItem.Highlight();
                    slotItem.Deactivate();
                    _onHoldItems.Add(slotItem);
                }
            }
            
            _slotItems = _slotItems.Except(_onHoldItems).ToList();
        }

        public Sequence DropItems()
        {
            float duration = 0.2f;
            float delay = 0.05f;

            dropSequence?.Kill();
            dropSequence = DOTween.Sequence();

            if (_onHoldItems.Count == 0) 
                return dropSequence;

            _slotItems.AddRange(_onHoldItems);
            _onHoldItems.Clear();
            
            for (int i = 0; i < _slotItems.Count; i++)
            {
                SlotItem slotItem = _slotItems[i];

                if (slotItem.isActive)
                {
                    _slotItems[i].SetCoordinate(new Vector2Int(i, reelIndex));
                    _slotGameData.SetSlotItem(_slotItems[i].ItemType, i, reelIndex);
                }
                else
                {
                    SlotItemData slotItemData = _slotItemsData.GetRandomSlotItem(isMultiplierAllowed:_slotGameData.isFreeSpin);
                    if (slotItemData.itemType == ItemTypes.Scatter)
                    {
                        if (CheckScatter())
                        {
                            slotItemData = _slotItemsData.GetRandomSlotItem(false);
                        }
                    }
                    slotItem.Initialize(slotItemData.itemType, slotItemData.sprite, new Vector2Int(i, reelIndex),slotItemData.multiplierValue);
                    _slotGameData.SetSlotItem(slotItemData.itemType, i, reelIndex);
                    
                    slotItem.Activate();
                }
                
                dropSequence.Insert((i * delay),
                    _slotItems[i].transform
                        .DOLocalMoveY((RowCount - _slotItems[i].Coordinate.x) * _yIncrement, duration));
            }

            return dropSequence;
        }

        public int GetMultiplierValue()
        {
            int multiplierValue = 1;
            foreach (var slotItem in _slotItems)
            {
                if (slotItem.ItemType == ItemTypes.Multiplier)
                {
                    multiplierValue += slotItem.MultiplierValue;
                }
            }

            return multiplierValue;
        }
    }
}