using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
public class WeaponAnimator : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private const string WEAPON_IS_GUN = "IsGun";
    private bool oldState;
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private float angle;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = gameObject.GetComponentInParent<Player>();
        oldState = animator.GetBool(WEAPON_IS_GUN);
        Assert.IsNotNull(player, "Player component not found in parent");
    }


    // Update is called once per frame
    void Update()
    {
        var isAiming = player.firingStage != FiringStage.notFiring;
        if (oldState == isAiming) return;
        animator.SetBool(WEAPON_IS_GUN, isAiming);
        oldState = isAiming;
    }
}
