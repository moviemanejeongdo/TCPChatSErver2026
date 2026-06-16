using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace TCPChatServer;

public class ChatServer
{
    // TCP 프로토콜을 사용해서 클라이언트가 접속하기를 대기하는 클래스 : Listener Server
    // 소켓 (Socket)
    private TcpListener? _listener;
    
    // 서버 실행 여부
    private bool _isRunning;
    
    // 서버 포트
    private int _port;
    
    // 연결된 클라이언트를 저장할 스레드 환경에서 사용하는 딕셔너리(Thread-Safe Dictionary)
    private readonly ConcurrentDictionary<string, ConnectedClient> _connectedClients;
    
    // 생성자 (Constructor)
    public ChatServer(int port)
    {
        _port = port;
        _isRunning = false;
        _connectedClients = new ConcurrentDictionary<string, ConnectedClient>();
    }
    
    // 서버 시작
    public void StartServer()
    {
        if (_isRunning)
        {
            Console.WriteLine("서버가 이미 실행 중 입니다.");
            return;
        }
        
        // TCP 소켓을 OPEN : TCP 리스너를 초기화 한다.
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
        _isRunning = true;
        
        // 백그라운드 스레드로 클라이언 연결을 수락시작
        _ = Task.Run(AcceptClientAsync);
        
        Console.WriteLine($"서버가 정상적으로 실행되었습니다. 포트:{_port}");
    }
    
    // 서버 종료
    public void StopServer()
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;
        _listener?.Stop();
        _listener = null;
        
        Console.WriteLine("서버가 종료되었습니다.");
    }
    
    // 클라이언 연결 (접속 요청) 비동기 처리
    private async Task AcceptClientAsync()
    {
        Console.WriteLine("클라이언트 접속을 기다리는 중 ...");
        // 무한 루프를 돌면서 접속 요청을 대기처리
        while (_isRunning)
        {
            try
            {
                // 클라이언트의 연결 요청할 때 까지 대기
                var client = await _listener!.AcceptTcpClientAsync();
                
                // 접속한 클라이언트 저장
                var connectedClient = new ConnectedClient(client);
                // 클라이언트 목록에 추가
                // _connectedClients.TryAdd(connectedClient.ClientId, connectedClient);
                // Indexer 방식
                _connectedClients[connectedClient.ClientId] = connectedClient;
                
                // 메시지 수신 시 브로드캐스트를 위한 이벤트 연결(구독)
                connectedClient.MessageReceived += OnMessageReceived;
                // 접속 종료 이벤트 구독
                connectedClient.Disconnected += OnClientDisconnected;
                
                // 클라이언트로 부터 메시지 수신 시작(비동기)
                _ = Task.Run(connectedClient.ReceiveMessageAsync);
                
                // 접속한 클라이언트 수 출력
                Console.WriteLine($"[정보] 현재 접속한 클라이언트 수 : {_connectedClients.Count}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    private void OnClientDisconnected(string clientId)
    {
        // 딕셔너리에서 접속 해제된 클라이언트 정보 삭제
        if (_connectedClients.TryRemove(clientId, out var client))
        {
            client.MessageReceived -= OnMessageReceived;
            client.Disconnected -= OnClientDisconnected;
            
            Console.WriteLine($"[연결종료] 클라이언트 {client.ClientId}가 접속 종료되었습니다.");
            Console.WriteLine($"[정보] 현재 접속한 클라이언트 수 : {_connectedClients.Count}");
        }
    }

    private void OnMessageReceived(ConnectedClient sender, string message)
    {
        // 메시지 포맷 
        // [발신자 ID] 메시지
        
        string msg = $"[{sender.ClientId}]  {message}";
        
        // 모든 클라이언트에게 메시지 브로드캐스팅
        _ = Task.Run(() => BroadcastMessageAsync(msg));
    }

    private async Task BroadcastMessageAsync(string message)
    {
        foreach (var client in _connectedClients.Values)
        {
            if (client.IsConnected)
            {
                // 전송 메시지 호출
                await client.SendMessageAsync(message);
            }
        }
    }
}
