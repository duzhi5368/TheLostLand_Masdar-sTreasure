using System;
//============================================================
namespace FKLib
{
    public class AbilityAddedEventArgs : EventArgs
    {
        public readonly IAbility ability;

        public AbilityAddedEventArgs(IAbility ability)
        {
            this.ability = ability;
        }
    }
}
