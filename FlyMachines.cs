
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using FlyAccessories;

namespace FlyingMachines;

//========== End FlyingMachines =======================

public class CExampleMachines
{
    public CExampleMachines()
    {
    }

    public List<FlyingMachine> GetExampleFlyingMachine()
    {
        string name = "Airbus A320"; //variables for information only
        double weight = 78000;    // kg
        double airDensity = 0.38; // kg/m^3 on 10 km;  1,225 kg/m^3 on the ground
        double velocity = 230;    // m/s
        double wingArea = 122.6;  // m^2

        //--- Indeksers ------------------
        CJetEngines cjet = new CJetEngines();
        CBaskets cbask = new CBaskets();
        //cbask.Print( cbask[0]);

        List<FlyingMachine> machines = new List<FlyingMachine>
        {
            //            name, weight, airDensity, velocity, wingArea, liftCoefficient,Engine, No engines
            //new JetPlane(  "Airbus A320", 78000, 1.225, 250  , 122.6, 0.62),
            new JetPlane(    "Airbus A318", 67000, 0.38, 237, 114.2, 0.62, cjet["CFM56-5A5"] , 2), //2 × CFM56-5 110 kN
            new JetPlane(    "Airbus A320", 78000, 0.38, 230, 122.6, 0.62, cjet["CFM56-5A1"] , 2), //A320 2 × CFM56-5 110 kN
            new JetPlane("Airbus A380-800",560000, 0.38, 262, 845.2, 0.52, cjet["TRENT-900"] , 4),
            new JetPlane( "Boeing 737-800", 79000, 0.38, 230, 124.6, 0.62, cjet["CFM56-7B27"], 2), //2xCFM56-7B27
            new JetPlane( "Boeing 747-400",412000, 0.38, 252, 520.1, 0.67, cjet["GEnx-2B"]   , 4), //4xGE62 320 kN 5815kg

            //                      name, weight, airDensity, rotorRadius, angularVelocity
            new Helicopter("UH-60 Black Hawk", 10660, 1.225, 8.18 , 16.02, 0.19),
            new Helicopter("Eurocopter EC135",  2980, 1.225, 5.1  , 22.12, 0.18),
            new Helicopter("Aitbus H135"     ,  3100, 1.225, 5.4  , 20.20, 0.18),
            new Helicopter("Leonardo AW101"  , 15600, 1.225, 9.3  , 18.40, 0.12),
            //new Helicopter("UH-60 Black Hawk", 10660, 1.225, 8.18 , 27.02, 0.6),

            new HotAirBalloon("Cameron Z-750"   ,3640,21238, 1.225, 0.925, cbask["Dry-14"]), //Indexer
            new HotAirBalloon("Ultramagic N-425",1930,12100, 1.225, 0.925, cbask["Bas-N12"]),//airballoon basket
            new HotAirBalloon("Cameron Z-160"   , 650, 4531, 1.225, 0.925, cbask["Bas-N05"]),
            new HotAirBalloon("Kubicek BB70-Z"  ,1230, 7000, 1.225, 0.925, cbask["Bas-N08"]),
            new HotAirBalloon("Schroeder G24"   , 340, 2200, 1.225, 0.925, cbask["Schr-II"]),
            new HotAirBalloon("Kubicek BB34Z"   , 475, 3400, 1.225, 0.925, cbask["Kub-K18"]),
        };

        return (machines);
    }

}

//===== Interface for calculating lift =====
public interface ILiftCalculator
{
    double CalculateLift();
}

public interface IMessageFormat
{
    string fMakeMessage(string xKind, string xName, float xMass, double xLiftForce)
    {
        double liftForce_kG = xLiftForce/ 9.81;

        string smess = $" {FixedString(xKind, 11)} " +

            $" {FixedString(xName, 18)}" +
            $" mass:{xMass,  10:### ### ###.0} kg" +
            $"  lift force:{liftForce_kG,10:### ### ###.0} kG";
        return (smess);
    }

    string FixedString(string xmessage, int xLen)
    {
        string mess = xmessage + "                       ";
        mess = mess.Substring(0, xLen);
        return (mess);
    }
}

// ordinaty class for air parameters
public class CAir 
{
    public double AirDensity { get; protected set; } // in kg/m^3 ambientAirDensity
    public double HeatedAirDensity { get; protected set; } // in kg/m^3
    protected CAir(double airDensity, double heatedAirDensity = 0)
    {
        this.AirDensity = airDensity; //double heatedAirDensity;
        this.HeatedAirDensity = heatedAirDensity;
    }
    public (double airCold, double airHot) GetAirDensity() //System.ValueTuple.dll
    {
        return (AirDensity, HeatedAirDensity);
    }
}

// Abstract base class for flying machines
public abstract class FlyingMachine : CAir, ILiftCalculator, IMessageFormat
{
    public string Name { get; protected set; }
    public double Weight { get; protected set; } // in kg
    public string Kind { get; protected set; }

    /*
    protected FlyingMachine(string name, double weight, double airDensity, string kind = "") : base(airDensity)
    {
        Name = name;
        Weight = weight;
        Kind = kind;
    } */
    protected FlyingMachine(string name, double weight, double airDensity, 
                            double heatedAirDensity= 0, string kind = "") 
                     : base(airDensity, heatedAirDensity)
    {
        Name = name;
        Weight = weight;
        Kind = kind;
    }

    public abstract double CalculateLift(); // Must be implemented by subclasses
    public abstract string FlyingMessage(bool bPrintHeader, bool bPrintAccessories);// Must be implemented by subclasses
}

// Jet Plane class
public class JetPlane : FlyingMachine
{
    private double velocity;
    private double wingArea;
    private double liftCoefficient;
    private int NumberOfEngines;
    private JetEngine jEngine;

    #region --- Constructors ---------------
    public JetPlane(string name, double weight, double airDensity, double velocity,
                    double wingArea, double liftCoefficient, JetEngine jeng, int numEng) 
                  : base(name, weight, airDensity, 0, "jet")
    {
        InitJetPlane(name, weight, airDensity, velocity, wingArea, liftCoefficient);
        this.jEngine = jeng;
        this.NumberOfEngines = numEng;
    }

    void InitJetPlane(string name, double weight, double airDensity, double velocity,
                    double wingArea, double liftCoefficient)
    {
        this.velocity = velocity;
        this.wingArea = wingArea;
        this.liftCoefficient = liftCoefficient;
    }
    #endregion ---------------------------


    public string Jet_message(bool bPrintAccessories)
    {
        string sRet = FlyingMessage(false, false) + "\n\r";

        if(bPrintAccessories) sRet += "                                " +
            Jet_Accessories() + "\n\r";
        return (sRet);
    }

    public string Jet_Accessories()
    {
        CJetEngines cjet = new CJetEngines();
        string sMess = $"{NumberOfEngines} X "+ cjet.GetMessageToPrint2(jEngine);
        return (sMess);
    }

    public string Jet_Accessories(bool bPrint)
    {
        CJetEngines cjet = new CJetEngines();
        string sMess = "\n\r" + cjet.GetMessageToPrint(jEngine) + "\n\r";
        if (bPrint) Console.Write(sMess);
        return (sMess);
    }

    public override string FlyingMessage(bool bPrintHeader, bool bPrintAccessories)
    {
        string sMess = String.Empty;
        if (bPrintHeader) sMess = "\n\rList of JetPlanes:\n\r";

        sMess += "   "+ (this as IMessageFormat).fMakeMessage("JetPlane", Name, (float)Weight, CalculateLift());

        if (bPrintAccessories) sMess += Jet_Accessories(false);

        return (sMess);
    }

    public override double CalculateLift()
    {   //Force in [N] Newton , not kG
        double _lift = 0.5 * AirDensity * velocity * velocity * wingArea * liftCoefficient;
        return (_lift); 
    }
}

// Helicopter class
public class Helicopter : FlyingMachine
{
    private double rotorRadius;
    private double angularVelocity;
    private double liftCoefficient;

    public Helicopter(string name, double weight, double airDensity,
                      double rotorRadius, double angularVelocity, double liftCoefficient)
        : base(name, weight, airDensity, 0, "heli")
    {
        this.rotorRadius = rotorRadius;
        this.angularVelocity = angularVelocity;
        this.liftCoefficient = liftCoefficient;
    }

    public override string FlyingMessage(bool bPrintHeader, bool bPrintAccessories)
    {   //This sMess variable is convenient for debugging
        string sMess = String.Empty;
        if (bPrintHeader) sMess = "\n\rList of Helicopters:\n\r";

        sMess += "   " + (this as IMessageFormat).fMakeMessage("Heli ground", Name, (float)Weight, CalculateLift());
        return (sMess);
    }

    public override double CalculateLift()
          {//new Helicopter("UH-60 Black Hawk", 10660, 1.225, 8.18 , 27.02, 0.6),
        double lift = liftCoefficient * (2.0 / 9.0) * AirDensity * Math.PI * Math.Pow(rotorRadius, 4) * Math.Pow(angularVelocity, 2);
        return (lift);
    }
}

// Hot Air Balloon class
public class HotAirBalloon : FlyingMachine
{
    private double volume;
    private Basket cBas;

    public HotAirBalloon(string name, double weight, double volume, double ambientAirDensity,
                         double heatedAirDensity, Basket cBas )
        : base(name, weight, ambientAirDensity, heatedAirDensity, "balloon") //base = CAir(double airDensity, double heatedAirDensity = 0)
    {
        this.volume = volume;
        this.cBas = cBas;
    }

    public string Balloon_message(bool bPrintAccessories)
    {
        string sRet = FlyingMessage(false, false) + "\n\r";

        if (bPrintAccessories) sRet += "                                " +
                Balloon_Accessories() + "\n\r";
        return (sRet);
    }

    public string Balloon_Accessories()
    {
        CBaskets cbask = new CBaskets();
        string sMess = cbask.GetMessageToPrint(cBas);
        return (sMess);
    }

    public string Balloon_Accessories(bool bPrint)
    {
        CBaskets cbask = new CBaskets();
        string sMess = "\n\r   " + cbask.GetMessageToPrint(cBas) + "\n\r";

        if (bPrint) Console.Write(sMess);
        return (sMess);
    }

    public override string FlyingMessage(bool bPrintHeader, bool bPrintAccessories)
    {   //This sMess variable is convenient for debugging
        string sMess = String.Empty;
        if (bPrintHeader) sMess = "\n\rList of HotAirBalloons:\n\r";

        sMess += "   " + (this as IMessageFormat).fMakeMessage("Balloon", Name, (float)Weight, CalculateLift());

        if (bPrintAccessories) sMess += Balloon_Accessories(false);

        return (sMess);
    }

    public override double CalculateLift()
    {
        var tupleAir = GetAirDensity(); //Use tuple type
        double balloonLift = volume * (tupleAir.airCold - tupleAir.airHot) * 9.81;
        return (balloonLift);
    }
}

