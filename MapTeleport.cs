using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.IO;

namespace MapTeleport
{
    public class MapTeleport : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte packetID = reader.ReadByte();

            switch (packetID)
            {
                case 0:
                    int playerID = reader.ReadInt32();
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        Player player = Main.player[playerID];
                        player.GetModPlayer<Content.Players.TeleportPlayer>().DoTeleport(x, y);
                    }
                    break;
            }
        }
    }
}