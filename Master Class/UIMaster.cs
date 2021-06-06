using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//Class for controling and handling all UI interactions while the level is running
public class UIMaster : MonoBehaviour
{
    [HideInInspector]
    public enum UIStates
    {
        PlayerInfo,
        WeaponWheel,
        PauseMenu,
        ChipCustomizer,
        ChipCustomizerPrompt,
        UIPopup
    }
    public UIStates UIState;
    //Variables and references for UI Logic
    GameMaster gm;
    [HideInInspector]
    public StyleMeter sm;
    float GhostTargetValue;
    bool IsWaitingForHealthGhost;
    public EventSystem eventSystem;
    //Player info gameobject references
    [SerializeField]
    GameObject PlayerInfoPanel,HealthBar,GhostHealthBar,OverclockBar;
    [SerializeField]
    GameObject OverclockSelectionDiamond;
    //Ammo info gameobject references
    [SerializeField]
    GameObject AmmoCounter,WeaponDisplay,WeaponModDisplay,BallisticAmmoBar,EnergyAmmoBar,SpecialAmmoBar;
    [SerializeField]
    TextMeshProUGUI noAmmoText;
    //Style Meter gameobject references
    [SerializeField]
    Image styleMeterLetter,styleMeterBar;
    //Weaponwheel gameobject references
    [SerializeField]
    GameObject WeaponWheelPanel;
    [SerializeField]
    WeaponWheelSlot[] WeaponWheelSlots;
    float PreviousWeaponWheelAngle;
    //Dash gameobject references
    [SerializeField]
    GameObject DashIconPanel, DashIconPrefab;
    List<GameObject> DashIcons = new List<GameObject>();
    float DashIconAlpha = 0;
    float DashFadeoutCounter = 0, DashFadeoutTime = 1;
    //Pause Menu gameobject references
    [SerializeField]
    GameObject PauseMenuParent;
    //Chip Customization gameobject references
    [SerializeField]
    GameObject ChipCustomizerPanel;
    [SerializeField]
    GameObject chipInventoryParent;
    [SerializeField]
    GameObject chipInventorySlotPrefab;
    [SerializeField]
    GameObject chipEquipPromptPanel;
    [SerializeField]
    GameObject chipEquipPromptSlotParent;
    [SerializeField]
    GameObject chipEquipPromptSlotPrefab;
    [SerializeField]
    GameObject chipEquipDisplayParent;
    [SerializeField]
    GameObject chipEquipDisplayPrefab;
    [SerializeField]
    TextMeshProUGUI chipDescription;
    [SerializeField]
    TextMeshProUGUI chipUpgradeDescription;
    [SerializeField]
    Image chipBigImage;
    [SerializeField]
    TextMeshProUGUI chipBigName;
    [SerializeField]
    Button upgradeButton;
    //Style Meter References
    [SerializeField]
    CanvasGroup styleMeterGroup;
    //Dialogue Popup References
    [SerializeField]
    CanvasGroup dialogueGroup;
    [SerializeField]
    TextMeshProUGUI dialogueText;
    [SerializeField]
    Image dialogueImage;
    [SerializeField]
    TextMeshProUGUI dialogueNameText;
    //Tutorial Popup References
    public CanvasGroup tutorialGroup;
    public Image tutotialImage;
    public TextMeshProUGUI tutorialTitle;
    public TextMeshProUGUI tutorialText;

    private void Awake()
    {
        gm = gameObject.GetComponent<GameMaster>();
        sm = gameObject.GetComponent<StyleMeter>();
        InitializeDashIcons();
    }
    private void Start()
    {
        gm.gc.Player.WeaponWheel.started += Context => EnableWeaponWheel();
        gm.gc.Player.WeaponWheel.canceled += Context => DisableWeaponWheel();
        gm.gc.UI.Pause.performed += Context => EscapePressed();
    }
    //Change this later you broke hoe
    //This shit is gross
    private void Update()
    {
        if(UIState == UIStates.PlayerInfo)
        {
            UpdatePlayerInfoUI();
            UpdateAmmoUI();
            UpdateStyleUI();
            UpdateDashIcons();
        }
        else if(UIState == UIStates.WeaponWheel)
        {
            RunWeaponWheel();
        }

        if(UIState == UIStates.PlayerInfo&&!gm.Player.es.IsDead)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }
    //Player info methods
    void UpdatePlayerInfoUI()
    {
        //Update Player health
        EntityStats PlayerES = gm.Player.GetComponent<EntityStats>();
        float HealthFill = PlayerES.EntityHealth / PlayerES.EntityHealthMax;
        GhostTargetValue = HealthFill;
        HealthBar.GetComponent<Image>().fillAmount = HealthFill;
        if (!IsWaitingForHealthGhost)
        {
            IsWaitingForHealthGhost = true;
            StartCoroutine(RunHealthGhost());
        }
        //Update Player Overclock
        float OverclockFill = gm.Player.overclock.GetCurrentOverclockCharge() / gm.Player.overclock.GetMaxOverclockCharge();
        OverclockBar.GetComponent<Image>().fillAmount = OverclockFill;
        //Update Overwatch Selection
        Image Diamond = OverclockSelectionDiamond.GetComponent<Image>();
        if (gm.Player.overclock.GetIsOverClockActive())
        {
            OverclockSelectionDiamond.SetActive(true);
        }
        else
        {
            OverclockSelectionDiamond.SetActive(false);
        }
    }
    void UpdateAmmoUI()
    {
        //Weapon display and ammo counter
        AmmoCounter.GetComponent<TextMeshProUGUI>().text = gm.GetAmmo(gm.ActivePlayerWeapon.AmmoType).ToString();
        WeaponDisplay.GetComponent<Image>().sprite = gm.ActivePlayerWeapon.DisplaySprite;
        if (gm.ActivePlayerWeapon.AltFire)
        {
            WeaponModDisplay.GetComponent<Image>().sprite = gm.ActivePlayerWeapon.secondaryChip.chipSprite;
        }
        else
        {
            WeaponModDisplay.GetComponent<Image>().sprite = gm.ActivePlayerWeapon.primaryChip.chipSprite;
        }
        //Ammo bars
        BallisticAmmoBar.GetComponent<Image>().fillAmount = (float)gm.GetAmmo(GameMaster.AmmoTypes.Ballistic) / gm.BallistcAmmoMax;
        EnergyAmmoBar.GetComponent<Image>().fillAmount = (float)gm.GetAmmo(GameMaster.AmmoTypes.Energy) / gm.EnergyAmmoMax;
        SpecialAmmoBar.GetComponent<Image>().fillAmount = (float)gm.GetAmmo(GameMaster.AmmoTypes.Special) / gm.SpecialAmmoMax;
        //Not enough ammo check
        if (gm.ActivePlayerWeapon.AltFire)
        {
            if (gm.CheckAmmo(gm.ActivePlayerWeapon.secondaryChip.ammoCost, gm.ActivePlayerWeapon.AmmoType) < 0)
            {
                noAmmoText.enabled = true;
            }
            else
            {
                noAmmoText.enabled = false;
            }
        }
        else
        {
            if (gm.CheckAmmo(gm.ActivePlayerWeapon.primaryChip.ammoCost, gm.ActivePlayerWeapon.AmmoType) < 0)
            {
                noAmmoText.enabled = true;
            }
            else
            {
                noAmmoText.enabled = false;
            }
        }
    }
    void UpdateStyleUI()
    {
        styleMeterBar.fillAmount = sm.GetStyleScoreRatio();
        styleMeterLetter.sprite = sm.GetStyleRankSprite();
    }

    IEnumerator RunHealthGhost()
    {
        for (float i = 0; i < 1;)
        {
            GhostHealthBar.GetComponent<Image>().fillAmount = Mathf.Lerp(GhostHealthBar.GetComponent<Image>().fillAmount, GhostTargetValue, 0.1f);
            i += 1 * Time.deltaTime;
            yield return null;
        }
        GhostHealthBar.GetComponent<Image>().fillAmount = GhostTargetValue;
        GhostTargetValue = 0;
        IsWaitingForHealthGhost = false;
    }

    //Weapon wheel methods
    void EnableWeaponWheel()
    {
        if(UIState != UIStates.PlayerInfo)
        {
            return;
        }
        UIState = UIStates.WeaponWheel;
        WeaponWheelPanel.SetActive(true);
        Time.timeScale = 0.5f;
        gm.Player.es.rb.velocity = Vector2.zero;
    }

    void DisableWeaponWheel()
    {
        if (UIState == UIStates.PauseMenu)
        {
            return;
        }
        UIState = UIStates.PlayerInfo;
        WeaponWheelPanel.SetActive(false);
        Time.timeScale = 1;
    }

    void RunWeaponWheel()
    {
        Vector2 SelectionDir = Vector2.zero;
        if (InputHandler.CurrentDevice == InputHandler.Devices.Gamepad)
        {
            SelectionDir = gm.gc.UI.UIMovement.ReadValue<Vector2>();
        }
        else
        {
            SelectionDir = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            SelectionDir = (SelectionDir - ScreenCenter).normalized;
        }
        float SelectionAngle = Mathf.Atan2(SelectionDir.y, SelectionDir.x);
        SelectionAngle = Mathf.Rad2Deg * SelectionAngle;
        if(SelectionAngle < 0)
        {
            SelectionAngle = 360 - Mathf.Abs(SelectionAngle);
        }
        if(SelectionAngle == PreviousWeaponWheelAngle)
        {
            return;
        }
        foreach(WeaponWheelSlot w in WeaponWheelSlots)
        {
            if (w.CheckSelectionAngle(SelectionAngle))
            {
                gm.EquipWeapon(w.GetWeaponIndex());
            }
        }
        PreviousWeaponWheelAngle = SelectionAngle;
    }

    void InitializeDashIcons()
    {
        for(int i = 0; i < gm.Player.MaxDashes; i++)
        {
            GameObject Icon = Instantiate(DashIconPrefab, DashIconPanel.transform);
            DashIcons.Add(Icon);
        }
    }

    void UpdateDashIcons()
    {
        if (gm.Player.CurrentDashes < DashIcons.Count)
        {
            DashFadeoutCounter = 0;
            DashIcons[gm.Player.CurrentDashes].GetComponent<Animator>().SetBool("IsEmpty", true);
            foreach(GameObject g in DashIcons)
            {
                DashIconAlpha = 255;
                g.GetComponent<Image>().color = new Color(255, 255, 255, DashIconAlpha);
            }
        }
        else
        {
            foreach (GameObject g in DashIcons)
            {
                g.GetComponent<Animator>().SetBool("IsEmpty", false);
            }
            if (DashFadeoutCounter >= DashFadeoutTime)
            {
                foreach (GameObject g in DashIcons)
                {
                    DashIconAlpha = Mathf.Lerp(DashIconAlpha, 0, 5 * Time.deltaTime);
                    g.GetComponent<Image>().color = new Color(255, 255, 255, DashIconAlpha);
                }
            }
            else
            {
                DashFadeoutCounter += Time.deltaTime;
            }
        }
    }

    void EscapePressed()
    {
        if(UIState == UIStates.ChipCustomizer)
        {
            CloseChipCustomizer();
        }
        else if(UIState == UIStates.ChipCustomizerPrompt)
        {
            CancelChipEquipPrompt();
        }
        else if(UIState == UIStates.UIPopup)
        {

        }
        else
        {
            TogglePauseMode();
        }
    }

    public void TogglePauseMode()
    {
        if(UIState != UIStates.PauseMenu)
        {
            Time.timeScale = 0;
            PauseMenuParent.SetActive(true);
            UIState = UIStates.PauseMenu;
        }
        else
        {
            Time.timeScale = 1;
            PauseMenuParent.SetActive(false);
            WeaponWheelPanel.SetActive(false);
            ChipCustomizerPanel.SetActive(false);
            UIState = UIStates.PlayerInfo;
        }
    }

    public void OpenChipCustomizer()
    {
        if(UIState == UIStates.PauseMenu)
        {
            return;
        }
        gm.gc.Player.Disable();
        gm.Player.es.ac.PlaySound("UIAccept", AudioMaster.SoundTypes.SFX, false, false);
        UIState = UIStates.ChipCustomizer;
        ChipCustomizerPanel.SetActive(true);
        PauseMenuParent.SetActive(false);
        WeaponWheelPanel.SetActive(false);
    }

    public void CloseChipCustomizer()
    {
        gm.Player.es.ac.PlaySound("UIDecline", AudioMaster.SoundTypes.SFX, false, false);
        gm.gc.Player.Enable();
        UIState = UIStates.PlayerInfo;
        ChipCustomizerPanel.SetActive(false);
    }

    public void StartChipEquipPrompt()
    {
        UIState = UIStates.ChipCustomizerPrompt;
        chipEquipPromptPanel.SetActive(true);
    }
    public void CancelChipEquipPrompt()
    {
        UIState = UIStates.ChipCustomizer;
        chipEquipPromptPanel.SetActive(false);
    }
    public GameObject GetChipInventorySlotPrefeb()
    {
        return chipInventorySlotPrefab;
    }
    public GameObject GetChipInventoryParent()
    {
        return chipInventoryParent;
    }
    public GameObject GetChipEquipPromptSlotPrefab()
    {
        return chipEquipPromptSlotPrefab;
    }
    public GameObject GetChipEquipPromptSlotParent()
    {
        return chipEquipPromptSlotParent;
    }
    public GameObject GetChipDisplayParent()
    {
        return chipEquipDisplayParent;
    }
    public GameObject GetChipEquipDisplayPrefab()
    {
        return chipEquipDisplayPrefab;
    }
    public TextMeshProUGUI GetDialogueText()
    {
        return dialogueText;
    }
    public TextMeshProUGUI GetDialogueNameText()
    {
        return dialogueNameText;
    }
    public Image GetDialogueImage()
    {
        return dialogueImage;
    }
    public CanvasGroup GetDialogueCanvasGroup()
    {
        return dialogueGroup;
    }
    public TextMeshProUGUI GetChipDescription()
    {
        return chipDescription;
    }
    public Image GetChipBigImage()
    {
        return chipBigImage;
    }
    public TextMeshProUGUI GetChipBigName()
    {
        return chipBigName;
    }
    public TextMeshProUGUI GetChipUpgradeDescription()
    {
        return chipUpgradeDescription;
    }
    public CanvasGroup GetStyleMeterCanvasGroup()
    {
        return styleMeterGroup;
    }
    public Button GetUpgradeButton()
    {
        return upgradeButton;
    }
}
