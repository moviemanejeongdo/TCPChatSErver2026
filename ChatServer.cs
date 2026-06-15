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
    
    // 생성자 (Constructor)
    public ChatServer(int port)
    {
        _port = port;
        _isRunning = false;
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
                
                // 연결된 클라정보 출력
                var endPoint = client.Client.RemoteEndPoint;
                Console.WriteLine($"[연결] 클라이언트가 접속했습니다.: {endPoint}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
