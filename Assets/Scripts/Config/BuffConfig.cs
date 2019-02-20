﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

[System.Serializable]
public struct Effect
{
    public int id;
    public int[] selfBuffIdList;
    public int[] friendBuffIdList;
    public int[] enemyBuffIdList;
    public int duration;
    public bool initial;
}

public class BuffConfig : ScriptableObject
{
    public Buff[] buffList;
    public Effect[] effectList;

    Buff _defaultBuff;
    Effect _defaultEffect;

    public Buff GetBuff(int buffId)
    {
        for (var i = 0; i < buffList.Length; i++)
        {
            var buff = buffList[i];
            if (buff.id == buffId)
            {
                return buff;
            }
        }
        return _defaultBuff;
    }

    public Effect GetEffect(int effectId)
    {
        for (var i = 0; i < effectList.Length; i++)
        {
            var effect = effectList[i];
            if (effect.id == effectId)
            {
                return effect;
            }
        }

        return _defaultEffect;
    }
}
