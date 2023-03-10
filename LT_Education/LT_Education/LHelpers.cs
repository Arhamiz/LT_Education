
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Extensions = TaleWorlds.Core.Extensions;


#nullable enable
namespace LT_Education
{
  internal class LHelpers
  {
    public static Settlement GetClosestSettlement(MobileParty heroParty)
    {
      Settlement closestSettlement = (Settlement) null;
      try
      {
        closestSettlement = Extensions.MinBy<Settlement, float>((IEnumerable<Settlement>) Settlement.FindAll((Func<Settlement, bool>) (s => s.IsTown || s.IsCastle || s.IsVillage)).ToList<Settlement>(), (Func<Settlement, float>) (s =>
        {
          Vec3 position = heroParty.GetPosition();
          return ((Vec3) position).DistanceSquared(s.GetPosition());
        }));
      }
      catch (Exception ex)
      {
        Logger.LogError(ex);
      }
      return closestSettlement;
    }

    public static List<Settlement> GetClosestTownsFromSettlement(
      Settlement settlement,
      int amount)
    {
      List<Settlement> townsFromSettlement = new List<Settlement>();
      try
      {
        if (settlement == null)
          return townsFromSettlement;
        if (settlement.IsTown)
          ++amount;
        townsFromSettlement = Settlement.FindAll((Func<Settlement, bool>) (s => s.IsTown)).ToList<Settlement>().OrderBy<Settlement, float>((Func<Settlement, float>) (s =>
        {
          Vec3 position = settlement.GetPosition();
          return ((Vec3)  position).DistanceSquared(s.GetPosition());
        })).Take<Settlement>(amount).ToList<Settlement>();
        if (settlement.IsTown)
          townsFromSettlement.RemoveAt(0);
      }
      catch (Exception ex)
      {
        Logger.LogError(ex);
      }
      return townsFromSettlement;
    }

    public static Settlement? GetRandomTown()
    {
      int num1 = 0;
      foreach (Settlement settlement in (List<Settlement>) Campaign.Current.Settlements)
      {
        if (settlement.IsTown)
          ++num1;
      }
      int num2 = MBRandom.RandomInt(0, num1 - 1);
      foreach (Settlement settlement in (List<Settlement>) Campaign.Current.Settlements)
      {
        if (settlement.IsTown)
        {
          --num2;
          if (num2 < 0)
            return settlement;
        }
      }
      return (Settlement) null;
    }
  }
}
