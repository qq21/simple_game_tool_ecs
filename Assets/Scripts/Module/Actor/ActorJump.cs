﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Module
{
    public class ActorJump : UpdateModule
    {
        protected override void InitRequiredDataType()
        {
            _requiredDataTypeList.Add(typeof(ActorData));
            _requiredDataTypeList.Add(typeof(ActorController2DData));
            _requiredDataTypeList.Add(typeof(Physics2DData));
            _requiredDataTypeList.Add(typeof(ActorJumpData));
            _requiredDataTypeList.Add(typeof(ResourceStateData));
        }

        public override bool IsUpdateRequired(Data.Data data)
        {
            return data.GetType() == typeof(ActorJumpData);
        }

        public override void Refresh(ObjectData objData)
        {
            var resourceStateData = objData.GetData<ResourceStateData>();
            if (!resourceStateData.isInstantiated)
            {
                return;
            }

            var jumpData = objData.GetData<ActorJumpData>();

            var currentJump = jumpData.currentJump;
            if (currentJump.y == 0 && currentJump.x == 0)
            {
                Stop(objData.ObjectId);
                return;
            }

            var actorData = objData.GetData<ActorData>();
            if (!ActorController.CanJump(actorData.currentState))
            {
                return;
            }

            if ((actorData.currentState & (int)ActorStateType.Jump) == 0)
            {
                jumpData.currentJump = Vector3Int.zero;
                return;
            }

            var physics2DData = objData.GetData<Physics2DData>();
            var actorControllerData = objData.GetData<ActorController2DData>();
            if (ActorPhysics2D.IsGround(actorControllerData) && jumpData.currentJump.y == 0)
            {
                jumpData.currentJump.x = 0;

                physics2DData.force.x = 0;
                physics2DData.force.y = 0;

                ActorAnimator.JumpGround(objData);
                ActorAnimator.Stop(objData, ActorStateType.Jump);

                actorData.currentState &= ~(int)ActorStateType.Jump;
                objData.SetDirty(actorData);
            }
            else
            {
                jumpData.currentJump.y = currentJump.y - physics2DData.airFriction;
                if (jumpData.currentJump.y < 0)
                {
                    jumpData.currentJump.y = 0;
                    physics2DData.force.y += -physics2DData.airFriction;
                }
                else
                {
                    physics2DData.force.y = currentJump.y;
                }

                physics2DData.force.x = jumpData.currentJump.x;

                objData.SetDirty(physics2DData);
            }
        }
    }
}
