using System;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace ItemTrader.Gameplay
{
    public class Gate : MonoBehaviour
    {
        [SerializeField]
        private Transform newItem;
        [SerializeField]
        private float rotationDuration;
        [SerializeField]
        private float updateTextDuration;
        [SerializeField]
        private MeshRenderer transparentWall;
        [Header("Movement")]
        [SerializeField]
        private bool moveSideToSide;
        [SerializeField]
        private float speed;
        [SerializeField]
        private Transform border;
        [Header("Cost")]
        [SerializeField]
        private int cost;
        [SerializeField]
        private TextMeshPro costText;
        [SerializeField]
        private Color coloCostText;
        [Header("VFX")]
        [SerializeField]
        private GameObject itemTakenEffectPrefab;
        [SerializeField]
        private ParticleSystem sadEffectPrefab;

        private Tween rotateTween;
        private PlayerController playerController;
        private bool toRight;

        [Inject]
        public void Contruct(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        private void Awake()
        {
            playerController.CostChanged += OnCostChanged;
        }

        private void Start()
        {
            rotateTween = newItem.DOLocalRotate(new Vector3(0.0f, 360f, 0.0f), rotationDuration, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);

            UpdateCost();
        }

        private void OnDestroy()
        {
            playerController.CostChanged -= OnCostChanged;
        }

        private void Update()
        {
            SideMovement();
        }

        private void SideMovement()
        {
            if (moveSideToSide == false)
            {
                return;
            }

            var newPosition = speed * Time.deltaTime * (toRight ? Vector3.right : -Vector3.right) + transform.position;

            if (CheckWithinTheBorder(newPosition) == false)
            {
                toRight = !toRight;
            }
            else
            {
                transform.position = newPosition;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == playerController.gameObject)
            {
                playerController.CostChanged -= OnCostChanged;

                rotateTween?.Kill();

                var effectInstans = Instantiate(itemTakenEffectPrefab, newItem.position, Quaternion.identity);
                effectInstans.GetComponent<ParticleSystem>().Play();

                if (sadEffectPrefab != null)
                {
                    sadEffectPrefab.Play();
                }

                playerController.ChangeItem(newItem, cost);
                transparentWall.material.DOColor(Color.grey, updateTextDuration);
                costText.text = "0 $";
                DOVirtual.Color(costText.outlineColor, Color.grey, updateTextDuration, (newColor) =>
                {
                    costText.outlineColor = newColor;
                }).OnComplete(() =>
                {
                    this.enabled = false;
                });
            }
        }

        private bool CheckWithinTheBorder(Vector3 position)
        {
            return position.x < border.position.x && position.x > -border.position.x;
        }

        private void UpdateCost()
        {
            int displayCost = cost - playerController.ItemCost;
            Color color = displayCost < 0 ? Color.red : Color.green;

            costText.text = GetFormattedText(displayCost);
            transparentWall.material.DOColor(color, updateTextDuration);
            DOVirtual.Color(costText.outlineColor, displayCost < 0 ? Color.red : coloCostText, updateTextDuration, (newColor) =>
            {
                costText.outlineColor = newColor;
            });
        }

        private void OnCostChanged()
        {
            UpdateCost();
        }

        private string GetFormattedText(int cost)
        {
            string costString = cost.ToString();

            if (Mathf.Abs(cost) > 10000)
            {
                costString = costString.Remove(costString.Length - 3, 3) + "K";
            }

            return (cost < 0 ? "" : "+") + costString + " $";
        }
    }
}