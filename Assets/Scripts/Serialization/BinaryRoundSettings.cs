using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinarySettingsStorage : ISettings, ISettingsStorage
{
    private const string _fileName = "settings.bin";
    private RoundSettingsSerialized _settings;
    public int CardsAmount
    {
        get => _settings.CardsAmount;
        set
        {
            _settings.CardsAmount = value;
            Save();
        }
    }

    public bool ShowFrontOnStartOfRound
    {
        get => _settings.ShowFrontOnStartOfRound;
        set
        {
            _settings.ShowFrontOnStartOfRound = value;
            Save();
        }
    }

    public bool HideCardsOnMatch
    {
        get => _settings.HideCardsOnMatch;
        set
        {
            _settings.HideCardsOnMatch = value;
            Save();
        }
    }

    public float RoundStartRevealDuration
    {
        get => _settings.RoundStartRevealDuration;
        set
        {
            _settings.RoundStartRevealDuration = value;
            Save();
        }
    }

    public float HintRevealDuration
    {
        get => _settings.HintRevealDuration;
        set
        {
            _settings.HintRevealDuration = value;
            Save();
        }
    }

    public float CardRevealOnPairFailDuration
    {
        get => _settings.CardRevealOnPairFailDuration;
        set
        {
            _settings.CardRevealOnPairFailDuration = value;
            Save();
        }
    }

    public float BaseRoundDuration
    {
        get => _settings.BaseRoundDuration;
        set
        {
            _settings.BaseRoundDuration = value;
            Save();
        }
    }

    public float RoundDurationIncrementPerCard
    {
        get => _settings.RoundDurationIncrementPerCard;
        set
        {
            _settings.RoundDurationIncrementPerCard = value;
            Save();
        }
    }

    public float GetRoundDuration(int cardPairs)
    {
        return _settings.GetRoundDuration(cardPairs);
    }

    public BinarySettingsStorage()
    {
        _settings = GetSettingsFromPersistentData();
    }

    private RoundSettingsSerialized GetSettingsFromPersistentData()
    {
        var path = Path.Combine(Application.persistentDataPath, _fileName);
        if (!Directory.Exists(Application.persistentDataPath) || !File.Exists(path))
        {
            return new RoundSettingsSerialized();
        }

        var fileInfo = new FileInfo(path);
        var settings = GetSettingsFromFileInfo(fileInfo);

        if (settings != null)
        {
            return settings;
        }

        return new RoundSettingsSerialized();
    }


    private RoundSettingsSerialized GetSettingsFromFileInfo(FileInfo fileInfo)
    {
        if (fileInfo.Extension != ".bin")
        {
            return null;
        }

        var json = File.ReadAllText(fileInfo.FullName);
        RoundSettingsSerialized settings;

        try
        {
            using (var stream = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                var obj = formatter.Deserialize(stream);
                settings = obj as RoundSettingsSerialized;
            }
        }
        catch
        {
            return null;
        }

        return settings;
    }

    private void Save()
    {
        var path = Path.Combine(Application.persistentDataPath, _fileName);

        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        using (var stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, _settings);
        }
    }
}