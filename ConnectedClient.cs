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

    // 메시지 수신 이벤트
    public event Action<ConnectedClient, string> MessageReceived;
    // 연결 종료 이벤트
    public event Action<string> Disconnected;
    
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
    
    // 메시지 수신 로직
    public async Task ReceiveMessageAsync()
    {
        try
        {
            while (!_isDisposed && IsConnected)
            {
                // 한줄씩 읽기 (비동기)
                string? message = await _reader.ReadLineAsync();

                // 연결이 끊어지면 null 반환
                if (message == null)
                {
                    break;
                }
                
                // 브로드캐스트 이벤트 발행
                MessageReceived?.Invoke(this, message);
                
                Console.WriteLine($"[수신] {_clientId} : {message}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Dispose();
        }
    }
    
    // 메시지 송신
    public async Task SendMessageAsync(string message)
    {
        if (_isDisposed || !IsConnected) return;
        await _writer.WriteLineAsync(message);
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        // 연결 종료 이벤트 발행
        Disconnected?.Invoke(_clientId);
        
        _reader.Dispose();
        _writer.Dispose();
        _stream.Dispose();
        _client.Dispose();
    }
}
