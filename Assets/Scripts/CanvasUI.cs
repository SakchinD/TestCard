using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class CanvasUI : MonoBehaviour
{
    [SerializeField] Button _cardModificationButton,_restartButton;
    [SerializeField] GameObject _gameOverPanel;

    HandCardsController _handCardsController;

    [Inject]
    void Construct(HandCardsController handCards)
    {
        _handCardsController = handCards;
    }

    private void Awake()
    {
        _cardModificationButton.onClick.AddListener(OnCardModificationClick);
        _restartButton.onClick.AddListener(OnRestartClick);
        _handCardsController.onHandIsEmptyEvent += OnHandEmpty;
    }
    private void OnDestroy()
    {
        _cardModificationButton.onClick.RemoveListener(OnCardModificationClick);
        _restartButton.onClick.RemoveListener(OnRestartClick);
        _handCardsController.onHandIsEmptyEvent -= OnHandEmpty;
    }

    void OnCardModificationClick()
    {
        _handCardsController.StartCardModification();
    }
    void OnRestartClick()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    void OnHandEmpty()
    {
        _gameOverPanel.SetActive(true);
    }
}

