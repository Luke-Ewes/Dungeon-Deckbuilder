using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private BaseCharacter character;


    private void Start()
    {
        character = GetComponentInParent<BaseCharacter>();
    }
    public void Attack()
    {

    }


}
