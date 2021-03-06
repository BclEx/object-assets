﻿using OA.Core;
using UnityEngine;

namespace OA.Components
{
    public class FlyingCameraComponent : MonoBehaviour
    {
        public float slowSpeed = 10;
        public float normalSpeed = 20;
        public float fastSpeed = 200;

        public float mouseSensitivity = 3;
        public float minVerticalAngle = -90;
        public float maxVerticalAngle = 90;

        public float arrowKeyRotSpeedMultiplier = 120;

        new Rigidbody rigidbody;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            Rotate();
            if (rigidbody == null)
                Translate();
        }

        void FixedUpdate()
        {
            if (rigidbody != null)
                UpdateVelocity();
        }

        void Rotate()
        {
            var eulerAngles = transform.eulerAngles;
            eulerAngles.z = 0;
            // Make eulerAngles.x range from -180 to 180 so we can clamp it between a negative and positive angle.
            if (eulerAngles.x > 180)
                eulerAngles.x = eulerAngles.x - 360;
            var deltaMouse = mouseSensitivity * (new Vector2(InputManager.GetAxis("Mouse X"), InputManager.GetAxis("Mouse Y")));
            var arrowKeysDirection = CalculateArrowKeysDirection();
            deltaMouse.x += Time.deltaTime * (arrowKeyRotSpeedMultiplier * arrowKeysDirection.x);
            deltaMouse.y += Time.deltaTime * (arrowKeyRotSpeedMultiplier * arrowKeysDirection.y);
            eulerAngles.x = Mathf.Clamp(eulerAngles.x - deltaMouse.y, minVerticalAngle, maxVerticalAngle);
            eulerAngles.y = Mathf.Repeat(eulerAngles.y + deltaMouse.x, 360);
            transform.eulerAngles = eulerAngles;
        }

        void Translate()
        {
            transform.Translate(Time.deltaTime * CalculateLocalVelocity());
        }

        void UpdateVelocity()
        {
            rigidbody.velocity = transform.TransformVector(CalculateLocalVelocity());
        }

        Vector3 CalculateLocalMovementDirection()
        {
            // Calculate the local movement direction.
            var direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
            if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
            if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
            if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
            if (Input.GetKey(KeyCode.Q)) direction += Vector3.down;
            if (Input.GetKey(KeyCode.E)) direction += Vector3.up;
            return direction.normalized;
        }

        float CalculateSpeed()
        {
            if (Input.GetKey(KeyCode.LeftShift)) return fastSpeed;
            else if (Input.GetKey(KeyCode.LeftControl)) return slowSpeed;
            else return normalSpeed;
        }

        Vector3 CalculateLocalVelocity()
        {
            return CalculateSpeed() * CalculateLocalMovementDirection();
        }

        Vector2 CalculateArrowKeysDirection()
        {
            var arrowKeysDirection = Vector2.zero;
            if (Input.GetKey(KeyCode.UpArrow)) arrowKeysDirection += Vector2.up;
            if (Input.GetKey(KeyCode.LeftArrow)) arrowKeysDirection += Vector2.left;
            if (Input.GetKey(KeyCode.DownArrow)) arrowKeysDirection += Vector2.down;
            if (Input.GetKey(KeyCode.RightArrow)) arrowKeysDirection += Vector2.right;
            return arrowKeysDirection.normalized;
        }
    }
}