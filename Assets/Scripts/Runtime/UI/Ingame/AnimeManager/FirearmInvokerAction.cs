using Cryptos.Runtime.UI.Ingame.Character.Player;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FirearmInvoker", story: "Invoke [firearm]", category: "Action", id: "0bab6c34198cd4a21f5aeccb84add8e3")]
public partial class FirearmInvokerAction : Action
{
    [SerializeReference] public BlackboardVariable<FirearmAnimeManager> Firearm;

    protected override Status OnStart()
    {
        Firearm.Value.Fire();
        return Status.Success;
    }
}

