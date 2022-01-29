using System.Collections.Generic;
using UnityEngine;

public class Strings : MonoBehaviour
{
    private GeneralSettings _generalSettings;

    private void Start()
    {
        _generalSettings = FindObjectOfType<GeneralSettings>();
    }

    public string GetString(string stringId)
    {
        var currentLanguageStrings = StringsByLanguage[_generalSettings.Language];
        return currentLanguageStrings.TryGetValue(stringId, out var localisedString) ? localisedString : stringId;
    }

    public static readonly Dictionary<string, string> SpanishStrings = new Dictionary<string, string>
    {
        [StringIds.CardTitle_FreeHeating] = "Calefacción gratis",
        [StringIds.CardTitle_PublicHospital] = "Hospital público",
        [StringIds.CardTitle_RenewableEnergy] = "Energías renovables",
        [StringIds.CardTitle_Scolarships] = "Becas",
    };

    public static readonly Dictionary<Language, Dictionary<string, string>> StringsByLanguage = new Dictionary<Language, Dictionary<string, string>>
    {
        [Language.Spanish] = SpanishStrings
    };

    private class StringIds
    {
        public const string CardTitle_FreeHeating = nameof(CardTitle_FreeHeating);
        public const string CardTitle_PublicHospital = nameof(CardTitle_PublicHospital);
        public const string CardTitle_RenewableEnergy = nameof(CardTitle_RenewableEnergy);
        public const string CardTitle_Scolarships = nameof(CardTitle_Scolarships);
    }
}
