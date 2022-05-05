# AR_Video_Streaming

## Table of Content:
 * [Overview](#overview)
 * [Installation](#installation)
 * [Dependencies](#dependencies)
   * [WebRTC](#webrtc)
   * [Render Streaming](#renderstreaming)
* [Streaming ARFoundation Camera to browser](#ar-camera-streaming)
* [Sending Web browser Input to Unity](#sending-web-browser-input-to-unity)
* [Applying RayCasting in Unity](#RayCasting)
* [Web Server](#webserver)
* [Browser Client](#browser-client)

>## Overview

This Project aims to apply remote guidance using Augmented Reality annotation on a Video stream between the web browser and an android phone. The android application streams an [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html) scene to the browser which is responsible for plane detection, then the browser can annotate that stream with AR objects e.g. Arrows. 


&nbsp;
&nbsp;

>## Installation
#### Building the Unity application:
1. Clone this repository `https://github.com/MohammedShetaya/AR_Video_Streaming.git`
2. Open project from disk using Unity Hub
3. After the project is open, check if the Build setting is using Android platform
4. On the player settings, check if the Auto graphics API is checked, buIld the project for ARM64, and Android version is 7.0 or above.
5. On the mobile phone, Allow developer mode, then allow USB debugging and Connect the device to the labtop.
6. On Build settings, click build and run.

#### Setting up the Web application:
1. Navigate to the project directory using powershell, then `cd WebApp`, then run the command `npm run dev -- -w`
2. the console will show a link to the local web application

#### Using the Application:
There are two modes, the first one is receiving a video steam from unity and showing this stream on the browser:
1. Open the application on mobile and allow access to the camera.
2. Then go back to the local web app on the browser and click receiver sample.
3. Click on play video button, then the stream should be started.
 
The second mode is video receiver with AR annotation:
1. Open the application on mobile and allow access to the camera.
2. Then go back to the local web app on the browser and click video player.
3. Click on play video button, then the stream should be started.
4. By now the mobile application should have started detecting the planes and the video stream is sent to the browser.
5. Click on any of the detected planes on the browser, then an arrow should point to the click position on both clients.

&nbsp;
&nbsp;

>## Dependencies
The project is based on:
- [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html) protocol for the browser and unity (version `2.4.0-exp.6`). 
- [UnityRenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html) package (version `3.1.0-exp.3`).


### [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html):
This package is an API for WebRTC protocol, but in unity with the same browser implementation which gives a great benifit in using this protocol in unity AR and VR applications. This package is compitble with the browser API so it can be used to allow real-time, peer-to-peer, media exchange between unity-unity application or unity-browser application. A connection is established through a discovery and negotiation process called signaling. The signaling between two peers is not supported by WebRTC protocol because every peer is connecting to the Internet behind a [NAT](https://en.wikipedia.org/wiki/Network_address_translation) so each peer has no information about his public IP address therefore each peer cannot give his IP to the other peer. The solution for this is to use a signaling server. 

#### Signaling Server: 
The signaling server acts as an interface between the Unity Android application and the browser clients so they can Start sending signaling messages to each other. Using an HTTP server would not help in the case of WebRTC as the signaling messages are being generated asyncrounosly, so it is optimal to use a WebSocket server. WebSocket connection is statefull (FullDuplex) connection. Unlike the HTTP connection where the server cannot send responses to the client unless the client sends a request. Websocket servers can send and receive requests at any moment in the connection lifetime. In the case of WebRTC, the server will never know when a client will send a signaling message so it can be forwarded to the other client.

#### Singnaling Process:
1. The browser client sends and Offer message to the websocket server
2. The Signaling server receives the message from the broswer and forwards it to the Unity client
3. The Unity client receives the offer and set this Offer as its RemoteDescription
4. The Unity client create an Answer and set this Answer as its LocalDescription
5. The Unity client sends the Answer to the browser client
7. The Signaling server receives the message from the broswer and forwards it to the Unity client
8. The browser client receives the Answer and set this Answer as its RemoteDescription
9. The Two clients register to the `onIceCandidate` event and once the event handler is called, it should send the collected iceCandidate to the other Peer.
10. Once the other Peer receives an iceCandidate, it should call `AddIceCanidate` in order to set the [SDP](https://en.wikipedia.org/wiki/Session_Description_Protocol).

![Signaling Process Browser API](./webrtc_signaling_diagram.svg) 

#### P2P Connection:
Once the two peers set their `LocalDescription` and `RemoteDescription` They can start exchaning real-time data (Video, Audio, etc..). A `MediaStream` object can be sent over the `RTCPeerConnection` using the `AddTrack` method. The other peer can register to the `OnTrack` event which is will be called once a track is received.

##### Remarks:
1. The `MediaStream` object should be added using the `AddTrack` method before sending an Offer/Answer to the other peer. Adding a track should be followed by new Offer/Answer in order for the other peer to have an updated SDP.
2. The `IceCandidate` should be handled on the remote peer after the `RemoteDescritption` is set.
3. The sending peer needs to register to `OnNegotiationNeeded` event which is called once `AddTrack` finishes execution. The handler of this event should send a new Offer to the remote peer with the new SDP. 

&nbsp;
&nbsp;

### [RenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html)
Unity Render streaming is based on the WebRTC protocol. It provides a high level implementation for the sinaling, sending, and receiving process. It allows streaming real-time data on a peer to peer connection using WebRTC. This package also allows sending input data from the browser to Unity by maping the browser Events to Unity Actions. With this package, it is possible to build streaming applications in Unity for both Windows and Andriod.

&nbsp;
&nbsp;

>## AR Camera Streaming 
In order to stream the Unity camera. The following components must be added to the scene:
1. ARSession Origin: This is an origin to the scene when the player start the application. It is responisble for managing all the Trackables that will be added on the run e.g. 3D cubes.
2. AR Session: Controls the lifecycle and configuration options for an AR session

The following scripts are added to the arCamera component which is a subComponent of the ARSession origin component:
#### Rendertreaming: 
This is the base class for the Unity render streaming package. It is responsible for connecting to the signaling server and streaming the provided real-time data. It supports two types of Signaling (HTTP/WebSocket). In this project the Websocket signaling were used as dicussed in the WebRTC part above. This script is expecting inputs of type `SignalingHandlerBase` which is a parent class for the `BroadCast` class used in this project.

#### Broadcast: 
This script is responsible for handling the singlaing messages (offer, answer, ice-candidate) and Sending the input streams to be used in `RenderStreaming` script. This class is expecting inputs of type `StreamSenderBase` which is the parent class for the ARCameraSender used in this project.

#### ARCameraSender: 
This script extends from Unity.RenderStreaming.VideoStreamSender class. It is responsible for sending the video stream as a RenderTexture. This script changes the TargetTexture of the camera to be a RenderTexture instead of rendering to the screen. It should be attached to the arCamera object.

#### CameraTextureMixer : 
This script is responsible for creating a copy from the Rendered image from the camera and print this image to a `RenderTexture` which will be used in the ARCameraSender script. This script should be attached to the arCamera object and should be used only if no other arCamera is used for screen rendering.

#### WSClient (not used)
This script is sample script that holds all the logic for sending and receiving a messages between the Unity client and the Web Server and it was used in the project early stages only.

&nbsp;
&nbsp;

>## Sending Web browser Input To Unity:
In this project the only browser input that is used is the mouse click, although it can be extendend to any browser event. Once the video is received from unity and is shown on the browser video element, the user can start clicking on any place on that video element. The click coordinates is sent to unity as a buffer array of bytes. The following files are used in this part: 

`WepApp/client/public/js/register-events.js`: in this file there are functions for event handling. The only used function is `registerMouseEvents` which is responsible for sending click events to untiy through a prviously created `RTCDataChannel`. Then function is being called in `WepApp/client/public/videoPlayer.js` file on the creation of the video element.

![Target Click coordinates](./TargetClick.png) 


&nbsp;
&nbsp;

>## RayCasting:

&nbsp;
&nbsp;

>## Web Server:

&nbsp;
&nbsp;


>## Browser Client

