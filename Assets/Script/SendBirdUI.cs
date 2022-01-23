using UnityEngine;
using UnityEngine.UI;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SendBird;
using SendBird.SBJson;
using UnityEngine.SceneManagement;

public class SendBirdUI : MonoBehaviour
{
	public TMPro.TMP_Text txtMessage;
	public GameObject messageListItemPrefab;

	#region OpenChannel

	public Text txtOpenChannelContent;
	public TMPro.TMP_InputField inputOpenChannel;
	
	#endregion

	#region OpenChannelList

	public GameObject messageLeftPrefab;
	public GameObject messageRightPrefab;
	public GameObject gameListPrefab;

	ArrayList btnChannels = new ArrayList();
	#endregion

	public GameObject openChannelGridPannel;
	private List<string> mUserList = new List<string>();
	
	#region GroupChannelList

	private PublicGroupChannelListQuery mGroupChannelListQuery;
	ArrayList btnGroupChannels = new ArrayList();
	public GameObject groupChannelListConetnt;
	
	public static GroupChannel myChannel = null;

	#endregion

    BaseChannel currentChannel;
	public Scrollbar openChannelScrollbar;
	GameObject messagePrefab;

	public Text txtPopupMessage;
	public Button btnBack;
	public Button btnOk;
	public Button btnCreateRoom;
	public Button btnAutoMatch;
	public Button btnSend;

	void Start()
	{
		btnBack.onClick.AddListener(() =>
		{
			SendBirdClient.Disconnect(() =>
			{
				SendBirdClient.RemoveChannelHandler("default");
			});
			myChannel = null;
			Destroy(GameObject.Find("SendbirdUnity"));
			SceneManager.LoadScene("Login");
		});

		btnAutoMatch.onClick.AddListener(() =>
		{
			if (!mUserList.Contains(SendBirdClient.CurrentUser.UserId))
			{
				mUserList.Add(SendBirdClient.CurrentUser.UserId);
			}
		});

		btnSend.onClick.AddListener(() =>
		{
			MessageOpenChannel();
		});

		btnCreateRoom.onClick.AddListener(() =>
		{
			CreateRoom();
		});

		//userId = PlayerPrefs.GetString("UserID");
		
		ChannelHandler();

		currentChannel = LoginUI.currentChannel;

		LoadOpenChannelChatHistory();

		GroupChannelList();
	}

	void CreateRoom()
	{
		if (myChannel == null)
		{
		List<string> operatorUserIds = new List<string>();
		operatorUserIds.Add(SendBirdClient.CurrentUser.UserId);

		mUserList.Add(SendBirdClient.CurrentUser.UserId);
		GroupChannelParams channelParams = new GroupChannelParams();
		channelParams.AddUserId(SendBirdClient.CurrentUser.UserId);
		channelParams.mIsPublic = true;
		channelParams.mIsEphemeral = true;
		channelParams.SetName(SendBirdClient.CurrentUser.Nickname + " VS");
		channelParams.SetChannelUrl(DateTime.Now.Ticks + "_" + SendBirdClient.CurrentUser.UserId);
		channelParams.SetOperatorUserIds(operatorUserIds);
		
			GroupChannel.CreateChannel(channelParams, (channel, e) =>
			{
				if (e != null)
				{
					Debug.Log(e.Code + ": " + e.Message);
					return;
				}
				txtMessage.text = "Waiting for your opponent";
				myChannel = channel;
				SendMessageData(currentChannel, "CreateGame", channelParams.mChannelUrl, channel);
				btnCreateRoom.gameObject.SetActive(false);
			});
		}
		else
		{
			txtMessage.text = "You already have a game room";
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
					//string EnTranslatedMessage = ((UserMessage)message).Translations["en"];
					//string koTranslatedMessate = ((UserMessage)message).Translations["ko"];

					if (channel.IsOpenChannel())
					{
						Debug.LogError("OnMessageReceived:::" + ((UserMessage)message).Message);
						
						//messages others receive
						if (((UserMessage)message).Message == "CreateGame" && message.Data != "")
						{
							JsonElement json = JsonParser.Parse(message.Data);
							Debug.LogError(json.ToJSON());							
							
							messagePrefab = Instantiate(messageLeftPrefab) as GameObject;

							messagePrefab.transform.SetParent(openChannelGridPannel.transform);
							messagePrefab.transform.localScale = Vector3.one;
							messagePrefab.GetComponent<MessageList>().userID.text = json.getAsJsonObject().get("Nickname").getAsstring();
							messagePrefab.GetComponent<MessageList>().chatMessage.text = json.getAsJsonObject().get("Message").getAsstring();
							messagePrefab.GetComponent<MessageList>().txtTime.text = json.getAsJsonObject().get("UpdatedAt").getAsstring();
							btnChannels.Add(messagePrefab);


							GameObject btnGroupChannel = Instantiate(gameListPrefab) as GameObject;

							btnGroupChannel.transform.SetParent(groupChannelListConetnt.transform);
							btnGroupChannel.transform.localScale = Vector3.one;
							btnGroupChannel.GetComponentInChildren<Text>().text = json.getAsJsonObject().get("ChannelUrl").getAsstring(); // channel_url

							TMPro.TMP_Text[] text = btnGroupChannel.GetComponentsInChildren<TMPro.TMP_Text>();
							text[0].text = string.Format("{0}:{1}", json.getAsJsonObject().get("Nickname").getAsstring(), " VS Wait");
							text[1].text = "-";
							text[2].text = json.getAsJsonObject().get("Nickname").getAsstring();

							text[3].text = "Play";  // statusPlay
							btnGroupChannel.GetComponentInChildren<Button>().onClick.AddListener(() =>
								{
									Text text1 = btnGroupChannel.GetComponentInChildren<Text>();
									GroupChannel.GetChannel(text1.text, new GroupChannel.GroupChannelGetHandler((GroupChannel groupChannel1, SendBirdException e1) =>
									{
										try
										{
											groupChannel1.Join(new GroupChannel.GroupChannelJoinHandler((SendBirdException e4) =>
											{
												SendMessageData(currentChannel, "StartGame", groupChannel1.Members[0].UserId, groupChannel1);
												myChannel = groupChannel1;
												SendBirdClient.RemoveChannelHandler("default");
												SceneManager.LoadScene("MainScene");
											}));

										}
										catch (Exception z)
										{
											Debug.Log(z);
										}
									}));
								});							
						}
						else if (((UserMessage)message).Message == "RoomCancel" && message.Data != "")
						{
							foreach (Transform child in groupChannelListConetnt.transform)
							{
								Text text1 = child.GetComponentInChildren<Text>();
								JsonElement json = JsonParser.Parse(message.Data);
								if (text1.text == json.getAsJsonObject().get("ChannelUrl").getAsstring())
								{
									GameObject.Destroy(child.gameObject); break;
								}
							}
						}
						else if (((UserMessage)message).Message == "StartGame" && message.Data != "") //message i get
						{
							if (message.Data != SendBirdClient.CurrentUser.UserId)
							{
								SendBirdClient.RemoveChannelHandler("default");
								SceneManager.LoadScene("MainScene");
							}
							else
							{
								foreach (Transform child in groupChannelListConetnt.transform)
								{
									Text text1 = child.GetComponentInChildren<Text>();
									JsonElement json = JsonParser.Parse(message.Data);
									if (text1.text == json.getAsJsonObject().get("ChannelUrl").getAsstring())
									{
										GameObject.Destroy(child.gameObject); break;
									}
								}
							}
						}
						else
						{
							//txtOpenChannelContent.text = txtOpenChannelContent.text + (UserMessageRichText((UserMessage)message) + "\n");
							if (message.GetSender().UserId == SendBirdClient.CurrentUser.UserId)
								messagePrefab = Instantiate(messageLeftPrefab) as GameObject;
							else messagePrefab = Instantiate(messageRightPrefab) as GameObject;

							messagePrefab.transform.SetParent(openChannelGridPannel.transform);
							messagePrefab.transform.localScale = Vector3.one;
							messagePrefab.GetComponent<MessageList>().userID.text = message.GetSender().Nickname;
							messagePrefab.GetComponent<MessageList>().chatMessage.text = message.Message;
							messagePrefab.GetComponent<MessageList>().txtTime.text = message.UpdatedAt.ToString();
							btnChannels.Add(messagePrefab);
						}
						ScrollToBottom();
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

		channelHandler.OnChannelDeleted = (string channelUrl, BaseChannel.ChannelType channelType) =>
		{
			Debug.LogError("OnChannelDeleted:::");
			foreach (Transform child in groupChannelListConetnt.transform)
			{
				Text text1 = child.GetComponentInChildren<Text>();
				if (text1.text == channelUrl)
				{
					GameObject.Destroy(child.gameObject); break;
				}
			}
		};

		SendBirdClient.AddChannelHandler("default", channelHandler);
	}

	void ResetOpenChannelContent()
	{
		txtOpenChannelContent.text = "";
		inputOpenChannel.text = "";
	}

	void SendMessageData(BaseChannel openChannel, string msg, string data="", GroupChannel groupChannel = null)
	{
		UserMessageParams userMessage = new UserMessageParams();
		userMessage.SetMessage(msg);
		userMessage.SetData(data);
		//List<MessageMetaArray> array = new List<MessageMetaArray>();   // you can meta array!!
		//MessageMetaArray data = new MessageMetaArray("pass");
		//data.AddValue("1111");
		//array.Add(data);
		//userMessage.SetMetaArrays(array);

		openChannel.SendUserMessage(userMessage, (message, e) => {
			if (e != null)
			{
				Debug.Log(e.Code + ": " + e.Message);
				return;
			}
						
			if (groupChannel != null && msg == "CreateGame")
			{
				UserMessageParams userMessage1 = new UserMessageParams();
				userMessage.SetMessage(msg);
				StringBuilder msgData = new StringBuilder();
				msgData.Append("{");
				msgData.AppendFormat("\"Nickname\":\"{0}\",\"Message\":\"{1}\",\"UpdatedAt\":\"{2}\",\"UserId\":\"{3}\", \"ChannelUrl\":\"{4}\""
					, message.GetSender().Nickname, message.GetSender().Nickname + " Create Room", message.UpdatedAt, SendBirdClient.CurrentUser.UserId, data);
				msgData.Append("}");
				userMessage1.SetData(msgData.ToString());

				currentChannel.SendUserMessage(userMessage1, (message1, e1) => {
					if (e1 != null)
					{
						Debug.Log(e1.Code + ": " + e1.Message);
						return;
					}

					txtOpenChannelContent.text = txtOpenChannelContent.text + (UserMessageRichText(message1) + "\n");
					if (message1.GetSender().UserId == SendBirdClient.CurrentUser.UserId)
						messagePrefab = Instantiate(messageLeftPrefab) as GameObject;
					else messagePrefab = Instantiate(messageRightPrefab) as GameObject;

					messagePrefab.transform.SetParent(openChannelGridPannel.transform);
					messagePrefab.transform.localScale = Vector3.one;
					messagePrefab.GetComponent<MessageList>().userID.text = message1.GetSender().UserId;
					messagePrefab.GetComponent<MessageList>().chatMessage.text = message1.GetSender().Nickname +" Create Room";
					messagePrefab.GetComponent<MessageList>().txtTime.text = message1.UpdatedAt.ToString();
					btnChannels.Add(messagePrefab);

					ScrollToBottom();
				});

				GameObject btnGroupChannel = Instantiate(gameListPrefab) as GameObject;

				btnGroupChannel.transform.SetParent(groupChannelListConetnt.transform);
				btnGroupChannel.transform.localScale = Vector3.one;

				TMPro.TMP_Text[] text = btnGroupChannel.GetComponentsInChildren<TMPro.TMP_Text>();
				text[0].text = string.Format("{0}:{1}", groupChannel.Name, GetDisplayMemberNames(groupChannel.Members, groupChannel.Members[0].UserId));
				text[1].text = groupChannel.Members[0].GetMetaData().ContainsKey("rank") == false ? "-" : groupChannel.Members[0].GetMetaData()["rank"];
				text[2].text = groupChannel.Members[0].Nickname;

				Text text1 = btnGroupChannel.GetComponentInChildren<Text>();
				text1.text = groupChannel.Url;

				GroupChannel final = groupChannel;

				if (SendBirdClient.CurrentUser.UserId == groupChannel.Members[0].UserId)
				{
					text[3].text = "Cancel";
					btnGroupChannel.GetComponentInChildren<Button>().onClick.AddListener(() =>
					{
						final.DeleteChannel(final.Url, new GroupChannel.GroupChannelLeaveHandler((SendBirdException e3) =>
						{
							btnCreateRoom.gameObject.SetActive(true);
							txtMessage.text = "";
							myChannel = null;

							userMessage = new UserMessageParams();
							userMessage.SetMessage("RoomCancel");
							msgData = new StringBuilder();
							msgData.Append("{");
							msgData.AppendFormat("\"ChannelUrl\":\"{0}\"", final.Url);
							msgData.Append("}");
							userMessage.SetData(msgData.ToString());
							currentChannel.SendUserMessage(userMessage, (message2, e2) =>
							{

							});
						}));
					});
				}
			}			
		});
	}

	void MessageOpenChannel()
	{
		if (inputOpenChannel.text.Length > 0)
		{
			if (currentChannel != null && currentChannel.IsOpenChannel())
			{
				UserMessageParams userMessage = new UserMessageParams();
				userMessage.SetMessage(inputOpenChannel.text);

				currentChannel.SendUserMessage(userMessage, (message, e) => {
					if (e != null)
					{
						Debug.Log(e.Code + ": " + e.Message);
						return;
					}

					txtOpenChannelContent.text = txtOpenChannelContent.text + (UserMessageRichText(message) + "\n");
					if (message.GetSender().UserId == SendBirdClient.CurrentUser.UserId)
						messagePrefab = Instantiate(messageLeftPrefab) as GameObject;
					else messagePrefab = Instantiate(messageRightPrefab) as GameObject;

					messagePrefab.transform.SetParent(openChannelGridPannel.transform);
					messagePrefab.transform.localScale = Vector3.one;
					messagePrefab.GetComponent<MessageList>().userID.text = message.GetSender().UserId;
					messagePrefab.GetComponent<MessageList>().chatMessage.text = message.Message;
					messagePrefab.GetComponent<MessageList>().txtTime.text = message.UpdatedAt.ToString();
					btnChannels.Add(messagePrefab);

					ScrollToBottom();
				});
				inputOpenChannel.text = "";
			}
		}
	}

	void LoadOpenChannelChatHistory()
	{
		PreviousMessageListQuery query = currentChannel.CreatePreviousMessageListQuery();

		ResetOpenChannelContent();
		query.Load(15, false, (List<BaseMessage> queryResult, SendBirdException e) => {
			if (e != null)
			{
				Debug.Log(e.Code + ": " + e.Message);
				return;
			}

			GameObject btnChannel = Instantiate(messageLeftPrefab) as GameObject;

			foreach (BaseMessage message in queryResult)
			{
				if (message is UserMessage)
				{
					//txtOpenChannelContent.text = txtOpenChannelContent.text + (UserMessageRichText((UserMessage)message) + "\n");
					
					if (message.GetSender().UserId == SendBirdClient.CurrentUser.UserId)
						messagePrefab = Instantiate(messageLeftPrefab) as GameObject;
					else messagePrefab = Instantiate(messageRightPrefab) as GameObject;

					messagePrefab.transform.SetParent(openChannelGridPannel.transform);
					messagePrefab.transform.localScale = Vector3.one;
					messagePrefab.GetComponent<MessageList>().userID.text = message.GetSender().UserId;
					messagePrefab.GetComponent<MessageList>().chatMessage.text = message.Message;
					messagePrefab.GetComponent<MessageList>().txtTime.text = message.UpdatedAt.ToString();
					btnChannels.Add(messagePrefab);

					Debug.LogError(message.GetSender().UserId+":"+message.GetSender().ConnectionStatus);
					
				}
			}
		});

	}

	void GroupChannelList()
	{
		btnGroupChannels.Clear();
		
		foreach (Transform child in groupChannelListConetnt.transform)
		{
			GameObject.Destroy(child.gameObject);
		}

		mGroupChannelListQuery = GroupChannel.CreatePublicGroupChannelListQuery();
		mGroupChannelListQuery.IncludeEmpty = true;
		mGroupChannelListQuery.SetMembershipFilter(0);
		mGroupChannelListQuery.Limit = 100;

		Debug.Log("LoadGroupChannels: " + DateTime.Now.ToString());
		mGroupChannelListQuery.Next((list, e) => {
			if (e != null)
			{
				Debug.Log(e.Code + ": " + e.Message);
				return;
			}

			foreach (GroupChannel groupChannel in list)
			{
				GameObject btnGroupChannel = Instantiate(gameListPrefab) as GameObject;
		
				btnGroupChannel.transform.SetParent(groupChannelListConetnt.transform);
				btnGroupChannel.transform.localScale = Vector3.one;

				TMPro.TMP_Text[] text = btnGroupChannel.GetComponentsInChildren<TMPro.TMP_Text>();
				text[0].text = string.Format("{0}:{1}", groupChannel.Name, GetDisplayMemberNames(groupChannel.Members, groupChannel.Members[0].UserId));
				text[1].text = groupChannel.Members[0].GetMetaData().ContainsKey("rank") == false ? "-": groupChannel.Members[0].GetMetaData()["rank"];
				text[2].text = groupChannel.Members[0].Nickname;

				Text text1 = btnGroupChannel.GetComponentInChildren<Text>();
				text1.text = groupChannel.Url;

				GroupChannel final = groupChannel;

				if (groupChannel.Url.Contains(SendBirdClient.CurrentUser.UserId))
				{
					text[3].text = "Cancel";
					btnGroupChannel.GetComponentInChildren<Button>().onClick.AddListener(() => {
						final.DeleteChannel(final.Url, new GroupChannel.GroupChannelLeaveHandler((SendBirdException e3) =>
						{
							btnCreateRoom.gameObject.SetActive(true);
							txtMessage.text = "";
							myChannel = null;
						}));
					});
				}
				else
				{
					text[3].text = "Play";  // statusPlay
					btnGroupChannel.GetComponentInChildren<Button>().onClick.AddListener(() => {
							myChannel = groupChannel;
							myChannel.Join(new GroupChannel.GroupChannelJoinHandler((SendBirdException e4) =>
							{
								SendMessageData(currentChannel, "StartGame", groupChannel.Members[0].UserId, final);
								SendBirdClient.RemoveChannelHandler("default");
								SceneManager.LoadScene("MainScene");
							}));
					});
				}

				btnGroupChannels.Add(btnGroupChannel);

			}

		});
	}

	string UserMessageRichText(UserMessage message)
	{
		//string trans = message.Translations["en"].ToString();
		return "<color=YELLOW>" + message.GetSender().Nickname + ": </color>" + message.Message;
	}

	void ScrollToBottom()
	{
		openChannelScrollbar.value = 0;
	}

	private string GetDisplayMemberNames(List<Member> members, string creatorId)
	{
		if (members.Count < 2)
		{
			return "Waiting..";
		}
		else if (members.Count == 2)
		{
			StringBuilder names = new StringBuilder();
			foreach (var member in members)
			{
				if (creatorId.Equals(member.UserId))
				{
					continue;
				}
				names.Append(", " + member.Nickname);
			}
			return (string)names.Remove(0, 2).ToString();
		}
		else
		{
			return "Group " + members.Count;
		}
	}
}
