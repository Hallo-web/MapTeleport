using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;

namespace MapTeleport
{
    public class MapTeleport : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte packetID = reader.ReadByte();

            if (packetID == 0) // teleport request
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();

                if (Main.netMode == NetmodeID.Server)
                {
                    Player player = Main.player[whoAmI];
                    var modPlayer = player.GetModPlayer<Content.Players.TeleportPlayer>();

                    if (modPlayer != null && modPlayer.HasRoom(x, y))
                    {
                        modPlayer.DoTeleport(x, y); // server executes teleport
                    }
                }
            }
        }
    }
}