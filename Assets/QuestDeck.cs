using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class QuestDeck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(ShuffleDeck)
        {
            Shuffle();
        }

        DrawCard();
    }

    public enum TARGET_TYPE { MIN, MAX, EXACT, OPTIONAL };

    public Transform QuestBox;
    public GameObject QuestCardPrefab;

    public bool ShuffleDeck;

    int cardsDrawn = 0;

    void Shuffle()
    {
            System.Random r = new System.Random();
            QuestCards = QuestCards.OrderBy( x => r.NextDouble() ).ToArray();
            cardsDrawn = 0;
    }

    [System.Serializable]
    public class QuestCardData
    {
        public string Title;
        public string Description;

        public QuestDiceSlot[] QuestDiceSlots;

        public int ValueTarget;

        public bool HasTargetDie;
        public QuestDiceSlot TargetDieSlot; // This die will gain a pip

        public int FireImmunityTurns;
        public int ChangeTargetDiePips;
        public int SetTargetDiePips;
    }

    [System.Serializable]
    public class QuestDiceSlot
    {
        public int TargetPipValue;
        public TARGET_TYPE TargetType;
    }

    public QuestCardData[] QuestCards;



    public void DrawCard()
    {
        Debug.Log("DrawCard");
        if(cardsDrawn >= QuestCards.Length)
        {
            Debug.Log("Shuffling!");
            Shuffle();
        }

        QuestCardData card = QuestCards[cardsDrawn++];

        GameObject go = Instantiate(QuestCardPrefab, Vector3.zero, Quaternion.identity, QuestBox);
        go.GetComponent<QuestCard>().SetCardData(card);
    }
}

