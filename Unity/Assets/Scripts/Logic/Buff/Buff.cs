using System;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    public interface IBuff { }

    public enum EEntityAttriType {
        MinEnum = 0,
        IsSilence,
        IsStone,
        IsFrozen,
        IsInvincible,
        IsInVisible,
        EnumOfState = 31,
        Hp,
        MaxHp,
        EnumCount,
    }

    public struct EntityAttri {
        public bool IsSilence;
        public bool IsStone;
        public bool IsFrozen;
        public bool IsInvincible;
        public bool IsInVisible;
        public int Hp;
        public int MaxHp;
        public LFloat Speed;

        public void CopyFrom(EntityAttri attri){
            IsSilence = attri.IsSilence;
            IsStone = attri.IsStone;
            IsFrozen = attri.IsFrozen;
            IsInvincible = attri.IsInvincible;
            IsInVisible = attri.IsInVisible;
            Hp = attri.Hp;
            MaxHp = attri.MaxHp;
            Speed = attri.Speed;
        }
    }

    public class Buff : IBuff {
        public bool IsFinished => remainTime <= 0;
        public BuffConfig config;
        public EntityAttri triggerAttri;
        public LFloat remainTime;
        public LFloat timer => config.remainTime - remainTime;

        public int triggerId;
        public int counter;

        public Buff(int triggerId, BuffConfig config){
            this.config = config;
            this.triggerId = triggerId;
        }

        public virtual void DoStart(EntityAttri ownerAttri){
            for (int i = 0; i < config.count; i++) {
                var at = config.Attris[i];
                bool isSet = at.val != 0;
                var type = (EEntityAttriType) System.Math.Abs(at.attriType);
                if (isSet) {
                    switch (type) {
                        case EEntityAttriType.IsSilence: {
                            ownerAttri.IsSilence = isSet;
                            break;
                        }
                        case EEntityAttriType.IsStone: {
                            ownerAttri.IsStone = isSet;
                            break;
                        }
                        case EEntityAttriType.IsFrozen: {
                            ownerAttri.IsFrozen = isSet;
                            break;
                        }
                        case EEntityAttriType.IsInvincible: {
                            ownerAttri.IsInvincible = isSet;
                            break;
                        }
                        case EEntityAttriType.IsInVisible: {
                            ownerAttri.IsInVisible = isSet;
                            break;
                        }
                    }
                }
            }
        }


        public virtual void DoDestroy(EntityAttri ownerAttri){
            for (int i = 0; i < config.count; i++) {
                var at = config.Attris[i];
                bool isUnSet = at.val == 0;
                var type = (EEntityAttriType) System.Math.Abs(at.attriType);
                if (!isUnSet) {
                    switch (type) {
                        case EEntityAttriType.IsSilence: {
                            ownerAttri.IsSilence = isUnSet;
                            break;
                        }
                        case EEntityAttriType.IsStone: {
                            ownerAttri.IsStone = isUnSet;
                            break;
                        }
                        case EEntityAttriType.IsFrozen: {
                            ownerAttri.IsFrozen = isUnSet;
                            break;
                        }
                        case EEntityAttriType.IsInvincible: {
                            ownerAttri.IsInvincible = isUnSet;
                            break;
                        }
                        case EEntityAttriType.IsInVisible: {
                            ownerAttri.IsInVisible = isUnSet;
                            break;
                        }
                    }
                }
            }
        }

        public virtual void DoUpdate(LFloat deltaTime, EntityAttri ownerAttri){
            if (timer > counter * config.triggerInterval) {
                counter++;
                //trigger buff
                for (int i = 0; i < config.count; i++) {
                    var at = config.Attris[i];
                    var type = (EEntityAttriType) System.Math.Abs(at.attriType);
                    switch (type) {
                        case EEntityAttriType.Hp: {
                            ownerAttri.Hp += at.val + LMath.FloorToInt(at.percent * triggerAttri.Hp);
                            break;
                        }
                        case EEntityAttriType.MaxHp: {
                            ownerAttri.MaxHp += at.val + LMath.FloorToInt(at.percent * triggerAttri.Hp);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class BuffConfig {
        public bool CanMutil;
        public int BigType;
        public int Id;
        public int OwnerId;
        public int count;
        public BuffAttri[] Attris = new BuffAttri[4];
        public LFloat triggerInterval;
        public LFloat remainTime;

        public struct BuffAttri {
            public int val;
            public LFloat percent;
            public int attriType;
        }
    }

    public class BuffAgent : ILifeCycle {
        public List<Buff> buffs = new List<Buff>();
        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        private EntityAttri attri;

        public void RemoveBuff(){ }

        public virtual void DoUpdate(LFloat deltaTime){
            for (int i = buffs.Count - 1; i >= 0; --i) {
                if (buffs[i].IsFinished) {
                    buffs[i].DoDestroy(attri);
                    buffs.RemoveAt(i);
                }
            }

            foreach (var buff in buffs) {
                buff.DoUpdate(deltaTime, attri);
            }
        }

        public static Dictionary<int, BuffConfig> id2BuffConfig = new Dictionary<int, BuffConfig>();

        public void AddBuff(int id, int ownerId){
            if (id2BuffConfig.TryGetValue(id, out var val)) {
                var type = val.BigType;
                if (!val.CanMutil) {
                    foreach (var va in buffs) {
                        if (va.config.BigType == type) {
                            va.remainTime = LMath.Max(va.config.remainTime, va.remainTime);
                        }
                    }
                }

                var buff = new Buff(ownerId, val);
                AddBuff(buff);
                return;
            }

            Debug.LogError("Miss id " + id);
        }

        private void AddBuff(Buff buff){
            buffs.Add(buff);
            buff.DoStart(attri);
        }

        public virtual void DoDestroy(){
            foreach (var buff in buffs) {
                buff.DoDestroy(attri);
            }

            buffs.Clear();
        }
    }
}