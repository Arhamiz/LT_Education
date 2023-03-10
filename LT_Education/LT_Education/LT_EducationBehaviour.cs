using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;


#nullable enable
namespace LT_Education
{
  public class LT_EducationBehaviour : CampaignBehaviorBase
  {
    private bool _debug = false;
    private static GauntletLayer? _gauntletLayer;
    private static GauntletMovie? _gauntletMovie;
    private static EducationPopupVM? _popupVM;
    private IEnumerable<ItemObject>? _bookList;
    private IEnumerable<Hero>? _vendorList;
    private float _canRead;
    private int _minINTToRead = 4;
    private int _readPrice = 10;
    private int _lastHourOfLearning;
    private int _bookInProgress;
    private float[] _bookProgress;

    private static LT_EducationBehaviour Instance { get; set; }

    public LT_EducationBehaviour()
    {
      LT_EducationBehaviour.Instance = this;
      this._canRead = 0.0f;
      this._bookInProgress = -1;
      this._bookProgress = new float[100];
      for (int index = 0; index < 100; ++index)
        this._bookProgress[index] = 0.0f;
    }

    public override void RegisterEvents()
    {
      CampaignEvents.WeeklyTickEvent.AddNonSerializedListener((object) this, new Action(this.WeeklyTickEvent));
      CampaignEvents.DailyTickEvent.AddNonSerializedListener((object) this, new Action(this.DailyTickEvent));
      CampaignEvents.GameMenuOpened.AddNonSerializedListener((object) this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
      CampaignEvents.SettlementEntered.AddNonSerializedListener((object) this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
      CampaignEvents.AfterSettlementEntered.AddNonSerializedListener((object) this, new Action<MobileParty, Settlement, Hero>(this.OnAfterSettlementEntered));
      CampaignEvents.HourlyTickEvent.AddNonSerializedListener((object) this, new Action(this.HourlyTickEvent));
      CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object) this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
      CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener((object) this, new Action<CampaignGameStarter>(this.OnGameLoaded));
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("debug", "lteducation")]
    public static string ConsoleDebug(List<string> args)
    {
      if (args.Count < 1)
        return "{=LT084}You must provide an argument";
      if (args[0] == "1")
      {
        LT_EducationBehaviour.Instance._debug = true;
        LT_EducationBehaviour.Instance._minINTToRead = 2;
        return "{=LT085}Debug enabled";
      }
      LT_EducationBehaviour.Instance._debug = false;
      LT_EducationBehaviour.Instance._minINTToRead = 4;
      return "{=LT086a}Debug disabled";
    }

    private void OnGameLoaded(CampaignGameStarter starter)
    {
      if (this._bookProgress.Length >= 100)
        return;
      Array.Resize<float>(ref this._bookProgress, 100);
    }

    private void OnSessionLaunched(CampaignGameStarter starter)
    {
      this._bookList = ((IEnumerable<ItemObject>) Items.All).Where<ItemObject>((Func<ItemObject, bool>) (x => ((MBObjectBase) x).StringId.Contains("education_book")));
      if (this._bookList == null)
        return;
      this._vendorList = Hero.FindAll((Func<Hero, bool>) (x => x.Name.Contains("Book Vendor")));
      if (this._vendorList.Count<Hero>() < 3)
        this.CreateBookVendors();
      if (this._vendorList == null)
        return;
      this.AddDialogs(starter);
      this.AddGameMenus(starter);
    }

    public static void CreatePopupVMLayer(
      string title,
      string smallText,
      string bigText,
      string textOverImage,
      string spriteName)
    {
      try
      {
        if (LT_EducationBehaviour._gauntletLayer != null)
          return;
        LT_EducationBehaviour._gauntletLayer = new GauntletLayer(1000, "GauntletLayer", false);
        if (LT_EducationBehaviour._popupVM == null)
          LT_EducationBehaviour._popupVM = new EducationPopupVM(title, smallText, bigText, textOverImage, spriteName);
        LT_EducationBehaviour._gauntletMovie = (GauntletMovie) LT_EducationBehaviour._gauntletLayer.LoadMovie("LTEducationBookPopup", (ViewModel) LT_EducationBehaviour._popupVM);
        ((ScreenLayer) LT_EducationBehaviour._gauntletLayer).InputRestrictions.SetInputRestrictions(true, (InputUsageMask) 7);
        ScreenManager.TopScreen.AddLayer((ScreenLayer) LT_EducationBehaviour._gauntletLayer);
        ((ScreenLayer) LT_EducationBehaviour._gauntletLayer).IsFocusLayer = true;
        ScreenManager.TrySetFocus((ScreenLayer) LT_EducationBehaviour._gauntletLayer);
        LT_EducationBehaviour._popupVM.Refresh();
      }
      catch (Exception ex)
      {
        Logger.LogError(ex);
      }
    }

    public static void DeletePopupVMLayer()
    {
      ScreenBase topScreen = ScreenManager.TopScreen;
      if (LT_EducationBehaviour._gauntletLayer != null)
      {
        ((ScreenLayer) LT_EducationBehaviour._gauntletLayer).InputRestrictions.ResetInputRestrictions();
        ((ScreenLayer) LT_EducationBehaviour._gauntletLayer).IsFocusLayer = false;
        if (LT_EducationBehaviour._gauntletMovie != null)
          LT_EducationBehaviour._gauntletLayer.ReleaseMovie((IGauntletMovie) LT_EducationBehaviour._gauntletMovie);
        topScreen.RemoveLayer((ScreenLayer) LT_EducationBehaviour._gauntletLayer);
      }
      LT_EducationBehaviour._gauntletLayer = (GauntletLayer) null;
      LT_EducationBehaviour._gauntletMovie = (GauntletMovie) null;
      LT_EducationBehaviour._popupVM = (EducationPopupVM) null;
    }

    private void OnLearningEnd()
    {
      if ((double) this._canRead > 100.0)
        this._canRead = 100f;
      SoundEvent.PlaySound2D("event:/ui/notification/peace");
      LT_EducationBehaviour.CreatePopupVMLayer("{=LT003}You can read!", "", "{=LT004}Let it be known throughout the land that you are literate!", "", !Hero.MainHero.IsFemale && !this._debug ? "lt_education_popup1" : "lt_education_popup2");
      GameMenu.SwitchToMenu("town");
    }

    private void AddGameMenus(CampaignGameStarter starter)
    {
      if (this._bookList == null)
        return;
      starter.AddGameMenuOption("town", "education_learn_read_menu_town", "{=LT006}Learn to read", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
        return (double) this._canRead < 100.0;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("education_learn_read_menu_town")), index: 9);
      starter.AddGameMenu("education_learn_read_menu_town", "{MENU_TEXT}", (OnInitDelegate) (args =>
      {
        if (Hero.MainHero.IsFemale || this._debug)
          args.MenuContext.SetBackgroundMeshName("book_menu_sprite5");
        else
          args.MenuContext.SetBackgroundMeshName("book_menu_sprite4");
        if (Hero.MainHero.GetAttributeValue(DefaultCharacterAttributes.Intelligence) < this._minINTToRead)
          MBTextManager.SetTextVariable("MENU_TEXT", "{=LT007}Nobody wants to teach you how to read. They think it's hopeless... (INT < " + this._minINTToRead.ToString() + " )", false);
        else
          MBTextManager.SetTextVariable("MENU_TEXT", "{=LT008}You found a scholar who agreed to teach you how to read for " + this._readPrice.ToString() + "{=LT009} {GOLD_ICON} per hour.", false);
      }), GameOverlays.MenuOverlayType.SettlementWithBoth);
      starter.AddGameMenuOption("education_learn_read_menu_town", "{=LT010}teach", "{=LT011}Let's do this!", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.WaitQuest;
        if (Hero.MainHero.GetAttributeValue(DefaultCharacterAttributes.Intelligence) < this._minINTToRead)
          return false;
        if (Hero.MainHero.Gold < this._readPrice)
        {
          args.IsEnabled = false;
          args.Tooltip = new TextObject("{=LT012}Not enough gold", (Dictionary<string, object>) null);
        }
        return true;
      }), (GameMenuOption.OnConsequenceDelegate) (args =>
      {
        this._lastHourOfLearning = -10;
        GameMenu.SwitchToMenu("education_learn_read_menu_town_progress");
      }), true);
      starter.AddGameMenuOption("education_learn_read_menu_town", "{=LT013}leave", "{LEAVE_MENU_TEXT}", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Leave;
        if (Hero.MainHero.GetAttributeValue(DefaultCharacterAttributes.Intelligence) < this._minINTToRead)
          MBTextManager.SetTextVariable("LEAVE_MENU_TEXT", "{=LT014}Leave", false);
        else
          MBTextManager.SetTextVariable("LEAVE_MENU_TEXT", "{=LT015}Maybe next time...", false);
        return true;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("town")), true);
      starter.AddWaitGameMenu("education_learn_read_menu_town_progress", "{=LT016}The scholar begins by introducing the concept of the alphabet and the sounds each letter makes. He moves on to basic words and teaches how to pronounce them, as well as how to read them in context. The scholar emphasizes the importance of pronunciation and enunciation to accurately read and understand texts.", (OnInitDelegate) (args =>
      {
        if (Hero.MainHero.IsFemale || this._debug)
          args.MenuContext.SetBackgroundMeshName("book_menu_sprite7");
        else
          args.MenuContext.SetBackgroundMeshName("book_menu_sprite6");
        args.MenuContext.GameMenu.SetTargetedWaitingTimeAndInitialProgress(100f / LTEducation.LearningToReadPerHourProgress(), this._canRead / 100f);
      }), (TaleWorlds.CampaignSystem.GameMenus.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Wait;
        return true;
      }), (TaleWorlds.CampaignSystem.GameMenus.OnConsequenceDelegate) (args => this.OnLearningEnd()), (OnTickDelegate) ((args, dt) =>
      {
        int currentHourInDay = (int) CampaignTime.Now.CurrentHourInDay;
        if (this._lastHourOfLearning != -10 && this._lastHourOfLearning != currentHourInDay)
        {
          Hero.MainHero.ChangeHeroGold(this._readPrice * -1);
          GameTexts.SetVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
          Logger.IMGrey(((object) new TextObject("Paid " + this._readPrice.ToString() + "{GOLD_ICON}", (Dictionary<string, object>) null)).ToString());
          this._canRead += LTEducation.LearningToReadPerHourProgress();
          if (this._debug)
            this._canRead += 10f;
        }
        this._lastHourOfLearning = currentHourInDay;
        args.MenuContext.GameMenu.SetProgressOfWaitingInMenu(this._canRead / 100f);
        if (Hero.MainHero.Gold >= this._readPrice)
          return;
        Logger.IMRed("Not enough gold to continue learning...");
        args.MenuContext.GameMenu.EndWait();
        GameMenu.ExitToLast();
      }), GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption);
      starter.AddGameMenuOption("education_learn_read_menu_town_progress", "{=LT018}leave", "{=LT019}Stop for now", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Leave;
        return true;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("town")));
      starter.AddGameMenuOption("village", "education_learn_read_menu_village", "{=LT020}Learn to read", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
        return (double) this._canRead < 100.0;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("education_learn_read_menu_village")), index: 4);
      starter.AddGameMenu("education_learn_read_menu_village", "{=LT021}Local villagers look confused. After a short talk between themselves, the brightest of them points towards the town...", (OnInitDelegate) (args => args.MenuContext.SetBackgroundMeshName("book_menu_sprite8")), GameOverlays.MenuOverlayType.SettlementWithBoth);
      starter.AddGameMenuOption("education_learn_read_menu_village", "{=LT022}leave", "{=LT023}Leave", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Leave;
        return true;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("village")), true);
      starter.AddGameMenuOption("castle", "education_learn_read_menu_castle", "{=LT024}Learn to read", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
        return (double) this._canRead < 100.0;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("education_learn_read_menu_castle")), index: 5);
      starter.AddGameMenu("education_learn_read_menu_castle", "{=LT025}Locals look puzzled. After scratching their heads they suggest you should go to the town...", (OnInitDelegate) (args => args.MenuContext.SetBackgroundMeshName("book_menu_sprite9")), GameOverlays.MenuOverlayType.SettlementWithBoth);
      starter.AddGameMenuOption("education_learn_read_menu_castle", "{=LT026}leave", "{=LT027}Leave", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Leave;
        return true;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("Castle")), true);
      starter.AddGameMenuOption("town", "education_menu", "{=LT029}Manage your education", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
        return (double) this._canRead >= 100.0 && LTEducation.GetPlayerBookAmount(this._bookList) > 0;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("education_menu")), index: 9);
      if (this._debug)
        starter.AddGameMenuOption("town", "test_popup", "{=LT030}Test popup", (GameMenuOption.OnConditionDelegate) (args =>
        {
          args.optionLeaveType = GameMenuOption.LeaveType.Default;
          return (double) this._canRead >= 100.0;
        }), (GameMenuOption.OnConsequenceDelegate) (args => LT_EducationBehaviour.CreatePopupVMLayer("{=LT031}Howdy!!!!", "", "{=LT032}You are awesome!", "{=LT033}You smart-ass skill increased by 10000!", "lt_education_book17")), index: 9);
      starter.AddGameMenuOption("village", "education_menu", "{=LT034}Manage your education", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
        return (double) this._canRead >= 100.0 && LTEducation.GetPlayerBookAmount(this._bookList) > 0;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("education_menu")), index: 4);
      starter.AddGameMenuOption("castle", "education_menu", "{=LT035}Manage your education", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
        return (double) this._canRead >= 100.0 && LTEducation.GetPlayerBookAmount(this._bookList) > 0;
      }), (GameMenuOption.OnConsequenceDelegate) (args => GameMenu.SwitchToMenu("education_menu")), index: 5);
      starter.AddGameMenu("education_menu", "{CURRENTLY_READING}", (OnInitDelegate) (args =>
      {
        if (PlayerEncounter.EncounterSettlement.IsCastle)
          args.MenuContext.SetBackgroundMeshName("book_menu_sprite3");
        else if (PlayerEncounter.EncounterSettlement.IsTown)
          args.MenuContext.SetBackgroundMeshName("book_menu_sprite1");
        else
          args.MenuContext.SetBackgroundMeshName("book_menu_sprite2");
        if (!LTEducation.PlayerHasBook(this._bookInProgress))
          this._bookInProgress = -1;
        if (this._bookInProgress != -1)
        {
          int num = (int) this._bookProgress[this._bookInProgress];
          MBTextManager.SetTextVariable("CURRENTLY_READING", "{=LT036}Currently reading: \n\n" + LTEducation.GetBookNameByIndex(this._bookInProgress, this._bookList) + " [" + num.ToString() + "%]", false);
        }
        else
          MBTextManager.SetTextVariable("CURRENTLY_READING", "{=LT037}You are not reading anything currently.", false);
      }), GameOverlays.MenuOverlayType.SettlementWithBoth);
      starter.AddGameMenuOption("education_menu", "select book", "{=LT038}Select what to read", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
        return true;
      }), (GameMenuOption.OnConsequenceDelegate) (args =>
      {
        List<InquiryElement> inquiryElementList = new List<InquiryElement>();
        foreach (ItemObject playerBook in LTEducation.GetPlayerBooks(this._bookList))
        {
          string str1 = ((object) new TextObject("{=LT039}This looks like a good book to read", (Dictionary<string, object>) null)).ToString();
          bool flag = true;
          string str2 = ((object) playerBook.Name).ToString();
          int num = (int) this._bookProgress[LTEducation.GetBookIndex(((MBObjectBase) playerBook).StringId)];
          if (num > 0)
          {
            str2 = str2 + " [" + num.ToString() + "%]";
            if (num == 100)
            {
              flag = false;
              str1 = "{=LT040}You have already read this book.";
            }
          }
          inquiryElementList.Add(new InquiryElement((object) playerBook, str2, new ImageIdentifier(playerBook), flag, str1));
        }
        MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("{=LT041}From now on, you will read:", "", inquiryElementList, true, 1, "{=LT042}Select", "{=LT043}Leave", (Action<List<InquiryElement>>) (list =>
        {
          foreach (InquiryElement inquiryElement in list)
          {
            if (inquiryElement != null && inquiryElement.Identifier != null && inquiryElement.Identifier is ItemObject identifier2)
            {
              Logger.IM("Will read " + ((object) identifier2.Name)?.ToString());
              this._bookInProgress = LTEducation.GetBookIndex(((MBObjectBase) identifier2).StringId);
              GameMenu.SwitchToMenu("education_menu");
            }
          }
        }), (Action<List<InquiryElement>>) (list => { }), ""), false, false);
      }));
      starter.AddGameMenuOption("education_menu", "stop_reading", "{=LT046}Decide not to read anything", (GameMenuOption.OnConditionDelegate) (args =>
      {
        if (this._bookInProgress < 0)
          return false;
        args.optionLeaveType = GameMenuOption.LeaveType.Surrender;
        return true;
      }), (GameMenuOption.OnConsequenceDelegate) (args =>
      {
        this._bookInProgress = -1;
        GameMenu.SwitchToMenu("education_menu");
      }));
      starter.AddGameMenuOption("education_menu", "{=LT047}leave", "{=LT048}Leave", (GameMenuOption.OnConditionDelegate) (args =>
      {
        args.optionLeaveType = GameMenuOption.LeaveType.Leave;
        return true;
      }), (GameMenuOption.OnConsequenceDelegate) (args =>
      {
        if (PlayerEncounter.EncounterSettlement.IsCastle)
          GameMenu.SwitchToMenu("castle");
        else if (PlayerEncounter.EncounterSettlement.IsTown)
          GameMenu.SwitchToMenu("town");
        else
          GameMenu.SwitchToMenu("village");
      }), true);
    }

    private void AddDialogs(CampaignGameStarter starter)
    {
      if (this._bookList == null)
        return;
      starter.AddPlayerLine("tavernkeeper_book", "tavernkeeper_talk", "tavernkeeper_book_seller_location", "{TAVERN_KEEPER_GREETING}", new ConversationSentence.OnConditionDelegate(this.TavernKeeperOnCondition), (ConversationSentence.OnConsequenceDelegate) (() => GiveGoldAction.ApplyBetweenCharacters((Hero) null, Hero.MainHero, -5)), clickableConditionDelegate: ((ConversationSentence.OnClickableConditionDelegate) ((out TextObject explanation) =>
      {
        if (Hero.MainHero.Gold < 5)
        {
          explanation = new TextObject("{=LT052}Not enough gold...", (Dictionary<string, object>) null);
          return false;
        }
        explanation = new TextObject("{=LT053}5 {GOLD_ICON}", (Dictionary<string, object>) null);
        return true;
      })));
      starter.AddDialogLine("tavernkeeper_book_a", "tavernkeeper_book_seller_location", "tavernkeeper_books_thanks", "{=LT054}Yeah, saw {VENDOR.FIRSTNAME} recently. Look around the town.", (ConversationSentence.OnConditionDelegate) (() => this.IsBookVendorInTown()), (ConversationSentence.OnConsequenceDelegate) null);
      starter.AddDialogLine("tavernkeeper_book_a", "tavernkeeper_book_seller_location", "tavernkeeper_books_thanks", "{=LT055}I heard you can find what you are looking for in {SETTLEMENT}.", (ConversationSentence.OnConditionDelegate) (() => this.IsBookVendorNearby()), (ConversationSentence.OnConsequenceDelegate) null);
      starter.AddDialogLine("tavernkeeper_book_b", "tavernkeeper_book_seller_location", "tavernkeeper_books_thanks", "{=LT056}No, haven't heard lately.", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) null);
      starter.AddPlayerLine("tavernkeeper_book", "tavernkeeper_books_thanks", "tavernkeeper_pretalk", "{=LT057}Thanks!", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) null);
      starter.AddDialogLine("tavernkeeper_book", "tavernkeeper_pretalk", "tavernkeeper_talk", "{=LT058}Anything else?", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) null);
      starter.AddDialogLine("bookvendor_talk", "start", "bookvendor", "{=!}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.BookVendorStartTalkOnCondition), (ConversationSentence.OnConsequenceDelegate) null, 110);
      starter.AddPlayerLine("bookvendor_talk", "bookvendor", "bookvendor_yes", "{=LT059}I would like to see what you have", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) (() =>
      {
        Hero heroObject = CharacterObject.OneToOneConversationCharacter.HeroObject;
        ItemRoster leftRoster = new ItemRoster();
        foreach (ItemObject vendorBook in LTEducation.GetVendorBooks(heroObject))
          leftRoster.Add(new ItemRosterElement(vendorBook, 1, (ItemModifier) null));
        Hero.MainHero.SpecialItems.RemoveAll((Predicate<ItemObject>) (x => ((MBObjectBase) x).StringId.Contains("education_book")));
        foreach (ItemObject playerBook in LTEducation.GetPlayerBooks(this._bookList))
          Hero.MainHero.SpecialItems.Add(playerBook);
        Town town = Settlement.CurrentSettlement.Town;
        InventoryManager.OpenScreenAsTrade(leftRoster, (SettlementComponent) town, doneLogicExtrasDelegate: new InventoryManager.DoneLogicExtrasDelegate(this.OnInventoryScreenDone));
      }), 110, (ConversationSentence.OnClickableConditionDelegate) ((out TextObject explanation) =>
      {
        if (Hero.MainHero.Gold < 100)
        {
          explanation = new TextObject("{=LT060}Not enough gold...", (Dictionary<string, object>) null);
          return false;
        }
        explanation = TextObject.Empty;
        return true;
      }));
      starter.AddPlayerLine("bookvendor_talk", "bookvendor", "bookvendor_bye", "{=LT061}I don't need anything else for now. Bye.", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) null, 110);
      starter.AddDialogLine("bookvendor_talk", "bookvendor_yes", "bookvendor_yes_resp", "{=!}{VOICED_LINE}", (ConversationSentence.OnConditionDelegate) (() =>
      {
        int vendorId = LTEducation.GetVendorID(CharacterObject.OneToOneConversationCharacter);
        string str = "{=LT062}Pleasure doing business with you. [ib:hip][rb:positive][rf:happy]";
        if (vendorId == 2)
          str = "{=LT063}Found everything you need? [ib:hip][rb:positive][rf:happy]";
        if (vendorId == 3)
          str = "{=LT064}Satisfied? [ib:hip][rb:negative][if:angry]";
        MBTextManager.SetTextVariable("VOICED_LINE", "{=lteducation_vendor_here_you_go}" + str, false);
        return true;
      }), (ConversationSentence.OnConsequenceDelegate) null, 110);
      starter.AddPlayerLine("bookvendor_talk", "bookvendor_yes_resp", "bookvendor_help", "{=LT065}Thank you!", (ConversationSentence.OnConditionDelegate) null, (ConversationSentence.OnConsequenceDelegate) null, 110);
      starter.AddDialogLine("bookvendor_talk", "bookvendor_help", "bookvendor", "{=!}{VOICED_LINE}", (ConversationSentence.OnConditionDelegate) (() =>
      {
        int vendorId = LTEducation.GetVendorID(CharacterObject.OneToOneConversationCharacter);
        string str = "{=LT066}Can I help you with anything else? [ib:confident][if:convo_calm_friendly]";
        if (vendorId == 2)
          str = "{=LT067}Anything else? [ib:confident][if:convo_calm_friendly]";
        if (vendorId == 3)
          str = "{=LT068}What else? [ib:hip][if:angry]";
        MBTextManager.SetTextVariable("VOICED_LINE", "{=lteducation_vendor_anything_else}" + str, false);
        return true;
      }), (ConversationSentence.OnConsequenceDelegate) null, 110);
      starter.AddDialogLine("bookvendor_talk", "bookvendor_bye", "{=LT069}end", "{=!}{VOICED_LINE}", (ConversationSentence.OnConditionDelegate) (() =>
      {
        LTEducation.FormatBookVendorByeRandomText(Hero.MainHero.IsFemale, LTEducation.GetVendorID(CharacterObject.OneToOneConversationCharacter));
        return true;
      }), (ConversationSentence.OnConsequenceDelegate) null, 110);
    }

    private void OnInventoryScreenDone()
    {
      if (this._bookList == null)
        return;
      Hero heroObject = CharacterObject.OneToOneConversationCharacter.HeroObject;
      List<ItemObject> list1 = ((IEnumerable<ItemObject>) LTEducation.GetPlayerBooks(this._bookList)).ToList<ItemObject>();
      List<ItemObject> list2 = ((IEnumerable<ItemObject>) Hero.MainHero.SpecialItems).ToList<ItemObject>();
      foreach (ItemObject itemObject in ((IEnumerable<ItemObject>) list1).ToList<ItemObject>())
      {
        if (list2.Contains(itemObject))
        {
          list2.Remove(itemObject);
          list1.Remove(itemObject);
        }
      }
      foreach (ItemObject itemObject in list1)
        Logger.IM("Bought: " + ((object) itemObject.Name)?.ToString());
      foreach (ItemObject itemObject in list2)
        Logger.IM("Sold: " + ((object) itemObject.Name)?.ToString());
      foreach (ItemObject itemObject in list1)
        heroObject.SpecialItems.Remove(itemObject);
      foreach (ItemObject itemObject in list2)
        heroObject.SpecialItems.Add(itemObject);
    }

    private bool TavernKeeperOnCondition()
    {
      MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">", false);
      Random random = new Random();
      string[] strArray = new string[11]
      {
        "{=LT070}Good sir, I was hoping to purchase a book, and I was wondering if you have seen any book vendors in the area. Have any passed through your establishment recently?",
        "{=LT071}Pray, kind sir, have you had any news of any book vendors traveling in the vicinity? I am in search of some new reading material.",
        "{=LT072}Excuse me, good sir. I am a bibliophile and I am on the lookout for a book vendor. Have any such merchants come through your establishment recently?",
        "{=LT073}My good man, I am in need of a book and was hoping to find a vendor nearby. Might you have seen any such tradespeople in recent days?",
        "{=LT074}Hail, sir. I am a lover of books and am seeking to purchase one. Would you happen to know of any book vendors that have passed through here lately?",
        "{=LT075}Greetings, sir. I am in quest of a book and I was wondering if you have come across any book vendors recently. I would be much obliged if you could assist me in my search.",
        "{=LT076}Might you be so kind as to inform me, good sir, if any book vendors have been seen in these parts recently? I am eager to add to my collection.",
        "{=LT077}Good day, sir. I am in need of a book, and I was hoping to find a vendor nearby. Have you heard of any such merchants traveling in the area?",
        "{=LT078}I hope this finds you well, sir. I am a passionate reader and I am searching for a book vendor. Have any such individuals come to your establishment in recent times?",
        "{=LT079}Excuse me, sir. I am a scholar and I am in need of some new reading material. Have you seen any book vendors around these parts lately?",
        "{=LT080}Good sir, I was hoping to expand my library, and I was wondering if you have seen any book vendors in the area recently. Any information you could provide would be greatly appreciated."
      };
      MBTextManager.SetTextVariable("TAVERN_KEEPER_GREETING", strArray[random.Next(strArray.Length)], false);
      return true;
    }

    private bool BookVendorStartTalkOnCondition()
    {
      if (!((BasicCharacterObject) CharacterObject.OneToOneConversationCharacter).Name.Contains("Book Vendor"))
        return false;
      LTEducation.FormatBookVendorWelcomeRandomText(Hero.MainHero.IsFemale, LTEducation.GetVendorID(CharacterObject.OneToOneConversationCharacter));
      return true;
    }

    private bool IsBookVendorInTown()
    {
      if (this._vendorList == null || this._vendorList.Count<Hero>() == 0)
        return false;
      foreach (Hero vendor in this._vendorList)
      {
        if (vendor.CurrentSettlement == Settlement.CurrentSettlement)
        {
          StringHelpers.SetCharacterProperties("VENDOR", vendor.CharacterObject);
          return true;
        }
      }
      return false;
    }

    private bool IsBookVendorNearby()
    {
      if (this._vendorList == null || this._vendorList.Count<Hero>() == 0)
        return false;
      List<Settlement> townsFromSettlement = LHelpers.GetClosestTownsFromSettlement(Settlement.CurrentSettlement, 10);
      foreach (Hero vendor in this._vendorList)
      {
        foreach (Settlement settlement in townsFromSettlement)
        {
          if (vendor.CurrentSettlement == settlement)
          {
            MBTextManager.SetTextVariable("SETTLEMENT", settlement.EncyclopediaLinkWithName, false);
            return true;
          }
        }
      }
      return false;
    }

    private void ReadPlayerBook()
    {
      if ((double) this._canRead < 100.0)
        return;
      Hero mainHero = Hero.MainHero;
      MobileParty partyBelongedTo = mainHero.PartyBelongedTo;
      if (this._bookInProgress <= -1 || this._bookInProgress >= 37 || (double) this._bookProgress[this._bookInProgress] >= 100.0 || mainHero.IsPrisoner || !partyBelongedTo.ComputeIsWaiting() || partyBelongedTo.BesiegedSettlement != null || partyBelongedTo.AttachedTo != null || mainHero.CurrentSettlement == null && (partyBelongedTo.LastVisitedSettlement == null || !partyBelongedTo.LastVisitedSettlement.IsVillage || (double) Campaign.Current.Models.MapDistanceModel.GetDistance(MobileParty.MainParty, partyBelongedTo.LastVisitedSettlement) >= 1.0499999523162842))
        return;
      if (!LTEducation.PlayerHasBook(this._bookInProgress))
      {
        this._bookInProgress = -1;
        Logger.IMRed("We don't have a book we were reading before...");
      }
      else
      {
        float num1 = (float) Hero.MainHero.GetAttributeValue(DefaultCharacterAttributes.Intelligence);
        if ((double) num1 < 2.0)
          num1 = 2f;
        float num2 = 15f;
        float num3 = 3f;
        float num4 = (float) (4.1666665077209473 / ((double) num2 - ((double) num2 - (double) num3) / 8.0 * ((double) num1 - 2.0)));
        if (this._bookInProgress > 18 && this._bookInProgress < 37)
          num4 *= 3f;
        this._bookProgress[this._bookInProgress] += num4;
        if (this._debug)
          this._bookProgress[this._bookInProgress] += 10f;
        float num5 = this._bookProgress[this._bookInProgress];
        if ((double) num5 < 100.0)
        {
          if ((int) CampaignTime.Now.CurrentHourInDay % 6 != 0 || this._bookList == null)
            return;
          Logger.IM("Reading " + LTEducation.GetBookNameByIndex(this._bookInProgress, this._bookList) + " [" + num5.ToString("0") + "%]");
        }
        else
        {
          this._bookProgress[this._bookInProgress] = 100f;
          if (this._bookInProgress < 19)
            LTEducation.FinishBook(this._bookInProgress, this._bookList);
          else
            LTEducation.FinishScroll(this._bookInProgress, this._bookList);
          this._bookInProgress = -1;
        }
      }
    }

    private void CreateBookVendors()
    {
      CharacterObject template1 = MBObjectManager.Instance.GetObject<CharacterObject>("lt_education_book_vendor1");
      CharacterObject template2 = MBObjectManager.Instance.GetObject<CharacterObject>("lt_education_book_vendor2");
      CharacterObject template3 = MBObjectManager.Instance.GetObject<CharacterObject>("lt_education_book_vendor3");
      if (template1 == null || template2 == null || template3 == null)
      {
        Logger.IMRed("LT_Education: Can't create book vendors... Reinstall mod manually.");
      }
      else
      {
        if (Hero.FindAll((Func<Hero, bool>) (x => x.Name.Contains("Eadric the Book Vendor"))).Count<Hero>() < 1)
        {
          Hero specialHero = HeroCreator.CreateSpecialHero(template1, age: 45);
          TextObject firstName = new TextObject("Eadric", (Dictionary<string, object>) null);
          TextObject fullName = new TextObject(((object) firstName)?.ToString() + " the Book Vendor", (Dictionary<string, object>) null);
          specialHero.SetName(fullName, firstName);
        }
        if (Hero.FindAll((Func<Hero, bool>) (x => x.Name.Contains("Ingeborg the Book Vendor"))).Count<Hero>() < 1)
        {
          Hero specialHero = HeroCreator.CreateSpecialHero(template2, age: 25);
          TextObject firstName = new TextObject("Ingeborg", (Dictionary<string, object>) null);
          TextObject fullName = new TextObject(((object) firstName)?.ToString() + " the Book Vendor", (Dictionary<string, object>) null);
          specialHero.SetName(fullName, firstName);
        }
        if (Hero.FindAll((Func<Hero, bool>) (x => x.Name.Contains("Ahsan the Book Vendor"))).Count<Hero>() < 1)
        {
          Hero specialHero = HeroCreator.CreateSpecialHero(template3, age: 65);
          TextObject firstName = new TextObject("Ahsan", (Dictionary<string, object>) null);
          TextObject fullName = new TextObject(((object) firstName)?.ToString() + " the Book Vendor", (Dictionary<string, object>) null);
          specialHero.SetName(fullName, firstName);
        }
        this._vendorList = Hero.FindAll((Func<Hero, bool>) (x => x.Name.Contains("Book Vendor")));
        foreach (Hero vendor in this._vendorList)
        {
          if (vendor.IsNotSpawned)
          {
            Settlement randomTown = LHelpers.GetRandomTown();
            if (randomTown != null)
            {
              vendor.SetNewOccupation(Occupation.Special);
              vendor.ChangeHeroGold(10000);
              HeroHelper.SpawnHeroForTheFirstTime(vendor, randomTown);
            }
          }
        }
        LTEducation.GiveBooksToVendors(this._bookList, this._vendorList);
        if (!this._debug)
          return;
        Hero.MainHero.ChangeHeroGold(1000000);
      }
    }

    private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
    {
    }

    private void OnAfterSettlementEntered(
      MobileParty mobileParty,
      Settlement settlement,
      Hero hero)
    {
    }

    private void OnGameMenuOpened(MenuCallbackArgs obj)
    {
      if (Campaign.Current.GameMode != CampaignGameMode.Campaign || MobileParty.MainParty.CurrentSettlement == null || LocationComplex.Current == null || this._vendorList == null)
        return;
      Settlement currentSettlement = MobileParty.MainParty.CurrentSettlement;
      if (!currentSettlement.IsTown)
        return;
      int currentHourInDay = (int) CampaignTime.Now.CurrentHourInDay;
      string locationName = currentHourInDay <= 15 ? (currentHourInDay <= 7 ? "lordshall" : "center") : "tavern";
      Random random = new Random();
      string[] strArray = new string[2]
      {
        "npc_common",
        "sp_notable"
      };
      string spawnTag = strArray[random.Next(strArray.Length)];
      foreach (Hero vendor in this._vendorList)
      {
        if (vendor.CurrentSettlement == currentSettlement)
          LTEducation.MoveVendorToLocation(vendor, locationName, spawnTag);
      }
    }

    private void WeeklyTickEvent()
    {
      if (this._vendorList == null || this._bookList == null)
        return;
      LTEducation.RelocateBookVendors(this._vendorList);
      LTEducation.GiveBooksToVendors(this._bookList, this._vendorList);
    }

    private void DailyTickEvent()
    {
      if (!this._debug || this._vendorList == null)
        return;
      foreach (Hero vendor in this._vendorList)
        Logger.IMGreen(((object) vendor.FirstName).ToString() + " in " + ((object) vendor.CurrentSettlement).ToString());
    }

    private void HourlyTickEvent() => this.ReadPlayerBook();

    public override void SyncData(IDataStore dataStore)
    {
      dataStore.SyncData<int>("LTEducation_bookInProgress", ref this._bookInProgress);
      dataStore.SyncData<float[]>("LTEducation_bookProgress", ref this._bookProgress);
      dataStore.SyncData<float>("LTEducation_canRead", ref this._canRead);
    }
  }
}
