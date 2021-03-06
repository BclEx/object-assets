﻿using OA.Core;
using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server
{
    public class SupportedFeaturesPacket : RecvPacket
    {
        /// <summary>
        /// From POLServer packet docs: http://docs.polserver.com/packets/index.php?Packet=0xB9
        /// 0x01: enable T2A features: chat, regions
        /// 0x02: enable renaissance features
        /// 0x04: enable third dawn features
        /// 0x08: enable LBR features: skills, map
        /// 0x10: enable AOS features: skills, map, spells, fightbook
        /// 0x20: 6th character slot
        /// 0x40: enable SE features
        /// 0x80: enable ML features: elven race, spells, skills
        /// 0x100: enable 8th age splash screen
        /// 0x200: enable 9th age splash screen, crystal/shadow housing tiles
        /// 0x400: enable 10th age
        /// 0x800: enable increased housing and bank storage
        /// 0x1000: 7th character slot
        /// 0x2000: 10th age KR faces
        /// 0x4000: enable trial account
        /// 0x8000: enable 11th age
        /// 0x10000: enable SA features: gargoyle race, spells, skills
        /// 0x20000: HS features
        /// 0x40000: Gothic housing tiles
        /// 0x80000: Rustic housing tiles
        /// </summary>
        public FeatureFlags Flags { get; private set; }
        public SupportedFeaturesPacket(PacketReader reader)
            : base(0xB9, "Enable Features")
        {
            if (reader.Buffer.Length == 3) Flags = (FeatureFlags)reader.ReadUInt16();
            else if (reader.Buffer.Length == 5) Flags = (FeatureFlags)reader.ReadUInt16();
            else
            {
                Flags = (FeatureFlags)reader.ReadUInt16();
                Utils.Error($"Bad feature flag size in SupportedFeaturesPacket; expected 16 or 32 bit features, received {((reader.Buffer.Length - 1) * 8)} bits.");
            }
        }
    }
}
