using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;
    public float useStamina;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    private Animator animator;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        animator = GetComponent<Animator>();
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate);
            }
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            var equipment = CharacterManager.Instance.Player.equip;
            Equip equippedItem = equipment.curEquip;

            if (equippedItem == null)
            {
                Debug.LogWarning("No equipped item found!");
                return;
            }

            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal, equippedItem.toolType);
            }
        }
    }
}
