using Cryptos.Runtime.Presenter.Ingame.Character.Enemy;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.CombatSystem;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NearAttack", story: "[Self] attack [Target] with [Pipeline]", category: "Action", id: "5520661cb56cb58e427cb2f3ce1088ef")]
public partial class NearAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyModelPresenter> Self;
    [SerializeReference] public BlackboardVariable<SymphonyPresenter> Target;
    [SerializeReference] public BlackboardVariable<CombatPipelineWrapper> Pipeline;
    protected override Status OnStart()
    {
        IAttackable self = Self.Value.Self;
        IHittable target = Target.Value.Self;

        CombatContext context = CombatProcessor.Execute(self, target, Pipeline.Value.CombatHandlers);
        target.AddHealthDamage(context);

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

