using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        fireSlotManager = GameObject.FindObjectOfType<FireSlotManager>();
    }

    FireSlotManager fireSlotManager;
    public GameObject DiceBox;

    public void Click()
    {
        // Decrease fire one step (if possible!)
        if(fireSlotManager.GetFireLevel() <= 0)
        {
            Debug.Log("Give the user a sad trombone noise.");
            return;
        }

        fireSlotManager.ChangeFireLevel(-1);

        // Re-roll dice in the pool.
        Die[] dice = DiceBox.GetComponentsInChildren<Die>();
        foreach(Die die in dice)
        {
            die.Roll();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
