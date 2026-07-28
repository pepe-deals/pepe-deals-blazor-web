//Comandos:
//Add-Migration "Initial Crate"
//Update-Database

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace pepeizqs_deals_web.Data;

public class Usuario : IdentityUser
{
	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? Nickname { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? Role { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(256)")]
	public string? SteamAccount { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(256)")]
	public string? SteamAccountLastCheck { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? SteamGames { get; set; }

	[Column(TypeName = "bit(1)")]
	public bool? SteamGamesAllow { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? SteamWishlist { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? SteamWishlistAllow { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? Wishlist { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(256)")]
	public string? Avatar { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? OfficialGroup { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? Keys { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationLows { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationWishlistBundles { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationBundles { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationFree { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationSubscriptions { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationWeb { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationDelisted { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationOthers { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? OfficialGroup2 { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? SteamId { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? WishlistPosition { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? WishlistSort { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? WishlistOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? WishlistOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? HistoricalLowsSort { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? HistoricalLowsOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? HistoricalLowsOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? HistoricalLowsOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? HistoricalLowsOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? HistoricalLowsStores { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? HistoricalLowsDRMs { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? HistoricalLowsTypes { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? HistoricalLowsTags { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? HistoricalLowsCategories2 { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? HistoricalLowsSteamDeck { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? HistoricalLowsSteamOS { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? HistoricalLowsRelease { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? HistoricalLowsAI { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? HistoricalLowsReviews { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? IndexOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? IndexOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? IndexOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? IndexOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? IndexOption5 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? IndexOption6 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? IndexOption7 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? IndexOption8 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? IndexOption9 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? IndexOption10 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? IndexOption11 { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? IndexDRMs { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? IndexCategories { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? Language { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? LanguageOverride { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? LanguageGames { get; set; }

	[PersonalData]
	[Column(TypeName = "datetime(8)")]
	public DateTime? PatreonLastCheck { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(128)")]
	public string? PatreonMail { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? PatreonCoins { get; set; }

	[PersonalData]
	[Column(TypeName = "datetime(8)")]
	public DateTime? PatreonLastLogin { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? PatreonContribution { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? PatreonOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? PatreonOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(256)")]
	public string? GogAccount { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(100)")]
	public string? GogId { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? GogWishlist { get; set; }

	[Column(TypeName = "bit(1)")]
	public bool? GogWishlistAllow { get; set; }

	[PersonalData]
	[Column(TypeName = "datetime(8)")]
	public DateTime? GogAccountLastCheck { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? GogGames { get; set; }

	[Column(TypeName = "bit(1)")]
	public bool? GogGamesAllow { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationPushLows { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationPushBundles { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationPushFree { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationPushSubscriptions { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationPushWeb { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NotificationPushOthers { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? AmazonGames { get; set; }

	[PersonalData]
	[Column(TypeName = "datetime(8)")]
	public DateTime? AmazonLastImport { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? EpicGames { get; set; }

	[PersonalData]
	[Column(TypeName = "datetime(8)")]
	public DateTime? EpicGamesLastImport { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? UbisoftGames { get; set; }

	[PersonalData]
	[Column(TypeName = "datetime(8)")]
	public DateTime? UbisoftLastImport { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? EaGames { get; set; }

	[PersonalData]
	[Column(TypeName = "datetime(8)")]
	public DateTime? EaLastImport { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? ForumRank { get; set; }

	[PersonalData]
	[Column(TypeName = "datetime(8)")]
	public DateTime? ForumLastChangeName { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? ForumIndex { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? ProfileShow { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(256)")]
	public string? ProfileNickname { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? ProfileLastPlayed { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? ProfileGames { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? ProfileWishlist { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? ProfileSteamAccount { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? ProfileGogAccount { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(512)")]
	public string? ProfileAvatar { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(1024)")]
	public string? ProfileNickname2 { get; set; }

	[PersonalData]
	[Column(TypeName = "nvarchar(512)")]
	public string? WishlistData { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? SteamDeckOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamDeckOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamDeckOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamDeckOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? SteamOSOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamOSOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamOSOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamOSOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GeforceNOWOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? GeforceNOWOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? GeforceNOWOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? GeforceNOWOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? BoosteroidOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? BoosteroidOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? BoosteroidOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? BoosteroidOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? AmazonLunaOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? AmazonLunaOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? AmazonLunaOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? AmazonLunaOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? Currency { get; set; } = 0;

	[PersonalData]
	[Column(TypeName = "nvarchar(max)")]
	public string? GamesExcluded { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption5 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption6 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption7 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption8 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption9 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption10 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption11 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption12 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption13 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption14 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption15 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? GameOption16 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? SteamMachineOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamMachineOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamMachineOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamMachineOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? SteamFrameOption1 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamFrameOption2 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamFrameOption3 { get; set; }

	[PersonalData]
	[Column(TypeName = "int(4)")]
	public int? SteamFrameOption4 { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? NoOfficial { get; set; }

	[PersonalData]
	[Column(TypeName = "bit(1)")]
	public bool? Marketplace { get; set; }
}