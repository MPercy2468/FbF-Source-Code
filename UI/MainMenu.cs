using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    CanvasGroup optionsWindow;
    OptionsMaster om;
    SceneMaster sm;
    private void Start()
    {
        om = GameObject.Find("_OptionsMaster").GetComponent<OptionsMaster>();
        sm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
    }
    private void Update()
    {
        if (InputHandler.GetAction(om.gc.UI.Cancel))
        {
            if(optionsWindow.alpha == 1)
            {
                ToggleOptionsMenu(false);
            }
        }
    }

    public void PlayButton()
    {
        sm.LoadScene(sm.chipEquipBuildIndex);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    public void ToggleOptionsMenu(bool input)
    {
        if (input)
        {
            optionsWindow.alpha = 1;
            optionsWindow.blocksRaycasts = true;
            optionsWindow.interactable = true;
        }
        else
        {
            optionsWindow.alpha = 0;
            optionsWindow.blocksRaycasts = false;
            optionsWindow.interactable = false;
        }
    }
}
