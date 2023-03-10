
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;


#nullable enable
namespace LT_Education
{
  internal class LTEducation
  {
    public static float LearningToReadPerHourProgress() => (float) (((double) Hero.MainHero.GetAttributeValue(DefaultCharacterAttributes.Intelligence) - 4.0) * 0.20000000298023224 + 1.0);

    public static int GetVendorID(CharacterObject co)
    {
      if (((BasicCharacterObject) co).Name.Contains("Ingeborg"))
        return 2;
      return ((BasicCharacterObject) co).Name.Contains("Ahsan") ? 3 : 1;
    }

    public static void FormatBookVendorWelcomeRandomText(bool isFemale, int vendorID)
    {
      Random random = new Random();
      string str = "NOT IMPLEMENTED";
      int num = random.Next(10) + 1;
      string[] strArray1 = new string[10]
      {
        "{=LT086}Good morrow, sir! It is my pleasure to welcome you to my humble stall.",
        "{=LT087}Greetings, my good man! Welcome to peruse the literary treasures I have to offer.",
        "{=LT088}Well met, my friend! I am delighted to welcome you to the finest collection of books in all the land.",
        "{=LT089}Hail to thee, noble sir! You are most welcome to explore the riches of my books.",
        "{=LT090}Welcome, good sir! It is an honor to have you peruse my selection of tomes.",
        "{=LT091}Greetings, kind sir! I bid you welcome to my humble stall, where knowledge and wonder await.",
        "{=LT092}Good day to you, my good fellow! Welcome to indulge in the knowledge and entertainment my books provide.",
        "{=LT093}Salutations, my esteemed patron! It is my pleasure to welcome you to my collection of fine literature.",
        "{=LT094}Well met, good sir! Welcome to explore the literary delights that await you.",
        "{=LT095}Greetings, my dear sir! I am honored to welcome you to my stall, where you'll find a wealth of knowledge and wisdom."
      };
      string[] strArray2 = new string[10]
      {
        "{=LT096}Good morrow, madam! It is my pleasure to welcome you to my humble stall.",
        "{=LT097}Greetings, fair lady! Welcome to peruse the literary treasures I have to offer.",
        "{=LT098}Well met, my lady! I am delighted to welcome you to the finest collection of books in all the land.",
        "{=LT099}Hail to thee, noble madam! You are most welcome to explore the riches of my books.",
        "{=LT100}Welcome, fair maiden! It is an honor to have you peruse my selection of tomes.",
        "{=LT101}Greetings, kind lady! I bid you welcome to my humble stall, where knowledge and wonder await.",
        "{=LT102}Good day to you, my dear lady! Welcome to indulge in the knowledge and entertainment my books provide.",
        "{=LT103}Salutations, my esteemed patroness! It is my pleasure to welcome you to my collection of fine literature.",
        "{=LT104}Well met, fair damsel! Welcome to explore the literary delights that await you.",
        "{=LT105}Greetings, my dear lady! I am honored to welcome you to my stall, where you'll find a wealth of knowledge and wisdom."
      };
      string[] strArray3 = new string[10]
      {
        "{=LT106}Welcome, good sir! May I interest you in a book on chivalry? I believe it would be a perfect match for a knight as handsome as yourself.",
        "{=LT107}Greetings, my lord! Your presence here has made my day brighter than the sun itself.",
        "{=LT108}My good sir, it seems as though the stars have aligned today, for I have the pleasure of meeting a man as gallant as yourself.",
        "{=LT109}Blessed be the day I set eyes upon you, my lord. How may I be of service to you today?",
        "{=LT110}Good day, kind sir! If only the pages of my books could be as charming as your smile.",
        "{=LT111}What a pleasure it is to see a man of such taste and refinement browsing my humble collection of literature.",
        "{=LT112}You must be a scholar, my lord, for only a man of great intellect could appreciate the beauty of my books as much as you do.",
        "{=LT113}It is said that knowledge is power, but I would argue that the real power lies in the company of a handsome gentleman such as yourself.",
        "{=LT114}I must confess, my lord, that I have been waiting for a man of your caliber to visit my humble bookshop for some time now.",
        "{=LT115}You must be a man of great discernment to have found your way to my little corner of the world. Might I suggest a volume on the art of courtly love to match your own romantic nature?"
      };
      string[] strArray4 = new string[10]
      {
        "{=LT116}Welcome, fair lady! You grace my humble bookshop with your beauty and charm.",
        "{=LT117}It is an honor to serve a woman of such refinement and grace. Might I recommend a volume on the art of courtly love to match your own romantic nature?",
        "{=LT118}My lady, I cannot help but be struck by your radiance. Please allow me to assist you in finding the perfect book to match your brilliance.",
        "{=LT119}It is rare to find a woman with such discerning taste as yourself. I am eager to show you the treasures within my collection.",
        "{=LT120}What a delight it is to see a woman of your stature in my bookshop! Allow me to introduce you to some of the finest literature in the land.",
        "{=LT121}My lady, your presence here is like a breath of fresh air. Might I interest you in a book on poetry to match your own lyrical beauty?",
        "{=LT122}I have never seen a woman with such elegance and grace as yourself. I would be honored to assist you in finding the perfect volume to match your sophistication.",
        "{=LT123}It is clear that you are a woman of great intellect and wisdom. Might I suggest a tome on philosophy or theology to match your own sharp mind?",
        "{=LT124}My lady, I cannot help but be captivated by your charm and charisma. Please allow me to be your guide in this literary journey.",
        "{=LT125}What a pleasure it is to see a woman of your beauty and intelligence in my humble bookshop. Might I suggest a volume on history to match your own knowledge and wisdom?"
      };
      string[] strArray5 = new string[10]
      {
        "{=LT126}What do you want, knave? Speak quickly, for I have better things to do than listen to your prattle.",
        "{=LT127}Who disturbs me from my rest? State your business or be gone, lest I set my dogs upon you.",
        "{=LT128}What brings you to my humble establishment, wretch? If it is coin you seek, know that I demand payment up front.",
        "{=LT129}Speak plainly, for I have no patience for those who waste my time with idle chatter. What do you desire?",
        "{=LT130}What is it you seek, traveler? Speak quickly, for my temper grows short with every passing moment.",
        "{=LT131}Greetings, stranger. What business have you with me? Make it brief, for I am not in the mood for company.",
        "{=LT132}Who dares disturb my peace? Speak your mind or leave me to my thoughts.",
        "{=LT133}What brings you to my doorstep, fool? State your business, or I shall have my guards remove you.",
        "{=LT134}Well met, traveler. State your purpose or be on your way. I have no patience for aimless wanderers.",
        "{=LT135}What do you want, peasant? I am not in the mood for pleasantries. Speak quickly and be on your way."
      };
      string[] strArray6 = new string[10]
      {
        "{=LT136}What do you want, wench? Speak quickly, for I have no time for idle chatter.",
        "{=LT137}Who are you to disturb me? State your business or face the consequences.",
        "{=LT138}What brings you to my doorstep, maiden? If it is coin you seek, be prepared to pay a fair price.",
        "{=LT139}Speak plainly, for I have no patience for those who prattle on. What is it you desire?",
        "{=LT140}What do you seek, woman? Speak quickly, for I have more important matters to attend to.",
        "{=LT141}Greetings, fair lady. What business have you with me? Make it brief, for my time is precious.",
        "{=LT142}Who dares to interrupt my solitude? Speak your mind or be on your way.",
        "{=LT143}What brings you here, girl? State your purpose or risk facing my wrath.",
        "{=LT144}Well met, traveler. What is it that you seek? Speak quickly and be on your way.",
        "{=LT145}What do you want, harlot? I have no time for your games. State your business and be gone."
      };
      if (vendorID == 1)
        str = isFemale ? strArray2[num - 1] : strArray1[num - 1];
      if (vendorID == 2)
        str = isFemale ? strArray4[num - 1] : strArray3[num - 1];
      if (vendorID == 3)
        str = isFemale ? strArray6[num - 1] : strArray5[num - 1];
      MBTextManager.SetTextVariable("VOICED_LINE", isFemale ? "{=lteducation_vendor_welcome_f" + num.ToString() + "}" + str : "{=lteducation_vendor_welcome_m" + num.ToString() + "}" + str, false);
    }

    public static void FormatBookVendorByeRandomText(bool isFemale, int vendorID)
    {
      Random random = new Random();
      string str = "NOT IMPLEMENTED";
      int num = random.Next(10) + 1;
      string[] strArray1 = new string[10]
      {
        "{=LT146}Farewell, good sir! I thank you for gracing my stall with your presence.",
        "{=LT147}Adieu, my friend! May the knowledge you've acquired from my books enrich your life.",
        "{=LT148}Farewell, my dear sir! It was a pleasure to assist you in your quest for knowledge.",
        "{=LT149}Godspeed, noble sir! I bid you farewell and hope to see you again soon.",
        "{=LT150}Farewell, my good man! I thank you for your patronage and bid you a safe journey.",
        "{=LT151}May the blessings of the Almighty be upon you, my friend! Farewell, and happy reading.",
        "{=LT152}Fare thee well, kind sir! I hope the books you've acquired bring you much joy and enlightenment.",
        "{=LT153}May your thirst for knowledge never be quenched, my dear sir! Farewell, and may you find what you seek.",
        "{=LT154}Farewell, my esteemed patron! It was an honor to serve you and share my love of books with you.",
        "{=LT155}Goodbye, my dear sir! I bid you farewell with the hope that my books will bring you many hours of joy and learning."
      };
      string[] strArray2 = new string[10]
      {
        "{=LT156}Farewell, fair lady! I thank you for gracing my stall with your presence.",
        "{=LT157}Adieu, my dear madam! May the knowledge you've acquired from my books enrich your life.",
        "{=LT158}Farewell, my esteemed patroness! It was a pleasure to assist you in your quest for knowledge.",
        "{=LT159}Godspeed, noble lady! I bid you farewell and hope to see you again soon.",
        "{=LT160}Farewell, my kind lady! I thank you for your patronage and bid you a safe journey.",
        "{=LT161}May the blessings of the Almighty be upon you, my dear lady! Farewell, and happy reading.",
        "{=LT162}Fare thee well, fair maiden! I hope the books you've acquired bring you much joy and enlightenment.",
        "{=LT163}May your thirst for knowledge never be quenched, my dear lady! Farewell, and may you find what you seek.",
        "{=LT164}Farewell, my dear lady! It was an honor to serve you and share my love of books with you.",
        "{=LT165}Goodbye, my esteemed patroness! I bid you farewell with the hope that my books will bring you many hours of joy and learning."
      };
      string[] strArray3 = new string[10]
      {
        "{=LT166}Farewell, good sir! I hope you will visit me again soon, so that we may continue our discussions of literature and romance.",
        "{=LT167}Until we meet again, my lord. May your heart be as full of joy as your mind is full of knowledge.",
        "{=LT168}It has been a pleasure serving you, my gallant knight. I shall look forward to the day when you return to my humble bookshop.",
        "{=LT169}I bid you adieu, my lord. May your journey be as bright and fulfilling as the pages of the books you so avidly peruse.",
        "{=LT170}Until next time, my good sir. May the knowledge you have gleaned from my books serve you well in all your endeavors.",
        "{=LT171}Farewell, my lord. Remember that my bookshop is always open to a man of your taste and refinement.",
        "{=LT172}It has been an honor serving a man of your stature and grace. May your travels be safe and your mind be ever expanded by the words within my books.",
        "{=LT173}Until we meet again, my lord. May the beauty and wisdom within my books be a constant source of inspiration for you.",
        "{=LT174}I shall miss your charming company, my dear sir. May the lessons you have learned within my books serve you well on your journey through life.",
        "{=LT175}Farewell, my lord. Know that the memory of your presence here shall linger within my heart like the fragrance of the roses in my garden."
      };
      string[] strArray4 = new string[10]
      {
        "{=LT176}Farewell, fair lady! May the pages of the books you have selected be as enlightening and beautiful as your own countenance.",
        "{=LT177}Until we meet again, my lady. May the knowledge you have gained within my books serve to further enhance your already sparkling intellect.",
        "{=LT178}It has been a pleasure serving a woman of such elegance and grace. May the journey of life bring you as much joy as your presence here has brought me.",
        "{=LT179}I bid you adieu, my lady. May the romance and passion within my books inspire you to ever greater heights of love and devotion.",
        "{=LT180}Until next time, my dear lady. May the words within my books continue to kindle the fires of your own creativity and inspiration.",
        "{=LT181}Farewell, fair maiden. Remember that my bookshop is always open to a woman of your beauty and discerning taste.",
        "{=LT182}It has been an honor serving you, my lady. May the lessons within my books guide you on your journey through life with ever greater wisdom and insight.",
        "{=LT183}Until we meet again, my lady. May the knowledge and beauty within my books serve as a constant source of inspiration for your own life and endeavors.",
        "{=LT184}I shall miss your charming company, my dear lady. May the memories of our conversations and the knowledge within my books be a constant comfort to you.",
        "{=LT185}Farewell, my lady. Know that the beauty and radiance of your presence here shall remain within my heart like the glow of the stars in the night sky..."
      };
      string[] strArray5 = new string[10]
      {
        "{=LT186}Depart swiftly from my sight, you pestilent rat!",
        "{=LT187}Away with you, you foul and contemptuous knave!",
        "{=LT188}May the devil take you quickly, and swiftly at that!",
        "{=LT189}I bid you farewell, but do not return to my establishment!",
        "{=LT190}Leave at once, and may you never darken my doorway again!",
        "{=LT191}Take your leave without delay, before I take my own.",
        "{=LT192}Your company has grown increasingly tiresome, away with you, and quickly!",
        "{=LT193}Be gone from my sight, before I summon the guards to remove you!",
        "{=LT194}May the road you travel be long and arduous for you, and may you not find rest!",
        "{=LT195}I care not where you go, so long as it is far away from my presence!"
      };
      if (vendorID == 1)
        str = (isFemale ? strArray2[num - 1] : strArray1[num - 1]) + "[ib:demure][if:convo_bemused]";
      if (vendorID == 2)
        str = (isFemale ? strArray4[num - 1] : strArray3[num - 1]) + "[ib:demure][if:convo_bemused]";
      if (vendorID == 3)
        str = strArray5[num - 1] + "[ib:hip][if:angry]";
      MBTextManager.SetTextVariable("VOICED_LINE", isFemale ? "{=lteducation_vendor_bye_f" + num.ToString() + "}" + str : "{=lteducation_vendor_bye_m" + num.ToString() + "}" + str, false);
    }

    public static void MoveVendorToLocation(Hero vendor, string locationName, string spawnTag = "sp_notable")
    {
      if (LocationComplex.Current == null || LocationComplex.Current.GetLocationWithId(locationName) == null)
        return;
      IFaction mapFaction1 = vendor.MapFaction;
      uint num1 = mapFaction1 != null ? mapFaction1.Color : 4291609515U;
      IFaction mapFaction2 = vendor.MapFaction;
      uint num2 = mapFaction2 != null ? mapFaction2.Color : 4291609515U;
      Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(vendor.CharacterObject.Race);
      LocationCharacter locationCharacter = new LocationCharacter(new AgentData((IAgentOriginBase) new PartyAgentOrigin(PartyBase.MainParty, vendor.CharacterObject, uniqueNo: new UniqueTroopDescriptor())).Monster(baseMonsterFromRace).NoHorses(true).ClothingColor1(num1).ClothingColor2(num2), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddCompanionBehaviors), spawnTag, false, LocationCharacter.CharacterRelations.Friendly, (string) null, true, isVisualTracked: true);
      LocationComplex.Current.GetLocationWithId(locationName)?.AddCharacter(locationCharacter);
    }

    public static void GiveBooksToVendors(
      IEnumerable<ItemObject>? bookList,
      IEnumerable<Hero> vendorList)
    {
      bool flag = false;
      int index1 = 12;
      int num1 = 8;
      int maxValue = bookList.Count<ItemObject>() - 1;
      Random random = new Random();
      foreach (Hero vendor in vendorList)
      {
        List<ItemObject> list = LTEducation.GetVendorBooks(vendor).ToList<ItemObject>();
        int count1 = list.Count;
        if (flag)
          Logger.IM(((object) vendor.FirstName).ToString() + " has books: " + count1.ToString());
        if (count1 > index1)
        {
          int count2 = count1 - index1;
          if (count2 > 0)
            list.RemoveRange(index1, count2);
          count1 = list.Count;
          if (flag)
            Logger.IM("Removed " + count2.ToString() + " from " + ((object) vendor.FirstName).ToString() + " books left: " + count1.ToString());
        }
        if (count1 > 3)
        {
          list.RemoveAt(random.Next(list.Count));
          list.RemoveAt(random.Next(list.Count));
          list.RemoveAt(random.Next(list.Count));
          count1 = list.Count;
          if (flag)
            Logger.IM("Removed 3 books from " + ((object) vendor.FirstName).ToString() + ", books left: " + count1.ToString());
        }
        if (count1 < num1)
        {
          int num2 = random.Next(num1 - 1) + (num1 - count1);
          string str = "";
          for (int index2 = 0; index2 < num2; ++index2)
          {
            int index3 = random.Next(maxValue) + 1;
            list.Add(bookList.ElementAt<ItemObject>(index3));
            str = str + " " + index3.ToString();
          }
          if (flag)
            Logger.IM("Added books to " + ((object) vendor.FirstName).ToString() + ": " + str);
        }
        vendor.SpecialItems.RemoveAll((Predicate<ItemObject>) (x => ((MBObjectBase) x).StringId.Contains("education_book")));
        foreach (ItemObject itemObject in list)
          vendor.SpecialItems.Add(itemObject);
        if (flag)
          Logger.IM(((object) vendor.FirstName).ToString() + " has " + LTEducation.GetVendorBooksCount(vendor).ToString() + " books");
      }
    }

    public static void FinishBook(int bookIndex, IEnumerable<ItemObject>? bookList)
    {
      if (bookIndex < 0 || bookIndex > 18 || bookList == null)
        return;
      if (bookIndex == 0)
        bookIndex = 1;
      string bookNameByIndex = LTEducation.GetBookNameByIndex(bookIndex, bookList);
      Logger.IMGreen("Finished reading: " + bookNameByIndex + "!");
      if (true)
        ;
      SkillObject skillObject;
      switch (bookIndex)
      {
        case 1:
          skillObject = DefaultSkills.OneHanded;
          break;
        case 2:
          skillObject = DefaultSkills.TwoHanded;
          break;
        case 3:
          skillObject = DefaultSkills.Polearm;
          break;
        case 4:
          skillObject = DefaultSkills.Bow;
          break;
        case 5:
          skillObject = DefaultSkills.Crossbow;
          break;
        case 6:
          skillObject = DefaultSkills.Throwing;
          break;
        case 7:
          skillObject = DefaultSkills.Riding;
          break;
        case 8:
          skillObject = DefaultSkills.Athletics;
          break;
        case 9:
          skillObject = DefaultSkills.Crafting;
          break;
        case 10:
          skillObject = DefaultSkills.Scouting;
          break;
        case 11:
          skillObject = DefaultSkills.Tactics;
          break;
        case 12:
          skillObject = DefaultSkills.Roguery;
          break;
        case 13:
          skillObject = DefaultSkills.Charm;
          break;
        case 14:
          skillObject = DefaultSkills.Leadership;
          break;
        case 15:
          skillObject = DefaultSkills.Trade;
          break;
        case 16:
          skillObject = DefaultSkills.Steward;
          break;
        case 17:
          skillObject = DefaultSkills.Medicine;
          break;
        case 18:
          skillObject = DefaultSkills.Engineering;
          break;
        default:
          skillObject = DefaultSkills.OneHanded;
          break;
      }
      if (true)
        ;
      SkillObject skill = skillObject;
      int skillValue = Hero.MainHero.GetSkillValue(skill);
      Random random = new Random();
      int changeAmount = 30 + random.Next(7) - 3;
      if (skillValue > 300)
        changeAmount = 0;
      else if (skillValue > 250)
        changeAmount = 5 + random.Next(3) - 1;
      else if (skillValue > 200)
        changeAmount = 10 + random.Next(3) - 1;
      else if (skillValue > 150)
        changeAmount = 15 + random.Next(3) - 1;
      else if (skillValue > 100)
        changeAmount = 20 + random.Next(5) - 2;
      Hero.MainHero.HeroDeveloper.ChangeSkillLevel(skill, changeAmount);
      PlayerEncounter.Current.IsPlayerWaiting = false;
      string textOverImage;
      if (changeAmount > 0)
        textOverImage = "{=LT206}That increased your " + ((object) skill).ToString() + "{=LT207} skill by " + changeAmount.ToString() + "!";
      else
        textOverImage = "{=LT208}You are too skilled in " + ((object) skill).ToString() + "{=LT209},\n so the book was a waste of time...";
      string spriteName = "lt_education_book" + bookIndex.ToString();
      SoundEvent.PlaySound2D("event:/ui/notification/peace");
      LT_EducationBehaviour.CreatePopupVMLayer("{=LT210}Finished reading", "", bookNameByIndex, textOverImage, spriteName);
    }

    public static void FinishScroll(int bookIndex, IEnumerable<ItemObject>? bookList)
    {
      if (bookIndex < 19 || bookIndex > 36 || bookList == null)
        return;
      string bookNameByIndex = LTEducation.GetBookNameByIndex(bookIndex, bookList);
      Logger.IMGreen("Finished reading: " + bookNameByIndex + "!");
      if (true)
        ;
      SkillObject skillObject;
      switch (bookIndex)
      {
        case 19:
          skillObject = DefaultSkills.OneHanded;
          break;
        case 20:
          skillObject = DefaultSkills.TwoHanded;
          break;
        case 21:
          skillObject = DefaultSkills.Polearm;
          break;
        case 22:
          skillObject = DefaultSkills.Bow;
          break;
        case 23:
          skillObject = DefaultSkills.Crossbow;
          break;
        case 24:
          skillObject = DefaultSkills.Throwing;
          break;
        case 25:
          skillObject = DefaultSkills.Riding;
          break;
        case 26:
          skillObject = DefaultSkills.Athletics;
          break;
        case 27:
          skillObject = DefaultSkills.Crafting;
          break;
        case 28:
          skillObject = DefaultSkills.Scouting;
          break;
        case 29:
          skillObject = DefaultSkills.Tactics;
          break;
        case 30:
          skillObject = DefaultSkills.Roguery;
          break;
        case 31:
          skillObject = DefaultSkills.Charm;
          break;
        case 32:
          skillObject = DefaultSkills.Leadership;
          break;
        case 33:
          skillObject = DefaultSkills.Trade;
          break;
        case 34:
          skillObject = DefaultSkills.Steward;
          break;
        case 35:
          skillObject = DefaultSkills.Medicine;
          break;
        case 36:
          skillObject = DefaultSkills.Engineering;
          break;
        default:
          skillObject = DefaultSkills.OneHanded;
          break;
      }
      if (true)
        ;
      SkillObject skill = skillObject;
      int skillValue = Hero.MainHero.GetSkillValue(skill);
      Random random = new Random();
      int changeAmount = 10 + random.Next(5) - 2;
      if (skillValue > 300)
        changeAmount = 0;
      else if (skillValue > 250)
        changeAmount = 1;
      else if (skillValue > 200)
        changeAmount = 3 + random.Next(3) - 1;
      else if (skillValue > 150)
        changeAmount = 5 + random.Next(3) - 1;
      else if (skillValue > 100)
        changeAmount = 8 + random.Next(3) - 1;
      Hero.MainHero.HeroDeveloper.ChangeSkillLevel(skill, changeAmount);
      PlayerEncounter.Current.IsPlayerWaiting = false;
      string textOverImage;
      if (changeAmount > 0)
        textOverImage = "{=LT212}That increased your " + ((object) skill).ToString() + "{=LT13} skill by " + changeAmount.ToString() + "!";
      else
        textOverImage = "{=LT14}You are too skilled in " + ((object) skill).ToString() + "{=LT15},\n so the scroll was a waste of time...";
      string spriteName = "lt_education_book" + bookIndex.ToString();
      SoundEvent.PlaySound2D("event:/ui/notification/peace");
      LT_EducationBehaviour.CreatePopupVMLayer("{=LT216}Finished reading", "", bookNameByIndex, textOverImage, spriteName);
    }

    public static bool PlayerHasBook(int Index) => Hero.MainHero.PartyBelongedTo.ItemRoster.FindIndex((Predicate<ItemObject>) (x => ((MBObjectBase) x).StringId.Contains(LTEducation.GetBookStringId(Index)))) >= 0;

    public static List<ItemObject> GetPlayerBooks(IEnumerable<ItemObject> allBookList)
    {
      List<ItemObject> playerBooks = new List<ItemObject>();
      if (allBookList == null)
        return playerBooks;
      foreach (ItemObject allBook in allBookList)
      {
        int itemNumber = MobileParty.MainParty.ItemRoster.GetItemNumber(allBook);
        if (itemNumber > 0)
        {
          for (int index = 0; index < itemNumber; ++index)
            playerBooks.Add(allBook);
        }
      }
      return playerBooks;
    }

    public static IEnumerable<ItemObject> GetVendorBooks(Hero vendor) => ((IEnumerable<ItemObject>) vendor.SpecialItems).Where<ItemObject>((Func<ItemObject, bool>) (x => ((MBObjectBase) x).StringId.Contains("education_book")));

    public static int GetVendorBooksCount(Hero vendor) => ((IEnumerable<ItemObject>) vendor.SpecialItems).Where<ItemObject>((Func<ItemObject, bool>) (x => ((MBObjectBase) x).StringId.Contains("education_book"))).Count<ItemObject>();

    public static void RelocateBookVendors(IEnumerable<Hero> vendorList)
    {
      if (vendorList == null || vendorList.Count<Hero>() == 0)
        return;
      Random random = new Random();
      foreach (Hero vendor in vendorList)
      {
        if (vendor.CurrentSettlement != null)
        {
          List<Settlement> townsFromSettlement = LHelpers.GetClosestTownsFromSettlement(vendor.CurrentSettlement, 4);
          if (townsFromSettlement.Count > 0)
          {
            int index = random.Next(townsFromSettlement.Count);
            TeleportHeroAction.ApplyImmediateTeleportToSettlement(vendor, townsFromSettlement[index]);
          }
        }
        else
        {
          Settlement randomTown = LHelpers.GetRandomTown();
          if (randomTown != null)
            TeleportHeroAction.ApplyImmediateTeleportToSettlement(vendor, randomTown);
        }
      }
    }

    public static int GetPlayerBookAmount(IEnumerable<ItemObject> bookList)
    {
      int playerBookAmount = 0;
      foreach (ItemObject book in bookList)
        playerBookAmount += MobileParty.MainParty.ItemRoster.GetItemNumber(book);
      return playerBookAmount;
    }

    public static string GetBookName(string StringId, IEnumerable<ItemObject> bookList)
    {
      if (bookList == null)
        return "No book found";
      foreach (ItemObject book in bookList)
      {
        if (book != null && ((MBObjectBase) book).StringId == StringId)
          return ((object) book.Name).ToString();
      }
      return "No book found";
    }

    public static string GetBookNameByIndex(int index, IEnumerable<ItemObject> bookList) => LTEducation.GetBookName(LTEducation.GetBookStringId(index), bookList);

    public static string GetBookStringId(int index)
    {
      if (true)
        ;
      string bookStringId;
      switch (index)
      {
        case 1:
          bookStringId = "education_book_onehanded1";
          break;
        case 2:
          bookStringId = "education_book_twohanded1";
          break;
        case 3:
          bookStringId = "education_book_polearm1";
          break;
        case 4:
          bookStringId = "education_book_bow1";
          break;
        case 5:
          bookStringId = "education_book_crossbow1";
          break;
        case 6:
          bookStringId = "education_book_throwing1";
          break;
        case 7:
          bookStringId = "education_book_riding1";
          break;
        case 8:
          bookStringId = "education_book_ahthletics1";
          break;
        case 9:
          bookStringId = "education_book_smithing1";
          break;
        case 10:
          bookStringId = "education_book_scouting1";
          break;
        case 11:
          bookStringId = "education_book_tactics1";
          break;
        case 12:
          bookStringId = "education_book_roguery1";
          break;
        case 13:
          bookStringId = "education_book_charm1";
          break;
        case 14:
          bookStringId = "education_book_leadership1";
          break;
        case 15:
          bookStringId = "education_book_trade1";
          break;
        case 16:
          bookStringId = "education_book_steward1";
          break;
        case 17:
          bookStringId = "education_book_medicine1";
          break;
        case 18:
          bookStringId = "education_book_engineering1";
          break;
        case 19:
          bookStringId = "education_book_onehanded2";
          break;
        case 20:
          bookStringId = "education_book_twohanded2";
          break;
        case 21:
          bookStringId = "education_book_polearm2";
          break;
        case 22:
          bookStringId = "education_book_bow2";
          break;
        case 23:
          bookStringId = "education_book_crossbow2";
          break;
        case 24:
          bookStringId = "education_book_throwing2";
          break;
        case 25:
          bookStringId = "education_book_riding2";
          break;
        case 26:
          bookStringId = "education_book_ahthletics2";
          break;
        case 27:
          bookStringId = "education_book_smithing2";
          break;
        case 28:
          bookStringId = "education_book_scouting2";
          break;
        case 29:
          bookStringId = "education_book_tactics2";
          break;
        case 30:
          bookStringId = "education_book_roguery2";
          break;
        case 31:
          bookStringId = "education_book_charm2";
          break;
        case 32:
          bookStringId = "education_book_leadership2";
          break;
        case 33:
          bookStringId = "education_book_trade2";
          break;
        case 34:
          bookStringId = "education_book_steward2";
          break;
        case 35:
          bookStringId = "education_book_medicine2";
          break;
        case 36:
          bookStringId = "education_book_engineering2";
          break;
        default:
          bookStringId = "education_book_onehanded1";
          break;
      }
      if (true)
        ;
      return bookStringId;
    }

    public static int GetBookIndex(string stringId)
    {
      if (true)
        ;
      int bookIndex;
      switch (stringId)
      {
        case "education_book_ahthletics1":
          bookIndex = 8;
          break;
        case "education_book_ahthletics2":
          bookIndex = 26;
          break;
        case "education_book_bow1":
          bookIndex = 4;
          break;
        case "education_book_bow2":
          bookIndex = 22;
          break;
        case "education_book_charm1":
          bookIndex = 13;
          break;
        case "education_book_charm2":
          bookIndex = 31;
          break;
        case "education_book_crossbow1":
          bookIndex = 5;
          break;
        case "education_book_crossbow2":
          bookIndex = 23;
          break;
        case "education_book_engineering1":
          bookIndex = 18;
          break;
        case "education_book_engineering2":
          bookIndex = 36;
          break;
        case "education_book_leadership1":
          bookIndex = 14;
          break;
        case "education_book_leadership2":
          bookIndex = 32;
          break;
        case "education_book_medicine1":
          bookIndex = 17;
          break;
        case "education_book_medicine2":
          bookIndex = 35;
          break;
        case "education_book_onehanded1":
          bookIndex = 1;
          break;
        case "education_book_onehanded2":
          bookIndex = 19;
          break;
        case "education_book_polearm1":
          bookIndex = 3;
          break;
        case "education_book_polearm2":
          bookIndex = 21;
          break;
        case "education_book_riding1":
          bookIndex = 7;
          break;
        case "education_book_riding2":
          bookIndex = 25;
          break;
        case "education_book_roguery1":
          bookIndex = 12;
          break;
        case "education_book_roguery2":
          bookIndex = 30;
          break;
        case "education_book_scouting1":
          bookIndex = 10;
          break;
        case "education_book_scouting2":
          bookIndex = 28;
          break;
        case "education_book_smithing1":
          bookIndex = 9;
          break;
        case "education_book_smithing2":
          bookIndex = 27;
          break;
        case "education_book_steward1":
          bookIndex = 16;
          break;
        case "education_book_steward2":
          bookIndex = 34;
          break;
        case "education_book_tactics1":
          bookIndex = 11;
          break;
        case "education_book_tactics2":
          bookIndex = 29;
          break;
        case "education_book_throwing1":
          bookIndex = 6;
          break;
        case "education_book_throwing2":
          bookIndex = 24;
          break;
        case "education_book_trade1":
          bookIndex = 15;
          break;
        case "education_book_trade2":
          bookIndex = 33;
          break;
        case "education_book_twohanded1":
          bookIndex = 2;
          break;
        case "education_book_twohanded2":
          bookIndex = 20;
          break;
        default:
          bookIndex = 1;
          break;
      }
      if (true)
        ;
      return bookIndex;
    }
  }
}
