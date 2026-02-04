using System.Threading.Tasks;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Enemy
{
    public interface IEnemyAnimeManager
    {
        public void Hit();
        public ValueTask Dead();
    }
}
