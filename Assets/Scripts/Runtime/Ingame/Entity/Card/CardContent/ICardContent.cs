using UnityEngine;

public interface ICardContent
{
    public void TriggerEnterContent(GameObject player, params GameObject[] target);
}
