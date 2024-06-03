using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoldHandler : MonoBehaviour
{
    [SerializeField] private SlotGameData slotGameData;
    
    private bool isHolding = false;
    private float holdTimeThreshold = 1.0f;
    private float holdTimer = 0.0f;
    
    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdTimeThreshold)
            {
                // Start auto-spin
                EventManager.Instance.AutoSpinClicked();
                isHolding = false; // Prevent from repeatedly starting auto-spin
            }
        }
    }

    public void OnPointerDown()
    {
        isHolding = true;
        holdTimer = 0.0f; // Reset the hold timer
    }

    public void OnPointerUp()
    {
        if (isHolding && holdTimer < holdTimeThreshold)
        {
            if (slotGameData.isAutoSpin)
            {
                // Stop auto-spin when button is released
                EventManager.Instance.StopAutoSpinClicked(); 
            }
            
            // If released before the threshold, it's a normal spin
            EventManager.Instance.SpinClicked();
        }
        isHolding = false; // Stop holding
        
    }
    
}