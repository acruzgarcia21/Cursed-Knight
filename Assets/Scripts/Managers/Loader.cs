using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        GameplayScene
    }

    private static Scene _targetScene;

    public static void Load(Scene targetScene)
    {
        _targetScene = targetScene;
        SceneManager.LoadScene("Loading Scene");
    }

    public static void LoaderCallback()
    {
        var sceneName = _targetScene switch
        {
            Scene.MainMenuScene => "Main Menu Scene",
            Scene.GameplayScene => "Gameplay Scene",
            _ => throw new System.ArgumentOutOfRangeException()
        };

        SceneManager.LoadScene(sceneName);
    }
}
