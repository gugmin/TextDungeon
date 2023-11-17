
using System;
using System.Numerics;
using System.Threading;

namespace TextDungeon
{
    //캐릭터 = 플레이어
    //캐릭에 회피 등 확률요소를 넣기, 방어력을 어떤식으로 써먹을지 정하기
    public class Character
    {
        public string Name { get; }
        public string Job { get; }
        public int Level { get; }
        public int Atk { get; }
        public int Def { get; }
        public int Hp { get; set; }
        public int Gold { get; }
        public bool IsDead { get; set; }

        public Character(string name, string job, int level, int atk, int def, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
        }
    }

    public class Monster
    {
        // 몬스터들을 구현하자 일단 예제에 있는 고블린,드래곤만 만들어둠
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Atk { get; set; }
        public bool IsDead { get; set; }


        public Monster(string name, int hp, int atk) // 몬스터의 정보를 스테이지가 가져올 수 있게 생성자를 만들어 줬습니다.
        {
            Name = name;
            Hp = hp;
            Atk = atk;

        }

    }
    public class Goblin : Monster
    {
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Atk { get; set; }

        public Goblin(string name, int hp, int atk) : base(name, hp, atk) // 부모 클래스의 생성자를 받아오기 위해 
        {
            Name = name;
            Hp = hp;
            Atk = atk;

        }
    }

    public class Dragon : Monster
    {
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Atk { get; set; }

        public Dragon(string name, int hp, int atk) : base(name, hp, atk)
        {
            Name = name;
            Hp = hp;
            Atk = atk;
        }
    }

    public class Item
    { //아이템을 웨폰,아머(부위별로),소모품 등 만들려면 List 써서 만드는게 편한데 Array 형식으로 만들어져 있음.
        //결국 담기는건 List로 해야 될거 같긴함(유동적이게 할 수 있음, 크기 조절이라던지..)
        //type을 만들어놔서 부위별 아머 장착은 간단할지도?
        public string Name { get; }
        public string Description { get; }
        public int Type { get; }
        public int Atk { get; }
        public int Def { get; }
        public int Hp { get; }
        public int Price { get; }

        public bool IsEquiped { get; set; }

        public static int ItemCnt = 0;

        public Item(string name, string description, int type, int atk, int def, int hp, int price, bool isEquiped = false)
        {
            Name = name;
            Description = description;
            Type = type;
            Atk = atk;
            Def = def;
            Hp = hp;
            Price = price;
            IsEquiped = isEquiped;
        }

        // 인벤토리 등 아이템 보이게 하는 부분임니다~!
        //여기 조절하면 배열 깔끔하게 맞출수 있음 대충 맞춰 놨는데 완벽하지 않아요!
        //지금 하지말고 추가 구현 끝나고 다 같이 맞춰 보시죠~
        public void PrintItemStatDescription(bool withNumber = false, int idx = 0)
        {
            Console.Write("- ");
            // 장착관리 전용
            if (withNumber)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("{0} ", idx);
                Console.ResetColor();
            }
            if (IsEquiped)
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("E");
                Console.ResetColor();
                Console.Write("]");
                Console.Write(PadRightForMixedText(Name, 9));
            }
            else Console.Write(PadRightForMixedText(Name, 12));

            Console.Write(" | ");

            if (Atk != 0) Console.Write($"Atk {(Atk >= 0 ? "+" : "")}{Atk} ");
            if (Def != 0) Console.Write($"Def {(Def >= 0 ? "+" : "")}{Def} ");
            if (Hp != 0) Console.Write($"Hp {(Hp >= 0 ? "+" : "")}{Hp}");

            Console.Write(" |\t ");

            Console.Write(Description + "\t");

            Console.Write(" |\t ");

            Console.WriteLine(Price + "G");
        }

        public static int GetPrintableLength(string str)
        {
            int length = 0;
            foreach (char c in str)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    length += 2; // 한글과 같은 넓은 문자에 대해 길이를 2로 취급
                }
                else
                {
                    length += 1; // 나머지 문자에 대해 길이를 1로 취급
                }
            }

            return length;
        }

        public static string PadRightForMixedText(string str, int totalLength)
        {
            int currentLength = GetPrintableLength(str);
            int padding = totalLength - currentLength;
            return str.PadRight(str.Length + padding);
        }

    }

    internal class Program
    {

        public static Character _player;
        public static Item[] _items;
        public static Goblin _goblin = new Goblin("고블린", 10, 3);
        public static Dragon _dragon = new Dragon("드래곤", 100, 20);
        public static Stage stage = new Stage();
        public bool IsDead = false;

        static void Main(string[] args)
        {
            //캐릭터 정보 및 아이템 불러오기
            GameDataSetting();
            //1번 시작화면 이후 선택창 불러오기
            Intro();
            //2번 메인화면 중 이름 , 직업선택(추가 가능) 설정 받아오기.
            GameStartScene();
            //2번 메인화면 : 상태창,인벤토리,전투 로 구성되어 있음.
            StartMenu();
        }

        static void GameDataSetting()
        {
            _items = new Item[10];
            // 이름/ 설명/ 타입/ 공격력/ 방어력/ 체력/ 가격  
            AddItem(new Item("무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", 0, 0, 5, 5, 50));
            AddItem(new Item("낡은 검", "쉽게 볼 수 있는 낡은 검입니다.", 1, 2, 0, 0, 50));
            AddItem(new Item("짱짱 칼", "짱짱 칼이당.", 1, 2, 0, 0, 50));
            AddItem(new Item("짱짱 모자", "짱짱 모자당.", 1, 0, 5, 0, 50));
            AddItem(new Item("짱짱 자켓", "짱짱 자켓이당.", 1, 0, 5, 0, 50));
            AddItem(new Item("짱짱 바지", "짱짱 바지당.", 1, 0, 5, 0, 50));
            AddItem(new Item("짱짱 장갑", "짱짱 장갑이당.", 1, 0, 5, 0, 50));
            AddItem(new Item("짱짱 신발", "짱짱 신발이당.", 1, 0, 5, 0, 50));
            AddItem(new Item("짱짱 벨트", "짱짱 벨트당.", 1, 0, 5, 0, 50));
            AddItem(new Item("짱짱 짱돌", "짱짱 짱..돌? 이건좀;", 1, 100, 0, 0, 50));
        }

        static void Intro()
        {
            //1번 시작화면 + 이후 선택창 구현(이어하기 , 새로하기)
            //DB가 아직 구현안됨
            Console.WriteLine("press any key to continue");
            Console.ReadLine();
            Console.Clear();
        }

        static void GameStartScene()
        {
            // 캐릭터 이름 세팅
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다."); //변경사항
            Console.WriteLine("이름을 입력하세요.");
            Console.WriteLine();
            string playerName = Console.ReadLine();
            Console.WriteLine("\n");
            Console.WriteLine($"이름 :{playerName}");
            Console.Clear();


            // 캐릭터 직업 세팅
            Console.WriteLine(" ※원하는 직업을 선택하세요.※");
            Console.WriteLine();
            Console.WriteLine("1. 전사 : 공격과 방어에 밸런스가 좋고 초기 체력이 높습니다.\n");
            Console.WriteLine("2. 궁수 : 공격력이 강한 대신 방어력과 체력이 낮습니다.\n");
            Console.WriteLine("3. 도적 : 초기스텟이 낮은 대신 추가 소지금이 있습니다.\n");

            int input = CheckValidInput(1, 3);

            switch (input)
            {
                case 1:
                    _player = new Character($"{playerName}", "전사", 1, 10, 10, 150, 1500);
                    break;
                case 2:
                    _player = new Character($"{playerName}", "궁수", 1, 2, 5, 80, 1500);
                    break;
                case 3:
                    _player = new Character($"{playerName}", "도적", 1, 7, 7, 77, 7777);
                    break;
                default:
                    Console.WriteLine("잘못된 선택입니다.");
                    break;
            }

        }//GameStartScene()

        static void StartMenu()
        {
            Console.Clear();

            Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다."); //변경사항
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
            Console.WriteLine("");
            Console.WriteLine("1. 상태 보기\n");
            Console.WriteLine("2. 인벤토리\n");
            Console.WriteLine("3. 던전 입장\n");

            switch (CheckValidInput(1, 3))
            {
                case 1:
                    StatusMenu();
                    break;
                case 2:
                    InventoryMenu();
                    break;
                case 3:
                    stage.Stage1(_player, _goblin); //던전입장
                    break;
            }
        }

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

        static void AddItem(Item item) //아이템 함수인데 List 쓰면 필요 없음
        {
            if (Item.ItemCnt == 10) return;
            _items[Item.ItemCnt] = item;
            Item.ItemCnt++;
        }

        static void StatusMenu()
        {
            Console.Clear();

            ShowHighlightedText("■ 상태 보기 ■");
            Console.WriteLine("캐릭터의 정보가 표기됩니다.");

            PrintTextWithHighlights("Lv. ", _player.Level.ToString("00"));
            Console.WriteLine("");
            Console.WriteLine("{0} ( {1} )", _player.Name, _player.Job);

            int bonusAtk = getSumBonusAtk();
            PrintTextWithHighlights("공격력 : ", (_player.Atk + bonusAtk).ToString(), bonusAtk > 0 ? string.Format(" (+{0})", bonusAtk) : "");

            int bonusDef = getSumBonusDef();
            PrintTextWithHighlights("방어력 : ", (_player.Def + bonusDef).ToString(), bonusDef > 0 ? string.Format(" (+{0})", bonusDef) : "");

            int bonusHp = getSumBonusHp();
            PrintTextWithHighlights("체 력 : ", (_player.Hp + bonusHp).ToString(), bonusHp > 0 ? string.Format(" (+{0})", bonusHp) : "");

            PrintTextWithHighlights("Gold : ", _player.Gold.ToString(), "  G");
            Console.WriteLine("");
            Console.WriteLine("0. 뒤로가기\n");

            switch (CheckValidInput(0, 0))
            {
                case 0:
                    StartMenu();
                    break;
            }
        }

        //아이템 장착시 공격력
        private static int getSumBonusAtk()
        {
            int sum = 0;
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                if (_items[i].IsEquiped) sum += _items[i].Atk;
            }
            return sum;
        }

        //아이템 장착시 방어력
        private static int getSumBonusDef()
        {
            int sum = 0;
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                if (_items[i].IsEquiped) sum += _items[i].Def;
            }
            return sum;
        }

        //아이템 장착시 체력
        private static int getSumBonusHp()
        {
            int sum = 0;
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                if (_items[i].IsEquiped) sum += _items[i].Hp;
            }
            return sum;
        }

        private static void PrintTextWithHighlights(string s1, string s2, string s3 = "")
        {
            Console.Write(s1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(s2);
            Console.ResetColor();
            Console.WriteLine(s3);
        }

        static void InventoryMenu()
        {
            Console.Clear();

            ShowHighlightedText("■ 인벤토리 ■");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                _items[i].PrintItemStatDescription();
            }
            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 장착관리");
            Console.WriteLine("");
            switch (CheckValidInput(0, 1))
            {
                case 0:
                    StartMenu();
                    break;
                case 1:
                    EquipMenu();
                    break;
            }
        }

        static void EquipMenu()
        {
            Console.Clear();

            ShowHighlightedText("■ 인벤토리 - 장착 관리 ■");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                _items[i].PrintItemStatDescription(true, i + 1); // 1, 2, 3에 매핑하기 위해 +1
            }
            Console.WriteLine("");
            Console.WriteLine("0. 나가기");

            int keyInput = CheckValidInput(0, Item.ItemCnt);

            switch (keyInput)
            {
                case 0:
                    InventoryMenu();
                    break;
                default:
                    ToggleEquipStatus(keyInput - 1);
                    EquipMenu();
                    break;
            }
        }

        static void ToggleEquipStatus(int idx)
        {
            _items[idx].IsEquiped = !_items[idx].IsEquiped;
        }

        static void ShowHighlightedText(string title)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(title);
            Console.ResetColor();
        }


    }//~
}