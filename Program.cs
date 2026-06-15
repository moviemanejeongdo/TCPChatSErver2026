namespace TCPChatServer;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Windows 콘솔에서 UTF-8 인코딩 설정
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        
        // 기본 포트 번호 설정 0 ~ 65536
        // 1000번 포트 이하는 미리 예약 http 80, ssh 21, ...
        const int port = 7777;
        
        // 채팅 서버 인스턴스 생성 및 실행
        var server = new ChatServer(port);
        // 서버 시작
        server.StartServer();
        
        Console.WriteLine("서버를 종료하려면 아무키나 입력하세요.");
        Console.ReadKey(true); // 입력한 키 값을 Hook해서 화면에 출력되지 않도록 하는 옵션
        
        // 서버 정지로직 추가
        server.StopServer();
    }
}