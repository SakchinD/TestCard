using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CardView : MonoBehaviour
{
    public int Id { get; private set; }
    [SerializeField, Header("Stat change animation delay")]
    float _animationDelay;
    [SerializeField,Header("Card hold time after move back")]
    float _cardHoldTime;
    [SerializeField] GameObject _outLine;
    [SerializeField] TMP_Text _attackText;
    [SerializeField] TMP_Text _hpText;
    [SerializeField] TMP_Text _manaText;
    [SerializeField] TMP_Text _titleText;
    [SerializeField] TMP_Text _descriptionText;
    [SerializeField] RawImage _image;
    [SerializeField] Image _cardTemplate;
    ImageLoader _imageLoader;

    [Inject]
    void Construct(ImageLoader imageLoader)
    {
        _imageLoader = imageLoader;
    }

    public async void SetCardInfo(int id,string title,string description,string url)
    {
        Id = id;
        _titleText.text = title;
        _descriptionText.text = description;
        Texture2D img = await _imageLoader.LoadImage(url);
        _image.texture = img;
    }

    public void UpdateStats(CardModel card)
    {
        _hpText.text = $"{card.Hp}";
    
        _manaText.text = $"{card.Mana}";

        _attackText.text = $"{card.Attack}";
    }
    public async UniTask StatChangeAnimation(int index, int newValue, CardModel card)
    {
        TMP_Text text;
        int prevValue;
        if (index == 0)
        {
            text = _attackText;
            prevValue = card.Attack;
        }
        else if (index == 1)
        {
            text= _hpText;
            prevValue = card.Hp;
        }
        else
        {
            text = _manaText;
            prevValue= card.Mana;
        }

        int step = prevValue < newValue ? 1: -1;

        while (prevValue != newValue)
        {
            prevValue += step;
            text.text = $"{prevValue}";
            text.transform.DOShakeScale(0.1f);
            await UniTask.Delay(TimeSpan.FromSeconds(_animationDelay), ignoreTimeScale: false);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(_cardHoldTime), ignoreTimeScale: false);
    }

    public void DragOn()
    {
        _outLine.SetActive(true);
        SwitchRaycastTarget();
    }

    public void SwitchRaycastTarget()
    {
        _cardTemplate.raycastTarget = !_cardTemplate.raycastTarget;
    }

    public void DragOff()
    {
        _outLine.SetActive(false);
        SwitchRaycastTarget();
    }
}
