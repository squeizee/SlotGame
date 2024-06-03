using DG.Tweening;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMachine
{
    public class SlotItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Image highLightImage;
        

        public ItemTypes ItemType { get; private set; }

        public Vector2Int Coordinate { get; private set; }
        public int MultiplierValue{ get; private set; }

        public bool isActive;
        public void Initialize(ItemTypes itemType, Sprite sprite, Vector2Int coordinate, int multiplier = 1)
        {
            MultiplierValue = multiplier;
            ItemType = itemType;
            image.sprite = sprite;
            Coordinate = coordinate;
            
            Activate();
        }
        
        public void SetCoordinate(Vector2Int coordinate)
        {
            Coordinate = coordinate;
        }
        
        public void Activate()
        {
            image.enabled = true;
            isActive = true;
        }
        public void Deactivate()
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                highLightImage.enabled = false;
                image.enabled = false;
                isActive = false;
                
                ResetYPosition();
                
            });
        }

        private void ResetYPosition()
        {
            var pos = transform.localPosition;
            pos.y = 0;
            
            transform.localPosition = pos;
        }
        public void Highlight()
        {
            highLightImage.enabled = true;
        }
    }
}