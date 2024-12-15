using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckNodeInfo
{
    [SerializeField] int deckCode;
    [SerializeField] int cardCode;
    [SerializeField] int cardNum;

    public int DeckCode => deckCode;
    public int CardCode => cardCode;
    public int CardNum => cardNum;
}
