using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;

namespace MapTeleport.Content.Players
{
    public class TeleportPlayer : ModPlayer
    {
        public bool teleportMode;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // If holding your book, enable teleport mode
            if (Player.HeldItem.ModItem is Items.BasicBook)
            {
                teleportMode = true;
            }
            else
            {
                teleportMode = false;
            }
        }

        public override void PostUpdate()
        {
            if (!teleportMode)
                return;

            if (!Main.mapFullscreen)
                return;

            if (Main.mouseRight && Main.mouseRightRelease)
            {
                Vector2 world;

                if (Main.mapFullscreen)
                {
                    Vector2 mouse = Main.MouseScreen;

                    float scale = Main.mapFullscreenScale;

                    world = (mouse - new Vector2(Main.screenWidth, Main.screenHeight) / 2f) 
                            / scale 
                            + Main.mapFullscreenPos;

                    world *= 16f; // convert tile → pixel
                }
                else
                {
                    world = Main.MouseWorld;
                }

                int tileX = (int)(world.X / 16f);
                int tileY = (int)(world.Y / 16f);

                TryTeleport(tileX, tileY);
            }
        }

                private bool HasRoom(int x, int y)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Tile tile = Framing.GetTileSafely(x + i, y - j);

                    if (tile.HasTile && Main.tileSolid[tile.TileType])
                        return false;
                }
            }

            return true;
        }

                private void TryTeleport(int x, int y)
        {
            if (!HasRoom(x, y))
                return;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                DoTeleport(x, y);
            }
            else
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)0); // packet ID
                packet.Write(Player.whoAmI);
                packet.Write(x);
                packet.Write(y);
                packet.Send();
            }
        }

        public void DoTeleport(int x, int y)
        {
            Vector2 position = new Vector2(x + 1, y - 1) * 16f;

            Player.Teleport(position, 0); // maybe put back to "TeleportationStyleID.RodOfDiscord"
            Player.velocity = Vector2.Zero;
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, Player.whoAmI, position.X, position.Y);
        }
    }
}