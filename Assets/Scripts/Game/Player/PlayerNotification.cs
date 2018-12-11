﻿using FlatBuffers;
using System;

public enum PlayerNotificationType
{
    LoginSuccess,
    LoginFailed,
    GetRoleInfoSuccess,
    GetRoleInfoFailed,
    CreateRoleSuccess,
    CreateRoleFailed
}

public class PlayerNotification : BaseNotification
{
    NotificationData notification;

    public PlayerNotification()
    {
        _id = Constant.NOTIFICATION_TYPE_NETWORK;
        _typeList = new int[] { (int)Protocols.ResLoginGame, (int)Protocols.ResRoleInfo, (int)Protocols.ResCreateRole };

        notification = new NotificationData();
        notification.id = Constant.NOTIFICATION_TYPE_PLAYER;

        Enabled = true;
    }

    public override void OnReceive(int type, ValueType notificationData)
    {
        var msg = (Message)notificationData;
        var byteBuffer = new ByteBuffer(msg.data);

        switch(type)
        {
            case (int)Protocols.ResLoginGame:
                ResLoginGame(byteBuffer);
                break;
            case (int)Protocols.ResRoleInfo:
                ResRoleInfo(byteBuffer);
                break;
        }
    }

    public enum LoginGameResult
    {
        Success = 0,
    }

    void ResLoginGame(ByteBuffer byteBuffer)
    {
        var worldMgr = WorldManager.Instance;
        var resLoginGame = Protocol.Login.ResLoginGame.GetRootAsResLoginGame(byteBuffer);

        if (resLoginGame.Result == (int)LoginGameResult.Success)
        {
            var playerBaseData = worldMgr.Player.GetData<Data.PlayerBaseData>();

            playerBaseData.accountId = resLoginGame.AccountId;

            var roleInfoLiteList = playerBaseData.roleInfoLiteList;
            roleInfoLiteList.Clear();
            for (var i = 0; i < resLoginGame.RoleInfoLitesLength; i++)
            {
                var roleInfoLite = new Data.RoleInfoLite();
                var roleInfoLiteTable = resLoginGame.RoleInfoLites(i);

                roleInfoLite.roleId = roleInfoLiteTable.Value.RoleId;
                roleInfoLite.roleName = roleInfoLiteTable.Value.RoleName;
                roleInfoLite.roleLevel = roleInfoLiteTable.Value.RoleLevel;

                roleInfoLiteList.Add(roleInfoLite);
            }

            notification.type = (int)PlayerNotificationType.LoginSuccess;
            notification.mode = NotificationMode.Object;
            notification.data1 = playerBaseData;
        }
        else
        {
            notification.type = (int)PlayerNotificationType.LoginFailed;
            notification.mode = NotificationMode.ValueType;
            notification.data2 = resLoginGame.Result;
        }

        worldMgr.NotificationCenter.Notificate(notification);
    }

    public enum GetRoleInfoResult
    {
        Success = 0,
    }

    void ResRoleInfo(ByteBuffer byteBuffer)
    {
        var worldMgr = WorldManager.Instance;
        var resRoleInfo = Protocol.Login.ResRoleInfo.GetRootAsResRoleInfo(byteBuffer);

        if (resRoleInfo.Result == (int)GetRoleInfoResult.Success)
        {
            var roleInfo = resRoleInfo.RoleInfo.Value;

            var playerBaseData = worldMgr.Player.GetData<Data.PlayerBaseData>();
            playerBaseData.roleInfo.roleId = roleInfo.RoleId;
            playerBaseData.roleInfo.roleName = roleInfo.RoleName;
            playerBaseData.roleInfo.roleLevel = roleInfo.RoleLevel;

            notification.type = (int)PlayerNotificationType.GetRoleInfoSuccess;
            notification.mode = NotificationMode.ValueType;
            notification.data2 = playerBaseData.roleInfo;
        }
        else
        {
            notification.type = (int)PlayerNotificationType.GetRoleInfoFailed;
            notification.mode = NotificationMode.ValueType;
            notification.data2 = resRoleInfo.Result;
        }

        worldMgr.NotificationCenter.Notificate(notification);
    }

    public enum CreateRoleResult
    {
        Success = 0,
    }

    void ResCreateRole(ByteBuffer byteBuffer)
    {
        var worldMgr = WorldManager.Instance;
        var resCreateRole = Protocol.Login.ResCreateRole.GetRootAsResCreateRole(byteBuffer);

        if (resCreateRole.Result == (int)CreateRoleResult.Success)
        {
            var roleInfo = resCreateRole.RoleInfo.Value;

            var playerBaseData = worldMgr.Player.GetData<Data.PlayerBaseData>();
            playerBaseData.roleInfo.roleId = roleInfo.RoleId;
            playerBaseData.roleInfo.roleName = roleInfo.RoleName;
            playerBaseData.roleInfo.roleLevel = roleInfo.RoleLevel;

            notification.type = (int)PlayerNotificationType.CreateRoleSuccess;
            notification.mode = NotificationMode.ValueType;
            notification.data2 = playerBaseData.roleInfo;
        }
        else
        {
            notification.type = (int)PlayerNotificationType.CreateRoleFailed;
            notification.mode = NotificationMode.ValueType;
            notification.data2 = resCreateRole.Result;
        }

        worldMgr.NotificationCenter.Notificate(notification);
    }
}