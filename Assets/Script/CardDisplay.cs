using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public GameObject backImage;

    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text descriptionText;

    public SpriteRenderer artworkSprite;
    public Image artworkImage;

    public TMPro.TMP_Text manaText;
    public TMPro.TMP_Text attackText;
    public TMPro.TMP_Text defenceText;
    public GameMain gameMain;
    public int cardNo;

    private int cardStatus = 0;

    public void SetDisplay()
	{
        nameText.text = card.name;
        descriptionText.text = card.description;
        if(artworkImage == null)
        artworkSprite.sprite = card.artwork;
        else artworkImage.sprite = card.artwork;
        manaText.text = card.manaCost.ToString();
        attackText.text = card.attack.ToString();
        defenceText.text = card.defence.ToString();
    }

    public void ClickEvent()
	{
        if (cardStatus == 0)
        {
            gameMain.SendMessageData("Turn", cardNo.ToString()+","+ cardStatus);
        }
    }
}
