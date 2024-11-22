using System;
using System.Threading;
using Lidgren.Network;
using UnityEngine;

namespace UI.Scripts.Game.Multiplayer
{
    public class Client
    {
        public NetClient client { get; set; }
        public LoginManage loginManager;
        public SignUpManage signUpManager;
        private bool connected = false;
        public Client(int port, string serverName, LoginManage loginManager, SignUpManage signUpManager)
        {
            /*
            var config = new NetPeerConfiguration(serverName)
            {
                AutoFlushSendQueue = false
            };
            
            client = new NetClient(config);
            
            client.RegisterReceivedCallback(ReceiveMessage);
            
            client.Start();
            client.Connect(server, port);
            this.loginManager = loginManager;
            this.signUpManager = signUpManager;
            */
            
            var config = new NetPeerConfiguration(serverName)
            {
                AutoFlushSendQueue = false
            };
            
            client = new NetClient(config);
            client.RegisterReceivedCallback(ReceiveMessage);
            client.Start();
            client.Connect("192.168.0.107", port);
            //DiscoverServers(port);
            this.loginManager = loginManager;
            this.signUpManager = signUpManager;
        }
    /*
        private void DiscoverServers(int port)
        {
            NetIncomingMessage message;
            var timeout = Time.time + 5f;
            Debug.Log("Starting discover server");
            client.DiscoverLocalPeers(port);

            while (Time.time < timeout)
            {
                while ((message = client.ReadMessage()) != null)
                {
                    if (message.MessageType == NetIncomingMessageType.DiscoveryResponse)
                    {
                        Debug.Log("Server found");
                        client.Connect(message.SenderEndPoint);
                        connected = true;
                        Debug.Log("Connected to server");
                        return;
                    }
                    client.Recycle(message);
                }
            }
            Debug.Log("Timeout: No server found");
        } */

        public void ReceiveMessage(object peer)
        {
            NetIncomingMessage message;

            while ((message = client.ReadMessage()) != null)
            {
                Debug.Log("Message received from server");
                switch (message.MessageType)
                {
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
                                
                    if (((Login)packet).isSuccess)
                    {
                        loginManager.LoginSuccess();
                    }
                    else
                    {
                        loginManager.LoginFail();
                    }
                    break;
                
                case PacketTypes.General.SignUp:
                    Debug.Log("Type: Received Sign Up Packet");
                    packet = new SignUp();
                    packet.NetIncomingMessageToPacket(message);
                    if (((SignUp)packet).isSuccess)
                    {
                        signUpManager.SignUpSuccess();
                    }
                    else
                    {
                        signUpManager.SignUpFail(((SignUp)packet).reason);
                    }
                    break;
                default:
                    Debug.Log($"Unhandled Packet type: {type}");
                    break;
            }
        }

        public void SendDisconnect()
        {
            NetOutgoingMessage message = client.CreateMessage();
            new PlayerDisconnectsPacket(){ player = GameStaticManager.LocalPlayerID }.PacketToNetOutGoingMessage(message);
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
        
    }
}