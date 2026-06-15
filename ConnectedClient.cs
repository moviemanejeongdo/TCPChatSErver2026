using System.Text;

namespace TCPChatServer;

using System.Net.Sockets;

public class ConnectedClient : IDisposable
{
    // TCP 클라이언트 객체
    private readonly TcpClient _client;
    // 네트워크 스트림
    private readonly NetworkStream _stream;
    // 읽기 전용 스트림 (수신)
    private readonly StreamReader _reader;
    // 쓰기 전용 스트림 (송신)
    private readonly StreamWriter _writer;
    
    // 클라이언트 ID
    private readonly string _clientId;
    // 연결 종료 여부
    private bool _isDisposed;
    
    // 클라이언트 ID 프로퍼티
    public string ClientId => _clientId;
    // 클라이언트 연결 여부
    public bool IsConnected => !_isDisposed && _client.Connected;

    // 생성자
    public ConnectedClient(TcpClient client)
    {
        _client = client;
        _stream = client.GetStream();
        
        // 스트림 UTF-8 초기화
        _reader = new StreamReader(_stream, Encoding.UTF8);
        _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
        
        _clientId = _client.Client.RemoteEndPoint?.ToString() ?? Guid.NewGuid().ToString();
        _isDisposed = false;
        
        Console.WriteLine($"[연결] 클라이언트가 접속했습니다.: {_clientId}");
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _client.Dispose();
        _stream.Dispose();
    }
}
