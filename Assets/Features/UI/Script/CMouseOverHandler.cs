using UnityEngine;
using UnityEngine.EventSystems;

public class CMouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CUnitSelectionController.Instance.IsMouseOverUi = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CUnitSelectionController.Instance.IsMouseOverUi = false;
    }
}
