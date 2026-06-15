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
}
