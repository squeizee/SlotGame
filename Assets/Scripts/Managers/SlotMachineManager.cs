using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ScriptableObjects;
using SlotMachine;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Managers
{
    public class SlotMachineManager : MonoBehaviour
    {
        [SerializeField] private SlotGameData slotGameData;
        [SerializeField] private SlotItemsData slotItemsData;
        [SerializeField] private SlotMachineUI slotMachineUI;
        [SerializeField] private GoldSystem goldSystem;
        [SerializeField] private int collectableItemCount;
        [SerializeField] private int scatterLimit;

        private int _freeSpinCount = 0;
        private bool _isSecondCheck;

        private void Start()
        {
            EventManager.Instance.OnSpinClicked += Spin;
            EventManager.Instance.OnAutoSpinClicked += StartAutoSpin;
            EventManager.Instance.OnStopAutoSpinClicked += StopAutoSpin;
            EventManager.Instance.OnSpinFinished += CheckBoard;

            slotGameData.lastPayout = 0;
            slotGameData.isAutoSpin = false;
        }


        private void OnDisable()
        {
            EventManager.Instance.OnSpinClicked -= Spin;
            EventManager.Instance.OnAutoSpinClicked -= StartAutoSpin;
            EventManager.Instance.OnStopAutoSpinClicked -= StopAutoSpin;
            EventManager.Instance.OnSpinFinished -= CheckBoard;
        }


        private void StartAutoSpin()
        {
            slotGameData.isAutoSpin = true;
            Spin();
        }

        private void Spin()
        {
            if (slotGameData.goldAmount < slotGameData.betAmount)
            {
                slotGameData.SetGameState(GameStates.Idle);
                slotGameData.isAutoSpin = false;
                return;
            }

            slotGameData.SetGameState(GameStates.Spinning);

            if (_freeSpinCount > 0)
            {
                _freeSpinCount--;

                if (_freeSpinCount == 0)
                {
                    slotGameData.isFreeSpin = false;
                    StopAutoSpin();
                }
                   
                
                EventManager.Instance.FreeSpinCountUpdated(_freeSpinCount);
            }
            else
            {
                goldSystem.DeductSpinCost();
            }

            EventManager.Instance.GoldUpdated();
            slotMachineUI.RollTheReels();
            
        }

        private void StopAutoSpin()
        {
            slotGameData.isAutoSpin = false;
        }

        private void CheckBoard()
        {
            var boardSummary = slotGameData.GetCountSlotItemsGroupByType();

            var scatterCount = boardSummary.ContainsKey(ItemTypes.Scatter) ? boardSummary[ItemTypes.Scatter] : 0;

            if (scatterCount >= scatterLimit && _freeSpinCount == 0)
            {
                _freeSpinCount = 10;
                slotGameData.isFreeSpin = true;
                EventManager.Instance.FreeSpinCountUpdated(_freeSpinCount);
                EventManager.Instance.AutoSpinClicked();
                return;
            }

            var winningTypes = boardSummary.Where(x => x.Value >= collectableItemCount).ToList();

            if (!winningTypes.Any())
            {
                CheckAutoSpin();
                return;
            }

            slotMachineUI.HighlightWinningItems(winningTypes.Select(x => x.Key).ToList());
            
            var sequence = DOVirtual.DelayedCall(2.3f, () => { slotMachineUI.DropSlotItems(); });

            sequence.OnComplete(() =>
            {
                int payout = 0;

                foreach (var item in winningTypes)
                {
                    payout += slotItemsData.GetPayoutValue(item.Key) * item.Value;
                }

                payout *= slotGameData.betAmount;

                if (slotGameData.isFreeSpin)
                {
                    var multiplierValue = slotMachineUI.GetMultiplierValue();
                    payout *= multiplierValue;
                    EventManager.Instance.MultiplierUpdated(multiplierValue);
                }

                if (payout > 0)
                {
                    goldSystem.AddGold(payout);
                    slotGameData.lastPayout = payout;
                    EventManager.Instance.OnWin();
                    if (!_isSecondCheck)
                    {
                        _isSecondCheck = true;
                        CheckBoard();
                    }
                    else
                    {
                        CheckAutoSpin();
                    }
                }
                else
                {
                    _isSecondCheck = false;
                }

                slotGameData.SetGameState(GameStates.Finished);

                CheckAutoSpin();
            });
        }

        private void CheckAutoSpin()
        {
            DOVirtual.DelayedCall(1.5f, () =>
            {
                if (slotGameData.isAutoSpin)
                {
                    Spin();
                }
            });
        }
    }
}