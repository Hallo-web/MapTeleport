using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MapTeleport.Content.Items
{
	// This is a basic item template.
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class BasicBook : ModItem
	{
		// The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.MapTeleport.hjson' file.
		public override void SetDefaults()
		{
			Item.reuseDelay = 20;
			Item.width = 10;
			Item.height = 10;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.ScaryScream;
			Item.autoReuse = false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.FragmentSolar, 5);
			recipe.AddIngredient(ItemID.FragmentStardust, 5);
			recipe.AddIngredient(ItemID.FragmentVortex, 5);
			recipe.AddIngredient(ItemID.FragmentNebula, 5);
			recipe.AddIngredient(ItemID.GoldWatch, 1);
			recipe.AddTile(TileID.GrandfatherClocks);
			recipe.Register();
		}
	}
}
