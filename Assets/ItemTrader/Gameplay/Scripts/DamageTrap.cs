using DG.Tweening;
using UnityEngine;
using Zenject;

namespace ItemTrader.Gameplay
{
    public class DamageTrap : MonoBehaviour
    {
        [SerializeField]
        private float rotationDuration;
        [SerializeField]
        private int damageCost;
        [SerializeField]
        private ParticleSystem damageEffectPrefab;

        private PlayerController playerController;

        [Inject]
        public void Construct(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        private void Start()
        {
            transform.DOLocalRotate(new Vector3(360f, 0.0f, 0.0f), rotationDuration, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == playerController.gameObject)
            {
                if (damageEffectPrefab != null)
                {
                    Instantiate(damageEffectPrefab, transform.position, Quaternion.identity).Play();
                }

                playerController.GetDamage(damageCost);
            }
        }
    }
}