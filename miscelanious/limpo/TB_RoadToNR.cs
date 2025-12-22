using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_RoadToNR : MissionEntity
{
  private static readonly AssetReference Tombs_of_Terror_Reno_BrassRing_Quote = new AssetReference("Tombs_of_Terror_Reno_BrassRing_Quote.prefab:4c0b79d4f597c464baabf02e06cf8ae7");
  private static readonly AssetReference Tombs_of_Terror_Finley_BrassRing_Quote = new AssetReference("Tombs_of_Terror_Finley_BrassRing_Quote.prefab:547ebc970764ec64da6eb3de26ed4698");
  private static readonly AssetReference Tombs_of_Terror_Brann_BrassRing_Quote = new AssetReference("Tombs_of_Terror_Brann_BrassRing_Quote.prefab:d521a1fe41518e24da6e4252b97fbeb7");
  private static readonly AssetReference Tombs_of_Terror_Elise_BrassRing_Quote = new AssetReference("Tombs_of_Terror_Elise_BrassRing_Quote.prefab:6e9e8faaae29f834183122d7ea9be68d");
  private static readonly AssetReference Bob_BrassRing_Quote = new AssetReference("Bob_BrassRing_Quote.prefab:89385ff7d67aa1e49bcf25bc15ca61f6");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Start_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Start_01.prefab:057649bc2d715e54c90bc2eaf0c06cd0");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss1_Exchange1Response_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss1_Exchange1Response_01.prefab:5a9b0920bb977d648a7ba8adc4ffe899");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Exchange1Response_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Exchange1Response_01.prefab:09c3998f6955b9f419db61a586e81079");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss1_Exchange2_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss1_Exchange2_01.prefab:5c3914c6ba61fa744b48603a37ebf66f");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss1_Exchange2Response_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss1_Exchange2Response_01.prefab:cc216a5c5160757469b6da3a933f5b40");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss1_Exchange3Response_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss1_Exchange3Response_01.prefab:7caffbd5bbef1c94c9b30e753e3c92b4");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Exchange3Response_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Exchange3Response_01.prefab:2e78ea75e1d23b048bd6f6102d49b65d");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Loss_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Loss_01.prefab:3a17d9b891c780745b4e06cea06464a6");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Victory_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Victory_01.prefab:14695e3b456b366468d89a5ba157d81c");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss2_StartResponse_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss2_StartResponse_01.prefab:a1451d0fedf18cf4c8f746a3e545e428");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss2_Exchange1_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss2_Exchange1_01.prefab:37f8afe94af1ca243ad4a5ecca061c9d");
  private static readonly AssetReference VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Exchange1Response_01 = new AssetReference("VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Exchange1Response_01.prefab:dfbf26920132d64449d71ed0b44abea2");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Exchange2_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Exchange2_01.prefab:8783ace3dd5add64d98f1d40d0a34f12");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Exchange3_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Exchange3_01.prefab:18b1bdb432448094e820521b4d9177b9");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Victory_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Victory_01.prefab:240d7f1e901556444a06dd3746e52f98");
  private static readonly AssetReference VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Loss_01 = new AssetReference("VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Loss_01.prefab:124dbe4154e5b23448fa034cb0b3da59");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss3_HostLine_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss3_HostLine_01.prefab:42bcec03b7d533b41bbc388e7825d52f");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss3_Start_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss3_Start_01.prefab:d23e1e7caf2455940bdc602de739e051");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss3_Exchange1_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss3_Exchange1_01.prefab:fca58ee9a17805648a7160d39885ce92");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss3_Exchange1Response_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss3_Exchange1Response_01.prefab:1c0bfa679ffa1ae489bb360eaa8c846f");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss3_Exchange2_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss3_Exchange2_01.prefab:610173ade6d3aa04e94d575b2238ff8e");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss3_Victory_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss3_Victory_01.prefab:1b8ee3bcf30f25340943c8ef75a75dfb");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_HostLine_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_HostLine_01.prefab:6ecc3b3012db61f4b947c7c8697174ea");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss4_HostLineResponse_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss4_HostLineResponse_01.prefab:d2aa424df7329f84b9c9e9f8ee42d967");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Start_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Start_01.prefab:89c73eb7bd94d2046ad9fc22d41ac434");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_Exchange1_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_Exchange1_01.prefab:5928ddb577a71924bbcb4edfae86b761");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss4_Exchange2_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss4_Exchange2_01.prefab:6b476b6d90a675b4f988a659ec17af31");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Exchange2Response_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Exchange2Response_01.prefab:84096eb707e0fbe4aa3b9f53803fb2b4");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Exchange3_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Exchange3_01.prefab:374a9663c1348b447b6c9759d28aee4f");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Loss_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Loss_01.prefab:aa3c5c10d6a7a9444b7878fe5f19cd88");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Victory_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Victory_01.prefab:9e7b9fd0980e83b40870712e0a38182b");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_VictoryResponse_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_VictoryResponse_01.prefab:bda4ef726431a7648992924367f2cb34");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss5_HostLine_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss5_HostLine_01.prefab:06e6b3d43ba630f4595858fecdb3a175");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss5_HostLineResponse_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss5_HostLineResponse_01.prefab:f7e40439e8c1372498485e68745b637c");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Start_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Start_02.prefab:5abc6795f97b49e4abb6d20b1bd94af4");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss5_Exchange1_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss5_Exchange1_01.prefab:f4f41f31cbd6eb3479d4eed327360a5e");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange1Response_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange1Response_01.prefab:e1c12b5789973b24eb89f341f6813e0b");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange2_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange2_01.prefab:d7f12649fd3a45d4793def83a3bf4513");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange3_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange3_01.prefab:a383f5fb67af80c449320d2e5ba5eb9c");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss5_Exchange3Response1_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss5_Exchange3Response1_01.prefab:d07c98f264a7abb459f8e454f3b5e1d6");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange3Response2_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange3Response2_01.prefab:e47388e8b2daa18428faa644b1bf3fd9");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange4_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange4_01.prefab:aad73eb715845014c96129f7198f120e");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss5_Victory_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss5_Victory_01.prefab:840ef4dd806d5724a90f04a479dfea2c");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss5_VictoryResponse_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss5_VictoryResponse_01.prefab:13e14a1a628e9764483d4922a97515fa");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_HostLine_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_HostLine_01.prefab:27ea455959d060c4589b311500fbb3fb");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss6_HostLineResponse_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss6_HostLineResponse_01.prefab:733c2d96d4859174aafc294fa001e572");
  private static readonly AssetReference VO_DAL_719_Male_Kobold_Northrend_Boss6_Start_01 = new AssetReference("VO_DAL_719_Male_Kobold_Northrend_Boss6_Start_01.prefab:28c4a1bb916b3c346bba998d31097341");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss6_Exchange1_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss6_Exchange1_01.prefab:be63fedec4ef8df448204c4d43be6c33");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss6_Exchange1Response_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss6_Exchange1Response_01.prefab:f98a0ae34c0916e4c80d428d06a57b9a");
  private static readonly AssetReference VO_DAL_719_Male_Kobold_Northrend_Boss6_Exchange2_01 = new AssetReference("VO_DAL_719_Male_Kobold_Northrend_Boss6_Exchange2_01.prefab:2c14109e147f0844c97df0e2e0d24265");
  private static readonly AssetReference VO_DAL_719_Male_Kobold_Northrend_Boss6_Exchange3_01 = new AssetReference("VO_DAL_719_Male_Kobold_Northrend_Boss6_Exchange3_01.prefab:41544ce2729bf9d41974b604d70060df");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_Victory1_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_Victory1_01.prefab:9dc21085ed648c8458fae6040c8213a8");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_Victory2_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_Victory2_01.prefab:585a13de5c099a1428fd038c482150c6");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_HostLine_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_HostLine_01.prefab:85b4cd41119500e4a9d1fbe439c51b3e");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Start_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Start_01.prefab:b4ec9ab6a5cef8844ada36c4db428c8a");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss7_StartResponse_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss7_StartResponse_01.prefab:b78bcadb4150612439896840737ce0c1");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss7_Exchange1_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss7_Exchange1_01.prefab:85c141a0c9c42894aa60d8c95e3265d3");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_Exchange1Response_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_Exchange1Response_01.prefab:5df5394abd7f91d4496cd61a8b348133");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Exchange2_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Exchange2_01.prefab:1ecc65440ee806d458f2fb146da958c5");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_Exchange2Response_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_Exchange2Response_01.prefab:cca9f91532be6f2468af1e4ce96a771c");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Victory_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Victory_01.prefab:39953f9661708234a8ccf94d6f35cd22");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss8_Start_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss8_Start_01.prefab:5ad5b4d2a50aac1499b1f1868e2b84f7");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss8_Start_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss8_Start_01.prefab:3fa81cc157bb74a42ad56ab662f79edd");
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss8_Loss_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss8_Loss_01.prefab:f01d31b97c6c47640bfaae1d407037f0");
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss8_Loss_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss8_Loss_01.prefab:435120e5f0bc0784cbc143a30a9a78d9");
  private static readonly AssetReference VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss8_Victory1_01 = new AssetReference("VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss8_Victory1_01.prefab:63cee9eae5ff3b74cbb55a39d5061422");
  private static readonly AssetReference VO_DALA_900h_Male_Human_PlayerBrood_01 = new AssetReference("VO_DALA_900h_Male_Human_PlayerBrood_01.prefab:456313fb3b1046a47a8313a452bb6415");
  private static readonly Vector3 LEFT_OF_FRIENDLY_HERO = new Vector3(1.3f, 0.0f, 1f);
  private static readonly Vector3 LEFT_OF_ENEMY_HERO = new Vector3(1.3f, 0.0f, -1.8f);
  private static readonly Vector3 RIGHT_OF_ENEMY_HERO = new Vector3(-6.3f, 0.0f, -1.8f);
  public List<string> m_BossVOLines = new List<string>();
  public List<string> m_PlayerVOLines = new List<string>();
  private HashSet<int> seen = new HashSet<int>();
  private static readonly Dictionary<int, TB_RoadToNR.PopupMessage> popupMsgs = new Dictionary<int, TB_RoadToNR.PopupMessage>()
  {
    {
      1001,
      new TB_RoadToNR.PopupMessage()
      {
        Message = "VO_DALA_BOSS_60h_Male_Human_FloatingHead_Trigger_Copycard_01"
      }
    },
    {
      2002,
      new TB_RoadToNR.PopupMessage()
      {
        Message = "VO_DALA_BOSS_60h_Male_Human_FloatingHead_Trigger_CopyCards_01"
      }
    }
  };
  private Notification m_popup;
  private float popupScale = 1.4f;
  private int m_PlayPlayerVOLineIndex;
  private int m_PlayBossVOLineIndex;

  public void SetBossVOLines(List<string> VOLines) => this.m_BossVOLines = new List<string>((IEnumerable<string>) VOLines);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Start_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss1_Exchange1Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Exchange1Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss1_Exchange2_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss1_Exchange2Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss1_Exchange3Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Exchange3Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Loss_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Victory_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss2_StartResponse_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss2_Exchange1_01,
      (string) TB_RoadToNR.VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Exchange1Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Exchange2_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Exchange3_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Victory_01,
      (string) TB_RoadToNR.VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Loss_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss3_HostLine_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss3_Start_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss3_Exchange1_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss3_Exchange1Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss3_Exchange2_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss3_Victory_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_HostLine_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss4_HostLineResponse_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Start_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_Exchange1_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss4_Exchange2_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Exchange2Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Exchange3_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Loss_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Victory_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_VictoryResponse_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss5_HostLine_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss5_HostLineResponse_01,
      (string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Start_02,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss5_Exchange1_01,
      (string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange1Response_01,
      (string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange2_01,
      (string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange3_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss5_Exchange3Response1_01,
      (string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange3Response2_01,
      (string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange4_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss5_Victory_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss5_VictoryResponse_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_HostLine_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss6_HostLineResponse_01,
      (string) TB_RoadToNR.VO_DAL_719_Male_Kobold_Northrend_Boss6_Start_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss6_Exchange1_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss6_Exchange1Response_01,
      (string) TB_RoadToNR.VO_DAL_719_Male_Kobold_Northrend_Boss6_Exchange2_01,
      (string) TB_RoadToNR.VO_DAL_719_Male_Kobold_Northrend_Boss6_Exchange3_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_Victory1_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_Victory2_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_HostLine_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Start_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss7_StartResponse_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss7_Exchange1_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_Exchange1Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Exchange2_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_Exchange2Response_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Victory_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss8_Start_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss8_Start_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss8_Loss_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss8_Loss_01,
      (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss8_Victory1_01,
      (string) TB_RoadToNR.VO_DALA_900h_Male_Human_PlayerBrood_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_RoadToNR tbRoadToNr = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    Actor playerActor = GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    if (!tbRoadToNr.seen.Contains(missionEvent))
    {
      tbRoadToNr.seen.Add(missionEvent);
      while (tbRoadToNr.m_enemySpeaking)
        yield return (object) null;
      switch (missionEvent)
      {
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (tbRoadToNr.m_PlayPlayerVOLineIndex + 1 >= tbRoadToNr.m_PlayerVOLines.Count)
            tbRoadToNr.m_PlayPlayerVOLineIndex = 0;
          else
            ++tbRoadToNr.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(tbRoadToNr.m_PlayerVOLines[tbRoadToNr.m_PlayPlayerVOLineIndex]);
          yield return (object) tbRoadToNr.PlayBossLine(playerActor, tbRoadToNr.m_PlayerVOLines[tbRoadToNr.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(tbRoadToNr.m_PlayerVOLines[tbRoadToNr.m_PlayPlayerVOLineIndex]);
          yield return (object) tbRoadToNr.PlayBossLine(playerActor, tbRoadToNr.m_PlayerVOLines[tbRoadToNr.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (tbRoadToNr.m_PlayBossVOLineIndex + 1 >= tbRoadToNr.m_BossVOLines.Count)
            tbRoadToNr.m_PlayBossVOLineIndex = 0;
          else
            ++tbRoadToNr.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(tbRoadToNr.m_BossVOLines[tbRoadToNr.m_PlayBossVOLineIndex]);
          yield return (object) tbRoadToNr.PlayBossLine(enemyActor, tbRoadToNr.m_BossVOLines[tbRoadToNr.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(tbRoadToNr.m_BossVOLines[tbRoadToNr.m_PlayBossVOLineIndex]);
          yield return (object) tbRoadToNr.PlayBossLine(enemyActor, tbRoadToNr.m_BossVOLines[tbRoadToNr.m_PlayBossVOLineIndex]);
          break;
        case 1234:
          int tag = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
          string msgString;
          if (tag == 0)
          {
            msgString = GameStrings.Get(TB_RoadToNR.popupMsgs[2000].Message);
          }
          else
          {
            string str1 = "";
            string str2 = "";
            string str3 = "";
            int num1 = tag / 3600;
            int num2 = tag % 3600 / 60;
            int num3 = tag % 60;
            if (num1 < 10)
              str1 = "0";
            if (num2 < 10)
              str2 = "0";
            if (num3 < 10)
              str3 = "0";
            msgString = GameStrings.Get(TB_RoadToNR.popupMsgs[missionEvent].Message) + "\n" + str1 + (object) num1 + ":" + str2 + (object) num2 + ":" + str3 + (object) num3;
            tbRoadToNr.popupScale = 1.7f;
          }
          Vector3 popUpPos = new Vector3();
          popUpPos.z = GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer() ? ((bool) UniversalInputManager.UsePhoneUI ? -40f : -40f) : ((bool) UniversalInputManager.UsePhoneUI ? 27f : 18f);
          yield return (object) new WaitForSeconds(4f);
          yield return (object) tbRoadToNr.ShowPopup(msgString, TB_RoadToNR.popupMsgs[missionEvent].Delay, popUpPos, tbRoadToNr.popupScale);
          msgString = (string) null;
          popUpPos = new Vector3();
          break;
        case 159924:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Start_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 0.0f);
          GameState.Get().SetBusy(false);
          break;
        case 159925:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Loss_01, Notification.SpeechBubbleDirection.TopRight, enemyActor));
          yield return (object) new WaitForSeconds(4.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss2_StartResponse_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 0.0f);
          GameState.Get().SetBusy(false);
          break;
        case 159926:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_HostLine_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 4.5f);
          yield return (object) new WaitForSeconds(1f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss4_HostLineResponse_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3.5f);
          GameState.Get().SetBusy(false);
          break;
        case 159927:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss3_HostLine_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3.5f);
          yield return (object) new WaitForSeconds(3.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss3_Start_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 1.5f);
          GameState.Get().SetBusy(false);
          break;
        case 159928:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Start_02, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 159929:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_HostLine_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3.5f);
          yield return (object) new WaitForSeconds(1f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss6_HostLineResponse_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft);
          GameState.Get().SetBusy(false);
          break;
        case 159930:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_HostLine_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3f);
          GameState.Get().SetBusy(false);
          break;
        case 159933:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss8_Start_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 159998:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss8_Start_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 160313:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Bob_BrassRing_Quote, (string) TB_RoadToNR.VO_DALA_900h_Male_Human_PlayerBrood_01, TB_RoadToNR.LEFT_OF_FRIENDLY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3f);
          GameState.Get().SetBusy(false);
          break;
        case 259924:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss1_Exchange1Response_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 0.0f);
          yield return (object) new WaitForSeconds(3f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Exchange1Response_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 0.0f);
          GameState.Get().SetBusy(false);
          break;
        case 259925:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss2_Exchange1_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 0.0f);
          yield return (object) new WaitForSeconds(2.5f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Exchange1Response_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          yield return (object) new WaitForSeconds(2.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Exchange2_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 4f);
          GameState.Get().SetBusy(false);
          break;
        case 259926:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Start_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 3.5f);
          GameState.Get().SetBusy(false);
          break;
        case 259928:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Brann_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss5_HostLine_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss5_HostLineResponse_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 3.5f);
          GameState.Get().SetBusy(false);
          break;
        case 259929:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DAL_719_Male_Kobold_Northrend_Boss6_Start_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          yield return (object) new WaitForSeconds(2.5f);
          GameState.Get().SetBusy(false);
          break;
        case 259930:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Start_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 4f);
          yield return (object) new WaitForSeconds(1f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Brann_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss7_StartResponse_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft);
          GameState.Get().SetBusy(false);
          break;
        case 259933:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss8_Loss_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          yield return (object) new WaitForSeconds(2.5f);
          GameState.Get().SetBusy(false);
          break;
        case 259998:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss8_Loss_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          yield return (object) new WaitForSeconds(2.5f);
          GameState.Get().SetBusy(false);
          break;
        case 359924:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Brann_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss1_Exchange2_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 0.0f);
          yield return (object) new WaitForSeconds(2.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss1_Exchange2Response_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 0.0f);
          GameState.Get().SetBusy(false);
          break;
        case 359925:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Exchange3_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3f);
          GameState.Get().SetBusy(false);
          break;
        case 359926:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_Exchange1_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3.5f);
          GameState.Get().SetBusy(false);
          break;
        case 359927:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss3_Exchange1_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 2f);
          yield return (object) new WaitForSeconds(2f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Brann_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss3_Exchange1Response_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 2f);
          yield return (object) new WaitForSeconds(2f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss3_Exchange2_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3.5f);
          GameState.Get().SetBusy(false);
          break;
        case 359928:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss5_Exchange1_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 4.5f);
          yield return (object) new WaitForSeconds(1f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange1Response_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          yield return (object) new WaitForSeconds(2.5f);
          GameState.Get().SetBusy(false);
          break;
        case 359929:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DAL_719_Male_Kobold_Northrend_Boss6_Exchange2_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 359930:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss7_Exchange1_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 4f);
          yield return (object) new WaitForSeconds(1f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_Exchange1Response_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft);
          GameState.Get().SetBusy(false);
          break;
        case 359933:
        case 359998:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss8_Victory1_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 5f);
          GameState.Get().SetBusy(false);
          break;
        case 459924:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss1_Exchange3Response_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 0.0f);
          yield return (object) new WaitForSeconds(3f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Exchange3Response_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 0.0f);
          GameState.Get().SetBusy(false);
          break;
        case 459925:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss2_Victory_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 5f);
          GameState.Get().SetBusy(false);
          break;
        case 459926:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Brann_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss4_Exchange2_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Exchange2Response_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 3.5f);
          GameState.Get().SetBusy(false);
          break;
        case 459927:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss3_Victory_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 4f);
          GameState.Get().SetBusy(false);
          break;
        case 459928:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange2_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 459929:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss6_Exchange1_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 4f);
          yield return (object) new WaitForSeconds(1f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Brann_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss6_Exchange1Response_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft);
          GameState.Get().SetBusy(false);
          break;
        case 459930:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Exchange2_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 4f);
          yield return (object) new WaitForSeconds(1f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss7_Exchange2Response_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight);
          GameState.Get().SetBusy(false);
          break;
        case 559924:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Loss_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 0.0f);
          GameState.Get().SetBusy(false);
          break;
        case 559925:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_TB_GALAPortals_OrgGuardH_Male_Orc_Northrend_Boss2_Loss_01, Notification.SpeechBubbleDirection.TopRight, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 559926:
          GameState.Get().SetBusy(true);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Exchange3_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 3.5f);
          GameState.Get().SetBusy(false);
          break;
        case 559928:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(1f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange3_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          yield return (object) new WaitForSeconds(3.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss5_Exchange3Response1_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 3f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange3Response2_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 559929:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(1f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DAL_719_Male_Kobold_Northrend_Boss6_Exchange3_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 559930:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss7_Victory_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 3f);
          GameState.Get().SetBusy(false);
          break;
        case 659924:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss1_Victory_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 0.0f);
          yield return (object) new WaitForSeconds(1f);
          GameState.Get().SetBusy(false);
          break;
        case 659926:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Loss_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 4f);
          yield return (object) new WaitForSeconds(3f);
          GameState.Get().SetBusy(false);
          break;
        case 659928:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2f);
          Gameplay.Get().StartCoroutine(tbRoadToNr.PlaySoundAndBlockSpeech((string) TB_RoadToNR.VO_DALA_BOSS_22h_Female_Pandaren_Northrend_Boss5_Exchange4_01, Notification.SpeechBubbleDirection.TopLeft, enemyActor));
          GameState.Get().SetBusy(false);
          break;
        case 659929:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_Victory1_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 8f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss6_Victory2_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 2f);
          GameState.Get().SetBusy(false);
          break;
        case 759926:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Finley_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_03h_Male_Murloc_Northrend_Boss4_Victory_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 4f);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Reno_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_01h_Male_Human_Northrend_Boss4_VictoryResponse_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft, 2f);
          GameState.Get().SetBusy(false);
          break;
        case 759928:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Brann_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_04h_Male_Dwarf_Northrend_Boss5_Victory_01, TB_RoadToNR.LEFT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopLeft);
          yield return (object) tbRoadToNr.PlayMissionFlavorLine((string) TB_RoadToNR.Tombs_of_Terror_Elise_BrassRing_Quote, (string) TB_RoadToNR.VO_DRGA_BOSS_02h_Female_NightElf_Northrend_Boss5_VictoryResponse_01, TB_RoadToNR.RIGHT_OF_ENEMY_HERO, Notification.SpeechBubbleDirection.TopRight, 6f);
          GameState.Get().SetBusy(false);
          break;
      }
    }
  }

  private IEnumerator ShowPopup(
    string stringID,
    float popupDuration,
    Vector3 popUpPos,
    float popupScale)
  {
    this.m_popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPos, TutorialEntity.GetTextScale() * popupScale, GameStrings.Get(stringID), false);
    NotificationManager.Get().DestroyNotification(this.m_popup, popupDuration);
    yield return (object) new WaitForSeconds(0.0f);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public TB_RoadToNR()
    : base()
  {
  }

  public struct PopupMessage
  {
    public string Message;
    public float Delay;
  }
}
