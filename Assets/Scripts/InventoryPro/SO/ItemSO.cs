using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO_", menuName = "Item SO")]
public class ItemSO : ScriptableObject
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("기본 정보")]
    [SerializeField] private EItem _id = EItem.None;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name = "이름";
    [SerializeField] private string _description = "설명";
    [SerializeField] private int _maxStack = 64; // 최대 중첩 수
    #endregion

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public EItem Id => _id;
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
    public int MaxStack => _maxStack;
    #endregion

    public bool IsVaild()
    {
        if (_id == EItem.None) return false;
        if (string.IsNullOrEmpty(_name)) return false;
        if (string.IsNullOrEmpty(_name)) return false;
        if (string.IsNullOrEmpty(_description)) return false;
        if (_maxStack <= 0) return false;
        return true;
    }

    private void OnValidate()
    {
        if (!IsVaild())
        {
            De.PrintOnce($"아이템SO({_id})의 값을 제대로 설정해주세요.", LogType.Assert);
        }
    }
}
