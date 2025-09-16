using Cryptos.Runtime.Presenter;
using SymphonyFrameWork.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    public partial class UIElementHealthBar : SymphonyVisualElement
    {
        public UIElementHealthBar() : base("UIToolKit/UXML/Ingame/HealthBar")
        {

        }

        public void RegisterTarget(HealthBarViewModel healthBarVM)
        {
            healthBarVM.OnHealthChaged += GuageChange;

            RegisterTrackingTarget(healthBarVM.TrackingTarget, healthBarVM.Token);

            healthBarVM.Token.Register(() =>
            {
                healthBarVM.OnHealthChaged -= GuageChange;

            });
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _base = container.Q(BASE_NAME);
            _redBar = container.Q(RED_BAR_NAME);
            _greenBar = container.Q(GREEN_BAR_NAME);

            return Task.CompletedTask;
        }

        private const string BASE_NAME = "base";
        private const string RED_BAR_NAME = "red-bar";
        private const string GREEN_BAR_NAME = "green-bar";

        private const float GREEN_DURATION = 0.5f;
        private const float RED_DURATION = 0.8f;
        private const float DERAY_BETWEEN = 1.5f;

        private VisualElement _base;
        private VisualElement _redBar;
        private VisualElement _greenBar;

        private Transform _trackingTarget;
        private float _lastValue;
        private CancellationTokenSource _cts;

        private async void RegisterTrackingTarget(Transform transform, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                MovePosition(transform.position);
                await Awaitable.NextFrameAsync();
            }
        }

        private void GuageChange(float value, float maxValue)
        {
            float proportion = _lastValue / value;

            if (_lastValue < value)
            {
                //回復だった時

                HealEffect(proportion);
            }
            else
            {
                //ダメージだった時

                _cts?.Cancel();
                _cts = new();

                DamageEffect(proportion, _cts.Token);
            }

            _lastValue = value;
        }


        private async void DamageEffect(float proportion, CancellationToken token)
        {
            float lastValue = _lastValue;

            try
            {
                // 緑ゲージを減らす。
                Task greenTweenTask = SymphonyTween.Tweening(lastValue, v =>
                    {
                        _greenBar.style.width = new Length(v * 100, LengthUnit.Percent);
                    }, proportion, GREEN_DURATION, token: token);

                // 緑ゲージが動き終わるまで待機。
                await greenTweenTask;

                // 少しディレイする。
                await Awaitable.WaitForSecondsAsync(DERAY_BETWEEN, token);

                Task redTweenTask = SymphonyTween.Tweening(lastValue, v =>
                {
                    _redBar.style.width = new Length(v * 100, LengthUnit.Percent);
                }, proportion, RED_DURATION, token: token);

                await redTweenTask;
            }
            catch (OperationCanceledException) { return; }
        }

        private void HealEffect(float proportion)
        {
            Length length = new Length(proportion * 100, LengthUnit.Percent);

            _greenBar.style.width = length;
            _redBar.style.height = length;
        }

        private void MovePosition(Vector3 position)
        {
            if (_base.panel == null) return;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
            Vector2 center = new(screenPos.x, screenPos.y);

            //背景の中心座標。
            Vector2 size = new Vector2(
                _base.resolvedStyle.width,
                _base.resolvedStyle.height)
                / 2;

            Vector2 selfPivot = center - size;

            //UITK座標系では値が高いほど下に移動する
            selfPivot = new(selfPivot.x, Screen.height - selfPivot.y);

            style.width = selfPivot.x;
            style.height = selfPivot.y;
        }
    }
}
