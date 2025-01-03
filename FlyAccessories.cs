//=============================================================
//🔷🔷🔷🔷🔷=== Author: janluksoft@interia.pl ===🔷🔷🔷🔷🔷
//─────────────────────────────────────────────────────────────

namespace FlyAccessories;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// Indexer class for Hot Air Balloon Baskets
public class CBaskets : IPrintJ
{
    private List<Basket> ListBaskets;
    public int Size => ListBaskets.Count;
    private readonly IPrintJ printer;

    public CBaskets()
    {
        printer = this;
        ListBaskets = GetExampleBaskets();
    }

    public void PrintOld(Basket b) //this working too, but not use
    { //printer.FixedString(b.Name,11)
        Console.WriteLine($"    Basket Name: {IPrintJ.FixedString(b.Name, 8)}, " +
            $"Material: {IPrintJ.FixedString(b.Material, 7)},  Weight: {b.Weight,6} kg.");
        printer.PrintHead(b.Name, 8);
    }

    public void Print(Basket b) =>
        Console.WriteLine(GetMessageToPrint(b));

    public string GetMessageToPrint(Basket b) =>
        $"    Basket Name: {b.Name,-8}, Material: {b.Material,-7}" +
                          $", Weight: {b.Weight,6} kg.";

    // Indexer by index
    public Basket this[int index]
    {
        get => (index < 0 || index >= Size) ? GetDefault() : ListBaskets[index];
        set { if (index >= 0 && index < Size) ListBaskets[index] = value; } // Assign the new value
        //A lambda (set => ...) is not ideal here because assignments must be inside {} blocks.
    }

    // Indexer by name (read-only, no set accessor needed)
    public Basket this[string name] => 
        ListBaskets.FirstOrDefault(b => b.Name == name) ?? GetDefault();

    private List<Basket> GetExampleBaskets() => new()
    {
        new("Bas-N05", "Wicker", 102.2, 4.5, 5),
        new("Bas-N08", "Wicker", 145.2, 6.2, 8),
        new("Bas-N12", "Wicker", 221.4, 8.1, 12),
        new("Dry-08" , "Alu   ", 165.4, 8.4, 14),
        new("Dry-14" , "Alu   ", 302.4,14.2, 30),
        new("Schr-II", "Wicker",  86.2, 3.5,  4), //Schroeder II
        new("Kub-K18", "Wicker",  85.1, 2.8,  4), //Kubicek K18 basket
    };
    private Basket GetDefault() => new("nothing", "", 0, 0, 0);
}
public record Basket(string Name, string Material, double Weight, double Surface, int Persons);

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// Indexer class for Jet Engines
public class CJetEngines : IPrintJ
{
    private List<JetEngine> ListEngines;
    public int Size => ListEngines.Count;
    private readonly IPrintJ printer;

    public CJetEngines()
    {
        printer = this;
        ListEngines = GetExampleEngines();
    }

    //public void Print(JetEngine b) =>
    //    Console.WriteLine($"    JetEngine Name: {b.Name,-11}, Thrust:{b.Thrust,9} N" +
    //        $", Fuel: {b.Fuel,-9}, Blades: {b.Blades,2}, Weight: {b.Weight} kg.");


    public void Print(JetEngine b) => Console.WriteLine(GetMessageToPrint(b));

    public string GetMessageToPrint(JetEngine b) =>
        $"    JetEngine Name: {b.Name,-11}, Thrust:{b.Thrust,9} N" +
        $", Fuel: {b.Fuel,-9}, Blades: {b.Blades,2}, Weight: {b.Weight} kg.";

    public string GetMessageToPrint2(JetEngine b) =>
        $"JetEngine: {b.Name,-10}, Thrust:{b.Thrust,7} N";


    // Indexer by index with both get and set accessors
    public JetEngine this[int index]
    {
        get => (index < 0 || index >= Size) ? GetDefault() : ListEngines[index];
        set { if (index >= 0 && index < Size) ListEngines[index] = value; } // Assign the new value
    }

    // Indexer by name (read-only, no set accessor needed)
    public JetEngine this[string name] => ListEngines.FirstOrDefault(e => e.Name == name) ?? GetDefault();

    private List<JetEngine> GetExampleEngines() => new()
    {
        new("CFM56-5A5" , 105000, eFuel.JetA_1   , 34, 2240),
        new("CFM56-5A1" , 110000, eFuel.JetA_1   , 38, 2270),
        new("TRENT-900" , 356000, eFuel.F_34     , 24, 6436),
        new("CF6-80E1A3", 102000, eFuel.JetA_1   , 38, 2020),
        new("CF6-80E1A4", 102000, eFuel.JetA_1   , 38, 2120),
        new("PW4168A"   , 110000, eFuel.JP_8     , 34, 2410),
        new("GE90-92B"  , 130000, eFuel.Avgas_100, 22, 2710),
        new("TRENT-892" , 146100, eFuel.F_34     , 24, 3100),
        new("GEnx-2B"   , 320000, eFuel.F_34     , 24, 5815),
        new("CFM56-7B27", 121000, eFuel.F_34     , 28, 2370)
    };

    private JetEngine GetDefault() => new("nothing", 0, null, 0, 0);
}
public record JetEngine(string Name, double Thrust, eFuel? Fuel, int Blades, double Weight);

#region ≡≡≡≡≡≡≡≡≡≡≡ Records, enum and interface ≡≡≡≡≡≡≡≡≡≡≡

public enum eFuel { Avgas_100, F_34, JetA_1, JP_8 }

// ✅ IPrintJ interface
public interface IPrintJ
{
    static string FixedString(string? xmessage, int xLen) => 
        xmessage?.PadRight(xLen).Substring(0, xLen) ?? string.Empty;

    void PrintHead(string xInfo, int xCount)
    {
        Console.WriteLine($"\n\r{xInfo} (count: {xCount})");
    }

    void PrintHead2(string xInfo, int xCount) =>
        Console.WriteLine($"\n\r{xInfo} (count: {xCount})");
}
#endregion ≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡
