using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runshooter : MonoBehaviour
{
    private Camera cam;

    [Header("Shotgun Settings")]
    [SerializeField]
    private int pelletCount = 8; // Количество дробинок

    [SerializeField]
    private float spreadAngle = 10f; // Угол рассеивания в градусах

    [SerializeField]
    private float pelletSpeed = 20f; // Скорость дробинок

    [SerializeField]
    private float pelletLifetime = 15f; // Время жизни дробинок

    [Header("Pellet Persistence")]
    [SerializeField]
    private bool pelletsStayForever = true; // Если true — дробинки не исчезают сами

    [SerializeField]
    private bool destroyPelletOnHit = false; // Если true — дробинка исчезнет при столкновении

    void Start()
    {
        Debug.Log("Runshooter Start() called"); // Добавляем отладку

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError(
                "Camera component not found! Make sure this script is attached to a camera."
            );
        }
        else
        {
            Debug.Log("Camera found successfully");
        }

        // Скрываем курсор и блокируем его в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Runshooter initialization complete");
    }

    void Update()
    {
        // Проверяем нажатие левой кнопки мыши
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Fire button pressed!"); // Добавляем отладку
            ShootShotgun();
        }

        // Тест стрельбы клавишей Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed - test shot!"); // Добавляем отладку
            ShootShotgun();
        }
    }

    private void ShootShotgun()
    {
        Debug.Log("ShootShotgun called! Creating " + pelletCount + " pellets"); // Добавляем отладку

        // Создаем базовый луч из центра экрана
        Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
        Ray baseRay = cam.ScreenPointToRay(point);

        // Запускаем несколько дробинок в конусе
        for (int i = 0; i < pelletCount; i++)
        {
            // Создаем случайное направление в конусе рассеивания
            Vector3 spreadDirection = GetSpreadDirection(baseRay.direction, spreadAngle);

            // Создаем сферу-дробинку
            GameObject pellet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pellet.transform.position = cam.transform.position;
            pellet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Очень маленькая сфера

            // Добавляем Rigidbody для физики
            Rigidbody rb = pellet.AddComponent<Rigidbody>();
            rb.useGravity = false; // Отключаем гравитацию для прямолинейного полета
            rb.linearVelocity = spreadDirection * pelletSpeed;

            // Добавляем collider для обнаружения столкновений
            SphereCollider collider = pellet.GetComponent<SphereCollider>();
            collider.isTrigger = true; // Делаем триггером для кастомной обработки

            // Добавляем скрипт для обработки столкновений
            Pellet pelletScript = pellet.AddComponent<Pellet>();
            pelletScript.Initialize(pelletsStayForever, pelletLifetime, destroyPelletOnHit);

            // Запускаем проверку попадания
            StartCoroutine(CheckPelletHit(pellet, spreadDirection));
        }
    }

    private Vector3 GetSpreadDirection(Vector3 baseDirection, float spreadAngle)
    {
        // Получаем случайные углы в пределах spreadAngle
        float spreadX = Random.Range(-spreadAngle, spreadAngle);
        float spreadY = Random.Range(-spreadAngle, spreadAngle);

        // Создаем кватернион для поворота
        Quaternion spreadRotation = Quaternion.Euler(spreadX, spreadY, 0);

        // Поворачиваем базовое направление
        return spreadRotation * baseDirection;
    }

    private IEnumerator CheckPelletHit(GameObject pellet, Vector3 direction)
    {
        RaycastHit hit;
        Vector3 startPos = pellet.transform.position;
        float distance = pelletSpeed * pelletLifetime;

        // Проверяем луч на столкновение
        if (Physics.Raycast(startPos, direction, out hit, distance))
        {
            // Если попали в объект, перемещаем сферу к точке попадания
            pellet.transform.position = hit.point;
            Rigidbody rb = pellet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // Останавливаем движение
            }

            // Выводим информацию о попадании
            Debug.Log("Pellet hit: " + hit.transform.name + " at " + hit.point);
        }

        yield return null;
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

// Отдельный скрипт для управления дробинкой
public class Pellet : MonoBehaviour
{
    private bool stayForever;
    private float lifetime;
    private bool destroyOnHit;

    public void Initialize(bool stayForever, float lifetime, bool destroyOnHit)
    {
        this.stayForever = stayForever;
        this.lifetime = lifetime;
        this.destroyOnHit = destroyOnHit;

        if (!stayForever)
        {
            Destroy(gameObject, lifetime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Обработка столкновения с объектом
        Debug.Log("Pellet collided with: " + other.gameObject.name);

        // Можно добавить эффекты попадания здесь
        // Например: Instantiate(hitEffect, transform.position, Quaternion.identity);

        // Останавливаем дробинку при столкновении, чтобы она "осталась" на месте
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Опционально уничтожаем дробинку при столкновении
        if (destroyOnHit && !stayForever)
        {
            Destroy(gameObject);
        }
    }
}
