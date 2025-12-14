using UnityEngine;

public class TestTarget : MonoBehaviour
{
    void Start()
    {
        // Создаем тестовый куб для стрельбы
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 10);
        cube.transform.localScale = new Vector3(2, 2, 2);

        // Добавляем компонент для обработки попаданий
        cube.AddComponent<TargetHit>();

        Debug.Log("Test target cube created at position: " + cube.transform.position);
    }
}

public class TargetHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pellet") || other.name.Contains("Sphere"))
        {
            Debug.Log("Target hit by: " + other.name);
            // Меняем цвет куба при попадании
            GetComponent<Renderer>().material.color = Color.red;
        }
    }
}
