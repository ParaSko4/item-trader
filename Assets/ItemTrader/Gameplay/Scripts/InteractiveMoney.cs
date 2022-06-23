using DG.Tweening;
using UnityEngine;
using Zenject;

namespace ItemTrader.Gameplay
{
    public class InteractiveMoney : MonoBehaviour
    {
        [SerializeField]
        private float rotationDuration;
        [SerializeField]
        private float fadeSpeed;
        [SerializeField]
        private GameObject moneyTextPrefab;
        [SerializeField]
        private float timeBeforeTextFade;

        private Tween rotationTween;
        private PlayerController playerController;

        [Inject]
        public void Construct(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        private void Start()
        {
            rotationTween = transform.DOLocalRotate(new Vector3(0.0f, 360f, 0.0f), rotationDuration, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerEnter(Collider other)
        {
            rotationTween.Kill();
            transform.DOScale(Vector3.zero, fadeSpeed);

            var moneyText = Instantiate(moneyTextPrefab, transform.position, Quaternion.identity);
            var moneyTextScale = moneyText.transform.localScale;
            moneyText.transform.localScale = Vector3.zero;

            playerController.ItemCost += 20;

            DOTween.Sequence()
                .Append(moneyText.transform.DOScale(moneyTextScale, fadeSpeed))
                .Append(moneyText.transform.DOScale(Vector3.zero, fadeSpeed).SetDelay(timeBeforeTextFade));
        }
    }
}