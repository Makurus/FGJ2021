using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAttackAnim()
    {
        anim.SetTrigger("att");
    }

    public void SetWalkingAnim(bool value)
    {
        anim.SetBool("walking", value);

    }

    public void SetStunnedAnim(bool value)
    {
        anim.SetBool("stunned", value);

    }

    public void SetDamagedAnim()
    {
        anim.SetTrigger("dmg");

    }

    public void PlayDie(bool b)
    {
        anim.SetBool("die",b);

    }

    public void PlayAnim(string name)
    {
        print(name);
        anim.Play(name);

    }
    public void Destroyer()
    {
        Destroy(transform.parent.gameObject);
    }
}
