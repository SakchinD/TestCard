using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class CardDragControll : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    [SerializeField] CardView _view;
    Vector3 _previusPos;
    Vector3 _previusRot;
    bool _onMove;
    bool _onField;
    HandCardsController _handCards;

    [Inject]
    void Construct(HandCardsController handCards)
    {
        _handCards = handCards;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_onField)
        {
            if (!_onMove && !_handCards.IsOnModification)
            {
                _onMove = true;
                _view.DragOn();
                _previusPos = transform.position;
                _previusRot = transform.transform.localEulerAngles;
                transform.DORotate(Vector3.zero, 0.2f);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_onMove && !_onField)
        {
            _view.DragOff();
            AfterPointerUp(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_onMove && !_onField)
        {
            transform.position = eventData.position;
        }
    }

    void AfterPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("PlayerField"))
        {
            MoveOnField(eventData.pointerEnter.transform);
        }
        else
        {
            MoveBack();
        }
    }
    void MoveOnField(Transform field)
    {
        var seq = DOTween.Sequence();
        seq.Append(transform.DOMove(field.position, 0.3f));
        seq.OnComplete(() => transform.SetParent(field, true));
        _handCards.MoveCardToField(_view);
        _onMove = false;
        _onField = true;
        _view.SwitchRaycastTarget();
    }
    void MoveBack()
    {
        var seq = DOTween.Sequence();
        seq.Append(transform.DOMove(_previusPos, 0.3f));
        seq.Join(transform.DORotate(_previusRot, 0.3f));
        seq.OnComplete(() => _onMove = false);
    }
}
