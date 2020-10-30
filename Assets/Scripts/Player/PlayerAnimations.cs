using UnityEngine;

namespace DefaultNamespace
{
    public struct PlayerAnimations
    {
        // Animator bool hashes
        public static readonly int PLAYER_IDLE = Animator.StringToHash("Player Idle");
        public static readonly int PLAYER_WALK = Animator.StringToHash("Player Walk");
        public static readonly int PLAYER_RUNN = Animator.StringToHash("Player Run");
        public static readonly int PLAYER_CLIMB = Animator.StringToHash("Player Climb");
        public static readonly int PLAYER_DIE = Animator.StringToHash("Player Death");
        public static readonly int PLAYER_ROLL = Animator.StringToHash("Player Roll");
    }
}