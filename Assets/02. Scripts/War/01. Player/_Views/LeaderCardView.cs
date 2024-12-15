using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderCardView : MonoBehaviour
{
    Image _playerLeaderCardImage, _enemyLeaderCardImage;
    PointerEnterHandler _playerLeaderCardPointerEnterHandler, _enemyLeaderCardPointerEnterHandler;

    public PointerEnterHandler PlayerLeaderCardPointerEnterHandler => _playerLeaderCardPointerEnterHandler;
    public PointerEnterHandler EnemyLeaderCardPointerEnterHandler => _enemyLeaderCardPointerEnterHandler;

    private void Awake()
    {
        _playerLeaderCardImage = gameObject.FindChild<Image>("PlayerLeaderCardView");
        _playerLeaderCardPointerEnterHandler = _playerLeaderCardImage.gameObject.GetOrAddComponent<PointerEnterHandler>();

        _enemyLeaderCardImage = gameObject.FindChild<Image>("EnemyLeaderCardView");
        _enemyLeaderCardPointerEnterHandler = _enemyLeaderCardImage.gameObject.GetOrAddComponent<PointerEnterHandler>();
    }

    public void SetPlayerLeaderCardImage(Sprite sprite)
    {
        _playerLeaderCardImage.sprite = sprite;
    }
    public void SetEnemyLeaderCardImage(Sprite sprite)
    {
        _enemyLeaderCardImage.sprite = sprite;
    }
}
