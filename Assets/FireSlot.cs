using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSlot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        dieSlot = GetComponent<DieSlot>();
    }

    public GameObject FireFX;
    DieSlot dieSlot;
    public bool IsLit = false;

    // Update is called once per frame
    void Update()
    {
        FireFX.SetActive( IsLit );
        dieSlot.Blocked = IsLit;
    }
}
