using System.Net.Sockets;
using System.Text;

namespace TCPChatClient;

public class ChatClient : IDisposable
{
    // TCP 클라이언트 소켓 선언
    private TcpClient? _tcpClient;
    // 네트워크 스트림
    private NetworkStream? _stream;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    
    // 연결 여부
    private bool _isConnected;
    // 리소스 해제여부
    private bool _isDisposed;
    
    // 서버 IP, Port
    private readonly string _serverIp;
    private readonly int _serverPort;
    
    // 연결 여부 프로퍼티
    public bool IsConnected => _isConnected && !_isDisposed;
    
    // 생성자
    public ChatClient(string serverIp, int serverPort)
    {
        _serverIp = serverIp;
        _serverPort = serverPort;

        _isConnected = false;
        _isDisposed = false;
    }

    #region 서버 연결
    public async Task ConnectServerAsync()
    {
        if (IsConnected)
        {
            Console.WriteLine("이미 서버에 연결되었습니다.");
            return;
        }

        try
        {
            Console.WriteLine($"{_serverIp}:{_serverPort} 서버 연결 중 ...");
            
            // TcpClient 생성 및 서버 연결
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_serverIp, _serverPort);
            
            // 스트림 생성
            _stream = _tcpClient.GetStream();
            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
            
            _isConnected = true;
            Console.WriteLine("서버에 연결되었습니다.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"서버 연결 실패 {e.Message}");
            throw;
        }
    }
    #endregion

    #region 메시지 수신

    public async Task ReceiveMessageAsync()
    {
        try
        {
            while (IsConnected)
            {
                // 한라인씩 읽기
                string? message = await _reader!.ReadLineAsync();

                if (message == null)
                {
                    Console.WriteLine("서버와 연결이 종료되었습니다.");
                    break;
                }
                
                // 수신된 메시지 콘솔에 출력
                Console.WriteLine(message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            Dispose();
        }
    }
    #endregion

    #region 메시지 송신

    public async Task SendMessageAsync(string message)
    {
        try
        {
            
            await _writer!.WriteLineAsync(message);
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    #endregion
    
    #region 리소스 해제
    public void Dispose()
    {
        _stream?.Dispose();
        _reader?.Dispose();
        _writer?.Dispose();
        _tcpClient?.Dispose();
    }
    #endregion
}