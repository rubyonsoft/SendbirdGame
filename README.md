# Sendbird Unity Game Template
![Platform](https://img.shields.io/badge/platform-UNITY%20%7C%20.NET-orange.svg)
![Languages](https://img.shields.io/badge/language-C%23-orange.svg)

## Introduction
어몽어스와 같은 실시간 온라인 게임을 만들기 위해 온라인 게임서버가 필요할까요? 요즘은 덕몽어스(Goose Goose Duck)가 유행하죠.
여기 온라인 채팅서버만을 이용해서 카드 트레이딩 게임(TCG)을 만들 수 있는 게임 템플릿이 있습니다.
어몽어스와 같은 실시간 채팅과 실시간 게임 역시 센드버드의 채팅 API를 이용해서 만들 수 있습니다.
게임 개발사나 개발자는 자신의 게임 서버 없이 채팅 서버의 API만으로 실시간 온라인 게임을 만들 수 있습니다.

[Sendbird](https://sendbird.com) 센드버드의 채팅 서비스는 몇 년전에 게임 개발시 채팅이 필요해서 검토를 한적이 있었는데 이제는 센드버드를 다니고 있습니다.
채팅을 위한 서비스뿐 아니라 소켓통신을 이용해서 온라인 게임으로 활용할 수 있는 샘플을 개인적인 호기심으로 만들게 되었습니다.

센드버드에 가입해서 채팅 서비스 아이디만 등록하면 바로 온라인 게임을 테스트 해 보세요.
PC로 빌드해서 테스트 하거나 모바일로 빌드해서 유니티 툴과 1:1로 지금 테스트해 보실 수 있습니다.

- **게임 샘플** 
로그인, 로비, 게임화면으로 구성되어 있으며 게임방을 만들고 참여하여 실시간으로 게임을 할 수 있도록 구성되어 있습니다.
완성된 게임 로직이 아니므로 TCG 게임을 즐길 수는 없지만 온라인 게임을 완성하기 위한 기본 요소들이 코딩되어 있습니다.
아래에 YouTube 영상으로 확인 해 보세요.
https://www.youtube.com/watch?v=JoIHQfmtFm8
https://www.youtube.com/watch?v=HSQlxe0_aSo

아래는 센드버드 API는 아니지만 1인 축구게임을(유니티 에셋 구매) 채팅 API를 이용해서 핑거 축구 온라인 게임으로 직접 만든 영상입니다.
https://www.youtube.com/watch?v=L9NDFUhd2dA
SignalR은 모든 코딩을 직접해야 하고 대규모위 온라인 게임을 서비스하기 위해서 개발자가 할 일이 너무 많습니다.

This sample is written in C# with [Sendbird Chat SDK](https://github.com/sendbird/SendBird-SDK-dotNET).

<br />

## Installation

To use our Unity samples, you should first install [Chat SDK for Unity](https://github.com/sendbird/SendBird-SDK-dotNET) on your system.

### Requirements

This sample project is tested on `Unity 2017.4.0f1`.

### Chat sample

The sample project works based on our test application's `APP_ID`. You should replace with your own Sendbird application's `APP_ID` which can be found in [Sendbird Dashboard](https://dashboard.sendbird.com).

### Try the sample app using your data 

If you would like to try the sample app specifically fit to your usage, you can do so by replacing the default sample app ID with yours, which you can obtain by [creating your Sendbird application from the dashboard](https://sendbird.com/docs/chat/v3/unity/getting-started/chat-sdk-setup#2-step-1-create-a-sendbird-application-from-your-dashboard). Furthermore, you could also add data of your choice on the dashboard to test. This will allow you to experience the sample app with data from your Sendbird application. 

<br />

## Previous versions

You can access the version 2 sample from the repository by switching from `master` to `v2` branch.
