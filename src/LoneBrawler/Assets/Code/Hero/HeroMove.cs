// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using CodeBase.CameraLogic;
using UnityEngine;

namespace CodeBase.Hero
{
    public class HeroMove : MonoBehaviour
    {
        public CharacterController CharacterController;
        public float MovementSpeed = 4.0f;

        private Camera _camera;

        private void Awake()
        {

        }

        private void Start()
        {
            _camera = Camera.main;
            CameraFollow();
        }

        private void Update()
        {
            //Vector3 movementVector = Vector3.zero;

            //if (_inputService.Axis.sqrMagnitude > Constants.Epsilon)
            //{
            //    //Трансформируем экранныые координаты вектора в мировые
            //    movementVector = _camera.transform.TransformDirection(_inputService.Axis);
            //    movementVector.y = 0;
            //    movementVector.Normalize();

            //    transform.forward = movementVector;
            //}

            //movementVector += Physics.gravity;

            //CharacterController.Move(MovementSpeed * movementVector * Time.deltaTime);
        }

        private void CameraFollow() => _camera.GetComponent<CameraFollow>().Follow(gameObject);
    }
}