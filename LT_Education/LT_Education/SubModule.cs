
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;


#nullable enable
namespace LT_Education
{
  public class SubModule : MBSubModuleBase
  {
    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
      try
      {
        if (!(gameStarterObject is CampaignGameStarter) || !(game.GameType is Campaign))
          return;
        ((CampaignGameStarter) gameStarterObject).AddBehavior((CampaignBehaviorBase) new LT_EducationBehaviour());
        Logger.IM("}LT_Education loaded");
      }
      catch (Exception ex)
      {
        Logger.IMRed("LT_Education: An Error occurred, when trying to load the mod into your current game.");
        Logger.LogError(ex);
      }
    }

    protected override void OnSubModuleLoad() => base.OnSubModuleLoad();

    public override void OnMissionBehaviorInitialize(Mission mission)
    {
    }
  }
}
