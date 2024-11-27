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
            ChangeDisplayNamePacket
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
            
        }

        public enum Friend : byte
        {
            FriendOnlinePacket = 30,
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

    public class SignUp : Packet
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool isSuccess { get; set; }
        public string reason { get; set; }
        
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            password = message.ReadString();
            isSuccess = message.ReadBoolean();
            reason = message.ReadString();
            Debug.Log($"NetIncomingMessageToPacket: username: {username}, password: {password}, isSuccess: {isSuccess}, reason: {reason}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.SignUp);
            message.Write(username);
            message.Write(password);
            message.Write(isSuccess);
            message.Write(reason);
            Debug.Log($"PacketToNetOutGoingMessage: username: {username}, password: {password}, isSuccess: {isSuccess}, reason: {reason}");
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
}
