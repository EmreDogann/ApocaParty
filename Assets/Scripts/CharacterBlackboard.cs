using System;
using UnityEngine;

public class CharacterBlackboard : MonoBehaviour
{
    // --- Movement ---
    public bool IsMoving { get; set; }
    public float MoveSpeed { get; set; }

    // private float _playerStride;
    // public float PlayerStride
    // {
    //     get => _playerStride;
    //     set
    //     {
    //         _playerStride = value;
    //         OnStrideChange?.Invoke(value);
    //     }
    // }

    public Action OnStride;
    public Action<float> OnStrideChange;
}