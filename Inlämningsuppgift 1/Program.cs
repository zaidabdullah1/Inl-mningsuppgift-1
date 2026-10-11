using System;
using System.Text; // Min favorit, gör det enklare att använda text funktioner

Console.OutputEncoding = Encoding.UTF8; // Så svenska bokstäver visas rätt haha

// Claes det går utmärkt att använda f5 därmed har jag ingen readme i denna uppgift


// 100 parkeringsplatser
string[] parkeringsplatser = new string[100];
Array.Fill(parkeringsplatser, ""); // Fyller alla 100 platser med tom text







// Huvudmeny
bool kör = true;
while (kör)
{
    Console.WriteLine("\n===== PRAGUE PARKING V1 =====");
    Console.WriteLine("1. Parkera fordon");
    Console.WriteLine("2. Flytta fordon");
    Console.WriteLine("3. Hämta ut fordon");
    Console.WriteLine("4. Sök fordon");
    Console.WriteLine("5. Visa parkeringsplatser");
    Console.WriteLine("0. Avsluta");
    Console.Write("Välj ett alternativ: ");

    string? val = Console.ReadLine(); // Switch case ungefär kopierat från lektionen
    switch (val)
    {
        case "1": ParkeraFordon(); break;
        case "2": FlyttaFordon(); break;
        case "3": HämtaUtFordon(); break;
        case "4": SökFordon(); break;
        case "5": VisaPlatser(); break;
        case "0": kör = false; Console.WriteLine("Programmet avslutas."); break;
        default: Console.WriteLine("Ogiltigt svar, Försök igen."); break;
    }
}









// Parkera nytt fordon
void ParkeraFordon() // Void för att metoden inte ska skicka tillbaka något värde
{
    string? reg = LäsRegistreringsnummer(); // ? betyder att den får vara null
    if (reg is null) return; // Om null avslutar vi metoden
    if (HittaPlats(reg) != -1) // -1 betyder att fordonet inte hittades
    {
        Console.WriteLine("Fordonet är redan parkerat.");
        return;
    }

    Console.Write("Fordonstyp (B = bil, M = motorcykel): ");
    string typ = (Console.ReadLine() ?? "").Trim().ToUpperInvariant(); // ?? om null använd tom text, Trim tar bort mellanslag och ToUpper gör stora bokstäver
    if (typ != "B" && typ != "M") // && betyder och
    {
        Console.WriteLine("Ogiltig fordonstyp.");
        return;
    }

    int plats = HittaLedigPlats(typ);
    if (plats == -1)
    {
        Console.WriteLine("Det finns ingen lämplig ledig plats.");
        return;
    }

    LäggTill(plats, typ, reg);
    Console.WriteLine($"Parkera {reg} på plats {plats + 1}."); // +1 för att array börjar på 0 men platserna på 1
    Console.WriteLine($"Kvitto: {reg} - parkeringsplats {plats + 1}");
}









// Flytta fordon
void FlyttaFordon()
{
    string? reg = LäsRegistreringsnummer();
    if (reg is null) return;

    int gammalPlats = HittaPlats(reg);
    if (gammalPlats == -1)
    {
        Console.WriteLine("Fordonet hittades inte.");
        return;
    }

    string typ = Fordonstyp(gammalPlats);
    Console.WriteLine($"Fordonet står på plats {gammalPlats + 1}.");
    Console.Write("Ange ny plats (1-100): ");

    if (!int.TryParse(Console.ReadLine(), out int platsnummer) || platsnummer < 1 || platsnummer > 100) // TryParse kollar om man skrev en siffra, || betyder eller
    {
        Console.WriteLine("Platsnumret måste vara mellan 1 och 100.");
        return;
    }

    int nyPlats = platsnummer - 1; // -1 för att få rätt index i arrayen
    if (nyPlats == gammalPlats)
    {
        Console.WriteLine("Fordonet står redan på den platsen.");
        return;
    }

    if (!KanTaEmot(nyPlats, typ)) // ! betyder inte, alltså om platsen inte kan ta emot fordonet
    {
        Console.WriteLine("Den nya platsen har inte utrymme för fordonet.");
        return;
    }

    TaBort(gammalPlats, reg);
    LäggTill(nyPlats, typ, reg);
    Console.WriteLine($"{reg} har flyttats till plats {platsnummer}.");
}











// Ta bort vid uthämtning
void HämtaUtFordon()
{
    string? reg = LäsRegistreringsnummer();
    if (reg is null) return;

    int plats = HittaPlats(reg);
    if (plats == -1)
    {
        Console.WriteLine("Fordonet hittades inte.");
        return;
    }

    TaBort(plats, reg);
    Console.WriteLine($"{reg} har hämtats ut från plats {plats + 1}.");
}


// Söka efter fordon
void SökFordon()
{
    string? reg = LäsRegistreringsnummer();
    if (reg is null) return;

    int plats = HittaPlats(reg);

    Console.WriteLine(plats == -1
        ? "Fordonet hittades inte."
        : $"{reg} ({(Fordonstyp(plats) == "B" ? "bil" : "MC")}) står på plats {plats + 1}."); // ? : är som en kort if else, väljer text beroende på villkoret
}


// Visa alla platser
void VisaPlatser()
{
    for (int i = 0; i < parkeringsplatser.Length; i++)
    {
        string innehåll = parkeringsplatser[i];
        Console.WriteLine($"Plats {i + 1,3}: {(innehåll == "" ? "Ledig" : innehåll)}"); // ,3 gör platsnumret jämnt i utskriften, ? : väljer Ledig eller innehållet
    }
}










// Läsa registreringsnummer
string? LäsRegistreringsnummer()
{
    Console.Write("Registreringsnummer (1-10 tecken): ");
    string reg = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

    if (reg.Length < 1 || reg.Length > 10 || reg.Contains('|') || reg.Contains(':')) // Kollar längden och stoppar tecken som vi använder för att dela upp fordon
    {
        Console.WriteLine("Ogiltigt registreringsnummer. Använd 1-10 tecken, utan | eller :.");
        return null;
    }

    return reg;
}


// Hitta fordonets plats
int HittaPlats(string reg)
{
    for (int i = 0; i < parkeringsplatser.Length; i++)
    {
        if (parkeringsplatser[i] == "") continue; // continue hoppar över tomma platser

        string nummer = parkeringsplatser[i].Substring(2); // Tar bort första två tecknen t.ex M: så bara regnumret blir kvar

        if (nummer.Split('|').Contains(reg)) return i; // Split delar två MC vid | och Contains kollar om regnumret finns
    }

    return -1;
}









// Hämtar fordonstyp
string Fordonstyp(int plats) => parkeringsplatser[plats].Substring(0, 1); // => kort metod, Substring tar första bokstaven B eller M


// Kolla om platsen har utrymme
bool KanTaEmot(int plats, string typ) // bool ger true eller false
{
    string innehåll = parkeringsplatser[plats];

    if (innehåll == "") return true;

    return typ == "M" && innehåll.StartsWith("M:") && !innehåll.Contains('|'); // Bara MC får dela plats, StartsWith kollar M: och | betyder redan två MC
}










// Två MC kan dela plats
int HittaLedigPlats(string typ)
{
    if (typ == "M") // Fråga om de är en motorcykel
    {
        for (int i = 0; i < parkeringsplatser.Length; i++) // gå igenom alla platser
        {
            if (parkeringsplatser[i].StartsWith("M:") && !parkeringsplatser[i].Contains('|')) // Letar först efter plats med bara en MC
                return i;
        }
    }

    for (int i = 0; i < parkeringsplatser.Length; i++)
    {
        if (parkeringsplatser[i] == "") return i;
    }

    return -1;
}











// Lägga till fordon
void LäggTill(int plats, string typ, string reg)
{
    if (parkeringsplatser[plats] == "")
        parkeringsplatser[plats] = $"{typ}:{reg}"; // Sparar t.ex B:ABC123
    else
        parkeringsplatser[plats] += $"|{reg}"; // Lägger till andra MC med | mellan regnumren
}










// Ta bort fordon
void TaBort(int plats, string reg)
{
    string typ = Fordonstyp(plats);

    string[] nummer = parkeringsplatser[plats].Substring(2).Split('|'); // Delar upp regnumren om två MC står på samma plats

    string[] kvar = nummer.Where(n => n != reg).ToArray(); // Where väljer alla utom regnumret vi tar bort, ToArray gör en ny array

    parkeringsplatser[plats] = kvar.Length == 0 ? "" : $"{typ}:{string.Join("|", kvar)}"; // Om inga kvar blir platsen tom, annars sätter Join ihop numren igen
}
// Jag nöjer mig med G uppgiften.