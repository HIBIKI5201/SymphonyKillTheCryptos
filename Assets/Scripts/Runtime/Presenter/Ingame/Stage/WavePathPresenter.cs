using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.System

{

    public class WavePathPresenter
    {
        public WavePathPresenter(PlayerPathContainer pathContainer,
            SymphonyPresenter player,
            float speed = 1)
        {
            _pathContainer = pathContainer;
            _player = player;
            _speed = speed;
        }

        public async Task NextWave(int index)
        {
            float distance = 0f;
            while (MovePositionOnSpline(index, distance,
                out Vector3 position, out Quaternion rotation))
            {
                distance += _speed * Time.deltaTime;

                _player.MovePosition();
                await Awaitable.NextFrameAsync();
            }

            _player.EndMoveAnimation();
        }

        private readonly PlayerPathContainer _pathContainer;
        private readonly SymphonyPresenter _player;

        private readonly float _speed = 1f;

        /// <summary>
        ///    スプライン上を移動する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private bool MovePositionOnSpline(int index, float distance,
            out Vector3 position, out Quaternion rotation)
        {
            //スプライン上の位置と回転を取得
            bool isSuccess = _pathContainer
                .GetPositionAndRotationByDistance(index, distance,
                out position, out rotation);

            //座標更新
            _pathContainer.MoveBattleCore(position, rotation);

            return isSuccess;
        }
    }
}