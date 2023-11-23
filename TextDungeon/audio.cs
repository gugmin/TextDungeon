using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDungeon
{
    internal class audio
    {

        EventWaitHandle _ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
        static int CheckValidInput(int min, int max)
        {
            int keyInput;
            bool result;
            do
            {
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                result = int.TryParse(Console.ReadLine(), out keyInput);
            } while (result == false || CheckIfValid(keyInput, min, max) == false);

            return keyInput;
        }

        static bool CheckIfValid(int checkable, int min, int max)
        {
            if (min <= checkable && checkable <= max) return true;
            return false;
        }
        public void PlayOne()
        {


            string mp3Path = @"Mediavel Song Consort [No Copyright Music].mp3";

   

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", mp3Path));
            
            using (IWavePlayer wavePlayer = new WaveOutEvent())
            using (AudioFileReader reader = new AudioFileReader(relativePath))
            {

                wavePlayer.Init(reader);

                wavePlayer.Play();
                wavePlayer.Volume = 0.1f;

               while (true)
                {
                    if (_ewh.WaitOne(1000) == true)
                  {
                        break;
                    }


                }


               void voluming()
                {
                    Console.WriteLine("볼륨을 올리려면 1, 끄려면 2를 눌러주세요");

                    switch (CheckValidInput(1, 2))
                    {
                        case 1:
                            wavePlayer.Volume = wavePlayer.Volume * 2;
                            break;

                        case 2:
                            wavePlayer.Volume = 0f;
                            break;

                    }
                }
               
            }
        }



        private void WavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
       {
            _ewh.Set();
        }
    }
}
