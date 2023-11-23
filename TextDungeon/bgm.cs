using System;
using System.Threading;
using WMPLib;

namespace Example
{
    class Program
    {
        static void Man(string[] args)
        {
            bool IsAlive = true;
            bool IsPause = false;
            WindowsMediaPlayer player = new WindowsMediaPlayer();
            // 스레드 시작
            ThreadPool.QueueUserWorkItem(_ =>
            {
                player.URL = @"D:\celtic-7136.mp3";
                while (true)
                {
                    if (IsAlive)
                    {
                        if (!IsPause)
                        {
                            Console.Clear();
                            Console.WriteLine("P : Play , S: Stop, A: Pause");
                            Console.WriteLine();
                            Console.WriteLine($"{player.controls.currentPositionString} / {player.currentMedia.durationString}");
                            Console.WriteLine();
                            Console.WriteLine($"Volume: {player.settings.volume}");
                        }
                        // 스레드 대기 0.1초
                        Thread.Sleep(100);
                        continue;
                    }
                    else
                    {
                        player.controls.stop();
                        break;
                    }
                }
            });


            

            while (true)
            {
                // 사용자로 부터 입력 받는다.
                var cmd = Console.ReadKey();
                switch (cmd.Key)
                {
                    // P키를 누르면
                    case ConsoleKey.P:
                        // 정지 플래그 해제
                        IsPause = false;
                        // 음악 플레이!
                        player.controls.play();
                        continue;
                    // S키를 누르면
                    case ConsoleKey.S:
                        // 스레드 종료
                        IsAlive = false;
                        // 음악 종료
                        player.controls.stop();
                        break;
                    // A키를 누르면
                    case ConsoleKey.A:
                        // 정지 플래그 설정
                        IsPause = true;
                        // 일시 정지
                        player.controls.pause();
                        // 명령어 삭제
                        Console.WriteLine("\r  ");
                        continue;
                    default:
                        continue;
                }
                break;
            }
            // 콘솔 출력
            Console.WriteLine("\rPress any key...");
            Console.ReadKey();
        }
    }
}
