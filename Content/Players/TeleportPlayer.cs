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
        public bool teleportMode;
        private bool _wasFullscreen = false;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            teleportMode = Player.HeldItem.ModItem is Items.BasicBook;
        }
        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer) return;
            if (!teleportMode || !Main.mapFullscreen) return;

            if (Main.mouseRight && Main.mouseRightRelease)
            {
                Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY);
                float scale = Main.mapFullscreenScale;
                Vector2 screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) / 2f;

                // mapFullscreenPos is already in tile units, no conversion needed
                Vector2 tilePos = Main.mapFullscreenPos + (mouse - screenCenter) / scale;

                int tileX = (int)tilePos.X;
                int tileY = (int)tilePos.Y;

                TryTeleport(tileX, tileY);
            }
        }

       public bool HasRoom(int x, int y)
        {
            // Bounds check
            if (x < 1 || x + 1 >= Main.maxTilesX || y < 3 || y >= Main.maxTilesY)
                return false;

            // Just check the 2x3 area at cursor is free of solid tiles
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

        private void TryTeleport(int tileX, int tileY)
        {
            if (!HasRoom(tileX, tileY)) return;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                DoTeleport(tileX, tileY);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)0);
                packet.Write(tileX);
                packet.Write(tileY);
                packet.Send();
            }
        }
        // Called both server-side AND client-side (via response packet)
       public void DoTeleport(int tileX, int tileY)
        {
            // tileY is the floor tile, player stands on top of it
            // Player feet go at the TOP of the floor tile = tileY * 16f
            // Player origin is top-left, height is 56px, so subtract height to place feet correctly
            Vector2 position = new Vector2(tileX * 16f, tileY * 16f - Player.height);

            Player.Teleport(position, 1);
            Player.velocity = Vector2.Zero;

            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item6, Player.position);
                Main.mapFullscreen = false;
            }

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1,
                    null, 0, Player.whoAmI, position.X, position.Y);
            }
        }
    }
}