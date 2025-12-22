using System.ComponentModel;

public enum SpecialEventType
{
  UNKNOWN = -1, // 0xFFFFFFFF
  [Description("none")] IGNORE = 0,
  [Description("fireside_gatherings_cardback")] FIRESIDE_GATHERINGS_CARDBACK = 1,
  [Description("gvg_promote")] GVG_PROMOTION = 7,
  [Description("lunar_new_year")] LUNAR_NEW_YEAR = 11, // 0x0000000B
  [Description("tb_pre_event")] SPECIAL_EVENT_PRE_TAVERN_BRAWL = 19, // 0x00000013
  [Description("feast_of_winter_veil")] FEAST_OF_WINTER_VEIL = 85, // 0x00000055
  [Description("never")] SPECIAL_EVENT_NEVER = 164, // 0x000000A4
  [Description("friend_week")] FRIEND_WEEK = 166, // 0x000000A6
  [Description("always")] SPECIAL_EVENT_ALWAYS = 203, // 0x000000CB
  [Description("event_happy_new_year")] SPECIAL_EVENT_HAPPY_NEW_YEAR = 219, // 0x000000DB
  [Description("fire_festival")] SPECIAL_EVENT_FIRE_FESTIVAL = 287, // 0x0000011F
  [Description("gold_doubled")] SPECIAL_EVENT_GOLD_DOUBLED = 289, // 0x00000121
  [Description("frost_festival")] SPECIAL_EVENT_FROST_FESTIVAL = 292, // 0x00000124
  [Description("icc_normal_sale")] SPECIAL_EVENT_ICC_NORMAL_SALE = 307, // 0x00000133
  [Description("frost_fest_free_arena_win")] SPECIAL_EVENT_FROST_FESTIVAL_FREE_ARENA_WIN = 315, // 0x0000013B
  [Description("pirate_day")] SPECIAL_EVENT_PIRATE_DAY = 316, // 0x0000013C
  [Description("icc_launch_freepacks")] SPECIAL_EVENT_ICC_LAUNCH_FREEPACKS = 320, // 0x00000140
  [Description("hearthstone_world_championship")] SPECIAL_EVENT_HEARTHSTONE_WORLD_CHAMPIONSHIP = 408, // 0x00000198
  [Description("wild_week_2018")] SPECIAL_EVENT_WILD_WEEK_2018 = 410, // 0x0000019A
  [Description("road_to_raven")] SPECIAL_EVENT_ROAD_TO_RAVEN = 414, // 0x0000019E
  [Description("noblegarden_event")] SPECIAL_EVENT_NOBLEGARDEN = 473, // 0x000001D9
  [Description("taverns_of_time")] SPECIAL_EVENT_TAVERNS_OF_TIME = 490, // 0x000001EA
  [Description("fire_festival_v2")] SPECIAL_EVENT_FIRE_FESTIVAL_V2 = 499, // 0x000001F3
  [Description("days_of_the_frozen_throne")] SPECIAL_EVENT_DAYS_OF_THE_FROZEN_THRONE = 525, // 0x0000020D
  [Description("blizzcon_2018_flare")] SPECIAL_EVENT_BLIZZCON_2018_FLARE = 528, // 0x00000210
  [Description("celebrate_the_players")] SPECIAL_EVENT_CELEBRATE_THE_PLAYERS = 541, // 0x0000021D
  [Description("feast_of_winter_veil_2018")] SPECIAL_EVENT_FEAST_OF_WINTER_VEIL_2018 = 567, // 0x00000237
  [Description("rastakhan_season_week_1")] SPECIAL_EVENT_SEASON_OF_RASTAKHAN_WK1 = 580, // 0x00000244
  [Description("rastakhan_season_week_2")] SPECIAL_EVENT_SEASON_OF_RASTAKHAN_WK2 = 581, // 0x00000245
  [Description("rastakhan_season_week_3")] SPECIAL_EVENT_SEASON_OF_RASTAKHAN_WK3 = 582, // 0x00000246
  [Description("henchmania_tb_quest")] SPECIAL_EVENT_HENCHMANIA_TB_SEASON = 583, // 0x00000247
  [Description("fire_festival_v3")] SPECIAL_EVENT_FIRE_FESTIVAL_V3 = 584, // 0x00000248
  [Description("tb_season_221")] SPECIAL_EVENT_TB_SEASON_221 = 585, // 0x00000249
  [Description("tb_season_222")] SPECIAL_EVENT_TB_SEASON_222 = 586, // 0x0000024A
  [Description("uldum_launch_quest")] SPECIAL_EVENT_ULDUM_LAUNCH_QUEST = 587, // 0x0000024B
  [Description("post_hall_of_fame_2020")] SPECIAL_EVENT_POST_HALL_OF_FAME_2020 = 588, // 0x0000024C
  [Description("pre_hall_of_fame_2020")] SPECIAL_EVENT_PRE_HALL_OF_FAME_2020 = 589, // 0x0000024D
  [Description("fire_festival_emote_ever_green")] SPECIAL_EVENT_FIRE_FESTIVAL_EMOTES_EVERGREEN = 590, // 0x0000024E
  [Description("fire_festival_box_dressing_ever_green")] SPECIAL_EVENT_FIRE_FESTIVAL_BOX_DRESSING_EVERGREEN = 591, // 0x0000024F
  BASE_SPECIAL_EVENT_DATA_ID = 10000000, // 0x00989680
}
