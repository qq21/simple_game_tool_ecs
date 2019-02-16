﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum ActorStateType
    {
        Idle = 1,
        Move = 2,
        Jump = 4,
        SkillDefault = 8,
        SkillCustom = 16
    }

    public class ActorData : Data
    {
        public int actorId;
        public int currentState;
        public SkillDefaultType defaultSkill;
    }

    public class ActorController2DData : Data
    {
        public Rigidbody2D rigidbody2D;
        public Transform foot;
        public Transform hurt;
        public int groundY;
        public int positionY;
    }

    public class ActorFlyData : Data
    {
        public int duration;
        public int currentDuration;
    }

    public class ActorDashData : Data
    {
        public int duration;
        public int currentDuration;
    }

    public class ActorStressData : Data
    {
        public int duration;
        public int currentDuration;
    }

    public class ActorSyncData : Data
    {
    }

    public class FollowCameraData : Data
    {
    }

    public class ActorBuffData : Data
    {
        public List<Buff> buffList = new List<Buff>();
    }

    [System.Serializable]
    public struct AttributeInfo
    {
        public int hp;
        public int mp;
        public int atk;
        public int def;
    }

    public class ActorAttributeData : Data
    {
        public AttributeInfo baseAttribute;
        public AttributeInfo buffAttribute;
    }
}