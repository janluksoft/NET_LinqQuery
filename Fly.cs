
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using FlyingMachines;

namespace Fly;
public class CMainFly
{
    public CMainFly()
    {
    }

    public void StartFly()
    {
        CExampleMachines cExMachine = new CExampleMachines();
        List<FlyingMachine> machines = cExMachine.GetExampleFlyingMachine();

        Console.WriteLine("\n\rHello, Aircrafts! Demonstration of SQL-like LINQ-QUERY queries");
        //xmachines, bool xPrintAccessories, bool xPrintHeaders
        PrintPureList(machines, false, true);

        PrintViaPatternMatching(machines);

        PrintViaLinqQuery(machines);
        PrintViaLinqQuery2(machines);

        Console.WriteLine("\n\r---- The End -----------");
    }

    void PrintPureList(List<FlyingMachine> xmachines, bool xPrintAccessories, bool xPrintHeaders)
    {
        Console.WriteLine("\n\rBelow are the parameters of several FlyingMachines and their calculated lift:");

        string skind = "srfwerf"; //random
        foreach (var machine in xmachines)
        {
            bool bPrintHeader = !(skind == machine.Kind);

            if (!xPrintHeaders) bPrintHeader = false;
            Console.WriteLine(machine.FlyingMessage(bPrintHeader, xPrintAccessories));
            skind = machine.Kind;
        }
    }

    void PrintViaPatternMatching(List<FlyingMachine> machines)
    {
        Console.WriteLine("\n\rStart print via Pattern Matching");

        string st = "";
        foreach (FlyingMachine d in machines)
        {
            switch (d) //•	Pattern Matching (dopasowywanie wzorców) 
            {
                case JetPlane _jetPlane:
                    st = _jetPlane.FlyingMessage(false, false) +
                        "\n\r                                " +
                        _jetPlane.Jet_Accessories() + "\n\r";
                    Console.WriteLine(st);

                    break;
                case Helicopter _heli:
                    Console.WriteLine(_heli.FlyingMessage(false, false) + "\n\r");
                    break;
                case HotAirBalloon _ball:
                    st = _ball.FlyingMessage(false, false) +
                        "\n\r                " +
                        _ball.Balloon_Accessories() + "\n\r";
                    Console.WriteLine(st);

                    break;
                default:
                    st = "there is no action to that Person";
                    Console.WriteLine(st);
                    break;
            }
        }

    }

    void PrintViaLinqQuery(List<FlyingMachine> machines)
    {
        Console.WriteLine("\n\rStart print via LinqQuery");

        bool bPrintAcc = true;
        var renewedMachine = //LINQ query syntax (like SQL)
          from machine in machines
          let action = machine

          switch
          {
              JetPlane _jetPlane => _jetPlane.Jet_message(bPrintAcc),
              //JetPlane _jetPlane => _jetPlane.FlyingMessage(false, false) + "\n\r",    //.Jet_message(bPrintAcc),

              Helicopter _heli => _heli.FlyingMessage(false, false) + "\n\r",

              HotAirBalloon _ball => _ball.Balloon_message(bPrintAcc),

              _ => "there is no action to that Person"
          }
          select new { Type = machine, Action = action };

        foreach (var person in renewedMachine)
            Console.WriteLine($"{person.Type.ToString()}" +
                    $" - {person.Action}");
    }

    void PrintViaLinqQuery2(List<FlyingMachine> machines)
    {

        //1. Filtering by Lift Force
        var highLiftMachines =
            from machine in machines
            where machine.CalculateLift() > (50000* 9.81) //Force in [N]
            select new { machine.Name, Lift = machine.CalculateLift() };

        Console.WriteLine("\n\r1) Filtering by Lift Force > 50000 Kg");
        foreach (var item in highLiftMachines)
            Console.WriteLine($"  Machine: {item.Name,-18}, lift force:{(item.Lift/9.81),12:### ### ###.0} kG");

        // 2. Ordering Machines by Weight
        var orderedByWeight =
            from machine in machines
            orderby machine.Weight descending
            select machine;

        Console.WriteLine("\n\r2) Ordering Machines by Weight");
        foreach (var item in orderedByWeight)
            Console.WriteLine($"  Machine: {item.Name,-18}, Weight:{item.Weight,10:### ### ###.0} kg" +
                $", Lift:{item.CalculateLift() / 9.81,9:### ### ###} kg");

        // 3. Grouping Machines by Kind
        var groupedByKind =
        from machine in machines
        group machine by machine.Kind into machineGroup
        select new { Kind = machineGroup.Key, Count = machineGroup.Count(), Machines = machineGroup };

        Console.WriteLine("\n\r3 Grouping Machines by Kind");
        foreach (var group in groupedByKind)
        {
            Console.WriteLine($"\n\rType: {group.Kind}, Count: {group.Count}");
            foreach (var m in group.Machines)
            {
                Console.WriteLine($"    {m.Name}");
            }
        }

        //4. Projection (Selecting Specific Fields)
        var machineSummaries =
        from machine in machines
        select new { machine.Name, machine.Kind, Lift = machine.CalculateLift() };

        Console.WriteLine("\n\r4) Projection (Selecting Specific Fields)");
        foreach (var summary in machineSummaries)
            Console.WriteLine($"    Machine: {summary.Name,-18}  {summary.Kind,-8} " +
                $"- Lift:{(summary.Lift / 9.81),12:### ### ###.0} kG");

        //5. Aggregation (Total Lift of All Machines)
        var totalLift =
        (from machine in machines select machine.CalculateLift()).Sum();

        Console.WriteLine($"\n\r                                      " +
            $"Total Lift: {(totalLift / 9.81),10:### ### ###.0} kG");

    }
}
