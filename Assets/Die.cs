using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Die : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        graphicRaycaster = GetComponentInParent<GraphicRaycaster>();

        dieSlot = GetComponentInParent<DieSlot>();
        originalDieSlot = dieSlot;
        if(dieSlot == null)
        {
            Debug.LogError("Die isn't in a DieSlot!");
        }
        dieSlot.SetDie(this, true);

        image = GetComponentInChildren<Image>();

        UpdateStatBlock();

        Roll();
    }

    RectTransform rectTransform;
    GraphicRaycaster graphicRaycaster;
    DieSlot dieSlot;
    DieSlot originalDieSlot;

    public CanvasGroup StatBlock;

    public Sprite[] SideImages;
    public int[] PipsOnSides;
    int upSide;

    Image image;

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetValue()
    {
        return PipsOnSides[upSide];
    }

    public void ChangeValue(int change)
    {
        PipsOnSides[upSide] = Mathf.Clamp(PipsOnSides[upSide]+change, 0, MaxPips());
        UpdateStatBlock();
    }

    public void SetValue(int change)
    {
        PipsOnSides[upSide] = Mathf.Clamp(change, 0, MaxPips());
        UpdateStatBlock();
    }

    void UpdateStatBlock()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = "[" + string.Join(" ", PipsOnSides) + "]";
    }

    public void Roll()
    {
        SetUpSide( Random.Range(0, MaxPips()) );
    }

    int MaxPips()
    {
        return PipsOnSides.Length;
    }

    void SetUpSide( int sideNum )
    {
        upSide = sideNum;

        if(upSide >= PipsOnSides.Length)
        {
            Debug.LogError("Dice only have 6 sides! Trying to set index: " + upSide);
            return;
        }

        int numPips = GetValue();

        if(numPips >= SideImages.Length)
        {
            Debug.LogError("Trying to assign too many pips to a die side: " + numPips);
            return;
        }

        image.sprite = SideImages[numPips];

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Unparent();
        StatBlock.alpha = 0;

        //Debug.Log("Start drag: " + rectTransform.position);
    }

    void Unparent()
    {
        // I'M ALMOST BATMAN
        // Parent ourselves directly to the canvas, and put us at the end so we render on top of everything.
        this.transform.SetParent( GetComponentInParent<Canvas>().transform );
        this.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move with the mouse

        float oldZ = rectTransform.position.z;
        Vector3 pos = Camera.main.ScreenToWorldPoint( eventData.position );
        pos.z = oldZ;
        rectTransform.position = pos;

        //Debug.Log("Drag: " + rectTransform.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if we're on a valid DieSlot, if not return to our old DieSlot

        List<RaycastResult> results = new List<RaycastResult>();

        graphicRaycaster.Raycast( eventData, results );

        foreach(RaycastResult result in results)
        {
            DieSlot ds = result.gameObject.GetComponentInParent<DieSlot>();
            if(ds != null)
            {
                // We're in a DieSlot!
                //Debug.Log("Hit!");
                if(IsLegalForSlot(ds))
                {
                    AttachToSlot(ds);
                    return;
                }

            }
        }

        // If we get here, we weren't dropped on a slot. Re-attach to the old spot.
        //Debug.Log("Missed!");
        ReturnToOrigin();
    }

    public void ReturnToOrigin()
    {
        AttachToSlot(originalDieSlot);
    }

    bool IsLegalForSlot( DieSlot ds )
    {
        if(ds.CanOccupy(this) == false)
            return false;

        // Make sure we are legal for this slot!
        if(ds.QuestDiceSlot.TargetType == QuestDeck.TARGET_TYPE.EXACT && GetValue() != ds.QuestDiceSlot.TargetPipValue)
            return false;
        if(ds.QuestDiceSlot.TargetType == QuestDeck.TARGET_TYPE.MIN && GetValue() < ds.QuestDiceSlot.TargetPipValue)
            return false;
        if(ds.QuestDiceSlot.TargetType == QuestDeck.TARGET_TYPE.MAX && GetValue() > ds.QuestDiceSlot.TargetPipValue)
            return false;

        return true;
    }

    void AttachToSlot( DieSlot ds )
    {
        Unparent();
        dieSlot.SetDie( null );

        dieSlot = ds;
        dieSlot.SetDie( this );

        //this.transform.localPosition = Vector3.zero;

        StartCoroutine( COMoveToSlot() );
    }

    IEnumerator COMoveToSlot()
    {
        Vector3 startPos = this.transform.position;

        for( float t = 0; t <= 1; t += Time.deltaTime * 4f )
        {
            this.transform.position = Vector3.Lerp(startPos, dieSlot.transform.position, t );
            yield return null;
        }

        this.transform.SetParent(dieSlot.transform);
        this.transform.localPosition = Vector3.zero;

        if(dieSlot == originalDieSlot)
            StatBlock.alpha = 1;

    }
}
