using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace ItemTrader.Gameplay
{
    public class SceneBehaviour : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider finish;
        [SerializeField]
        private Transform endPosition;
        [SerializeField]
        private float playerToEndPositionDuration;
        [SerializeField]
        private CinemachineVirtualCamera endCameraView;
        [SerializeField]
        private PriceWall priceWall;

        private PlayerController playerController;

        [Inject]
        public void Construct(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        private void Awake()
        {
            finish.OnTriggerEnterAsObservable()
                .Subscribe(OnPlayerFinished)
                .AddTo(this);
        }

        private async void OnPlayerFinished(Collider collider)
        {
            playerController.SetFinishState();

            endCameraView.Priority = 11;

            await playerController.transform.DOMove(endPosition.position, playerToEndPositionDuration).AsyncWaitForCompletion();
            endPosition.parent = playerController.transform;
            priceWall.StartScale(playerController.ItemCost);
        }
    }
}