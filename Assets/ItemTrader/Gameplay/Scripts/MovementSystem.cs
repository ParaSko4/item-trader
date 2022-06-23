using System.Collections;
using UnityEngine;

namespace ItemTrader.Gameplay
{
    public class MovementSystem : MonoBehaviour
    {
        [SerializeField]
        private Transform border;
        [SerializeField]
        private float forwardSpeed;
        [SerializeField]
        private float sideSpeed;

        private float firstTouchXCoordinate;
        private float screenHalfWidthDividedBy100;

        private void Awake()
        {
            screenHalfWidthDividedBy100 = 100f / Screen.width / 2;
        }

        public void ForwardMovement()
        {
            transform.position += forwardSpeed * Time.deltaTime * Vector3.forward;
        }

        public void SideMovement()
        {
            float xCoordinate = transform.position.x;

            if (Input.GetMouseButtonDown(0))
            {
                firstTouchXCoordinate = Input.mousePosition.x;
            }
            else if (Input.GetMouseButton(0))
            {
                float touchXCoordinate = Input.mousePosition.x - firstTouchXCoordinate;
                float precent = Mathf.Sign(touchXCoordinate) * touchXCoordinate * screenHalfWidthDividedBy100;
                xCoordinate = (touchXCoordinate < 0 ? -border.position.x : border.position.x) * precent / 100f;
            }

            if (xCoordinate == transform.position.x)
            {
                return;
            }

            var newPosition = Time.deltaTime * sideSpeed * (xCoordinate < 0 ? -Vector3.right : Vector3.right) + transform.position;

            if (CheckWithinTheBorder(newPosition))
            {
                transform.position = newPosition;
            }
        }

        private bool CheckWithinTheBorder(Vector3 position)
        {
            return position.x < border.position.x && position.x > -border.position.x;
        }
    }
}