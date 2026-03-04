using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.Audio;
using System.IO;

namespace MapTeleport.Content.Players
{
    public class TeleportPlayer : ModPlayer
    {
        public bool teleportMode; // true when holding the book

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // Enable teleport mode if holding the book
            teleportMode = Player.HeldItem.ModItem is Items.BasicBook;
        }

        public override void PostUpdate()
        {
            if (!teleportMode || !Main.mapFullscreen)
                return;

            // Right-click map to teleport
            if (Main.mouseRight && Main.mouseRightRelease)
            {
                Vector2 mouse = Main.MouseScreen;
                float scale = Main.mapFullscreenScale;

                // Convert screen coords → world coords
                Vector2 world = (mouse - new Vector2(Main.screenWidth, Main.screenHeight) / 2f) 
                                / scale 
                                + Main.mapFullscreenPos;

                world *= 16f; // tile → pixel

                int tileX = (int)(world.X / 16f);
                int tileY = (int)(world.Y / 16f);

                TryTeleport(tileX, tileY);
            }
        }

        // Public so server packet handler can access it
        public bool HasRoom(int x, int y)
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
            if (!HasRoom(x, y)) return;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                DoTeleport(x, y);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Send teleport request to server
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)0); // packet ID = teleport
                packet.Write(x);
                packet.Write(y);
                packet.Send();
            }
        }

        // Executed server-side (or singleplayer)
        public void DoTeleport(int x, int y)
        {
            Vector2 position = new Vector2(x * 16f, y * 16f - Player.height);

            Player.Teleport(position, 0); // server handles sync
            Player.velocity = Vector2.Zero;

            // Optional teleport sound
            SoundEngine.PlaySound(SoundID.Item6, Player.position);
        }
    }
}