using UnityEngine;
using UnityEngine.UI;
using Fenrir.Multiplayer;
using TicTacToe.Shared;
using Fenrir.Multiplayer.Rooms;

public class TicTacToeBehaviour : MonoBehaviour
    , IEventHandler<TicTacToeGameStartEvent>
    , IEventHandler<TicTacToeGameFinishEvent>
    , IEventHandler<TicTacToeMoveEvent>
{
    [SerializeField]
    private GameObject _connectionScreen;
    
    [SerializeField]
    private GameObject _ticTacToeScreen;

    [SerializeField]
    private InputField _serverUriInputField;

    [SerializeField]
    private InputField _roomIdInputField;

    [SerializeField]
    private InputField _playerNameInputField;

    [SerializeField]
    private Button _connectButton;


    [SerializeField]
    private Text _playerNameLabel;

    [SerializeField]
    private Text _opponentNameLabel;

    [SerializeField]
    private Button[] _ticTacToeButtons;

    [SerializeField]
    private Text _statusLabel;


    private NetworkClient _networkClient;

    private TicTacToeGameState _state = TicTacToeGameState.WaitingForPlayers;

    private TicTacToePlayerReference _thisPlayer; // This player
    private TicTacToePlayerReference _opponentPlayer; // Opponent player

    private TicTacToePlayerReference _currentPlayer; // Player making a move


    private void Awake()
    {
        _connectButton.onClick.AddListener(OnConnectButtonClicked);

        _connectionScreen.SetActive(true);
        _ticTacToeScreen.SetActive(false);

        for (int i = 0; i < _ticTacToeButtons.Length; i++)
        {
            int numButton = i;
            Button button = _ticTacToeButtons[numButton];
            button.onClick.AddListener(() => OnTicTacToeButtonClicked(numButton));
        }

        SetStatusLabel("Waiting for other player");
    }

    private void Start()
    {
        // Create Network Client
        _networkClient = new NetworkClient(new UnityLogger());

        // Add event listeners
        _networkClient.AddEventHandler<TicTacToeGameStartEvent>(this);
        _networkClient.AddEventHandler<TicTacToeGameFinishEvent>(this);
        _networkClient.AddEventHandler<TicTacToeMoveEvent>(this);
    }


    private void OnDestroy()
    {
        _networkClient.Dispose();
    }

    private async void OnConnectButtonClicked()
    {
        string serverUri = _serverUriInputField.text;
        string roomId = _roomIdInputField.text;
        string playerName = _playerNameInputField.text;

        // Connect 
        ConnectionResponse connectionResponse = await _networkClient.Connect(serverUri);
        if(!connectionResponse.Success)
        {
            Debug.LogError("Connection failed: " + connectionResponse.Reason);
            return;
        }

        // Join room
        RoomJoinResponse joinRoomResponse = await _networkClient.JoinRoom(roomId, playerName);
        if(!joinRoomResponse.Success)
        {
            Debug.LogError("Join room failed: " + joinRoomResponse.Reason);
            return;
        }

        // Show tic tac toe screen
        _ticTacToeScreen.SetActive(true);
        _connectionScreen.SetActive(false);
    }

    private async void OnTicTacToeButtonClicked(int numButton)
    {
        if(_state != TicTacToeGameState.Started || _currentPlayer != _thisPlayer)
        {
            return; // Not my turn, do nothing
        }

        // Disable field (prevent double clicking etc)
        SetButtonsInteractable(false);

        // Send move
        var response = await _networkClient.Peer.SendRequest<TicTacToeMoveRequest, TicTacToeMoveResponse>(new TicTacToeMoveRequest() { Position = (byte)numButton });

        // Check response
        if(!response.Success)
        {
            Debug.LogError("Move failed");
        }

        // Enable field
        SetButtonsInteractable(true);
    }

    public void OnReceiveEvent(TicTacToeGameStartEvent evt)
    {
        // This method is invoked when server sends "Game Start" event

        _state = TicTacToeGameState.Started;

        _thisPlayer = _networkClient.Peer.Id == evt.Player1.PeerId ? evt.Player1 : evt.Player2;
        _opponentPlayer = _networkClient.Peer.Id == evt.Player1.PeerId ? evt.Player2 : evt.Player1;
        
        _playerNameLabel.text = _thisPlayer.Name;
        _opponentNameLabel.text = _opponentPlayer.Name;

        StartTurn(_thisPlayer.PieceType == TicTacToePieceType.X ? _thisPlayer : _opponentPlayer);
    }

    public void OnReceiveEvent(TicTacToeGameFinishEvent evt)
    {
        // This method is invoked when server sends "Game Finish" event

        _state = TicTacToeGameState.Finished;

        // Disable buttons
        foreach (var button in _ticTacToeButtons)
        {
            button.interactable = false;
        }

        // Set status
        if (evt.IsDraw)
        {
            SetStatusLabel("Finished, draw!");
        }
        else
        {
            SetStatusLabel("Finished, winner: " + evt.Winner.Name);
        }
    }

    public void OnReceiveEvent(TicTacToeMoveEvent evt)
    {
        // This method is invoked when server sends "Move" event aka when player makes a move

        var button = _ticTacToeButtons[evt.Position];
        var label = button.GetComponentInChildren<Text>();
        label.text = evt.PieceType.ToString();
        SetStatusLabel("Move made: " + _currentPlayer.Name);

        StartTurn(_currentPlayer == _thisPlayer ? _opponentPlayer : _thisPlayer);
    }

    private void StartTurn(TicTacToePlayerReference nextPlayer)
    {
        _currentPlayer = nextPlayer;

        // Enable/disable buttons
        SetButtonsInteractable(_currentPlayer == _thisPlayer);

        // Set status
        SetStatusLabel(_currentPlayer == _thisPlayer ? "Your turn" : "Opponent turn");
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in _ticTacToeButtons)
        {
            button.interactable = interactable;
        }
    }

    private void SetStatusLabel(string status)
    {
        _statusLabel.text = status;
    }
}
