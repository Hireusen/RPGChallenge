using UnityEngine;

/// <summary>
/// 힐 오브젝트에 붙여보세요~
/// </summary>
public class HealItem : MonoBehaviour
{
    [Header("참조 연결")]
    [SerializeField] private float _healAmount = 15f;
    [SerializeField] private float _rotateSpeed = 120f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHealable>(out IHealable target))
        {
            De.Print($"{_healAmount} 회복합니다. 그런데 플레이어 말고 적도 아이템을 먹을 수 있습니다.");
            target.Healing(_healAmount);
            Destroy(gameObject);
        }
    }
    // 회전
    private void Update()
    {
        Quaternion rotation = Quaternion.AngleAxis(_rotateSpeed * Time.deltaTime, Vector3.up);
        transform.rotation *= rotation;
    }
}
