﻿using System;
using System.Collections.Generic;
using Lidgren.Network;
using NetworkThread.Multiplayer.Packets;
using RoomEnum;
using UnityEngine;
using static NetworkThread.Multiplayer.PacketTypes;

namespace NetworkThread.Multiplayer
{
    public class Client
    {
        public NetClient client { get; set; }
        private bool connected = false;
        private MonoBehaviour _uiScripts;
        private MonoBehaviour _invitePopupScripts;
        private string _username = "";
        public bool loadBasicUserInfo { get; set; } = false;
        public RoomMode RoomMode;
        
        
        public Client()
        {
            var config = new NetPeerConfiguration("FruitsBattle2DGame")
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
                        else if (Enum.IsDefined (typeof(PacketTypes.Room), type))
                        {
                            HandleRoomPacket((PacketTypes.Room)type, message);
                        }
                        else if (Enum.IsDefined(typeof(PacketTypes.Friend), type))
                        {
                            HandleFriendPacket((PacketTypes.Friend)type, message);
                        }
                        else if (Enum.IsDefined(typeof(PacketTypes.Character), type))
                        {
                            HandleCharacterPacket((PacketTypes.Character)type, message);
                        }
                        else if (Enum.IsDefined(typeof(PacketTypes.GameBattle), type))
                        {
                            HandleGameBattlePacket((PacketTypes.GameBattle)type, message);
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

        private void HandleGameBattlePacket(PacketTypes.GameBattle type, NetIncomingMessage message)
        {
            string deviceId = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);
            List<NetConnection> players = new List<NetConnection>();
            Packet packet;
            GameBattle script;
            switch (type)
            {
                case PacketTypes.GameBattle.StartGamePacket:

                    break;
                case PacketTypes.GameBattle.SpawnPlayerPacketToAll:
                    Debug.Log("get spawn packet");
                    packet = new SpawnPlayerPacketToAll();
                    packet.NetIncomingMessageToPacket(message);
                    if (_uiScripts is WaitingRoomScript wrscript)
                    {
                        wrscript.ReceiveStartGame((SpawnPlayerPacketToAll)packet);
                    }
                    break;
                case PacketTypes.GameBattle.PlayerOutGamePacket:
                    break;
                case PacketTypes.GameBattle.StartTurnPacket:
                    Debug.Log("get turn packet");
                    packet = new StartTurnPacket();
                    packet.NetIncomingMessageToPacket(message);
                    script = (GameBattle)_uiScripts;
                    script.GetTurn((StartTurnPacket)packet);
                    break;
                case PacketTypes.GameBattle.EndTurnPacket:
                    packet = new EndTurnPacket();
                    packet.NetIncomingMessageToPacket(message);
                    script = (GameBattle)_uiScripts;
                    script.EndTurn((EndTurnPacket)packet);
                    break;
                case PacketTypes.GameBattle.EndGamePacket:
                    packet = new EndGamePacket();
                    packet.NetIncomingMessageToPacket(message);
                    script = (GameBattle)_uiScripts;
                    script.EndGame((EndGamePacket)packet);
                    break;
                case PacketTypes.GameBattle.PositionPacket:
                    packet = new PositionPacket();
                    packet.NetIncomingMessageToPacket(message);
                    script = (GameBattle)_uiScripts;
                    script.UpdatePosition((PositionPacket)packet);
                    break;

                case PacketTypes.GameBattle.HealthPointPacket:
                    packet = new HealthPointPacket();
                    packet.NetIncomingMessageToPacket(message);
                    script = (GameBattle)_uiScripts;
                    script.UpdateHP((HealthPointPacket)packet);
                    break;
                case PacketTypes.GameBattle.PlayerDiePacket:
                    packet = new PlayerDiePacket();
                    packet.NetIncomingMessageToPacket(message);
                    script = (GameBattle)_uiScripts;
                    script.PlayerDie((PlayerDiePacket)packet);
                    break;
                case PacketTypes.GameBattle.Shoot:
                    Debug.Log("get shoot packet");
                    packet = new Shoot();
                    packet.NetIncomingMessageToPacket(message);
                    script = (GameBattle)_uiScripts;
                    script.Shoot((Shoot)packet);
                    break;
                default: break;

            }
        }
        public void SendPlayerDie(string playerName)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new PlayerDiePacket() { player = playerName }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            Debug.Log("Send player die for player" + playerName);
        }
        public void SendEndTurn(string playerName)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new EndTurnPacket() { playerName = playerName }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            Debug.Log("Send end turn for player" + playerName);
        }

        public void SendHPPacket(string playerName, int HP)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new HealthPointPacket() { PlayerName = playerName, HP = HP }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }
        public void SendShootPacket(string playerName, float angle, float force, float X, float Y)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new Shoot() { playerName = playerName, angle = angle, force = force, X = X, Y = Y }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            Debug.Log("Send shoot packet for " + playerName + " angle: " + angle + " force: " + force + " X: " + X + " Y: " + Y);
        }
        public void SendPositionPacket(string playerName, float X, float Y)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new PositionPacket() { playerName = playerName, X = X, Y = Y }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();

        }

        private void HandleCharacterPacket(PacketTypes.Character type, NetIncomingMessage message)
        {
            Packet packet;
            switch (type)
            {
                case PacketTypes.Character.GetCurrentCharacterPacket:
                    packet = new GetCurrentCharacterPacket();
                    packet.NetIncomingMessageToPacket(message);
                    if (_uiScripts is MainMenu mainMenu)
                    {
                        mainMenu.LoadCharacterScene((GetCurrentCharacterPacket)packet);
                    }
                    break;
                default:
                    Debug.Log("Unhandled Character packet type");
                    break;
            }

        }

        private void HandleFriendPacket(PacketTypes.Friend type, NetIncomingMessage message)
        {
            Packet packet;
            FriendSceneScript scriptNow;
            switch (type)
            {
                /*case PacketTypes.Friend.FriendOnlinePacket:
                    packet = new FriendOnlinePacket();
                    packet.NetIncomingMessageToPacket(message);
                    Debug.Log("Friend Online");
                    try
                    {
                        var friendPanel = GameObject.Find("FriendOnline");
                        FriendOnlineStatus friendOnlineStatus = friendPanel.GetComponent<FriendOnlineStatus>();
                        friendOnlineStatus.friendName.text = ((FriendOnlinePacket)packet).displayName;
                    }
                    catch
                    {
                        // ignored
                    }

                    break;*/
                case PacketTypes.Friend.SuggestFriendPacket:
                    packet = new SuggestFriendPacket();
                    packet.NetIncomingMessageToPacket(message);
                    Debug.Log("Suggest friend packet received from server");
                    scriptNow = (FriendSceneScript)_uiScripts;
                    scriptNow.ParseSuggestFriendInfo((SuggestFriendPacket)packet);
                    
                    break;
                    
                default:
                    Debug.Log("Unhandled message type");
                    break;
            }
        }

        

        private void HandleRoomPacket(PacketTypes.Room type, NetIncomingMessage message)
        {
            Packet packet;
            WaitingRoomScript scriptNow;
            switch (type)
            {
                case PacketTypes.Room.JoinRoomPacket:
                    packet = new JoinRoomPacket();
                    packet.NetIncomingMessageToPacket(message);
                    Debug.Log("Room joined");
                    if (_uiScripts is SelectPlayModeScript selectMode1)
                    {
                        selectMode1.ParseRoomInfoData(((JoinRoomPacket)packet).room);
                    } else if (_uiScripts is WaitingRoomScript waitingRoomScript)
                    {
                        waitingRoomScript.PasteRoomInfo(((JoinRoomPacket)packet).room);
                    }
                    break;
                
                case PacketTypes.Room.JoinRoomPacketToAll:
                    packet = new JoinRoomPacketToAll();
                    packet.NetIncomingMessageToPacket(message);
                    if (_uiScripts is SelectPlayModeScript selectMode2)
                    {
                        selectMode2.ParsePlayerInRoomData((JoinRoomPacketToAll)packet);
                    } else if (_uiScripts is WaitingRoomScript waitingRoomScript)
                    {
                        waitingRoomScript.SetUIForAll(((JoinRoomPacketToAll)packet).Players);
                    }
                    break;
                
                case PacketTypes.Room.InviteFriendPacket:
                    packet = new InviteFriendPacket();
                    packet.NetIncomingMessageToPacket(message);
                    ((FriendInviteUI)_invitePopupScripts).FriendName.text = ((InviteFriendPacket)packet).friendDisplayName;
                    ((FriendInviteUI)_invitePopupScripts).roomPacket = ((InviteFriendPacket)packet).room;
                    var modeAndRoomType = ((InviteFriendPacket)packet).room.roomMode + " " +
                                          ((InviteFriendPacket)packet).room.roomType;
                    
                    ((FriendInviteUI)_invitePopupScripts).ModeandRoomType.text = modeAndRoomType;
                    ((FriendInviteUI)_invitePopupScripts).ShowInvite();
                    break;
                case PacketTypes.Room.SendChatMessagePacket:
                    packet = new SendChatMessagePacket();
                    packet.NetIncomingMessageToPacket(message);
                    scriptNow = (WaitingRoomScript)_uiScripts;
                    scriptNow.ReceiveChatMessage(
                        ((SendChatMessagePacket)packet).Username,
                        ((SendChatMessagePacket)packet).DisplayName,
                        ((SendChatMessagePacket)packet).Message);
                    break;
                /*case PacketTypes.Room.GameStartPacket:
                    Debug.Log("recive Game start packet");
                    packet = new GameStartPacket();
                    packet.NetIncomingMessageToPacket(message);
                    scriptNow = (WaitingRoomScript)_uiScripts;
                    scriptNow.LoadMap();
                    scriptNow.SendStartGameInBattle((GameStartPacket)packet);
                    break;*/
                
                case PacketTypes.Room.RoomListPacket:
                    packet = new RoomListPacket();
                    packet.NetIncomingMessageToPacket(message);
                    if (_uiScripts is SelectPlayModeScript selectMode3)
                    {
                        selectMode3.ParseRoomList((RoomListPacket)packet);
                    }
                    break;
                default:
                    Debug.Log("Unhandled Room message type");
                    break;
            }
        }

        public void SendChatMessagePacket(string message, int roomId)
        {
            NetOutgoingMessage msg = client.CreateMessage();
            new SendChatMessagePacket()
            {
                Username = _username,
                Message = message,
                RoomID = roomId
            }.PacketToNetOutGoingMessage(msg);
            Debug.Log($"Sending Chat Message: Username: {_username}, Room ID: {roomId}, message: {message}");
            client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            
        }

        public void SetInvitePopupScripts(FriendInviteUI script)
        {
            _invitePopupScripts = script;
        }

        public void DiscoveryServer()
        {
            client.DiscoverLocalPeers(14242);
            //client.Connect("35.232.232.21", 14242);
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
                        _username = ((Login)packet).username;
                    }
                    else
                    {
                        loginManager.LoginFail();
                        _username = "";
                    }
                    
                    break;
                case PacketTypes.General.RequireVerifyPacket:
                    ((LoginScenesScript)_uiScripts).SignUpSuccess();
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
                    
                    if (_uiScripts is LoginScenesScript loginScript)
                    {
                        loginScript.ParseUserData((BasicUserInfoPacket)packet);
                    }
                    break;
                
                case PacketTypes.General.ChangeDisplayNamePacket:
                    Debug.Log("Type: Received ChangeDisplayName Packet");
                    packet = new ChangeDisplayNamePacket();
                    packet.NetIncomingMessageToPacket(message);

                    if (((ChangeDisplayNamePacket)packet).Ok)
                    {
                        RequestBasicUserInfo();
                    }
                    break;
                
                case PacketTypes.General.ResetPassword:
                    Debug.Log("Type: Received ResetPassword Packet");
                    packet = new ResetPassword();
                    packet.NetIncomingMessageToPacket(message);
                    if (((ResetPassword)packet).isSuccess)
                    {
                        ((LoginScenesScript)_uiScripts).ShowLoginPanel("Check Your Email to get New Password");
                    }
                    else
                    {
                        ((LoginScenesScript)_uiScripts).ResetPasswordFail(((ResetPassword)packet).reason);
                    }
                    break;
                
                case PacketTypes.General.VerifyRegistrationPacket:
                    Debug.Log("Type: Received VerifyRegistration Packet");
                    packet = new VerifyRegistrationPacket();
                    packet.NetIncomingMessageToPacket(message);
                    if (((VerifyRegistrationPacket)packet).isSuccess)
                    {
                        ((LoginScenesScript)_uiScripts).VerificationSuccess();
                    }
                    else
                    {
                        ((LoginScenesScript)_uiScripts).VerificationFail(((VerifyRegistrationPacket)packet).reason);
                    }
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

        public void SendVerifyRegistrationPacket(string un, string otpCode)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new VerifyRegistrationPacket()
            {
                username = un,
                otp = otpCode
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }

        public void SendPlayerReadyPacket(int roomId, bool isReady)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new PlayerReadyPacket()
            {
                Username = _username,
                IsReady = isReady,
                RoomId = roomId
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
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

        public void SendSignUpPackage(string username, string password, string email)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new SignUp()
            {
                username = username,
                email = email,
                password = password
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            
            Debug.Log("Sending Sign Up package to server");
            
        }

        public void SendChangeDisplayNamePacket(string newDisplayName)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new ChangeDisplayNamePacket()
            {
                username = _username,
                newDisplayName = newDisplayName,
                error = "",
                Ok = false
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            Debug.Log("Sending Sign Up package to server");
        }

        public void SendJoinRoomPacket(RoomMode selectedRoomMode, RoomType roomType, int roomId = 0)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new JoinRoomPacket()
            {
                room = new RoomPacket()
                {
                    Id = roomId,
                    roomMode = selectedRoomMode,
                    roomType = roomType
                }
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }
        
        public void SendExitRoomPacket(int roomId)
        {
            Debug.Log("Sending Exit Room Packet to server");
            NetOutgoingMessage message = client.CreateMessage();
            new ExitRoomPacket()
            {
                username = _username,
                roomId = roomId
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }
        
        public void SendLogoutPacket()
        {
            NetOutgoingMessage message = client.CreateMessage();
            new Logout()
            {
                username = _username,
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }
        public void SendSuggestFriendPacket()
        {
            NetOutgoingMessage message = client.CreateMessage();
            new SuggestFriendPacket()
            {
                username = _username,
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }

        public void SendResetPasswordPacket(string username, string email)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new ResetPassword()
            {
                username = username,
                email = email,
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }
        public void SendChangePasswordPacket(string userName, string oldPass, string newPass, string confirmPass)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new ChangePassword()
            {
                username = userName,
                oldPassword = oldPass,
                newPass = newPass,
                confirmPass = confirmPass,
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            
            Debug.Log("Sending PassWord package to server");
        }
        
        

        public void SendStartGamePacket(int rId)
        {
            NetOutgoingMessage message = client.CreateMessage();
            new StartGamePacket()
            {
                roomId = rId
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }

        public bool IsConnected()
        {
            return connected;
        }

        public string GetUsername()
        {
            return _username;
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
            else if(_uiScripts is global::ChangePassword)
            {
                Debug.Log("Here is ChangePassword script");
            }
            else
            {
                Debug.LogError("Không nhận diện được script");
            }
        }


        public void SendCurrentCharacterPacket()
        {
            NetOutgoingMessage message = client.CreateMessage();
            new GetCurrentCharacterPacket()
            {
                Username = _username,
            }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }
        public void SendRoomListPacket()
        {
            NetOutgoingMessage outmsg = client.CreateMessage();
            new RoomListPacket().PacketToNetOutGoingMessage(outmsg);
            client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }
    }
}