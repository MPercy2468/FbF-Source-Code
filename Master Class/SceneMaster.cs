using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMaster : MonoBehaviour
{
    public int mainMenuBuildIndex;
    public int levelGeneratorSceneBuildIndex;
    public int levelPrepSceneBuildIndex;
    public int deathSceneBuildIndex;
    public int chipShopBuildIndex;
    public int chipEquipBuildIndex;
    public int sectorSelectBuildIndex;

    [HideInInspector]
    public bool isChipEquipBack = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void LoadScene(int buildIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(buildIndex);
    }
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetSceneByName(sceneName).buildIndex);
    }
    public void RestartCurrentScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}
