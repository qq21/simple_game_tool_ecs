﻿using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Module
{
    public class ResourceLoader : Module
    {
        NotificationData _notificationData;

        public ResourceLoader()
        {
            _notificationData.id = Constant.NOTIFICATION_TYPE_RESOURCE_LOADER;
        }

        protected override void InitRequiredDataType()
        {
            _requiredDataTypeList.Add(typeof(ResourceData));
            _requiredDataTypeList.Add(typeof(ResourceStateData));
        }

        public override bool IsUpdateRequired(Data.Data data)
        {
            return data.GetType() == typeof(ResourceStateData) || data.GetType() == typeof(ResourceData);
        }

        public override void Refresh(ObjectData objData)
        {
            var resourceStateData = objData.GetData<ResourceStateData>();
            if (resourceStateData.resourceStateType == ResourceStateType.Load)
            {
                LoadResource(objData, resourceStateData);
            }
            else
            {
                ReleaseResource(objData, resourceStateData);
            }
        }

        void LoadResource(ObjectData objData, ResourceStateData resourceStateData)
        {
            var resourceData = objData.GetData<ResourceData>();

            var worldMgr = WorldManager.Instance;
            worldMgr.ResourceMgr.LoadAsync(resourceData.resource, delegate (Object obj)
            {
                resourceStateData.isInstantiated = true;
                var resource = worldMgr.PoolMgr.GetGameObject(resourceData.resource, obj);
                var transform = resource.transform;
                resourceData.gameObject = resource;
                resource.name = resourceStateData.name;
                resource.transform.position = resourceData.initialPosition;

                if (resourceStateData.resourceType == ResourceType.Actor)
                {
                    var controller = objData.GetData<ActorController2DData>();
                    var rigidbody2D = resource.GetComponent<Rigidbody2D>();
                    controller.rigidbody2D = rigidbody2D;
                    controller.foot = transform.Find("Foot");
                    controller.positionY = Mathf.RoundToInt(controller.foot.position.y);
                }

                var battleData = worldMgr.GameCore.GetData<BattleResourceData>();
                var attackData = objData.GetData<ResourceAttackData>();
                if (attackData != null)
                {
                    attackData.attack = transform.Find("Attack").gameObject;
                    battleData.attackDictionary.Add(attackData.attack, objData.ObjectId);

                    var attackCollider2DList = transform.GetComponentsInChildren<AttackCollider2D>();
                    for (var i = 0; i < attackCollider2DList.Length; i++)
                    {
                        attackCollider2DList[i].Init(objData);
                    }

                    attackData.attack.SetActive(attackData.attackEffect.initial);
                }

                var hurtData = objData.GetData<ResourceHurtData>();
                if (hurtData != null)
                {
                    hurtData.hurt = transform.Find("Hurt").gameObject;
                    battleData.hurtDictionary.Add(hurtData.hurt, objData.ObjectId);
                }

                _notificationData.mode = NotificationMode.Object;
                _notificationData.type = (int)ResourceStateType.Load;
                _notificationData.data1 = objData;

                worldMgr.NotificationCenter.Notificate(_notificationData);
            });
        }

        void ReleaseResource(ObjectData objData, ResourceStateData resourceStateData)
        {
            var worldMgr = WorldManager.Instance;

            var resourceData = objData.GetData<ResourceData>();

            resourceStateData.isInstantiated = false;

            if (resourceData.gameObject != null)
            {
                worldMgr.PoolMgr.ReleaseGameObject(resourceData.resource, resourceData.gameObject);
            }

            var battleData = worldMgr.GameCore.GetData<BattleResourceData>();
            var hurtData = objData.GetData<ResourceHurtData>();
            if (hurtData != null)
            {
                battleData.hurtDictionary.Remove(hurtData.hurt);
            }

            var attackData = objData.GetData<ResourceAttackData>();
            if (attackData != null)
            {
                battleData.attackDictionary.Remove(attackData.attack);
            }

            _notificationData.mode = NotificationMode.Object;
            _notificationData.type = (int)ResourceStateType.Release;
            _notificationData.data1 = objData;

            worldMgr.NotificationCenter.Notificate(_notificationData);
        }
    }
}
