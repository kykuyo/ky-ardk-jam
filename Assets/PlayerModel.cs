using JetBrains.Annotations;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    private enum PlayerModelState
    {
        Idle,
        Walk,
        Run,
    }

    [SerializeField]
    private Animator _animator;

    private static readonly int WalkAnimation = Animator.StringToHash("Walk");
    private static readonly int RunAnimation = Animator.StringToHash("Run");
    private static readonly int IdleAnimation = Animator.StringToHash("Idle");

    private const float WalkThreshold = 0.5f;
    private const float RunThreshold = 10f;

    private float _lastIdleUpdateTime;
    private PlayerModelState _currentPlayerState = PlayerModelState.Idle;

    private bool IsMoving => _currentPlayerState is PlayerModelState.Walk or PlayerModelState.Run;

    private bool IsIdle => _currentPlayerState is PlayerModelState.Idle;

    public void UpdatePlayerState(float movementDistance)
    {
        switch (movementDistance)
        {
            case > RunThreshold:
            {
                if (_currentPlayerState != PlayerModelState.Run)
                {
                    _animator.CrossFade(RunAnimation, 1f);
                    _currentPlayerState = PlayerModelState.Run;
                }

                break;
            }
            case > WalkThreshold:
            {
                if (IsIdle)
                {
                    _animator.CrossFade(WalkAnimation, 0f);
                    _currentPlayerState = PlayerModelState.Walk;
                }
                else if (_currentPlayerState != PlayerModelState.Walk)
                {
                    _animator.CrossFade(WalkAnimation, 1f);
                    _currentPlayerState = PlayerModelState.Walk;
                }

                break;
            }
            default:
                HandleIdleState();
                break;
        }
    }

    [UsedImplicitly]
    public void ReturnIdle()
    {
        _animator.CrossFade(IdleAnimation, 1f);
        _currentPlayerState = PlayerModelState.Idle;
        _lastIdleUpdateTime = 0f;
    }

    private void HandleIdleState()
    {
        if (IsMoving)
        {
            _animator.CrossFade(IdleAnimation, 1f);
            _currentPlayerState = PlayerModelState.Idle;
            _lastIdleUpdateTime = 0f;
        }

        _lastIdleUpdateTime += Time.deltaTime;
    }
}
