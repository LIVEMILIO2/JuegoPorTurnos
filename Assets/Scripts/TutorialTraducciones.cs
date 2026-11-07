using System.Collections.Generic;

public static class TutorialTraducciones
{
    private static readonly Dictionary<string, string> en = new Dictionary<string, string>
    {
        { "¡Bienvenido! Este es un juego de estrategia por turnos.\nCada personaje actúa según su iniciativa.",
          "Welcome! This is a turn-based strategy game.\nEach character acts according to its initiative." },
        { "El orden de turnos depende de la iniciativa de cada personaje.\nQuien tenga más iniciativa actúa primero.",
          "Turn order depends on each character's initiative.\nWhoever has the most initiative acts first." },
        { "Es el turno del Warrior.\nPresiona MOVER y muévete cerca del enemigo.",
          "It's the Warrior's turn.\nPress MOVE and get close to the enemy." },
        { "¡Bien! Ahora ataca al enemigo.\nPresiona ATACAR y haz clic en el enemigo.",
          "Nice! Now attack the enemy.\nPress ATTACK and click on the enemy." },
        { "El Warrior puede usar ACELERAR para subir su iniciativa y actuar antes.",
          "The Warrior can use ACCELERATE to raise its initiative and act sooner." },
        { "También puede usar DEFENDER para reducir a la mitad el daño que recibe.",
          "It can also use DEFEND to halve the damage it receives." },
        { "Es el turno del Mage.\nPresiona MOVER y muévete cerca del enemigo.",
          "It's the Mage's turn.\nPress MOVE and get close to the enemy." },
        { "Es el turno del Mage.\nEl Mage puede usar STUN para bajar la iniciativa de un enemigo.",
          "It's the Mage's turn.\nThe Mage can use STUN to lower an enemy's initiative." },
        { "El Mage puede usar STUN para bajar la iniciativa de un enemigo.",
          "The Mage can use STUN to lower an enemy's initiative." },
        { "Es el turno del Support.\nEl Support puede CURAR a un aliado cercano.",
          "It's the Support's turn.\nThe Support can HEAL a nearby ally." },
        { "El Support también puede usar BUFF para subir la iniciativa de un aliado.",
          "The Support can also use BUFF to raise an ally's initiative." },
        { "¡Perfecto! Ya conoces todas las mecánicas del juego.\n¡Buena suerte en la batalla!",
          "Perfect! You now know all the game mechanics.\nGood luck in battle!" },
        { "¡Demasiado lejos! Muévete más cerca del objetivo e intenta de nuevo.",
          "Too far! Move closer to the target and try again." },
    };

    /// <summary>Devuelve el texto en el idioma actual. Si no hay traducción, devuelve el original.</summary>
    public static string Obtener(string es)
    {
        if (Loc.Actual == Idioma.ES || string.IsNullOrEmpty(es)) return es;
        string clave = es.Replace("\r\n", "\n").Trim();
        foreach (var par in en)
            if (par.Key.Trim() == clave) return par.Value;
        return es;
    }
}
