using BitManipulation;
using System;
using System.Collections.Generic;
using System.Text;

public static class PacketData
{
    #region PacketType
    public static void Put(this BitWriter bitWriter, PacketType value)
    {
        bitWriter.Put((byte)value);
    }
    public static PacketType GetPacketType(this BitReader bitReader)
    {
        return (PacketType)bitReader.GetPacketRoutingType();
    }
    #endregion PacketType

    #region PacketRoutingType
    public static void Put(this BitWriter bitWriter, PacketRoutingType value)
    {
        bitWriter.Put((byte)value);
    }
    public static PacketRoutingType GetPacketRoutingType(this BitReader bitReader)
    {
        return (PacketRoutingType)bitReader.GetByte();
    }
    #endregion PacketRoutingType

    #region GameInfoType
    public static void Put(this BitWriter bitWriter, GameInfoType value)
    {
        bitWriter.Put((byte)value);
    }
    public static GameInfoType GetGameInfoType(this BitReader bitReader)
    {
        return (GameInfoType)bitReader.GetByte();
    }
    #endregion GameInfoType
}
