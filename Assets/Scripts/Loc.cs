using System;
using UnityEngine;

public enum Idioma { ES, EN }

/// <summary>
/// Sistema simple de idiomas. Uso en código: Loc.T("texto en español", "english text")
/// </summary>
public static class Loc
{
    private const string KEY = "idioma";
    public static event Action OnIdiomaCambiado;

    public static Idioma Actual { get; private set; } = (Idioma)PlayerPrefs.GetInt(KEY, 0);

    public static string T(string es, string en) => Actual == Idioma.EN ? en : es;

    public static void Set(Idioma idioma)
    {
        if (idioma == Actual) return;
        Actual = idioma;
        PlayerPrefs.SetInt(KEY, (int)idioma);
        PlayerPrefs.Save();
        OnIdiomaCambiado?.Invoke();
    }

    public static void Toggle() => Set(Actual == Idioma.ES ? Idioma.EN : Idioma.ES);
}