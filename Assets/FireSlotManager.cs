using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSlotManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateFireSlots();
    }

    int fireLevel = 3;

    public int FireImmunityTurns;

    public int GetFireLevel(){
        return fireLevel;
    }

    public void DoSeasonFireLevel()
    {
        if(FireImmunityTurns > 0)
        {
            FireImmunityTurns--;
            return;
        }
        
        ChangeFireLevel(-1);
    }

    public void ChangeFireLevel( int change )
    {
        Debug.Log("ChangeFireLevel");
        fireLevel = Mathf.Clamp(fireLevel + change, 0, transform.childCount);
        UpdateFireSlots();
    }

    void UpdateFireSlots()
    {
        for(int i=0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<FireSlot>().IsLit = (i < fireLevel);
        }

    }

}
