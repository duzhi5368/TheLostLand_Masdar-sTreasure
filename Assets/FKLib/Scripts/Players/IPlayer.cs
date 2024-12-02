using UnityEngine;
//============================================================
namespace FKLib
{
    /// <summary>
    /// A game player.
    /// The player is not a human player, it can be seen as a force.
    /// - A human player can be a player.
    /// - An AI player can be a player.
    /// - A NPC friend player can be a player.
    /// - A network controller player can be a player.
    /// </summary>
    public abstract class IPlayer : MonoBehaviour
    {
        public int              PlayerID;       // this ID must be unique
        public ENUM_PlayerForce PlayerForce;    // which force this player belongs to

        public virtual void Initialize(ICellGrid cellGrid){}

        // Method will be called every turn.
        // Allow player to control his units.
        public abstract void Play(ICellGrid cellGrid);
    }
}
