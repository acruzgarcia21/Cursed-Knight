using UnityEngine;
using UnityEngine.EventSystems;

public class ExhaustPileViewTrigger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardViewDisplay cardViewDisplay;
    
    private ExhaustManager _exhaustManager;
    
    private void Awake()
    {
        _exhaustManager = FindAnyObjectByType<ExhaustManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cardViewDisplay.SetTitleText("Exhaust Pile View");
        cardViewDisplay.SetCurrentViewerToExhaustPile();
        cardViewDisplay.DisplayCards(_exhaustManager.GetExhaustPile());
    }
}
