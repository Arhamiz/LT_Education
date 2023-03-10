
using TaleWorlds.Library;


#nullable enable
namespace LT_Education
{
  public class EducationPopupVM : ViewModel
  {
    private string _title;
    private string _smallText;
    private string _bigText;
    private string _textOverImage;
    private string _spriteName;

    public EducationPopupVM(
      string title,
      string smallText,
      string bigText,
      string textOverImage,
      string spriteName)
    {
      this._title = title;
      this._smallText = smallText;
      this._bigText = bigText;
      this._textOverImage = textOverImage;
      this._spriteName = spriteName;
    }

    public void Close() => LT_EducationBehaviour.DeletePopupVMLayer();

    public void Refresh()
    {
      this.Title = this._title;
      this.SmallText = this._smallText;
      this.BigText = this._bigText;
      this.TextOverImage = this._textOverImage;
      this.SpriteName = this._spriteName;
    }

    public string Title
    {
      get => this._title;
      set
      {
        this._title = value;
        this.OnPropertyChangedWithValue<string>(value, "PopupTitle");
      }
    }

    public string SmallText
    {
      get => this._smallText;
      set
      {
        this._smallText = value;
        this.OnPropertyChangedWithValue<string>(value, "PopupSmallText");
      }
    }

    public string BigText
    {
      get => this._bigText;
      set
      {
        this._bigText = value;
        this.OnPropertyChangedWithValue<string>(value, "PopupBigText");
      }
    }

    public string TextOverImage
    {
      get => this._textOverImage;
      set
      {
        this._textOverImage = value;
        this.OnPropertyChangedWithValue<string>(value, "PopupTextOverImage");
      }
    }

    public string SpriteName
    {
      get => this._spriteName;
      set
      {
        this._spriteName = value;
        this.OnPropertyChangedWithValue<string>(value, nameof (SpriteName));
      }
    }
  }
}
