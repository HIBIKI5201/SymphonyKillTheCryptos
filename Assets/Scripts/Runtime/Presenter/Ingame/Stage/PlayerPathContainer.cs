using UnityEngine;
using UnityEngine.Splines;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    /// <summary>
    ///     プレイヤーの移動経路を管理するコンテナ
    /// </summary>
    public class PlayerPathContainer : MonoBehaviour
    {
        /// <summary>
        ///     指定したインデックスのSplineから、tに応じた位置を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetPoint(int index, float t)
        {
            if (index < 0 || index >= _splines.Length)
                return Vector3.zero;

            var spline = _splines[index].Spline;

            if (spline.Count <= 0)
                return Vector3.zero;

            Vector3 position = spline.EvaluatePosition(t);

            Vector3 tangent = spline.EvaluateTangent(t);
            if (tangent != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(tangent);
            }

            return position;
        }

        [SerializeField]
        private SplineContainer[] _splines;
    }
}
