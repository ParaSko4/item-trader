using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace ItemTrader.Gameplay
{
    public class PriceWall : MonoBehaviour
    {
        [SerializeField]
        private int[] prices;
        [SerializeField]
        private float duration;
        [SerializeField]
        private Vector3 scale;

        private TextMeshPro[] priceTexts;
        private Dictionary<int, Transform> pricesYCoordinates = new Dictionary<int, Transform>();

        private PlayerController playerController;

        [Inject]
        public void Construct(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        private void Awake()
        {
            priceTexts = GetComponentsInChildren<TextMeshPro>();
        }

        private void Start()
        {
            for (int i = 0; i < priceTexts.Length; i++)
            {
                priceTexts[i].text = GetFormattedText(prices[i]);
                pricesYCoordinates.Add(prices[i], priceTexts[i].transform.parent);
            }
        }

        public async void StartScale(int cost)
        {
            int key = pricesYCoordinates.Keys.OrderBy(x => Mathf.Abs(x - cost)).ElementAt(0);
            var scalePriceBlocks = new List<Transform>();
            Transform endBlock = null;

            foreach (var dicKey in pricesYCoordinates.Keys)
            {
                scalePriceBlocks.Add(pricesYCoordinates[dicKey].transform);

                if (dicKey == key)
                {
                    endBlock = pricesYCoordinates[dicKey].transform;

                    break;
                }
            }

            float stepDuration = duration / scalePriceBlocks.Count / 1.5f;

            playerController.transform.DOMoveY(endBlock.transform.position.y, duration);

            foreach (var scalePriceBlock in scalePriceBlocks)
            {
                var startScale = scalePriceBlock.transform.localScale;

                if (endBlock == scalePriceBlock)
                {
                    scalePriceBlock.DOScale(scale, stepDuration).SetDelay(stepDuration);
                    break;
                }

                await UniTask.WaitUntil(() => playerController.transform.position.y > scalePriceBlock.transform.position.y);

                DOTween.Sequence()
                    .Append(scalePriceBlock.DOScale(scale, stepDuration))
                    .Append(scalePriceBlock.DOScale(startScale, stepDuration));
            }
        }

        private string GetFormattedText(int price)
        {
            string priceString = price.ToString();

            if (Mathf.Abs(price) >= 10000)
            {
                priceString = priceString.Remove(priceString.Length - 3, 3) + "K";
            }

            return priceString + " $";
        }
    }
}
