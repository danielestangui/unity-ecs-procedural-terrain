using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;

namespace Destrom.Player
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class GetPlayerInputSystem : SystemBase
    {
        private PlayerInputActions inputActions;
        private Entity playerEntity;

        protected override void OnCreate() 
        {
            RequireForUpdate<PlayerMoveInput>();

            inputActions = new PlayerInputActions();
        }

        protected override void OnStartRunning()
        {
            //base.OnStartRunning();

            inputActions.Enable();
            playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            Vector2 moveInput = inputActions.PlayerMap.PlayerActions.ReadValue<Vector2>();

            SystemAPI.SetSingleton(new PlayerMoveInput { Value = moveInput });
        }

        protected override void OnStopRunning()
        {
            inputActions.Disable();
            playerEntity = Entity.Null;

            //base.OnStopRunning();
        }
    }
}


