using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DieSlot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetText();
    }

    public QuestDeck.QuestDiceSlot QuestDiceSlot;  // Hang on to this so we can filter drops


    Die starterDie;
    Die die;

    public bool Blocked = false; // Prevents dice from being assigned to this slot for some reason.

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetText()
    {
        if(QuestDiceSlot.TargetPipValue > 0 || QuestDiceSlot.TargetType == QuestDeck.TARGET_TYPE.EXACT )
        {
            TextMeshProUGUI t = GetComponentInChildren<TextMeshProUGUI>();

            if(QuestDiceSlot.TargetType == QuestDeck.TARGET_TYPE.EXACT)
            {
                t.text = "=";
            }

            t.text += QuestDiceSlot.TargetPipValue.ToString();

            if(QuestDiceSlot.TargetType == QuestDeck.TARGET_TYPE.MIN)
            {
                t.text += "▲";
            }
            else if(QuestDiceSlot.TargetType == QuestDeck.TARGET_TYPE.MAX)
            {
                t.text += "▼";
            }
        }
    }

    public bool CanOccupy( Die d )
    {
        if(Blocked)
            return false;

        if(starterDie != null && d != starterDie)
            return false;

        return die == null;
    }

    public bool IsOccupied()
    {
        return die != null;
    }

    public Die GetDie()
    {
        return die;
    }

    public void SetDie( Die d, bool setStarter = false )
    {
        die = d;

        if(setStarter)
        {
            starterDie = d;
        }
        
    }
}
