using System;
using System.Collections;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;

namespace NetworkThread.Multiplayer
{
    public class Client
    {
        public NetClient client { get; set; }
        private bool connected = false;
        private MonoBehaviour _uiScripts;
        private string _username = "";
        public bool loadBasicUserInfo { get; set; } = false;
        
        
        public Client()
        {
            var config = new NetPeerConfiguration("game")
            {
                AutoFlushSendQueue = false
            };
            
            client = new NetClient(config);
            client.Start();
            client.RegisterReceivedCallback(ReceiveMessage);
            
        }

        public void ReceiveMessage(object peer)
        {
            NetIncomingMessage message;
            
            while ((message = client.ReadMessage()) != null)
            {
                Debug.Log("Message received from server");
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        Debug.Log("Server found");
                        client.Connect(message.SenderEndPoint);
                        break;
                    
                    case NetIncomingMessageType.Data:
                        byte type = message.ReadByte();
                        
                        if (Enum.IsDefined(typeof(PacketTypes.General), type))
                        {
                            HandleGeneralPacket((PacketTypes.General)type, message);
                        }
                        else if (Enum.IsDefined (typeof(PacketTypes.Shop), type))
                        {
                            HandleShopPacket((PacketTypes.Shop)type, message);
                        }
                        else
                        {
                            Debug.Log("Unhandled message type");
                        }
                        break;
                    
                    case NetIncomingMessageType.StatusChanged:
                        if (message.SenderConnection.Status == NetConnectionStatus.Connected)
                        {
                            Debug.Log("Connected to server");
                            connected = true;
                        }
                        else if (message.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        {
                            Debug.Log("Disconnected from server");
                            connected = false;
                            loadBasicUserInfo = false;
                        } else if (message.SenderConnection.Status == NetConnectionStatus.InitiatedConnect)
                        {
                            Debug.Log("Initiated connection to server");
                        }
                            
                        break;
                    
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        Debug.Log(message.ReadString());
                        break;
                    
                    default:
                        Debug.Log("Unhandled message type");
                        break;
                    
                }
                client.Recycle(message);
            }
        }

        public void DiscoveryServer()
        {
            client.DiscoverLocalPeers(14242);
        }
        
        
        private void HandleShopPacket(PacketTypes.Shop type, NetIncomingMessage message)
        {
            
            throw new NotImplementedException();
        }

        private void HandleGeneralPacket(PacketTypes.General type, NetIncomingMessage message)
        {
            Packet packet;
            switch (type)
            {
                /*
                case (int)PacketTypes.PlayerDisconnectsPacket:
                    packet = new PlayerDisconnectsPacket();
                    packet.NetIncomingMessageToPacket(message);
                    DisconnectPlayer((PlayerDisconnectsPacket)packet);
                    break;
                */
                            
                            
                case PacketTypes.General.Login:
                    Debug.Log("Type: Received Login Packet");
                    
                    packet = new Login();
                    packet.NetIncomingMessageToPacket(message);
                    var loginManager = (LoginScenesScript)_uiScripts;
                    if (((Login)packet).isSuccess)
                    {
                        loginManager.LoginSuccess();
                    }
                    else
                    {
                        loginManager.LoginFail();
                        _username = "";
                    }
                    
                    break;
                
                case PacketTypes.General.SignUp:
                    Debug.Log("Type: Received Sign Up Packet");
                    
                    packet = new SignUp();
                    packet.NetIncomingMessageToPacket(message);
                    
                    var signUpManager = (LoginScenesScript)_uiScripts;
                    if (((SignUp)packet).isSuccess)
                    {
                        signUpManager.SignUpSuccess();
                    }
                    else
                    {
                        signUpManager.SignUpFail(((SignUp)packet).reason);
                    }
                    
                    break;
                
                case PacketTypes.General.BasicUserInfoPacket:
                    Debug.Log("Type: Received BasicUserInfo Packet");
                    packet = new BasicUserInfoPacket();
                    packet.NetIncomingMessageToPacket(message);
                    var mainMenuScript = (MainMenu)_uiScripts;
                    mainMenuScript.SetDisplayNameTMP(((BasicUserInfoPacket)packet).displayName);
                    mainMenuScript.SetCoinsTMP(((BasicUserInfoPacket)packet).coin);
                    loadBasicUserInfo = true;
                    break;
                
                default:
                    Debug.Log($"Unhandled Packet type: {type}");
                    break;
            }
        }

        public void RequestBasicUserInfo()
        {
            NetOutgoingMessage message = client.CreateMessage();
            new BasicUserInfoPacket()
            {
                userName = _username,
                coin = 0,
                displayName = ""
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            Debug.Log("Sending request basic user info to server");
        }
        

        public void SendDisconnect()
        {
            NetOutgoingMessage message = client.CreateMessage();
            new PlayerDisconnectsPacket()
            {
                username = _username
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();

            client.Disconnect("Bye!");
        }
        /*
         public void DisconnectPlayer(PlayerDisconnectsPacket packet)
        {
            Debug.Log("Removing player " + packet.player);

            MonoBehaviour.Destroy(GameStaticManager.Players[packet.player]);
            GameStaticManager.Players.Remove(packet.player);
        }
         */
        public void SendLoginPackage(string username, string password)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new Login()
            {
                username = username,
                password = password,
                isSuccess = false
            }.PacketToNetOutGoingMessage(message);
            this._username = username;
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            
            Debug.Log("Sending login package to server");
            
        }

        public void SendSignUpPackage(string username, string password)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new SignUp()
            {
                username = username,
                password = password,
                isSuccess = false,
                reason = ""
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            
            Debug.Log("Sending Sign Up package to server");
            
        }

        public bool IsConnected()
        {
            return connected;
        }

        public void SetUiScripts(MonoBehaviour uiScripts)
        {
            _uiScripts = uiScripts;
        }
        
        public void GetScriptNameNow()
        {
            if (_uiScripts is LoginScenesScript)
            {
                Debug.Log("Script hiện tại là LoginScenesScript");
            }
            else if (_uiScripts is MainMenu)
            {
                Debug.Log("Script hiện tại là MainMenu");
            }
            else
            {
                Debug.LogError("Không nhận diện được script");
            }
        }

    }
}