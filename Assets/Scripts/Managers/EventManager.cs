using System;
using ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        [SerializeField] private SlotGameData slotGameData;

        public event Action OnSpinClicked;
        public event Action OnAutoSpinClicked;
        public event Action OnStopAutoSpinClicked;

        public event Action OnSpinFinished;
        public event Action OnGoldUpdated;
        public event Action<int> OnFreeSpinCountUpdated;
        public event Action<int> OnMultiplierUpdated;

        public event Action OnGameStateChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SpinClicked()
        {
            OnSpinClicked?.Invoke();
        }

        public void GoldUpdated()
        {
            OnGoldUpdated?.Invoke();
        }

        public void AutoSpinClicked()
        {
            OnAutoSpinClicked?.Invoke();
        }

        public void StopAutoSpinClicked()
        {
            OnStopAutoSpinClicked?.Invoke();
        }

        public void SpinFinished()
        {
            slotGameData.SetGameState(GameStates.Stopped);
            OnSpinFinished?.Invoke();
        }

        public void OnWin()
        {
            OnGoldUpdated?.Invoke();
        }

        public void FreeSpinCountUpdated(int freeSpinCount)
        {
            OnFreeSpinCountUpdated?.Invoke(freeSpinCount);
        }

        public void GameStateChanged()
        {
            OnGameStateChanged?.Invoke();
        }

        public void MultiplierUpdated(int multiplier)
        {
            OnMultiplierUpdated?.Invoke(multiplier);
        }
    }
}