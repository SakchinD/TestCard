using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class HandCardsController : MonoBehaviour
{
    public event Action<CardModel, CardView> onMoveToFieldEvent;
    public event Action onHandIsEmptyEvent;
    [SerializeField] List<Card> _cardsTemplates;
    [SerializeField] CardView _cardViewTemplate;
    [SerializeField] Transform _playerHand, _viewPosition;
    [SerializeField] int _playerRandomMinCards, _playerRandomMaxCards;
    
    [SerializeField,Header("DOTween duration")] 
    float _cardAnimatiomDuration;

    [SerializeField, Header("Cards Positions settings")]
    float _radius;
    [SerializeField] 
    float _startAngle, _distanseBetweenAngle;

    List<CardView> _cardsInHandList = new();
    List<CardModel> _cardsModes = new();

    ObjectPool _objectPool;
    int _cardIndex;
    bool _isModificationStart;
    public bool IsOnModification { get; private set; }

    [Inject]
    void Construct(ObjectPool objectPool)
    {
        _objectPool = objectPool;
    }

    void Start()
    {
        CreateCards();
    }

    async void CreateCards()
    {
        ShuffleCardsTemplates();
        int randomCardCount = UnityEngine.Random.Range(_playerRandomMinCards, _playerRandomMaxCards + 1);

        for (int i = 0; i < randomCardCount; i++)
        {
            Card cardTemplate = _cardsTemplates[i];
            CardModel card = new CardModel(cardTemplate.Id, cardTemplate.Attack, cardTemplate.Hp, cardTemplate.Mana);
            _cardsModes.Add(card);

            CardView cardView = _objectPool.GetPooledObject("view");
            cardView.transform.SetParent(_playerHand, false);
            cardView.SetCardInfo(cardTemplate.Id, cardTemplate.Title, cardTemplate.Description, cardTemplate.ImageUrl);
            cardView.UpdateStats(card);
            cardView.gameObject.SetActive(true);
            _cardsInHandList.Add(cardView);
        }
        _cardIndex = _cardsInHandList.Count - 1;
        await UpdateCardsPositions();
    }

    public void StartCardModification()
    {
        _isModificationStart = !_isModificationStart;
        if(_isModificationStart)
        {
            CardModification();
        }
    }
    public void CardModification()
    {
        IsOnModification = true;
        var seq = DOTween.Sequence();
        var view = _cardsInHandList[_cardIndex];
        Vector3 startPos = view.transform.position;
        Vector3 startRotate = view.transform.localEulerAngles;
        seq.Append(view.transform.DOMove(_viewPosition.position, _cardAnimatiomDuration));
        seq.Join(view.transform.DORotate(Vector3.zero, _cardAnimatiomDuration));
        seq.OnComplete(()=> ChangStats(view, startPos, startRotate)); 
    }

    async void ChangStats(CardView view, Vector3 startPos, Vector3 startRotate)
    {
        int newValue = UnityEngine.Random.Range(-2, 10);

        int index = UnityEngine.Random.Range(0, 3);

        CardModel card = _cardsModes.Find(s=>s.Id == view.Id);

        await view.StatChangeAnimation(index, newValue, card);

        switch (index)
        {
            case 0:
                {
                    card.SetAttack(newValue);
                    break;
                }
            case 1:
                {
                    card.SetHp(newValue);
                    break;
                }
            default:
                {
                    card.SetMana(newValue);
                    break;
                }
        }
        view.UpdateStats(card);

        if (card.Hp < 1)
        {
            _cardsModes.Remove(card);
            _cardsInHandList.Remove(view);
            view.gameObject.SetActive(false);
            view.transform.SetParent(_objectPool.transform, false);
            _cardIndex--;
            CheckIndex();
            if (_cardsInHandList.Count > 0)
            {
                await UpdateCardsPositions();
                IsOnModification = false;
                RestartSequens();
            }
            else
            {
                onHandIsEmptyEvent?.Invoke();
            }
        }
        else
        {
            MoveCardBack(view, startPos, startRotate);
        }
    }

    void MoveCardBack(CardView view, Vector3 startPos, Vector3 startRotate)
    {
        var seq = DOTween.Sequence();
        seq.Append(view.transform.DOMove(startPos, _cardAnimatiomDuration));
        seq.Join(view.transform.DORotate(startRotate, _cardAnimatiomDuration));
        seq.AppendCallback(() => IsOnModification = false);

        _cardIndex--;
        CheckIndex();

        seq.OnComplete(()=> RestartSequens());
    }
    void RestartSequens()
    {
        if (_isModificationStart)
        {
            CardModification();
        }
    }

    void ShuffleCardsTemplates()
    {
        for (int i = 0; i < _cardsTemplates.Count; i++)
        {
            Card card = _cardsTemplates[i];
            int randomIndex = UnityEngine.Random.Range(i, _cardsTemplates.Count);
            _cardsTemplates[i] = _cardsTemplates[randomIndex];
            _cardsTemplates[randomIndex] = card;
        }
    }

    void CheckIndex()
    {
        if (_cardIndex < 0 || _cardIndex > _cardsInHandList.Count - 1)
        {
            _cardIndex = _cardsInHandList.Count - 1;
        }
    }
    public async void MoveCardToField(CardView view)
    {
        _cardsInHandList.Remove(view);
        CardModel card = _cardsModes.Find(x => x.Id == view.Id);
        _cardsModes.Remove(card);
        onMoveToFieldEvent?.Invoke(card, view);
        CheckIndex();
        await UpdateCardsPositions();
        if (_cardsInHandList.Count == 0)
        {
            onHandIsEmptyEvent?.Invoke();
        }
    }
    public async UniTask UpdateCardsPositions()
    {
        float bufferAng = (_distanseBetweenAngle / (_cardsInHandList.Count + 1));
        float check = _startAngle;

        Vector3 center = _playerHand.position;
        center.y -= _radius;

        bool needInvers = true;

        foreach (var cardObj in _cardsInHandList)
        {
            check += bufferAng;
            if (check >= 180f)
            {
                needInvers = false;
                check -= 360f;
            }

            Vector3 cardPosition;
            cardPosition.x = center.x + _radius * Mathf.Sin(check * Mathf.Deg2Rad);
            cardPosition.y = center.y - _radius * Mathf.Cos(check * Mathf.Deg2Rad);
            cardPosition.z = center.z;

            cardObj.transform.DOMove(cardPosition, _cardAnimatiomDuration);

            double hypotenuse = Math.Sqrt(Math.Pow(center.x - cardPosition.x, 2) + Math.Pow(center.y - cardPosition.y, 2));
            double cathetus = Math.Sqrt(Math.Pow(center.y - cardPosition.y, 2));


            float angleZ = (float)(90f - (Math.Asin(cathetus / hypotenuse) * Mathf.Rad2Deg));
            angleZ = needInvers ? angleZ * -1 : angleZ;

            Vector3 cardScale;
            cardScale.x = 0f;
            cardScale.y = 0f;
            cardScale.z = angleZ;
            cardObj.transform.DOLocalRotate(cardScale, _cardAnimatiomDuration);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: false);
    }
}
