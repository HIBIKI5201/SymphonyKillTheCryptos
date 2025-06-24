using UnityEngine;

public class CardContentAttack : CardContentBase
{
    [SerializeField]
    private float _damage;

    public override void TriggerEnterContent(GameObject player, params GameObject[] target)
    {
        
    }
}
