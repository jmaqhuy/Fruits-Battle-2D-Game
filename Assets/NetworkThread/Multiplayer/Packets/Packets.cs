using System;
using System.Collections.Generic;
using Lidgren.Network;
using NetworkThread.Multiplayer.Packets;
using RoomEnum;
using UnityEngine;

namespace NetworkThread.Multiplayer
{
    public class PacketTypes
    {
        public enum General : byte
        {
            Login = 0,
            SignUp,
            PlayerDisconnectsPacket,
            BasicUserInfoPacket,
            ChangeDisplayNamePacket,
            Logout,
            ResetPassword,
            VerifyRegistrationPacket,
            RequireVerifyPacket
        }

        public enum Shop : byte
        {
            LoadShopPacket = 10,
            RequestBuyPacket,
        }
        
        public enum Room : byte
        {
            JoinRoomPacket = 20,
            JoinRoomPacketToAll,
            ExitRoomPacket,
            InviteFriendPacket,
            SendChatMessagePacket,
            PlayerReadyPacket,
            RoomListPacket,

        }
        public enum GameBattle : byte
        {
            StartGamePacket= 30,
            PlayerOutGamePacket,
            StartTurnPacket,
            EndTurnPacket,
            EndGamePacket,
            PositionPacket,
            HealthPointPacket,
            PlayerDiePacket,
            Shoot,
            SpawnPlayerPacketToAll,
            AlreadyInMatchPacket
        }

        public enum Friend : byte
        {
            AllFriendPacket = 50,
            FriendRequestPacket,
            SentRequestPacket,
            SuggestFriendPacket,
            SearchFriendPacket,
            BlockFriendPacket,
        }

        public enum Character : byte
        {
            GetCurrentCharacterPacket = 60,
        }

        
    }
    public interface IPacket
    {
        void PacketToNetOutGoingMessage(NetOutgoingMessage message);

        void NetIncomingMessageToPacket(NetIncomingMessage message);
    }

    public abstract class Packet : IPacket
    {
        public abstract void PacketToNetOutGoingMessage(NetOutgoingMessage message);

        public abstract void NetIncomingMessageToPacket(NetIncomingMessage message);
    }

    public class SendChatMessagePacket : Packet
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public int RoomID { get; set; }
        public string Message { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.SendChatMessagePacket);
            message.Write(Username);
            message.Write(DisplayName);
            message.Write(RoomID);
            message.Write(Message);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Username = message.ReadString();
            DisplayName = message.ReadString();
            RoomID = message.ReadInt32();
            Message = message.ReadString();
        }
    }
    
    

    /*public class FriendOnlinePacket : Packet
    {
        public string username;
        public string displayName;
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.FriendOnlinePacket);
            message.Write(username);
            message.Write(displayName);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            displayName = message.ReadString();
        }
    }*/

    public class InviteFriendPacket : Packet
    {
        public string username;
        public string displayName;
        public RoomPacket room;
        public string friendUserName;
        public string friendDisplayName;

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.InviteFriendPacket);
            message.Write(username);
            message.Write(displayName);
            room.Serialize(message);
            message.Write(friendUserName);
            message.Write(friendDisplayName);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            displayName = message.ReadString();
            room = RoomPacket.Deserialize(message);
            friendUserName = message.ReadString();
            friendDisplayName = message.ReadString();
        }
    }

    public class Login : Packet
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool isSuccess { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            password = message.ReadString();
            isSuccess = message.ReadBoolean();
            Debug.Log($"NetIncomingMessageToPacket: username: {username}, password: {password}, isSuccess: {isSuccess}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.Login);
            message.Write(username);
            message.Write(password);
            message.Write(isSuccess);
            Debug.Log($"PacketToNetOutGoingMessage: username: {username}, password: {password}, isSuccess: {isSuccess}");
        }
    }

    public class Logout : Packet
    {
        public string username;

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.Logout);
            message.Write(username);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
        }
    }

    public class ResetPassword : Packet
    {
        public string username { get; set; }
        public string email { get; set; }
        public bool isSuccess { get; set; }
        public string reason { get; set; }


        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.ResetPassword);
            message.Write(username);
            message.Write(email);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            isSuccess = message.ReadBoolean();
            reason = message.ReadString();
        }
    }

    public class VerifyRegistrationPacket : Packet
    {
        public string username { get; set; }
        public string otp { get; set; }
        public bool isSuccess { get; set; }
        public string reason {get;set;}
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.VerifyRegistrationPacket);
            message.Write(username);
            message.Write(otp);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            isSuccess = message.ReadBoolean();
            reason = message.ReadString();
        }
    }

    public class SignUp : Packet
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool isSuccess { get; set; }
        public string reason { get; set; }
        
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            isSuccess = message.ReadBoolean();
            reason = message.ReadString();
            Debug.Log($"NetIncomingMessageToPacket: username: {username}, isSuccess: {isSuccess}, reason: {reason}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.SignUp);
            message.Write(username);
            message.Write(email);
            message.Write(password);
            Debug.Log($"PacketToNetOutGoingMessage: username: {username}, email: {email}, password: {password}");
        }
    }
    public class PlayerDisconnectsPacket : Packet
    {
        public string username { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.PlayerDisconnectsPacket);
            message.Write(username);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
        }
    }

    public class BasicUserInfoPacket : Packet
    {
        public string userName { get; set; }
        public int coin { get; set; }
        public string displayName { get; set; }


        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            userName = message.ReadString();
            coin = message.ReadInt32();
            displayName = message.ReadString();
            Debug.Log($"NetIncomingMessageToPacket: username: {userName}, coin: {coin}, displayName {displayName}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.BasicUserInfoPacket);
            message.Write(userName);
            message.Write(coin);
            message.Write(displayName);
            Debug.Log($"PacketToNetOutGoingMessage: username: {userName}, coin: {coin}, displayName {displayName}");
        }
    }
    
    public class ChangeDisplayNamePacket : Packet
    {
        public string username { get; set; }
        public string newDisplayName { get; set; }
        public string error {  get; set; }
        
        public bool Ok { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            newDisplayName = message.ReadString();
            error = message.ReadString();
            Ok = message.ReadBoolean();
            Debug.Log($"NetIncomingMessageToPacket: username: {username}, newDisplayName: {newDisplayName}, error: {error}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.ChangeDisplayNamePacket);
            message.Write(username);
            message.Write(newDisplayName);
            message.Write(error);
            message.Write(Ok);
            Debug.Log($"PacketToNetOutGoingMessage: username: {username}, newDisplayName: {newDisplayName}, error: {error}");
        }
    }
    
    public class JoinRoomPacket : Packet
    {
        public RoomPacket room { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            room = RoomPacket.Deserialize(message);
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.JoinRoomPacket);
            room.Serialize(message);
        }
    }
    
    public class RoomListPacket : Packet
    {
        public List<RoomPacket> rooms { get; set; } = new List<RoomPacket>();

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.RoomListPacket);
            message.Write(rooms.Count);
            foreach (var r in rooms)
            {
                r.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            var roomCount = message.ReadInt32();
            rooms.Clear();
            for (int i = 0; i < roomCount; i++)
            {
                rooms.Add(RoomPacket.Deserialize(message));
            }
        }
    }

    public class ExitRoomPacket : Packet
    {
        public string username { get; set; }
        public int roomId { get; set; }
        
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.ExitRoomPacket);
            message.Write(username);
            message.Write(roomId);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            roomId = message.ReadInt32();
        }
    }
    
    public class JoinRoomPacketToAll : Packet
    {
        public List<PlayerInRoomPacket> Players { get; set; } = new List<PlayerInRoomPacket>();
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            int playerCount = message.ReadInt32();
            Players.Clear();
            for (int i = 0; i < playerCount; i++)
            {
                Players.Add(PlayerInRoomPacket.Deserialize(message));
            }
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.JoinRoomPacketToAll);
            message.Write(Players.Count);
            foreach (var p in Players)
            {
                p.Serialize(message);
            }   
        }
    }

    public class SuggestFriendPacket : Packet
    {
        public string username { get; set; }
        public List<FriendTabPacket> Friends { get; set; } = new List<FriendTabPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.SuggestFriendPacket);
            message.Write(username);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            int friendCount = message.ReadInt32();
            Friends.Clear();
            for (int i = 0; i < friendCount; i++)
            {
                Friends.Add(FriendTabPacket.Deserialize(message));
            }
        }
    }

    public class GetCurrentCharacterPacket : Packet
    {
        public string Username { get; set; }
        public CharacterPacket Character { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Character.GetCurrentCharacterPacket);
            message.Write(Username);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Character = CharacterPacket.Deserialize(message);
        }
    }

    public class PlayerReadyPacket : Packet
    {
        public string Username { get; set; }
        public bool IsReady { get; set; }
        public int RoomId { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.PlayerReadyPacket);
            message.Write(Username);
            message.Write(IsReady);
            message.Write(RoomId);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Username = message.ReadString();
            IsReady = message.ReadBoolean();
            RoomId = message.ReadInt32();
        }
    }

    public class StartGamePacket : Packet
    {
        public int roomId { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.StartGamePacket);
            message.Write(roomId);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            roomId = message.ReadInt32();
        }
    }
    public class PlayerOutGamePacket : Packet
    {
        public string player { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            player = message.ReadString();
        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.PlayerOutGamePacket);
            message.Write(player);
        }
    }
    public class EndTurnPacket : Packet
    {
        public string playerName { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            playerName = message.ReadString();
        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.EndTurnPacket);
            message.Write(playerName);
        }
    }
    public class EndGamePacket : Packet
    {
        public Team TeamWin {  get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            TeamWin = (Team)message.ReadByte();
        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.EndGamePacket);
        }
    }
    public class PositionPacket : Packet
    {
        public string playerName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            playerName = message.ReadString();
            X = message.ReadFloat();
            Y = message.ReadFloat();

        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.PositionPacket);
            message.Write(playerName);
            message.Write(X);
            message.Write(Y);

        }
    }
    public class HealthPointPacket : Packet
    {
        public int HP { get; set; }
        public string PlayerName { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            HP = message.ReadInt32();
            PlayerName = message.ReadString();
        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.HealthPointPacket);
            message.Write(HP);
            message.Write(PlayerName);
        }
    }
    public class PlayerDiePacket : Packet
    {
        public string player { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            player = message.ReadString();

        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.PlayerDiePacket);
            message.Write(player);
        }
    }
    public class SpawnPlayerPacketToAll : Packet
    {
        public List<SpawnPlayerPacket> SPPacket = new List<SpawnPlayerPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.SpawnPlayerPacketToAll); 
            message.Write(SPPacket.Count);
            foreach (SpawnPlayerPacket sp in SPPacket)
            {
                sp.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            var cnt = message.ReadInt32();
            SPPacket.Clear();
            for (int i = 0; i < cnt; i++)
            {
                SPPacket.Add(SpawnPlayerPacket.Deserialize(message));
            }
        }
    }
    public class StartTurnPacket : Packet
    {
        public string playerName { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.StartTurnPacket);

        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            playerName = message.ReadString();
        }

    }
    public class Shoot : Packet
    {
        public string playerName { get; set; }
        public float force { get; set; }
        public float angle { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.Shoot);
            message.Write(force);
            message.Write(angle);
            message.Write(X);
            message.Write(Y);
            message.Write(playerName);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {

            force = message.ReadFloat();
            angle = message.ReadFloat();
            X = message.ReadFloat();
            Y = message.ReadFloat();
            playerName = message.ReadString();
        }

    }
    public class AlreadyInMatchPacket : Packet
    {
        public int roomId { get; set; }
        public string username { get; set; }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            roomId = message.ReadInt32();
            username = message.ReadString();
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.AlreadyInMatchPacket);
            message.Write(roomId);
            message.Write(username);
        }
    }
}
