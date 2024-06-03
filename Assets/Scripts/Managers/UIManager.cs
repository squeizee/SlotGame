using System;
using ScriptableObjects;
using SlotMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private SlotGameData slotGameData;
        [SerializeField] private SlotItemsData slotItemsData;

        [SerializeField] private SlotMachineUI slotMachineUI;

        [SerializeField] private Button spinButton;
        [SerializeField] private Button increaseBetButton;
        [SerializeField] private Button decreaseBetButton;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text betText;
        [SerializeField] private TMP_Text freeSpinText;
        [SerializeField] private TMP_Text spinButtonText;
        [SerializeField] private TMP_Text multiplierText;

        private void Start()
        {
            slotMachineUI.Initialize(slotGameData, slotItemsData);
            UpdateUI();

            increaseBetButton.onClick.AddListener(IncreaseBet);
            decreaseBetButton.onClick.AddListener(DecreaseBet);

            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            EventManager.Instance.OnGoldUpdated += UpdateGoldUI;
            EventManager.Instance.OnFreeSpinCountUpdated += UpdateFreeSpin;
            EventManager.Instance.OnGameStateChanged += UpdateButtons;
            EventManager.Instance.OnAutoSpinClicked += UpdateButtons;
            EventManager.Instance.OnMultiplierUpdated += UpdateMultiplier;
        }


        private void OnDisable()
        {
            EventManager.Instance.OnGoldUpdated -= UpdateGoldUI;
            EventManager.Instance.OnFreeSpinCountUpdated -= UpdateFreeSpin;
            EventManager.Instance.OnGameStateChanged -= UpdateButtons;
            EventManager.Instance.OnAutoSpinClicked -= UpdateButtons;
        }

        private void UpdateFreeSpin(int count)
        {
            var str = count > 0 ? $"Free Spin : {count}" : " ";

            freeSpinText.text = str;
        }

        private void UpdateMultiplier(int multiplier)
        {
            multiplierText.text = multiplier > 1 ? $"X{multiplier}" : " ";
        }

        private void UpdateUI()
        {
            goldText.text = slotGameData.goldAmount.ToString();

            if (slotGameData.betAmount < slotGameData.betChangeAmount)
                slotGameData.betAmount = slotGameData.betChangeAmount;

            betText.text = slotGameData.betAmount.ToString();
        }

        void UpdateButtons()
        {
            bool isInteractable = slotGameData.currentGameState == GameStates.Idle || !slotGameData.isAutoSpin;

            spinButtonText.text = slotGameData.isAutoSpin ? "STOP" : "SPIN";

            spinButton.interactable = isInteractable;
            increaseBetButton.interactable = isInteractable;
            decreaseBetButton.interactable = isInteractable;
        }

        void UpdateGoldUI()
        {
            goldText.text = slotGameData.goldAmount.ToString();
        }

        private void DecreaseBet()
        {
            if (slotGameData.betAmount <= slotGameData.betChangeAmount)
                return;

            slotGameData.betAmount -= slotGameData.betChangeAmount;
            UpdateUI();
        }

        private void IncreaseBet()
        {
            slotGameData.betAmount += slotGameData.betChangeAmount;
            UpdateUI();
        }
    }
}