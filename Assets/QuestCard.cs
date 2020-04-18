    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestCard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;

    public GameObject DieSlotPrefab;
    public Sprite DieSlot_OptionalArt;
    public Transform DieSlotContainer;

    public GameObject TargetDieSlot;

    QuestDeck.QuestCardData cardData;

    public void SetCardData(QuestDeck.QuestCardData cd)
    {
        Debug.Log("SetCardData");

        cardData = cd;

        Title.text = cardData.Title;

        string desc = cardData.Description;

        if(cardData.ValueTarget > 0)
        {
            if(desc.Length > 0)
            {
                desc += " ";
            }
            desc += "Required Total: " + cardData.ValueTarget.ToString();
        }

        if(desc != null && desc.Length > 0)
        {
            Description.text = desc;
        }
        else
        {
            Description.gameObject.SetActive(false);
        }

        foreach( QuestDeck.QuestDiceSlot qds in cardData.QuestDiceSlots )
        {
            SetDieSlot(qds);
        }

        // Do we have a target die?
        if(cardData.HasTargetDie)
        {
            SetDieSlot(cardData.TargetDieSlot);
        }
        else 
        {
            TargetDieSlot.SetActive(false);
        }
    }

    void SetDieSlot(QuestDeck.QuestDiceSlot qds)
    {
            GameObject slot = Instantiate( DieSlotPrefab, Vector3.zero, Quaternion.identity, DieSlotContainer );
            slot.GetComponent<DieSlot>().QuestDiceSlot = qds;

            if(qds.TargetPipValue > 0 || qds.TargetType == QuestDeck.TARGET_TYPE.EXACT )
            {
                TextMeshProUGUI t = slot.GetComponentInChildren<TextMeshProUGUI>();

                if(qds.TargetType == QuestDeck.TARGET_TYPE.EXACT)
                {
                    t.text = "=";
                }

                t.text += qds.TargetPipValue.ToString();

                if(qds.TargetType == QuestDeck.TARGET_TYPE.MIN)
                {
                    t.text += "▲";
                }
                else if(qds.TargetType == QuestDeck.TARGET_TYPE.MAX)
                {
                    t.text += "▼";
                }
            }

            if(qds.TargetType == QuestDeck.TARGET_TYPE.OPTIONAL)
            {
                // Optional slot, change the look of it.
                slot.GetComponentInChildren<Image>().sprite = DieSlot_OptionalArt;
            }

    }

    public bool IsCompleted()
    {
        // Check if all dice slots are filled.

        DieSlot[] slots = GetComponentsInChildren<DieSlot>();

        int total = 0;
        foreach(DieSlot slot in slots)
        {
            if(slot.IsOccupied() == true)
            {
                // Slot has a die, add to total
                total += slot.GetDie().GetValue();
            } 
            else if(slot.QuestDiceSlot.TargetType != QuestDeck.TARGET_TYPE.OPTIONAL)
            {
                // Slot is empty and not optional
                return false;
            }
            

        }

        if(cardData.ValueTarget > total)
        {
            // Not enough pips on this card!
            return false;
        }

        return true;
    }

    public void DoComplete()
    {
        if(IsCompleted() == false)
        {
            Debug.LogError("DoComplete on an incomplete quest.");
            return;
        }
    
        // Return dice to the box
        DieSlot[] slots = GetComponentsInChildren<DieSlot>();
        foreach(DieSlot slot in slots)
        {
            Die die = slot.GetDie();
            if( die != null)
            {
                die.ReturnToOrigin();
            }
        }


        ProcessBonusEffects();
        
        // Make pretty and then destroy self
        Destroy(gameObject);
    }

    void ProcessBonusEffects() {
        FireSlotManager fireSlotManager =  GameObject.FindObjectOfType<FireSlotManager>();
        TargetDieSlot targetDieSlot = GetComponentInChildren<TargetDieSlot>();
        Die targetDie = null;

        if(targetDieSlot != null)
        {
            targetDie = targetDieSlot.GetComponentInChildren<Die>();
        }
        
        fireSlotManager.FireImmunityTurns += cardData.FireImmunityTurns;

        if(targetDie != null)
        {
            targetDie.ChangeValue(cardData.ChangeTargetDiePips);
            targetDie.SetValue(cardData.SetTargetDiePips);
        }

    }

}
