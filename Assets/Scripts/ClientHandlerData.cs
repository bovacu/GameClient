using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ClientHandlerData {
    public delegate Response pckt_ (byte[] _data);
    public static Dictionary<int, pckt_> packetListener;
    private static int pcktLength;
    private static ByteBuffer byteBuffer;

    public static void initPacketListener() {
        ClientHandlerData.packetListener = new Dictionary<int, pckt_>();
        ClientHandlerData.packetListener.Add((int)ServerPckts.WELCOME_MSG, handleWelcomeMsg);
        ClientHandlerData.packetListener.Add((int)ServerPckts.SERVER_RESPONSE, handleServerResponse);
        ClientHandlerData.packetListener.Add((int)ServerPckts.GET_PLAYER_INFO, handleGetPlayerInfo);
        ClientHandlerData.packetListener.Add((int)ServerPckts.ADDED_TO_MATCH, handleAddedToMatch);
        ClientHandlerData.packetListener.Add((int)ServerPckts.PLAYER_JOINED_MATCH, handlePlayerJoinedMatch);
    }

    public static Response handleData(byte[] _data) {
        byte[] _buffer = (byte[])_data.Clone();

        if (ClientHandlerData.byteBuffer == null) {
            ClientHandlerData.byteBuffer = new ByteBuffer();
        }

        ClientHandlerData.byteBuffer.writeBytes(_buffer);

        if (ClientHandlerData.byteBuffer.count() == 0) {
            ClientHandlerData.byteBuffer.clear();
            return Response.ERROR;
        }

        // We check if is just an empty packet (4 bytes of the int) or a packet with information.
        if (ClientHandlerData.byteBuffer.length() >= 4) {
            ClientHandlerData.pcktLength = ClientHandlerData.byteBuffer.readInteger(false);

            // No packet or invalid packet
            if (ClientHandlerData.pcktLength <= 0) {
                ClientHandlerData.byteBuffer.clear();
                return Response.ERROR;
            }
        }

        while (ClientHandlerData.pcktLength > 0 & ClientHandlerData.pcktLength <= ClientHandlerData.byteBuffer.length() - 4) {
            if (ClientHandlerData.pcktLength <= ClientHandlerData.byteBuffer.length() - 4) {
                ClientHandlerData.byteBuffer.readInteger();
                _data = ClientHandlerData.byteBuffer.readBytes(ClientHandlerData.pcktLength);
                return handleDataPacket(_data);
            }

            ClientHandlerData.pcktLength = 0;
            if (ClientHandlerData.byteBuffer.length() >= 4) {
                ClientHandlerData.pcktLength = ClientHandlerData.byteBuffer.readInteger(false);

                if (ClientHandlerData.pcktLength <= 0) {
                    ClientHandlerData.byteBuffer.clear();
                    return Response.ERROR;
                }
            }

            if (ClientHandlerData.pcktLength <= 1)
                ClientHandlerData.byteBuffer.clear();
        }

        return Response.ERROR;
    }

    private static Response handleDataPacket(byte[] _data) {
        ByteBuffer _byteBuffer = new ByteBuffer();
        _byteBuffer.writeBytes(_data);
        int _pcktId = _byteBuffer.readInteger();

        if (ClientHandlerData.packetListener.TryGetValue(_pcktId, out pckt_ _pckt))
            return _pckt.Invoke(_data);

        return Response.ERROR;
    }

    private static Response handleWelcomeMsg(byte[] _data) {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);

        int _pcktId = _buffer.readInteger();
        string _msg = _buffer.readString();

        Debug.Log("Correclty connected to the server");
        return Response.OK;
    }

    private static Response handleServerResponse(byte[] _data) {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);

        int _pcktId = _buffer.readInteger();
        int _response = _buffer.readInteger();

        return (Response)_response; 
    }

    /// <summary>
    /// This method sets GlobalInfo.playerInfo.currencyAmount and GlobalInfo.playerInfo.reports INSIDE.
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    private static Response handleGetPlayerInfo(byte[] _data) {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);

        int _pcktId = _buffer.readInteger();
        GlobalInfo.playerInfo.currencyAmount = _buffer.readInteger();
        GlobalInfo.playerInfo.reports = _buffer.readInteger();

        return Response.OK;
    }

    private static Response handleAddedToMatch(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();

        var _numberOfPlayers = _buffer.readInteger();
        
        
        return Response.OK;
    }

    private static Response handlePlayerJoinedMatch(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();
        
        
        
        return Response.OK;
    }
}
