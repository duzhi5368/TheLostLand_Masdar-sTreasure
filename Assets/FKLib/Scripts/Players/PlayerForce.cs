//============================================================
namespace FKLib
{
    public enum ENUM_PlayerForce
    {
        eF_Player = 0,          // Control player
        eF_PlayerFriendly = 1,  // Player's friend, but player can control it.
        eF_AIFriendly = 2,      // Player's friend, controlled by AI
        eF_AINeutrally = 3,     // Neutral force, controlled by AI
        eF_PlayerEnemy = 4,     // Enemy force, controlled by network.
        eF_AIEnemy = 5,         // Enemy force, controlled by AI.
    }
}
