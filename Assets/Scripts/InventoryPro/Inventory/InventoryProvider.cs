/// <summary>
/// 그냥 1차원 배열
/// 데이터 저장과 인벤토리 관리를 같이하는 클래스
/// </summary>
public class InventoryProvider
{
    private readonly ItemSO[] _items;
    private int _cursor; // 빈 칸중 맨 앞
    private int _remainSpace; // 남은 칸

    public bool Add(ItemSO item, int count = 1)
    {

        return true;
    }

    public bool Sub(ItemSO item, int count = 1)
    {

        return true;
    }

    public ItemSO Get(int index) => _items[index];

    public bool IsEmpty()
    {

        return false;
    }

    // 칸 단위 이동
    public bool Move(int prev, int next)
    {

        return true;
    }

    public InventoryProvider(int capacity)
    {
        capacity = capacity <= 0 ? 1 : capacity;
        this._items = new ItemSO[capacity];
        this._cursor = 0;
        this._remainSpace = 0;
    }
}
