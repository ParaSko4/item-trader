using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ItemTrader.Gameplay
{
    [RequireComponent(typeof(MovementSystem))]
    public class PlayerController : MonoBehaviour
    {
        public event Action CostChanged;

        [SerializeField]
        private float transferDuration;
        [SerializeField]
        private float oldItemFadeDuration;
        [SerializeField]
        private int itemCost;
        [SerializeField]
        private TextMeshPro priceText;

        private MovementSystem movementSystem;
        private Transform itemTransform;
        private bool allowMovement = true;

        public int ItemCost
        {
            get { return itemCost; }
            set
            {
                itemCost = value;

                string costString = value.ToString();

                if (Mathf.Abs(value) > 10000)
                {
                    costString = costString.Remove(costString.Length - 3, 3) + "K";
                }

                priceText.text = costString + " $";
            }
        }

        private void Awake()
        {
            movementSystem = GetComponent<MovementSystem>();
            itemTransform = transform.GetChild(0);

            ItemCost = itemCost;
        }

        private void Update()
        {
            if (allowMovement)
            {
                movementSystem.ForwardMovement();
                movementSystem.SideMovement();
            }
        }

        public void ChangeItem(Transform newItem, int newCost)
        {
            ItemCost = newCost;

            Destroy(itemTransform.gameObject, 3f);
            itemTransform.DOScale(Vector3.zero, oldItemFadeDuration);

            itemTransform.parent = null;
            itemTransform = newItem;
            itemTransform.parent = transform;

            itemTransform.DOLocalMove(Vector3.zero, transferDuration);
            itemTransform.DORotate(Vector3.zero, transferDuration);

            CostChanged?.Invoke();
        }

        public void GetDamage(int damageCost)
        {
            ItemCost -= damageCost;

            itemTransform.DOLocalRotate(new Vector3(360f, 0.0f, 0.0f), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);

            CostChanged?.Invoke();
        }

        public void SetFinishState()
        {
            priceText.DOFade(0f, 1f);
            priceText.transform.parent.GetComponent<SpriteRenderer>().DOFade(0f, 1f);

            allowMovement = false;
        }
    }
}
