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

           if (packetID == 0)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32(); // already resolved floor Y from client

                if (Main.netMode == NetmodeID.Server)
                {
                    Player player = Main.player[whoAmI];
                    var modPlayer = player.GetModPlayer<Content.Players.TeleportPlayer>();

                    // y is already the floor, just validate it's still safe on server side
                    if (modPlayer != null && modPlayer.HasRoom(x, y - 1))
                    {
                        modPlayer.DoTeleport(x, y);

                        ModPacket response = GetPacket();
                        response.Write((byte)1);
                        response.Write(x);
                        response.Write(y);
                        response.Send(whoAmI);
                    }
                }
            }
            else if (packetID == 1) // server → client: teleport confirm
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    var modPlayer = Main.LocalPlayer.GetModPlayer<Content.Players.TeleportPlayer>();
                    modPlayer.DoTeleport(x, y);
                }
            }
        }
    }
}