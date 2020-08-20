using System.Collections.Generic;
using System.Security.Cryptography;
using Games;
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
        ClientHandlerData.packetListener.Add((int)ServerPckts.MATCH_STARTS, handleMatchStarts);
        ClientHandlerData.packetListener.Add((int)ServerPckts.SENDING_CARD, handleReceivingCard);
        ClientHandlerData.packetListener.Add((int)ServerPckts.SENDING_LIST_CARDS, handleReceivingCardList);
        ClientHandlerData.packetListener.Add((int)ServerPckts.CARDS_PER_PLAYER_AND_INITIAL_TURN, handleCardsPerPlayerAndInitialTurn);
        ClientHandlerData.packetListener.Add((int)ServerPckts.TEST_GAME_UPDATE, handleTestGameUpdate);
        ClientHandlerData.packetListener.Add((int)ServerPckts.HAND_CARDS_UPDATE, handleHandCardsUpdate);
        ClientHandlerData.packetListener.Add((int)ServerPckts.PLAYER_FINISHED, handlePlayerFinished);
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
        GlobalInfo.playerInfo.id = _buffer.readInteger();

        Debug.Log($"Correclty connected to the server with id {GlobalInfo.playerInfo.id}");
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

        var _playersIds = new List<int>();
        for(var _i = 0; _i < _numberOfPlayers; _i++)
            _playersIds.Add(_buffer.readInteger());
 
        var _playersNames = new List<string>();
        for(var _i = 0; _i < _numberOfPlayers; _i++)
            _playersNames.Add(_buffer.readString());

        for (var _i = 0; _i < _numberOfPlayers; _i++)
            GlobalInfo.otherPlayers.Add(new GlobalInfo.OtherPlayer(_playersIds[_i], _playersNames[_i]));

        GlobalInfo.playerInfo.matchId = _buffer.readInteger();
        GlobalInfo.playerInfo.inMatch = true;
        
        Debug.LogError("HANDLED ADDED TO MATCH.");

        return Response.ADDED_TO_MATCH;
    }

    private static Response handlePlayerJoinedMatch(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();

        var _playerId = _buffer.readInteger();
        var _playerName = _buffer.readString();

        GlobalInfo.otherPlayers.Add(new GlobalInfo.OtherPlayer(_playerId, _playerName));
        
        Debug.LogError("HANDLED PLAYER JOINED.");
        
        return Response.PLAYER_JOINED_MATCH;
    }

    private static Response handleMatchStarts(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();

        // None sense info.
        _buffer.readString();
        
        Debug.LogError("HANDLED MATCH STARTS.");
        
        return Response.LOAD_MATCH_SCENE;
    }

    private static Response handleReceivingCard(byte[] _data) {
        Debug.LogError("NOT YET IMPLEMENTED CORRECTLY");
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();
        var _value = _buffer.readInteger();
        var _suit = (Suit) _buffer.readInteger();
        var _card = new CardInfo(_value, _suit);
        Debug.Log($"{_value} of {_suit}");

        return Response.OK;
    }

    private static Response handleReceivingCardList(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();

        var _numberOfCards = _buffer.readInteger();

        for (var _i = 0; _i < _numberOfCards; _i++) {
            var _value = _buffer.readInteger();
            var _suit = (Suit) _buffer.readInteger();
            GlobalInfo.playerCards.Add(new CardInfo(_value, _suit));
        }

        Debug.LogError("HANDLED CARD LIST RECEIVED.");
        
        return Response.RECEIVED_CARD_LIST;
    }

    private static Response handleCardsPerPlayerAndInitialTurn(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();

        for (var _i = 0; _i < GlobalInfo.otherPlayers.Count; _i++) {
            var _playerId = _buffer.readInteger();
            var _numberOfCards = _buffer.readInteger();
            GlobalInfo.otherPlayersCardCount.Add(_playerId, _numberOfCards);
        }

        var _turn = _buffer.readInteger();
        GlobalInfo.isMyTurn = GlobalInfo.playerInfo.id == _turn;

        var _cardOnTableValue = _buffer.readInteger();
        var _cardOnTableSuit = (Suit)_buffer.readInteger();
        
        GlobalInfo.game = new TestGame(_cardOnTableValue, _cardOnTableSuit);
        
        Debug.LogError("HANDLED CARDS PER PLAYER AND INITIAL TURN.");

        return Response.RECEIVED_CARDS_PER_PLAYER_AND_TURN;
    }

    private static Response handleHandCardsUpdate(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();

        var _whoUpdated = _buffer.readInteger();
        var _howMany = _buffer.readInteger();

        GlobalInfo.otherPlayersCardCount[_whoUpdated] += _howMany;
        GameManager.updateEnemyHand = true;
        return Response.OK;
    }
    
    private static Response handleTestGameUpdate(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();

        var _whoMadeTheUpdateId = _buffer.readInteger();
        var _cardValue = _buffer.readInteger();
        var _cardSuit = (Suit)_buffer.readInteger();
        var _nextTurn = _buffer.readInteger();

        GlobalInfo.isMyTurn = GlobalInfo.playerInfo.id == _nextTurn;

        if (_cardValue >= 0) {
            GameManager.whoMadeTheUpdate = _whoMadeTheUpdateId;
            GlobalInfo.otherPlayersCardCount[_whoMadeTheUpdateId] = GlobalInfo.otherPlayersCardCount[_whoMadeTheUpdateId] - 1;
            TestGame _testGame = (TestGame) GlobalInfo.game;
            _testGame.getCardOnTable().Value = _cardValue;
            _testGame.getCardOnTable().Suit = _cardSuit;
            GameManager.newUpdateInGame = GlobalInfo.isMyTurn;
        }else
            GameManager.justTurnUpdate = true;

        return Response.TEST_GAME_UPDATE;
    }

    private static Response handlePlayerFinished(byte[] _data) {
        var _buffer = new ByteBuffer();
        _buffer.writeBytes(_data);
        var _packetId = _buffer.readInteger();

        var _winnerId = _buffer.readInteger();

        GameManager.winner = _winnerId;
        
        return Response.OK;
    }
    
}
