using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2,
    }

    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    [Header("Options")]
    public bool invertY = false;

    private float verticalRot = 0f;
    private float horizontalRot = 0f;

    void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.freezeRotation = true;
        }

        // Инициализируем углы из текущего поворота, чтобы не было "скачка"
        Vector3 euler = transform.localEulerAngles;
        horizontalRot = euler.y;
        verticalRot = euler.x;
        if (verticalRot > 180f)
        {
            verticalRot -= 360f;
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityHor;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityVert;
        if (invertY)
        {
            mouseY = -mouseY;
        }

        if (axes == RotationAxes.MouseX)
        {
            // Вращение вокруг оси Y (горизонтальное)
            horizontalRot += mouseX;
            transform.localRotation = Quaternion.Euler(0f, horizontalRot, 0f);
            return;
        }

        if (axes == RotationAxes.MouseY)
        {
            // Вращение вокруг оси X (вертикальное)
            verticalRot -= mouseY;
            verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);
            transform.localRotation = Quaternion.Euler(verticalRot, horizontalRot, 0f);
            return;
        }

        // Комбинированное вращение
        horizontalRot += mouseX;
        verticalRot -= mouseY;
        verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);
        transform.localRotation = Quaternion.Euler(verticalRot, horizontalRot, 0f);
    }
}
