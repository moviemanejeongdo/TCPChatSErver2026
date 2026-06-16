using System;
using System.Threading.Tasks;

namespace TCPChatClient;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("====================== TCP 채팅 클라이언트 ======================");
        
        // 연결할 서버 정보
        const string ip = "127.0.0.1";
        const int port = 7777;

        // ChatClient 인스턴스 생성
        using var client = new ChatClient(ip, port);
        
        try
        {
            // 서버 연결
            await client.ConnectServerAsync();

            //메시지 수신 매서드 시작(백그라운드 스레드)
            _ = Task.Run(client.ReceiveMessageAsync);


            //사용자 입력 루프
            Console.WriteLine("메시지를 입력하세요 (종료하려면 'exit' 입력): ");    
            Console.WriteLine("=========================================================");
            
            
            // 메시지 입력 및 전송 루프
            while (client.IsConnected)
            {
                //사용자 메시지 입력 받기
                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("채팅을 종료합니다.");
                    break;

                }   

                //서버에 메시지 전송
                await client.SendMessageAsync(input);
                

            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"서버 연결 실패: {e.Message}");
        }

        Console.WriteLine("클라이언트를 종료하려면 아무 키나 입력하세요.");
        Console.ReadKey(true);
    }
}