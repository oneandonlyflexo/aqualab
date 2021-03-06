using System;
using BeauData;
using BeauUtil;
using UnityEngine;

namespace ProtoAqua.Energy
{
    public sealed class EnergySimState : IEnergySimStateReader
    {
        public const int MaxActors = 512;

        public EnvironmentState Environment;
        
        public readonly ActorState[] Actors = new ActorState[MaxActors];
        public ushort ActorCount;

        public ushort[] Populations;
        public uint[] Masses;
        
        public ushort Timestamp;
        public uint NextSeedA;
        public uint NextSeedB;
        public ushort NextActorId;

        public bool RequestResimulate;

        public void Reset()
        {
            Environment = default(EnvironmentState);
            Array.Clear(Actors, 0, Actors.Length);
            ActorCount = 0;
            Timestamp = 0;
            NextSeedA = 0;
            NextSeedB = 0;
            NextActorId = 0;
            RequestResimulate = false;
        }

        public void Reset(in EnergySimContext inContext)
        {
            Reset();

            Configure(inContext);
        }

        public void CopyFrom(in EnergySimState inState)
        {
            Environment = inState.Environment;

            Array.Copy(inState.Actors, Actors, MaxActors);
            ActorCount = inState.ActorCount;

            if (inState.Populations != null)
            {
                Array.Resize(ref Populations, inState.Populations.Length);
                Array.Copy(inState.Populations, Populations, Populations.Length);
            }

            if (inState.Masses != null)
            {
                Array.Resize(ref Masses, inState.Masses.Length);
                Array.Copy(inState.Masses, Masses, Masses.Length);
            }
            
            Timestamp = inState.Timestamp;
            NextSeedA = inState.NextSeedA;
            NextSeedB = inState.NextSeedB;
            NextActorId = inState.NextActorId;
            RequestResimulate = inState.RequestResimulate;
        }

        public void Configure(in EnergySimContext inContext)
        {
            FourCC[] actorTypeIds = inContext.Database.Actors.Ids();
            
            Array.Resize(ref Populations, actorTypeIds.Length);
            Array.Resize(ref Masses, actorTypeIds.Length);
            for(int i = 0; i < Populations.Length; ++i)
            {
                Populations[i] = 0;
                Masses[i] = 0;
            }
        }

        public ref ActorState AddActor(ActorType inType)
        {
            if (ActorCount >= MaxActors)
            {
                throw new Exception("Already at max actor count " + MaxActors);
            }

            ref ActorState state = ref Actors[ActorCount++];
            state.Id = NextActorId++;
            inType.CreateActor(ref state);
            return ref state;
        }

        public void AddActors(ActorType inType, int inCount, System.Random inRandom)
        {
            if (ActorCount >= MaxActors)
            {
                throw new Exception("Already at max actor count " + MaxActors);
            }
            
            while(--inCount >= 0)
            {
                ref ActorState actor = ref Actors[ActorCount++];
                actor.Id = NextActorId++;
                inType.CreateActor(ref actor);

                actor.OffsetA = (byte) inRandom.Next(3);
                actor.OffsetB = (byte) inRandom.Next(17);

                var growthSettings = inType.OriginalType().GrowthSettings();
                ushort maxAge = inType.OriginalType().DeathSettings().Age;
                if (maxAge > 0)
                {
                    actor.Age = (ushort) inRandom.Next(0, maxAge / 2);
                }
                else
                {
                    actor.Age = (ushort) inRandom.Next(0, 5);
                }

                if (actor.Age > 0 && growthSettings.Interval > 0 && growthSettings.MaxMass > growthSettings.StartingMass)
                {
                    ushort minMass = growthSettings.StartingMass;
                    ushort maxMass = growthSettings.MaxMass;

                    ushort estimatedGrowth = (ushort) ((actor.Age / growthSettings.Interval) * growthSettings.MinGrowth);
                    actor.Mass = (ushort) Math.Min(minMass + estimatedGrowth, maxMass);
                }
            }
        }

        public void DeleteActor(ushort inActorIndex)
        {
            ref ActorState actor = ref Actors[inActorIndex];
            actor = default(ActorState);

            if (inActorIndex < ActorCount - 1)
            {
                Ref.Swap(ref Actors[inActorIndex], ref Actors[ActorCount - 1]);
            }
            --ActorCount;
        }
    
        public void AddResourceToEnvironment(ISimDatabase inDatabase, FourCC inResourceType, ushort inCount)
        {
            int idx = inDatabase.Resources.IdToIndex(inResourceType);
            Environment.OwnedResources[idx] += inCount;
        }

        public void SetPropertyInEnvironment(ISimDatabase inDatabase, FourCC inPropertyType, float inValue)
        {
            int idx = inDatabase.Properties.IdToIndex(inPropertyType);
            Environment.Properties[idx] = inValue;
        }

        #region IEnergySimStateReader

        ushort IEnergySimStateReader.GetEnvironmentResource(FourCC inResourceId, ISimDatabase inDatabase)
        {
            int idx = inDatabase.Resources.IdToIndex(inResourceId);
            return Environment.OwnedResources[idx];
        }

        float IEnergySimStateReader.GetEnvironmentProperty(FourCC inPropertyId, ISimDatabase inDatabase)
        {
            int idx = inDatabase.Properties.IdToIndex(inPropertyId);
            return Environment.Properties[idx];
        }

        ushort IEnergySimStateReader.GetActorCount(FourCC inActorId, ISimDatabase inDatabase)
        {
            int idx = inDatabase.Actors.IdToIndex(inActorId);
            return Populations[idx];
        }

        uint IEnergySimStateReader.GetActorMass(FourCC inActorId, ISimDatabase inDatabase)
        {
            int idx = inDatabase.Actors.IdToIndex(inActorId);
            return Masses[idx];
        }

        FourCC IEnergySimStateReader.GetEnvironmentType()
        {
            return Environment.Type;
        }

        ushort IEnergySimStateReader.GetTickId()
        {
            return Timestamp;
        }

        #endregion // IEnergySimStateReader
    }
}