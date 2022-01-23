# Sendbird Unity Game Template
![Platform](https://img.shields.io/badge/platform-UNITY%20%7C%20.NET-orange.svg)
![Languages](https://img.shields.io/badge/language-C%23-orange.svg)

## Introduction
어몽어스와 같은 실시간 온라인 게임을 만들기 위해 온라인 게임서버가 필요할까요? 요즘은 덕몽어스(Goose Goose Duck)가 유행하죠.<br />
여기 온라인 채팅서버만을 이용해서 카드 트레이딩 게임(TCG)을 만들 수 있는 게임 템플릿이 있습니다.<br />
어몽어스와 같은 실시간 채팅과 실시간 게임 역시 센드버드의 채팅 API를 이용해서 만들 수 있습니다.<br />
게임 개발사나 개발자는 자신의 게임 서버 없이 채팅 서버의 API만으로 실시간 온라인 게임을 만들 수 있습니다.

[Sendbird](https://sendbird.com) 채팅을 위한 서비스뿐 아니라 소켓통신을 이용해서 온라인 게임으로 활용할 수 있는 샘플을 개인적인 호기심으로 만들게 되었습니다.

센드버드에 가입해서 채팅 서비스 아이디만 등록하면 바로 온라인 게임을 테스트 할수 있습니다.<br />
PC로 빌드해서 테스트 하거나 모바일로 빌드해서 유니티 툴과 1:1로 온라인 게임을 지금 테스트해 보세요.

- **게임 샘플** <br />
로그인, 로비, 게임화면으로 구성되어 있으며 게임방을 만들고 참여하여 실시간으로 게임을 할 수 있도록 구성되어 있습니다.<br />
완성된 게임 로직이 아니므로 TCG 게임을 즐길 수는 없지만 온라인 게임을 완성하기 위한 기본 요소들이 코딩되어 있습니다.<br />
아래에 YouTube 영상으로 확인 해 보세요.

 https://www.youtube.com/watch?v=JoIHQfmtFm8
<br /> https://www.youtube.com/watch?v=HSQlxe0_aSo

아래는 센드버드 API는 아니지만 1인 축구게임을(유니티 에셋 구매) 채팅 API를 이용해서 핑거 축구 온라인 게임으로 직접 만든 영상입니다.
<br />
https://www.youtube.com/watch?v=L9NDFUhd2dA <br />
SignalR은 모든 코딩을 직접해야 하고 대규모의 온라인 게임을 서비스하기 위해서 개발자가 할 일이 너무 많습니다.<br />

채팅과 채팅 API를 사용한 또 다른 방법입니다. 웹에서 채팅을 하면서 학생들과 선생님이 그림을 그리면서 가르치는 장면을 만들어 보았습니다<br />
그림을 그리는 화면을 공유하는 것도 채팅 소켓 데이터 전송으로 작업하는 것입니다.<br />
2분쯤에는 디자인 회사가 원격으로 채팅과 화면을 보면서 업무를 이야기 하는 장면을 연출 했습니다. 물론 센드버드 Calls를 이용하면 화상회의도 가능합니다.<br />
https://www.youtube.com/watch?v=rJ4mJCprVuA
<br />
This sample is written in C# with [Sendbird Chat SDK](https://github.com/sendbird/SendBird-SDK-dotNET).

<br />

## Installation

Unity 샘플을 사용하려면 먼저 [Unity용 Chat SDK](https://github.com/sendbird/SendBird-SDK-dotNET)를 설치해야 하지만 이미 소스 폴더의 /Assets/SDK에 dll 파일 두 개가 있습니다.
지금은 필요 없지만 만일 SDK가 업데이트 된다면 이 파일을 덮어 씌우는 것 만으로 됩니다.

### Requirements
2020.3.25f1, 2019.3.12f1 으로 테스트 되었으며  `Unity 2017.4.0f1` 이상의 모든 버전에서 작동될 것입니다.

### 게임 채팅 앱 생성

[Start your free trial](https://dashboard.sendbird.com/auth/signup) 여기에서 무료로 가입하여 채팅 앱 아이디를 생성할 수 있습니다.<br />
SendBirdUnity.cs 코드에서  SendBirdClient.Init("Your App ID") `APP_ID` 를 [Sendbird Dashboard](https://dashboard.sendbird.com) 에서 찾아 넣기만 하면 모든 준비는 완료됩니다.<br />

Application ID  75822989-C2A3-5103-A416-2354AAC1DB01  이런 형식의 아이디 입니다.

## 추가 팁

free trial 계정을 만들 때 "Continue with Google"로 만들면 브라우저에서 구글 로그인이 되어 있으면 언제든지 접속이 가능합니다.<br />
사용하시는 일반 이메일로 생성하시고 패스워드를 잃어 버린 경우 여러번 비밀번호를 잘못 입력하면 등록 이메일로 재설정 메일이 오게 됩니다.<br />
한 달간 무료로 사용할 수 있으며 한 달 후 다시 테스트 하고 싶으면 Security > Delete account 에서 앱을 삭제하고 다시 만드시면 됩니다.

