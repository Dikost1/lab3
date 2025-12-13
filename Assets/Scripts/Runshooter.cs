using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayShooter : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Скрываем курсор и блокируем его в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Проверяем нажатие левой кнопки мыши
        if (Input.GetMouseButtonDown(0)) {
            // Создаем луч из центра экрана
            Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
            Ray ray = cam.ScreenPointToRay(point);
            RaycastHit hit;

            // Если луч во что-то попал...
            if (Physics.Raycast(ray, out hit)) {
                // ...выводим имя объекта в консоль
                Debug.Log("Hit: " + hit.transform.name);
                // Запускаем корутину для отрисовки сферы в точке попадания
                StartCoroutine(SphereIndicator(hit.point));
            }
        }
    }

    // Корутина для временного отображения сферы в точке попадания
    private IEnumerator SphereIndicator(Vector3 pos)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // Маленькая сфера

        yield return new WaitForSeconds(1); // Ждем 1 секунду

        Destroy(sphere); // Уничтожаем сферу
    }

    // Отрисовка прицела в центре экрана (вызывается автоматически для GUI)
    void OnGUI()
    {
        int size = 12;
        float posX = cam.pixelWidth / 2 - size / 4;
        float posY = cam.pixelHeight / 2 - size / 2;
        // Рисуем белую точку в центре экрана
        GUI.Label(new Rect(posX, posY, size, size), "*");
    }
}