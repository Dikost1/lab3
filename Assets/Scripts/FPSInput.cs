using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour
{
    public float speed = 4.0f;
    public float gravity = -9.8f;

    private CharacterController charController;

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;

        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed); // Ограничиваем диагональное движение
        movement.y = gravity; // Применяем гравитацию
        movement *= Time.deltaTime; // Делаем движение независимым от частоты кадров

        // Преобразуем движение из локальных в глобальные координаты
        movement = transform.TransformDirection(movement);
        charController.Move(movement);
    }
}