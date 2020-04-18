using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndTurnButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        fireSlotManager = GameObject.FindObjectOfType<FireSlotManager>();
    }

    public GameObject QuestBox;
    public GameObject FireBox;
    public GameObject DiceBox;
    public GameObject ClickBlocker;
    public QuestDeck QuestDeck;
    FireSlotManager fireSlotManager;

    public void Click()
    {
        StartCoroutine( CoClick() );
    }

    IEnumerator CoClick()
    {
        ClickBlocker.SetActive(true);
        TextMeshProUGUI BlockerText = ClickBlocker.GetComponentInChildren<TextMeshProUGUI>();

        BlockerText.text = "Checking quests...";
        yield return new WaitForSeconds(0.25f);

        // Check quests for completion
        QuestCard[] qcs = QuestBox.GetComponentsInChildren<QuestCard>();
        foreach(QuestCard qc in qcs)
        {
            if(qc.IsCompleted())
            {
                // NOTE: Destroy happens after the update loop, so the child isn't instantly removed.
                Debug.Log("Quest completed: " + qc.Title.text);
                qc.DoComplete();
                yield return new WaitForSeconds(0.25f);
            }
        }

        BlockerText.text = "Checking fire...";
        yield return new WaitForSeconds(0.25f);

        // Check for dice allocated to the fire
        DieSlot[] dss = FireBox.GetComponentsInChildren<DieSlot>();
        foreach(DieSlot ds in dss)
        {
            if(ds.IsOccupied())
            {
                // EAT A PIP, BUTTFACE
                Debug.Log("Fan the flames.");
                ds.GetDie().ChangeValue(-1);
                ds.GetDie().ReturnToOrigin();
                fireSlotManager.ChangeFireLevel(+1);
                yield return new WaitForSeconds(0.25f);
            }
        }

        BlockerText.text = "Returning dice...";
        yield return new WaitForSeconds(0.25f);

        // All dice in the dice pool need to be re-rolled.
        Die[] dice = DiceBox.GetComponentsInChildren<Die>();
        foreach(Die die in dice)
        {
            die.Roll();
        }

        // Draw a new quest
        QuestDeck.DrawCard();

        BlockerText.text = "Decreasing  fire...";
        yield return new WaitForSeconds(1f);

        // Decrease fire level
        if(fireSlotManager.GetFireLevel() <= 0)
        {
            Debug.Log("GAME OVER");
        }
        fireSlotManager.DoSeasonFireLevel();

        ClickBlocker.SetActive(false);
    }

}
