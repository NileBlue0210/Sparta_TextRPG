using static Sparta_TextRPG.Program;

namespace Sparta_TextRPG
{
    /*
     * to do:
     * 최우선 목표: 클래스 파일별로 분리, 메뉴 별 반복 구조 개선
     * 레벨이 일정 이상이 되면 전직 이벤트를 발생시켜 보자
     *  Level프로퍼티를 활용해보자
     * 인벤토리를 표시할 때 무언가의 오류로 0보다 작은 수가 입력될 경우 0과 같은 처리를 할 것, 그리고 시스템 에러로 개발자가 알 수 있도록 해볼 것
     * 숫자나 글자가 올라갈 때, 꾸며놓은 UI가 깨진다. 숫자나 글자 수가 올라갈 때마다 유동적으로 UI가 변경되도록 해보자
     * 
     * 이슈: 상점에 들어가지 않고 상점 레벨이 변화할 만큼 플레이어가 레벨을 올리면 하위 상점의 품목이 상점에 추가되지 않는다
     */
    internal class Program
    {
        public class Player
        {
            private int level = 1;

            public int Level
            {
                get
                {
                    return level;
                }
                set
                {
                    if (level != value)
                    {
                        level = value;
                    }
                    else  // 기존 레벨과 같은 레벨로 변경하려 할 경우, 레벨 업 처리를 시행하지 않음
                    {
                        return;
                    }
                }
            }
            public string Name { get; set; } = "";
            public float Attack { get; set; } = 10f;
            public float PlusAttack { get; set; } = 0f; // 장비로 증가된 공격력
            public float Defense { get; set; } = 5f;
            public float PlusDefense { get; set; } = 0f; // 장비로 증가된 방어력
            public float Health { get; set; } = 100f;
            public float PlusHealth { get; set; } = 0f; // 장비로 증가된 HP
            public int Class { get; set; } = 0; // 표시되는 형식은 string 형이지만, 실제로는 int 형으로 저장
            public int Gold { get; set; } = 1500;

            public bool SupplyFlag { get; set; } = false; // 보급 아이템 지급 여부 확인용 플래그

            public void LevelUp(int level)
            {
                Level += level;

                // 레벨 업 시, 스테이터스 증가
                // question: 이 처리도 프로퍼티에서 하는 게 좋을까? -> 레벨이 하락 할 때의 스테이터스 변경 공식을 다르게 설정할 수도 있으니 레벨 업 함수에서 처리하는 게 좋을 것 같다
                Attack += level * 5f;
                Defense += level * 3f;
                Health += level * 10f;

                Console.WriteLine("레벨 {0}...{1}님의 힘이 강해지는 것이 느껴져요..!", Level, Name);
                Console.Write("\n");
            }

            public void AddGold(int gold)
            {
                Gold += gold;

                Console.WriteLine("{0} G를 획득했어요!", gold);
                Console.Write("\n");
            }
        }

        public class Inventory
        {
            public List<Item> items = new List<Item>(); // 인벤토리 내 모든 아이템 리스트
            // public List<Item> equippedItems = new List<Item>(); // 장착된 아이템 배열 -> 폐기 이유: 장착된 아이템만 따로 보는 기능이 없기 때문에 Item클래스 내부의 Equipped 플래그 확인으로 충분함

            public void AddItem(List<Item> addItem) // 인벤토리에 아이템 추가
            {
                foreach (Item item in addItem)
                {
                    items.Add(item);

                    Console.WriteLine("{0}(을)를 획득했어요!", item.Name);
                }

                Console.Write("\n");
            }

            public void EquipItem(Player player, Item item)
            {
                Console.WriteLine("{0}(을)를 장착했어요!", item.Name);

                player.PlusAttack += item.Attack;
                player.PlusDefense += item.Defense;
                player.PlusHealth += item.Health;

                Console.Write("\n");
            }

            public void DisrobeItem(Player player, Item item)
            {
                Console.WriteLine("{0}(이)가 해제되었어요!", item.Name);

                player.PlusAttack -= item.Attack;
                player.PlusDefense -= item.Defense;
                player.PlusHealth -= item.Health;

                Console.Write("\n");
            }
        }

        public class Shop
        {
            public string shopName = "상점";

            private int shopLevel = 0;

            // 상점 레벨이 변화할 때마다 상점 정보를 변경
            public int ShopLevel
            {
                get
                {
                    return shopLevel;
                }
                set
                {
                    if (shopLevel != value)
                    {
                        shopLevel = value;

                        AddGoods();
                    }
                    else  // 기존 상점 레벨과 변경된 상점 레벨이 같을 경우, 상점 정보를 변경하지 않음
                    {
                        return;
                    }
                }
            }

            public List<Goods> goods = new List<Goods>();  // 상점 내 모든 아이템 배열

            // 고레벨의 유저가 낮은 장비를 원하는 상황을 고려, 상점 내용을 덮어씌우는 것이 아닌 추가하는 방향으로 구현
            public void AddGoods()
            {
                if (ShopLevel == 1)
                {
                    shopName = "샵 노비스";
                    goods.AddRange(new List<Goods> {
                            new Goods { Name = "무딘 검", Description = "오래된 것 같은 무딘 검입니다. 벤다기 보다는 때린다고 봐야되겠죠.", Attack = 10, Defense = 3.5f, Price = 10, SoldOut = false },
                            new Goods { Name = "가죽 메일", Description = "무두질 된 가죽 메일입니다. 생각보다 튼튼한 것 같습니다.", Defense = 5, Health = 50, Price = 10, SoldOut = false },
                            new Goods { Name = "나무 방패", Description = "나무로 만든 방패입니다. 테두리는 금속재질이라 급할 때는 공격수단이 되기도 합니다.", Attack = 1.5f, Defense = 15, Price = 10, SoldOut = false }
                        });
                }
                else if (ShopLevel == 2)
                {
                    shopName = "샵 베테랑";
                    goods.AddRange(new List<Goods> {
                            new Goods { Name = "손질된 강철 검", Description = "날카롭게 빛을 반사하는 강철 검입니다. 아읏, 눈부셔..", Attack = 50, Defense = 15f, Price = 30, SoldOut = false },
                            new Goods { Name = "체인 메일", Description = "체인을 튼튼하게 덧댄 갑옷입니다. 조금 무겁습니다.", Defense = 20, Health = 150, Price = 30, SoldOut = false },
                            new Goods { Name = "가벼운 금속 방패", Description = "단단하지만 가벼운 금속 방패입니다.", Attack = 3.5f, Defense = 35, Price = 30, SoldOut = false }
                        });
                }
                else if (ShopLevel == 3)
                {
                    shopName = "샵 스파르타";
                    goods.AddRange(new List<Goods> {
                            new Goods { Name = "영원히 타오르는 판교의 불빛", Description = "착용자의 열정을 힘으로 바꾸는 능력이 있다. 열정이 떨어지면 생명력을 소비한다.", Attack = 99999, Defense = -100, Price = 10000, SoldOut = false },
                            new Goods { Name = "번아웃", Description = "화려하게 타오르는 불꽃일수록, 꺼진 자리가 더 싸늘해 보이는 법이라 했던가.", Attack = -100, Defense = 1, Health = 99999, Price = 10000, SoldOut = false },
                            new Goods { Name = "패션 페이", Description = "만월이 차오르는 날, 상응하는 댓가를 지불해야 하리라.", Defense = 99999, Health = 99999, Price = 10000, SoldOut = false }
                        });
                }
                else
                {
                    ShopLevel = 1;  // 예상되지 않은 변경이 감지되면, 기본 상점으로 변경
                }
            }
        }

        /*
         * to do:
         * 1. 등급을 추가해 cmd에서 등급별로 글자 색을 바꿔 출력할 수 있도록 해보기
         * 2. 장비 타입을 설정해 장비 타입별로 장착할 수 있는 아이템을 구분해보기
         * 3. 소모품 구현해보기
         * 4. 세트 속성을 추가해 세트 옵션 구현해보기
        */
        public class Item
        {
            public string Name = "";
            public string Description = "";
            public float Attack = 0;
            public float Defense = 0;
            public float Health = 0;
            public bool Equipped = false;

            // 아이템 발동 스킬
            public void ItemSkill()
            {
                Console.WriteLine("{0}(이)에서 강한 힘이 느껴지고 있어요!", Name);
            }
        }

        // 상점의 판매 아이템을 정의하는 클래스
        public class Goods : Item
        {
            public int Price = 0;
            public bool SoldOut = false;
        }

        static void Main(string[] args)
        {
            Player player = new Player();
            Inventory inventory = new Inventory();
            Shop shop = new Shop();

            string playerInput = ""; // 플레이어 입력값 저장 변수

            Console.WriteLine("안녕하세요! 저는 당신의 모험을 도와드릴 스파르타라고 해요!");
            Console.Write("\n");

            while (string.IsNullOrEmpty(player.Name))    // 플레이어명 입력 과정
            {
                Console.WriteLine("모험을 시작하시기 전에 먼저 당신의 이름을 알려주시겠어요?");
                Console.Write(">> ");

                player.Name = Console.ReadLine();

                if (string.IsNullOrEmpty(player.Name))  // 플레이어가 입력을 하지 않았을 경우, 플레이어 명을 공백으로 만들기 보다는 무언가 채우는 방식을 채택 -> 몰입감 상승을 위함
                {
                    while (true)    // 이름 입력 중 사용자가 공백을 입력했을 경우
                    {
                        Console.WriteLine("제 목소리가 조금 작았나요? 아니면 혹시 말 못하실 사정이 있으신 걸까요?");
                        Console.Write("\n");
                        Console.WriteLine("+-------------------------------+");
                        Console.WriteLine("|1. 잘 안들렸다.\t\t|");
                        Console.WriteLine("|2. ......\t\t\t|");
                        Console.WriteLine("+-------------------------------+");
                        Console.Write("\n");
                        Console.Write(">> ");

                        playerInput = Console.ReadLine();

                        bool checkPlayerInput = CheckInputIsNull(playerInput);

                        if (!checkPlayerInput)    // 입력값이 null 일 경우, 다시 입력받기
                        {
                            InvalidAnswer();
                            continue;
                        }

                        if (playerInput == "1")    // '잘 안들렸다' 선택 시, 최초로 이름을 묻는 부분으로 되돌아감
                        {
                            break;
                        }
                        else if (playerInput == "2")   // 이름을 끝까지 입력하지 않을 경우, 플레이어명을 '이름없는 용사' 로 지정
                        {
                            Console.WriteLine("이름없는 용사님..언젠가 당신의 이름을 들을 수 있다면 좋겠네요.");
                            Console.Write("\n");

                            player.Name = "이름없는 용사";
                            break;
                        }
                        else   // 잘못된 입력 시, 되돌아감
                        {
                            InvalidAnswer();
                            continue;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("{0}...멋진 이름이네요!", player.Name);
                    Console.Write("\n");
                    break;
                }
            }

            playerInput = ""; // 플레이어 입력값 초기화

            while (true)
            {
                Console.WriteLine("{0}님! 이곳에서 던전에 들어가기 전의 준비를 하실 수 있어요.", player.Name);
                Console.Write("\n");
                Console.WriteLine("+-------------------------------+");
                Console.WriteLine("|1. 상태 보기\t\t\t|");
                Console.WriteLine("|2. 인벤토리\t\t\t|");
                Console.WriteLine("|3. 상점\t\t\t|");
                Console.WriteLine("|4. 모험\t\t\t|");
                Console.WriteLine("|0. 하늘의 은총\t\t\t|");
                Console.WriteLine("+-------------------------------+");
                Console.Write("\n");

                AskAnswer();

                playerInput = Console.ReadLine();

                bool checkPlayerInput = CheckInputIsNull(playerInput);

                if (!checkPlayerInput)    // 입력값이 null 일 경우, 다시 입력받기
                {
                    InvalidAnswer();
                    continue;
                }

                if (playerInput == "1")    // '상태 보기' 선택 시, 상태 보기 함수 호출
                {
                    ShowStatus(player, inventory);
                    continue;
                }
                else if (playerInput == "2")   // '인벤토리' 선택 시, 인벤토리 함수 호출
                {
                    ShowInventory(player, inventory);
                    continue;
                }
                else if (playerInput == "3")   // '상점' 선택 시, 상점 함수 호출
                {
                    ShowShop(player, inventory, shop);
                    continue;
                }
                else if (playerInput == "4")   // '모험' 선택 시, 모험 시작
                {
                    Console.WriteLine("준비는 철저히 마치셨나요? 그럼 출발할게요!");
                    Console.Write("\n");
                    break;
                }
                else if (playerInput == "0")   // 개발자 도구: 플레이어의 레벨을 10 올리고, 10000 G를 추가한다
                {
                    Console.WriteLine("대가 없는 보상만큼 두려운게 없죠.");
                    Console.Write("\n");

                    player.LevelUp(10);
                    player.AddGold(10000);

                    Console.Write("\n");
                    continue;
                }
                else   // 잘못된 입력 시, 되돌아감
                {
                    InvalidAnswer();
                    continue;
                }
            }
        }

        public static void InvalidAnswer()
        {
            Console.WriteLine("잘 못들었어요..다시 한번 말씀해주시겠어요?");
            Console.Write("\n");
        }

        public static void AskAnswer()
        {
            Console.WriteLine("어떻게 행동하시겠어요?");
            Console.Write(">> ");
        }

        public static bool CheckInputIsNull(string playerInput)
        {
            if (string.IsNullOrEmpty(playerInput) || string.IsNullOrWhiteSpace(playerInput))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /*
         * to do: 공, 방, 체 부분의 스테이터스를 +연산자로 계산했다가는 나중에 스테이터스가 오르는 다른 상황이 추가될 경우 관리가 힘들어진다. 최종 스테이터스를 저장하는 부분이 필요할 것 같다
         *  레벨 업이나 아이템 장착으로 스테이터스가 변경될 경우, 프로퍼티 set을 통해 최종 공격력도 변화하도록 구현
         */
        public static void ShowStatus(Player player, Inventory inventory)
        {
            while (true)
            {
                Console.WriteLine("{0}님의 스테이터스를 보여드릴게요.", player.Name);
                Console.Write("\n");
                Console.WriteLine("+-------------------------------+");
                Console.WriteLine("|Lv. {0}\t\t\t\t|", player.Level);
                Console.WriteLine("|Chad ( {0} )\t\t|", ConvertClassCodeToName(player.Class));
                Console.WriteLine($"|공격력 : {player.Attack + player.PlusAttack} {(player.PlusAttack > 0 ? "(" + (player.PlusAttack >= 0 ? $"+{player.PlusAttack}" : player.PlusAttack) + ")\t\t" : "\t\t\t")}|");
                Console.WriteLine($"|방어력 : {player.Defense + player.PlusDefense} {(player.PlusDefense > 0 ? "(" + (player.PlusDefense >= 0 ? $"+{player.PlusDefense}" : player.PlusDefense) + ")\t\t" : "\t\t\t")}|");
                Console.WriteLine($"|체  력 : {player.Health + player.PlusHealth} {(player.PlusHealth > 0 ? "(" + (player.PlusHealth >= 0 ? $"+{player.PlusHealth}" : player.PlusHealth) + ")\t\t" : "\t\t\t")}|");
                Console.WriteLine("|Gold : {0} G\t\t\t|", player.Gold);
                Console.WriteLine("+-------------------------------+");
                Console.Write("\n");
                Console.WriteLine("0. 나가기");
                Console.Write("\n");

                AskAnswer();

                string playerInput = Console.ReadLine();

                bool checkPlayerInput = CheckInputIsNull(playerInput);

                if (!checkPlayerInput)    // 입력값이 null 일 경우, 다시 입력받기
                {
                    InvalidAnswer();
                    continue;
                }

                if (playerInput == "0")    // '나가기' 선택 시, 준비 메뉴로 되돌아감
                {
                    Console.WriteLine("준비 메뉴로 돌아갈게요.");
                    Console.Write("\n");
                    break;
                }
                else   // 잘못된 입력 시, 되돌아감
                {
                    InvalidAnswer();
                    continue;
                }
            }
        }

        public static void ShowInventory(Player player, Inventory inventory)
        {
            bool equipMode = false;
            string inventoryGuideText = "";

            while (true)
            {
                inventoryGuideText = equipMode ? "아이템을 장비하시거나 해제하실 수 있어요." : "가지고 계신 아이템들을 보여드릴게요.";

                Console.WriteLine("{0}", inventoryGuideText);
                Console.Write("\n");
                Console.WriteLine("[아이템 목록]");
                Console.Write("\n");

                if (inventory.items.Count == 0)
                {
                    Console.WriteLine("가방이 텅 비었어요. 가벼워서 좋네요!");
                    Console.Write("\n");
                }
                else
                {
                    for (int i = 0; i < inventory.items.Count; i++)
                    {
                        Item item = inventory.items[i];

                        Console.WriteLine($"- {(equipMode ? (i + 1) + " " : "")}{(item.Equipped ? "[E]" : "")}{item.Name}\t| {GetValidItemStatusToText(item)}\t| {item.Description}");
                    }

                    Console.Write("\n");
                }

                if (equipMode)
                {
                    Console.WriteLine("+-------------------------------+");
                    Console.WriteLine("|0. 나가기\t\t\t|");
                    Console.WriteLine("+-------------------------------+");
                }
                else
                {
                    Console.WriteLine("+-------------------------------+");
                    Console.WriteLine("|1. 장착 관리\t\t\t|");

                    if (!player.SupplyFlag)
                    {
                        Console.WriteLine("|2. 보급 아이템 지급\t\t|");
                    }

                    Console.WriteLine("|0. 나가기\t\t\t|");
                    Console.WriteLine("+-------------------------------+");
                }

                Console.Write("\n");

                AskAnswer();

                string playerInput = Console.ReadLine();

                bool checkPlayerInput = CheckInputIsNull(playerInput);

                if (!checkPlayerInput)    // 입력값이 null 일 경우, 다시 입력받기
                {
                    InvalidAnswer();
                    continue;
                }

                if (playerInput == "0")
                {
                    if (equipMode)  // 장착 관리 모드일 경우, 나가기 를 선택하면 인벤토리 메뉴로 되돌아감
                    {
                        equipMode = false;
                        continue;
                    }
                    else  // 인벤토리 메뉴일 경우, 나가기 를 선택하면 준비 메뉴로 되돌아감
                    {
                        Console.WriteLine("준비 메뉴로 돌아갈게요.");
                        Console.Write("\n");
                        break;
                    }
                }

                // 장착 관리 모드, 혹은 인벤토리 모드에서의 사용자 입력 처리
                if (equipMode)  // 장비 관리 모드일 경우
                {
                    // int ItemCount = Inventory.ReferenceEquals(inventory.items, null) ? 0 : inventory.items.Count; -> ReferenceEquals메서드란?
                    int ItemCount = inventory.items.Count;

                    // to do: 이 방식으로는 플레이어가 문자를 입력하면 에러가 나버린다. 수정이 필요
                    if (int.Parse(playerInput) > ItemCount || int.Parse(playerInput) < 0)
                    {
                        InvalidAnswer();
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < ItemCount; i++)
                        {
                            if (playerInput == (i + 1).ToString())
                            {
                                inventory.items[i].Equipped = !inventory.items[i].Equipped;

                                if (inventory.items[i].Equipped)
                                {
                                    inventory.EquipItem(player, inventory.items[i]);
                                }
                                else
                                {
                                    inventory.DisrobeItem(player, inventory.items[i]);
                                }
                            }
                        }
                    }
                }
                else  // 인벤토리 모드일 경우
                {
                    if (playerInput == "1")
                    {
                        equipMode = true;
                        continue;
                    }
                    else if (playerInput == "2" && !player.SupplyFlag)
                    {
                        List<Item> supplyItems = new List<Item>
                        {
                            new Item { Name = "갈라진 나무 막대", Description = "어딘가 두근거림이 느껴지는 나무 막대입니다.", Attack = 1 },
                            new Item { Name = "손때 묻은 가죽 튜닉", Description = "조금만 세게 긁혀도 찢어질 것 같은 가죽 튜닉입니다.", Health = 1.2f, Defense = 1 },
                            new Item { Name = "갈라진 판자 조각", Description = "방어력에 비해 무거운 나무 판자입니다.", Defense = 1.2f }
                        };

                        Console.WriteLine("제가 드리는 조그마한 선물이에요! 당분간의 모험에 도움이 됐으면 좋겠네요!");
                        Console.Write("\n");

                        inventory.AddItem(supplyItems);
                        player.SupplyFlag = true;

                        continue;
                    }
                    else   // 공백이나 나가기 이외의 잘못된 입력 시, 되돌아감
                    {
                        InvalidAnswer();
                        continue;
                    }
                }
            }
        }

        /*
         * to do:
         *  복수 갯수를 판매하는 아이템 추가, 복수 갯수 구매 추가
         *  아이템 판매 추가
         */
        public static void ShowShop(Player player, Inventory inventory, Shop shop)
        {
            bool purchaseMode = false;
            string shopGuideText = "";

            SetShopLevel(shop, player.Level);

            while (true)
            {
                shopGuideText = purchaseMode ? "찾으시는 물건이 있으신가요?" : $"{shop.shopName}입니다~무엇을 사러 오셨나요?";

                Console.WriteLine("{0}", shopGuideText);
                Console.Write("\n");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine("{0} G", player.Gold);
                Console.Write("\n");
                Console.WriteLine("[아이템 목록]");
                Console.Write("\n");

                for (int i = 0; i < shop.goods.Count; i++)
                {
                    Goods goods = shop.goods[i];

                    Console.WriteLine($"- {(purchaseMode ? (i + 1) + " " : "")}{goods.Name}\t| {GetValidItemStatusToText(goods)}\t| {goods.Description}\t| {(goods.SoldOut ? "구매완료" : goods.Price + " G")}");
                }

                Console.Write("\n");

                if (purchaseMode)
                {
                    Console.WriteLine("+-------------------------------+");
                    Console.WriteLine("|0. 나가기\t\t\t|");
                    Console.WriteLine("+-------------------------------+");
                }
                else
                {
                    Console.WriteLine("+-------------------------------+");
                    Console.WriteLine("|1. 아이템 구매\t\t\t|");
                    Console.WriteLine("|0. 나가기\t\t\t|");
                    Console.WriteLine("+-------------------------------+");
                }

                AskAnswer();

                string playerInput = Console.ReadLine();

                bool checkPlayerInput = CheckInputIsNull(playerInput);

                if (!checkPlayerInput)    // 입력값이 null 일 경우, 다시 입력받기
                {
                    InvalidAnswer();
                    continue;
                }

                if (playerInput == "0")    // question: modeChange() 와 같은 형식으로 캡슐화 할 수 없을까?
                {
                    if (purchaseMode)  // 아이템 구매 모드일 경우, 나가기 를 선택하면 상점 메뉴로 되돌아감
                    {
                        purchaseMode = false;
                        continue;
                    }
                    else  // 상점 메뉴일 경우, 나가기 를 선택하면 준비 메뉴로 되돌아감
                    {
                        Console.WriteLine("또 오세요, 손님!");
                        Console.Write("\n");
                        break;
                    }
                }

                // 상점, 혹은 아이템 구매 모드에서의 사용자 입력 처리
                if (purchaseMode)  // 아이템 구매 모드일 경우
                {
                    int goodsCount = shop.goods.Count;

                    // to do: 이 방식으로는 플레이어가 문자를 입력하면 에러가 나버린다. 수정이 필요
                    if (int.Parse(playerInput) > goodsCount || int.Parse(playerInput) < 0)
                    {
                        InvalidAnswer();
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < goodsCount; i++)
                        {
                            if (playerInput == (i + 1).ToString())
                            {
                                if (shop.goods[i].SoldOut)
                                {
                                    Console.WriteLine($"{shop.goods[i].Name} 말씀이시죠! 어라, 이미 가지고 계신 것 같은데요..다른 물건은 어떠신가요?");
                                    Console.Write("\n");

                                    continue;
                                }
                                else if (player.Gold < shop.goods[i].Price)
                                {
                                    Console.WriteLine("그 돈으로는 조금 힘드실 것 같은데요~");
                                    Console.Write("\n");

                                    continue;
                                }
                                else if (player.Gold >= shop.goods[i].Price)    // 예상치 못한 상황을 피하기 위해 else if문으로 조건 명시
                                {
                                    while (true)
                                    {
                                        Console.WriteLine("{0}, 이거면 될까요?", shop.goods[i].Name);
                                        Console.Write("\n");
                                        Console.WriteLine("+-------------------------------+");
                                        Console.WriteLine("|1. 구매\t\t\t|");
                                        Console.WriteLine("|0. 취소\t\t\t|");
                                        Console.WriteLine("+-------------------------------+");
                                        Console.Write("\n");

                                        playerInput = Console.ReadLine();

                                        checkPlayerInput = CheckInputIsNull(playerInput);

                                        if (!checkPlayerInput)    // 입력값이 null 일 경우, 다시 입력받기
                                        {
                                            InvalidAnswer();
                                            continue;
                                        }
                                        if (playerInput == "1")    // 아이템 구매 처리
                                        {
                                            shop.goods[i].SoldOut = true;
                                            player.Gold -= shop.goods[i].Price;

                                            Console.WriteLine("탁월한 선택이십니다!");
                                            Console.Write("\n");

                                            inventory.AddItem(new List<Item> { new Item { Name = shop.goods[i].Name, Description = shop.goods[i].Description, Attack = shop.goods[i].Attack, Defense = shop.goods[i].Defense, Health = shop.goods[i].Health } });

                                            break;
                                        }
                                        else if (playerInput == "0")   // 아이템 구매 취소
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else  // 상점 모드일 경우
                {
                    if (playerInput == "1")    // 구매 모드로 전환
                    {
                        purchaseMode = true;
                        continue;
                    }
                    else   // 공백이나 나가기 이외의 잘못된 입력 시, 되돌아감
                    {
                        InvalidAnswer();
                        continue;
                    }
                }
            }
        }

        // 플레이어의 레벨에 따라 상점 정보를 변경
        public static void SetShopLevel(Shop shop, int playerLevel)
        {
            if (playerLevel <= 10)
            {
                shop.ShopLevel = 1;
            }
            else if (playerLevel <= 20)
            {
                shop.ShopLevel = 2;
            }
            else
            {
                shop.ShopLevel = 3;
            }
        }

        internal static string ConvertClassCodeToName(int classCode)
        {
            switch (classCode)
            {
                case 0:
                    return "노비스";
                case 1:
                    return "전사";
                case 2:
                    return "마법사";
                case 3:
                    return "사제";
                default:
                    return "투명드래곤";
            }
        }

        // 아이템이 올려주는 스테이터스를 구분
        public static string GetValidItemStatusToText(Item item)
        {
            string statusName = "";
            float statusValue = 0f;

            Dictionary<string, float> validItemStatusDictionary = new Dictionary<string, float>();

            string resultText = "";

            // question: 이 방식이 맞나? 아이템에 스테이터스가 늘어날 경우, 이 if문을 반복해서 쓸 수 밖에 없나?
            // 아이템의 스테이터스 증가, 감소량이 없을 경우 표시할 필요가 없으므로, 0이 아닐 경우에만 표시
            if (item.Attack != 0)
            {
                statusName = "공격력";
                statusValue = item.Attack;

                validItemStatusDictionary.Add(statusName, statusValue);
            }
            if (item.Defense != 0)
            {
                statusName = "방어력";
                statusValue = item.Defense;

                validItemStatusDictionary.Add(statusName, statusValue);
            }
            if (item.Health != 0)
            {
                statusName = "체력";
                statusValue = item.Health;

                validItemStatusDictionary.Add(statusName, statusValue);
            }

            for (int i = 0; i < validItemStatusDictionary.Count; i++)
            {
                if (i == 0)
                {
                    resultText += $"{validItemStatusDictionary.Keys.ToList()[i]} {(validItemStatusDictionary.Values.ToList()[i] >= 0 ? "+" : "")}{validItemStatusDictionary.Values.ToList()[i]}";
                }
                else
                {
                    resultText += $", {validItemStatusDictionary.Keys.ToList()[i]} {(validItemStatusDictionary.Values.ToList()[i] >= 0 ? "+" : "")}{validItemStatusDictionary.Values.ToList()[i]}";
                }
            }

            return resultText;
        }
    }
}
