using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GasMaskEquip : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public static int equipGasMaskTriggerHash = Animator.StringToHash("EquipGasMask");
    public static int unequipGasMaskTriggerHash = Animator.StringToHash("UnequipGasMask");

    private IEnumerator Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // === just for testing ===
        while (true)
        {
            EquipMask();
            yield return new WaitForSeconds(5f);
            UnequipMask();
            yield return new WaitForSeconds(5f);
        }

        // === just for testing ===
    }
    

    /// <summary>
    /// Triggers the animator to play the equip mask animation then go into GasMaskEquippedIdle
    /// </summary>
    public void EquipMask()
    {
        animator.SetTrigger(equipGasMaskTriggerHash); 
    }

    /// <summary>
    /// Triggers the animator to play the unequip mask animation then go into GasMaskUnequippedIdle
    /// </summary>
    public void UnequipMask()
    {
        animator.SetTrigger(unequipGasMaskTriggerHash); 
    }
}
