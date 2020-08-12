using System;
using System.Diagnostics;
using System.Net.Sockets;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using Debug = UnityEngine.Debug;

public class ClientTCP {
    private const float EXPIRATION_TIME = 10.0f;
    private const int MAX_BUFFER_SIZE = 4096;
    private static bool canGetResponse = false;
    private static Response serverResponse = Response.NONE_RESPONSE;

    private static TcpClient clientSocket;
    private static NetworkStream inOutStream;
    private static byte[] buffer;

    private static UnityThread thread;

    public static void initClientSocket(String _address, int _port) {
        ClientTCP.clientSocket = new TcpClient();
        ClientTCP.clientSocket.ReceiveBufferSize = MAX_BUFFER_SIZE;
        ClientTCP.clientSocket.SendBufferSize = MAX_BUFFER_SIZE;

        // Because we are sending and receiving at the same time over the same stream.
        ClientTCP.buffer = new byte[MAX_BUFFER_SIZE * 2];

        ClientTCP.clientSocket.BeginConnect(_address, _port, new AsyncCallback(onClientConnect), ClientTCP.clientSocket);
        
    }

    public static bool isConnectionActive() {
        return ClientTCP.clientSocket.Connected;
    }

    private static void onClientConnect(IAsyncResult _result) {
        clientSocket.EndConnect(_result);
        if (!ClientTCP.clientSocket.Connected)
            return;
        else {
            ClientTCP.inOutStream = ClientTCP.clientSocket.GetStream();
            ClientTCP.inOutStream.BeginRead(ClientTCP.buffer, 0, MAX_BUFFER_SIZE * 2, onReceive, null);
            
            // // The welcome message
            // ClientTCP.getResponseFromServer("primero");
        }
    }

    public static Response getResponseFromServer(string _debugging = "") {
        
        var _t = new Thread(() => {
            DateTime _start = DateTime.Now;

            while (((DateTime.Now - _start).TotalMilliseconds / 1000f) < 5f && !canGetResponse) {
                Thread.SpinWait(100);
            }

            Debug.Log($"Sale del bucle: {_debugging} con tiempo {((DateTime.Now - _start).TotalMilliseconds / 1000f)}");
            if (((DateTime.Now - _start).TotalMilliseconds / 1000f) <= 5f) return;
            Debug.Log("Devuelve el error: " + _debugging);
            serverResponse = Response.EXPIRATION_TIME_ERROR;
            canGetResponse = true;

        });
        _t.Start();
        
        while (!canGetResponse) {  }
        _t.Interrupt();
        Debug.Log($"Obtiene: {_debugging}");

        canGetResponse = false;
        return serverResponse;
    }

    private static void onReceive(IAsyncResult _result) {
        try {
            int _bytesRead = ClientTCP.inOutStream.EndRead(_result);

            if (_bytesRead <= 0) 
                return;

            byte[] _newData = new byte[_bytesRead];
            Buffer.BlockCopy(ClientTCP.buffer, 0, _newData, 0, _bytesRead);

            canGetResponse = false;
            ClientTCP.serverResponse = ClientHandlerData.handleData(_newData);
            canGetResponse = true;

            ClientTCP.inOutStream.BeginRead(ClientTCP.buffer, 0, MAX_BUFFER_SIZE * 2, onReceive, null);
        } catch(Exception) {  }
    }

    private static void sendData(byte[] _data) {
        if(ClientTCP.isConnectionActive()) {
            var _byteBuffer = new ByteBuffer();
            _byteBuffer.writeInteger(_data.GetUpperBound(0) - _data.GetLowerBound(0) + 1);
            _byteBuffer.writeBytes(_data);
            ClientTCP.inOutStream.Write(_byteBuffer.toArray(), 0, _byteBuffer.toArray().Length);
            _byteBuffer.dispose();
        }
    }

    public static void sendPacketThanksWelcome() {
        // 1. Create ByteBuffer
        var _buffer = new ByteBuffer();

        // 2. Add packet ID.
        _buffer.writeInteger((int)ClientPckts.THANKS_WELCOME);

        // 3. Write desired info.
        _buffer.writeString("Thanks!");
        
        // 4. Send the data.
        sendData(_buffer.toArray());
    }

    public static void sendPacketAppVersion() {
        // 1. Create ByteBuffer
        var _buffer = new ByteBuffer();

        // 2. Add packet ID.
        _buffer.writeInteger((int)ClientPckts.APP_VERSION);

        // 3. Write desired info.
        _buffer.writeString(InnerFileReader.getProperty("appVersion"));

        // 4. Send the data.
        sendData(_buffer.toArray());
    }

    public static void sendPacketRegisterAccount(string _user, string _password) {
        // 1. Create ByteBuffer
        var _buffer = new ByteBuffer();

        // 2. Add packet ID.
        _buffer.writeInteger((int)ClientPckts.REGISTER_ACCOUNT); 

        // 3. Write desired info.
        _buffer.writeString(_user);
        _buffer.writeString(_password);

        // 4. Send the data.
        sendData(_buffer.toArray());
    }

    public static void sendPacketLogin(string _user, string _password) {
        // 1. Create ByteBuffer
        var _buffer = new ByteBuffer();

        // 2. Add packet ID.
        _buffer.writeInteger((int)ClientPckts.LOGIN);

        // 3. Write desired info.
        _buffer.writeString(_user);
        _buffer.writeString(_password);

        // 4. Send the data.
        sendData(_buffer.toArray());
    }

    public static void sendPacketPlayerInfo() {
        // 1. Create ByteBuffer
        var _buffer = new ByteBuffer();

        // 2. Add packet ID.
        _buffer.writeInteger((int)ClientPckts.GET_PLAYER_INFO);

        // 3. Write desired info.
        _buffer.writeString("P.I.");

        // 4. Send the data.
        sendData(_buffer.toArray());
    }

    public static void sendPacketSearchMatch(TypeOfGame _typeOfGame) {
        // 1. Create ByteBuffer
        var _buffer = new ByteBuffer();

        // 2. Add packet ID.
        _buffer.writeInteger((int)ClientPckts.SEARCH_MATCH);

        // 3. Write desired info.
        _buffer.writeInteger((int)_typeOfGame);

        // 4. Send the data.
        sendData(_buffer.toArray());
    }

    public static void sendPacketQuitApp() {
        var _buffer = new ByteBuffer();

        // 2. Add packet ID.
        _buffer.writeInteger((int)ClientPckts.QUIT);

        // 3. Write desired info.
        _buffer.writeString("bye");

        // 4. Send the data.
        sendData(_buffer.toArray());
    }
}
