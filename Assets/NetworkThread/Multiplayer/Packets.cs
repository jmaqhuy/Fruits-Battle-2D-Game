using System;
using Lidgren.Network;
using UnityEngine;

namespace NetworkThread.Multiplayer
{
    public class PacketTypes
    {
        public enum General : byte
        {
            Login,
            SignUp,
            PlayerDisconnectsPacket,
            BasicUserInfoPacket
        }

        public enum Shop : byte
        {
            LoadShopPacket,
            RequestBuyPacket,
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
}
