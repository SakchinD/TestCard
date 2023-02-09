using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FieldCardsController : MonoBehaviour
{
    [SerializeField] Transform _playerField;
    [SerializeField] float _cardAnimatiomDuration;
    [SerializeField] float _radius;
    [SerializeField] int _distanseBetweenAngle;
    [SerializeField] float _startAngle;

    List<CardView> _cardsInHandList = new();
    List<CardModel> _cardsModes = new();
    HandCardsController _handCards;

    [Inject]
    void Construct(HandCardsController handCards)
    {
        _handCards = handCards;
    }

    private void Awake()
    {
        _handCards.onMoveToFieldEvent += GetCardFromHand;
    }

    private void OnDestroy()
    {
        _handCards.onMoveToFieldEvent -= GetCardFromHand;
    }

    void GetCardFromHand(CardModel card, CardView view)
    {
        _cardsInHandList.Add(view);
        _cardsModes.Add(card);
        UpdateCardsPositions();
    }
    public void UpdateCardsPositions()
    {
        float bufferAng = (_distanseBetweenAngle / (_cardsInHandList.Count + 1));
        float check = _startAngle;

        Vector3 center = _playerField.position;

        foreach (var cardObj in _cardsInHandList)
        {
            check += bufferAng;

            if (check >= 180f)
            {
                check -= 360f;
            }

            Vector3 cardPosition;
            cardPosition.x = center.x + _radius * Mathf.Sin(check * Mathf.Deg2Rad);
            cardPosition.y = center.y;
            cardPosition.z = center.z;

            cardObj.transform.DOMove(cardPosition, _cardAnimatiomDuration);
        }
    }
}
