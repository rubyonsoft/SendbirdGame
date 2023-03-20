using UnityEngine;
using UnityEngine.UI;

using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using SendBird;
using System.Linq;
using SendBird.SBJson;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
	public TMPro.TMP_Text txtInfo;
	public Button btnLogin;
	public Button btnNew;
	public Button btnCreate;
	public GameObject PanelJoin;
	public GameObject imgWait;

	public TMPro.TMP_InputField txtLoginId;
	public TMPro.TMP_InputField txtPassword;
	public TMPro.TMP_InputField txtPassword2;
	public TMPro.TMP_InputField txtNickName;

	private bool isNew = false;
	private string loginID;

	private OpenChannelListQuery mChannelListQuery;
	
	public static BaseChannel currentChannel;

	public GameObject channelListItemPrefab;
	GameObject channelGridPannel;
	ArrayList btnChannels = new ArrayList();

	void Start()
	{
		channelGridPannel = GameObject.Find("Canvas/OpenChannelListPanel/ScrollArea/GridPanel");

		btnLogin.onClick.AddListener(() =>
		{
			isNew = false;
			Login();
		});

		btnNew.onClick.AddListener(() =>
		{
			isNew = true;
			PanelJoin.SetActive(true);
		});

		btnCreate.onClick.AddListener(() =>
		{
			isNew = true;
			imgWait.SetActive(true);
			Login();
		});

		loginID = PlayerPrefs.GetString("UserID");
		string password = PlayerPrefs.GetString("Password");
		Debug.Log("loginID:" + loginID + " password:" + password);

		if (loginID != "")
		{
			txtLoginId.text = loginID;
			//txtLoginId.enabled = false;
			txtPassword.text = password;
			isNew = false;
			PanelJoin.SetActive(false);
		}
		else
		{
			isNew = true;
			PanelJoin.SetActive(true);
		}
	}

	void Login()
	{
		string AccessToken = ""; // if use accessToken, server is required
		Dictionary<string, string> data = new Dictionary<string, string>();

		if (txtPassword.text.Length < 5) {
			txtInfo.text = "Short password! 5 or more characters";
			return; }
		string password = txtPassword.text;
		
		if (loginID != "")
		{
			SendBirdClient.Connect(txtLoginId.text, AccessToken, (user, e) =>
			{
				if (e != null)
				{
					Debug.Log(e.Code + ": " + e.Message);
					return;
				}
				Debug.LogError("CurrentUser:" + SendBirdClient.CurrentUser.UserId);

				data = user.GetMetaData();
				if (!data.ContainsKey("password"))
				{
					txtInfo.text = "Wrong Password, temporary password 11111";

					data.Add("password", "11111");
					user.CreateMetaData(data, (map, e1) =>
					{
					});

					PlayerPrefs.SetString("UserID", txtLoginId.text);
					PlayerPrefs.SetString("Password", txtPassword.text);
					PlayerPrefs.SetString("NickName", txtNickName.text);
					PlayerPrefs.Save();

					return;
				}

				password = data["password"];
				if (txtPassword.text != password)
				{
					SendBirdClient.Disconnect(() =>
					{
						SendBirdClient.RemoveChannelHandler("default");
					});

					txtInfo.text = "Wrong Password";
					return;
				}

				imgWait.SetActive(false);
				OpenOpenChannelList();
				// 직접 채널 입력시
				//OpenChannel.GetChannel("sendbird_open_channel_2500_d355bd7407b710fc191f3853475c816895d1969b", new OpenChannel.OpenChannelGetHandler((channel, e1) =>
				//{
				//	try
				//	{
				//		if (e1 != null)
				//		{
				//		}

				//		currentChannel = channel;
				//		//SceneManager.LoadScene("Lobby");

				//	}
				//	catch (Exception z)
				//	{
				//		Debug.Log(z);
				//	}
				//}));
			});
		}
		else
		{
			if (txtNickName.text.Length < 5 || txtPassword2.text.Length < 5) return;
			SendBirdClient.Connect(txtLoginId.text, (user, e) =>
			{
				if (e != null)
				{
					Debug.Log(e.Code + ": " + e.Message);
					return;
				}

				data.Add("password", txtPassword.text);

				user.CreateMetaData(data, (map, e1) =>
				{
					if (e1 != null)
					{
						Debug.Log(e1.Code + ": " + e1.Message);
						return;
					}
				});

				SendBirdClient.UpdateCurrentUserInfo(txtNickName.text, null, (e1) =>
				{
					if (e1 != null)
					{
						Debug.Log(e.Code + ": " + e.Message);
						return;
					}
				});

				PlayerPrefs.SetString("UserID", txtLoginId.text);
				PlayerPrefs.SetString("Password", txtPassword.text);
				PlayerPrefs.SetString("NickName", txtNickName.text);
				PlayerPrefs.Save();

				OpenOpenChannelList();
				imgWait.SetActive(false);
			});
		}			
	}

	void OpenOpenChannelList()
	{
		mChannelListQuery = OpenChannel.CreateOpenChannelListQuery();
		mChannelListQuery.Limit = 50;

		mChannelListQuery.Next((list, e) => {
			if (e != null)
			{
				Debug.Log(e.Code + ": " + e.Message);
				return;
			}

			foreach (OpenChannel channel in list)
			{
				GameObject btnChannel = Instantiate(channelListItemPrefab) as GameObject;

				Text text = btnChannel.GetComponentInChildren<Text>();
				text.text = "#" + channel.Name;
				btnChannel.transform.SetParent(channelGridPannel.transform);
				btnChannel.transform.localScale = Vector3.one;
				btnChannels.Add(btnChannel);

				OpenChannel final = channel;
				btnChannel.GetComponent<Button>().onClick.AddListener(() => {

					final.Enter((e1) => {
						if (e1 != null)
						{
							Debug.Log(e1.Code + ": " + e1.Message);
							return;
						}
						currentChannel = final;
						SceneManager.LoadScene("Lobby");
					});
				});
			}
		});
	}

}
