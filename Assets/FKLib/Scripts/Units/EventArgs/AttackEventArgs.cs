using System;
//============================================================
namespace FKLib
{
    public class AttackEventArgs : EventArgs
    {
        public IUnit Attacker;
        public IUnit Defender;
        public int Damage;

        public AttackEventArgs(IUnit attacker, IUnit defender, int damage)
        {
            Attacker = attacker;
            Defender = defender;
            Damage = damage;
        }
    }
}

