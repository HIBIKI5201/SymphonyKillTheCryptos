using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character
{
    public readonly struct CombatContextViewModel
    {
        public CombatContextViewModel(CombatContext context)
        {
            _context = context;
        }

        public float Damage => _context.Damage;
        public int CriticalCount => _context.CriticalCount;
        public string Element => _context.Element;

        private readonly CombatContext _context;
    }
}
