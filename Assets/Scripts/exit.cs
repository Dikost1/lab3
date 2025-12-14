using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // Метод для кнопки "Выход"
    public void QuitGame()
    {
        Debug.Log("Выход из игры");

        // Если игра запущена как билд
        Application.Quit();

        // Если запущено в редакторе Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
