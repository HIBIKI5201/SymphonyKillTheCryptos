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
        ///    指定されたスプラインインデックスと距離に基づいて、スプライン上の位置と回転を取得します。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns>成功した場合はtrue、失敗した場合はfalseを返します。</returns>
        public bool GetPositionAndRotationByDistance(int index, float distance, out Vector3 position, out Quaternion rotation)
        {
            if (_splines == null || _splines.Length <= 0
                || _splines[index] == null || _splines[index].Spline == null)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                Debug.LogError("SplineContainer or Spline is not assigned.");
                return false;
            }

            Spline spline = _splines[index].Spline;

            float totalLength = spline.GetLength();

            if (totalLength <= 0f)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                Debug.LogError("Spline length is zero or negative.");
                return false;
            }

            if (totalLength < distance)
            {
                position = spline.EvaluatePosition(1);
                rotation = Quaternion.LookRotation(spline.EvaluateTangent(1));
                return false;
            }

            distance = Mathf.Clamp(distance, 0f, totalLength);

            // 正規化パラメータに変換
            float t = Mathf.Clamp01(distance / totalLength);

            // スプライン上の位置と接線を取得
            position = spline.EvaluatePosition(t);
            Vector3 tangent = spline.EvaluateTangent(t);

            rotation = Quaternion.LookRotation(tangent);

            return true;
        }

        [SerializeField]
        private SplineContainer[] _splines;
    }
}
