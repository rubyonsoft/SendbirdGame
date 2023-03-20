using UnityEngine;
using UnityEngine.UI;

using System;
using SendBird;
using UnityEngine.SceneManagement;

public class GameMain : MonoBehaviour
{
	public CardDisplay[] myCard;
	public CardDisplay[] otherCard;
	public GameObject[] field1;
	public GameObject[] field2;
	public CardDisplay[] fieldCard1;
	public CardDisplay[] fieldCard2;

	GroupChannel currentChannel;
	public Button btnBack;

	public CardDisplay myEventCard;
	public GameObject waitTurn;

	int cardNo = 0;

	void Start()
	{
		currentChannel = SendBirdUI.myChannel;
		ChannelHandler();
		//userId = PlayerPrefs.GetString("UserID");
		btnBack.onClick.AddListener(() =>
		{
			currentChannel.DeleteChannel(currentChannel.Url, new GroupChannel.GroupChannelLeaveHandler((SendBirdException e3) =>
			{
				UserMessageParams userMessage = new UserMessageParams();
				userMessage.SetMessage("RoomCancel");
				userMessage.SetData("Cancel");
				currentChannel.SendUserMessage(userMessage, (message, e) =>
				{

				});
			}));

			SendBirdClient.RemoveChannelHandler("default");
			SendBirdUI.myChannel = null;
			SceneManager.LoadScene("Lobby");
		});

		Debug.Log("currentChannel:::" + SendBirdClient.CurrentUser.UserId + " " + currentChannel.Members[0].Nickname + " " + currentChannel.Members[1].Nickname);

		if (currentChannel.Url.Contains(SendBirdClient.CurrentUser.UserId))
		{
			OwnerDeck();
			OtherDeck();
			waitTurn.SetActive(true);
		}
		else
		{
			OtherDeck();
			OwnerDeck();
			waitTurn.SetActive(false);
		}

	}

	static Card GetCard(string path)
	{
		return Resources.Load<Card>(path);
	}

	void OwnerDeck()
	{
		myCard[0].card = GetCard("Card/Bandit"); 
		myCard[1].card = GetCard("Card/Barbarian");
		myCard[2].card = GetCard("Card/Caesar");
		myCard[3].card = GetCard("Card/Cleopatra");
		myCard[4].card = GetCard("Card/Doctor");
		myCard[5].card = GetCard("Card/Dwarf");

		for (int i = 0; i < 6; i++)
		{
			myCard[i].SetDisplay();
			myCard[i].backImage.SetActive(false);
			otherCard[i].backImage.SetActive(true);
		}		
	}

	void OtherDeck()
	{
		//You can also send and receive your opponent's deck cards through messages. 상대방의 덱 카드도 메시지로 주고 받으면 된다
		otherCard[0].card = GetCard("Card/Fishmonster");
		otherCard[1].card = GetCard("Card/Fox");
		otherCard[2].card = GetCard("Card/Goblin");
		otherCard[3].card = GetCard("Card/Golem");
		otherCard[4].card = GetCard("Card/Piratecaptain");
		otherCard[5].card = GetCard("Card/Smileman");

		for (int i = 0; i < 6; i++)
		{
			otherCard[i].SetDisplay();
			otherCard[i].backImage.SetActive(false);
			myCard[i].backImage.SetActive(true);
		}		
	}

	void ChannelHandler()
	{
		SendBirdClient.ChannelHandler channelHandler = new SendBirdClient.ChannelHandler();

		channelHandler.OnMessageReceived = (BaseChannel channel, BaseMessage message) =>
		{
			if (currentChannel.Url == channel.Url)
			{
				if (message is UserMessage)
				{
					if (channel.IsGroupChannel())
					{
						Debug.LogError("OnMessageReceived:::" + ((UserMessage)message).Message);
						string[] data = message.Data.Split(',');
						waitTurn.SetActive(false);

						if (((UserMessage)message).Message == "Turn")
						{
							ShowCard(Convert.ToInt16(data[0]), data[1]);
							Invoke("StartBattle", 1f);  // delay 1 secound
							SendMessageData("Battle", data[0] + "," + data[1]);
						}
						else if (((UserMessage)message).Message == "Battle")
						{
							Invoke("StartBattle", 1f);
						}
					}
					else
					{

					}
				}
			}
		};

		channelHandler.OnChannelChanged = (BaseChannel channel) =>
		{
			Debug.LogError("OnChannelChanged:::" + channel.Url);

		};

		SendBirdClient.AddChannelHandler("gamechannel", channelHandler);
	}

	public void SendMessageData(string msg, string data = "")
	{
		
		UserMessageParams userMessage = new UserMessageParams();
		userMessage.SetMessage(msg);
		userMessage.SetData(data);

		string[] mycard = data.Split(',');
		if (msg == "Turn")
		{			
			ShowCard(Convert.ToInt16(mycard[0]), mycard[1]);
		}
		cardNo = Convert.ToInt16(mycard[0]);

		currentChannel.SendUserMessage(userMessage, (message, e) => {
			if (e != null)
			{
				Debug.Log(e.Code + ": " + e.Message);
				return;
			}
		});
	}

	public void ShowCard(int card, string status)
	{
		if (status == "0")
		{
			int iNext = 0;
			if( Convert.ToInt32(card) < 6)
			{
				for(int i=0; i<3;i++)
				 if (field1[iNext].gameObject.GetComponentInChildren<CardDisplay>() != null) iNext++;
				myCard[card].transform.parent = field1[iNext].transform;
				myCard[card].transform.localPosition = new Vector3(0, 0, 0);
				myCard[card].backImage.SetActive(false);
			}
			else
			{
				for (int i = 0; i < 3; i++)
				 if (field2[iNext].gameObject.GetComponentInChildren<CardDisplay>() != null) iNext++;
				otherCard[card - 6].transform.parent = field2[iNext].transform;
				otherCard[card - 6].transform.localPosition = new Vector3(0, 0, 0);
				otherCard[card - 6].backImage.SetActive(false);
			}
		}
	}

	void StartBattle()
	{
		for(int i=0; i<3; i++)
		 fieldCard1[i] = field1[i].gameObject.GetComponentInChildren<CardDisplay>();
		for (int i = 0; i < 3; i++)
		 fieldCard2[i] = field2[i].gameObject.GetComponentInChildren<CardDisplay>();

		if (cardNo < 6)
		{
			for (int i = 0; i < 3; i++)
				if (fieldCard1[i] != null)
				{
					int attack1 = Convert.ToInt32(fieldCard1[i].attackText.text);
					int dedence1 = Convert.ToInt32(fieldCard1[i].defenceText.text);

					if (dedence1 > 0)
					{
						for (int j = 0; j < 3; j++)
							if (fieldCard2[j].gameObject.GetComponentInChildren<CardDisplay>() != null)
							{
								int attack2 = Convert.ToInt32(fieldCard2[j].attackText.text);
								int dedence2 = Convert.ToInt32(fieldCard2[j].defenceText.text);

								fieldCard2[i].defenceText.text = (dedence2 - attack1).ToString();

								GameObject impact = (GameObject)Instantiate(Resources.Load("Impact"));
								impact.transform.parent = fieldCard1[i].gameObject.transform;
								impact.transform.localScale = new Vector3(20, 20, 20);
								impact.transform.localPosition = new Vector3(0, 100, -10);

								if (dedence2 - attack1 <= 0)
								{
									Destroy(fieldCard2[i].gameObject, 2f);
								}
								break;
							}
					}
				}
		}
		else
		{
			for (int i = 0; i < 3; i++)
				if (fieldCard2[i] != null)
				{
					int attack1 = Convert.ToInt32(fieldCard2[i].attackText.text);
					int dedence1 = Convert.ToInt32(fieldCard2[i].defenceText.text);

					if (dedence1 > 0)
					{
						for (int j = 0; j < 3; j++)
							if (fieldCard1[j].gameObject.GetComponentInChildren<CardDisplay>() != null)
							{
								int attack2 = Convert.ToInt32(fieldCard1[j].attackText.text);
								int dedence2 = Convert.ToInt32(fieldCard1[j].defenceText.text);

								fieldCard1[i].defenceText.text = (dedence2 - attack1).ToString();

								GameObject impact = (GameObject)Instantiate(Resources.Load("Impact"));
								impact.transform.parent = fieldCard2[i].gameObject.transform;
								impact.transform.localScale = new Vector3(20, 20, 20);
								impact.transform.localPosition = new Vector3(0, 100, -10);

								if (dedence2 - attack1 <= 0)
								{
									Destroy(fieldCard1[i].gameObject, 2f);
								}
								break;
							}
					}
				}
		}
	}
}
