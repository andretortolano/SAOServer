using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerClient
{
    public int connectionId;
    public string playerName;
}

public class Server : MonoBehaviour
{
    private const int MAX_USER = 100;
    private const int PORT = 62000;
    private const int WEB_PORT = 62001;
    private const int BYTE_SIZE = 1024;

    private byte reliableChannel;
    private byte unreliableChannel;

    private int hostId;
    private int webHostId;

    private bool isStarted = false;
    private byte error;

    private List<ServerClient> clients = new List<ServerClient>();

    #region MonoBehavior
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    public void Init()
    {
        NetworkTransport.Init();

        ConnectionConfig connConfig = new ConnectionConfig();
        reliableChannel = connConfig.AddChannel(QosType.Reliable);
        unreliableChannel = connConfig.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(connConfig, MAX_USER);

        // Server only code
        hostId = NetworkTransport.AddHost(topo, PORT, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);

        Debug.Log(string.Format("Opening connection on port {0} and web port {1}.", PORT, WEB_PORT));
        isStarted = true;
    }
    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    public void UpdateMessagePump()
    {
        if(!isStarted)
            return;

        int recHostId; // Is this from Web? Or standalone
        int connectionId; // Which user is sending me this?
        int channelId; // Which lane is he sending that message from

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);

        switch(type)
        {
            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("User {0} has connected through host {1}!", connectionId, recHostId));
                OnConnection(connectionId, recHostId);
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("User {0} has disconnected. ;(", connectionId));
                break;

            case NetworkEventType.DataEvent:
                OnData(connectionId, channelId, recHostId, NetMessageSerializer.Deserialize(recBuffer));
                break;

            case NetworkEventType.Nothing:
                break;

            default:
                Debug.Log("Received Event of Type: " + type);
                break;
        }
    }

    #region OnData
    private void OnData(int connectionId, int channelId, int recHostId, NetMessage netMessage)
    {
        switch(netMessage.Code)
        {
            case NetCodes.None:
                Debug.Log("NetOperation Code: NONE");
                break;

            //case NetOperationCode.CreateAccount:
            //    CreateAccount(connectionId, channelId, recHostId, (NetCreateAccount)netMessage);
            //    break;
        }
    }

    private void OnConnection(int cnnId, int recHostId)
    {
        // Add him to a list
        GameObject playerObject = GetComponent<PlayersManager>().SpawnNewPlayer(cnnId);

        // tell him his Id
        NetClientSpawned message = new NetClientSpawned();
        PlayerDTO playerModel = new PlayerDTO();
        playerModel.id = cnnId;
        playerModel.posX = playerObject.transform.position.x;
        playerModel.posY = playerObject.transform.position.y;
        playerModel.posZ = playerObject.transform.position.z;
        message.Player = playerModel;
        SendClient(cnnId, recHostId, message, true);

        // broadcast this Id to all others
    }
    #endregion

    #region Send
    public void SendClient(int connectionId, int recHostId, NetMessage netMessage, bool reliable = true)
    {
        int host = recHostId == 0 ? hostId : webHostId;
        int channel = reliable ? reliableChannel : unreliableChannel;

        NetworkTransport.Send(host, connectionId, channel, NetMessageSerializer.Serialize(netMessage, BYTE_SIZE), BYTE_SIZE, out error);
    }
    #endregion
}
