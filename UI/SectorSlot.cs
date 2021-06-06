using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SectorSlot : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField]
    int sectorIndex;
    SectorSelector ss;
    private void Start()
    {
        ss = GameObject.Find("_SectorSelector").GetComponent<SectorSelector>();
    }
    public void SlotPressed()
    {
        ss.LoadSector(sectorIndex);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ss.DisplayScores(sectorIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(eventData.selectedObject);
        ss.eventSystem.SetSelectedGameObject(eventData.selectedObject);
        ss.DisplayScores(sectorIndex);
    }
}
