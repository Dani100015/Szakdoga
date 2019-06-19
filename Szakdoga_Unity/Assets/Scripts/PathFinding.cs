using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PathFinding : MonoBehaviour
{
    Game game;

    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    /// <summary>
    /// Két naprendszer összekötő útat keres meg
    /// </summary>
    /// <param name="startSystem"></param>
    /// <param name="endSystem"></param>
    /// <returns>Az útvonal elemeinek a listájával tér vissza</returns>
   public List<SolarSystem> FindWay(SolarSystem startSystem, SolarSystem endSystem)
   {
        List<SolarSystem> path = new List<SolarSystem>();

        AbsztraktAllapot Szk = new SolarSystemAllapot(startSystem, endSystem,game.Systems);
        GrafKereso Gk = new BackTrack(5);
        Csucs celcsucs = Gk.Keres(new Csucs(Szk));
        if (celcsucs == null)
        {
            Debug.Log("Nemjó");
        }
        else
        {
            Csucs cs = celcsucs;
            path.Add(cs.allapot.getCurrentSolarSystem());
            while (cs != null)
            {
                cs = cs.Szulo;
                path.Add(cs.allapot.getCurrentSolarSystem());
            }
        }

        return path;
   }
}

abstract class AbsztraktAllapot
{
    public abstract bool AllapotE();
    public abstract bool CelAllapotE();
    public abstract int OperatorokSzama();
    public abstract AbsztraktAllapot SzuperOperator(int i);
    public abstract SolarSystem getCurrentSolarSystem();
}
class SolarSystemAllapot : AbsztraktAllapot
{
    public SolarSystem currentSystem;
    public SolarSystem endSystem;
    public List<SolarSystem> systems;

    public SolarSystemAllapot(SolarSystem startSystem, SolarSystem endSystem, List<SolarSystem> systems)
    {
        this.currentSystem = startSystem;
        this.endSystem = endSystem;
        this.systems = systems;
    }

    public override bool AllapotE()
    {
        return (systems.Contains(this.currentSystem)); //?????? még kell?
    }
    public override bool CelAllapotE()
    {
        return currentSystem.Name == endSystem.Name;
    }
    public override int OperatorokSzama()
    {
        return currentSystem.neighbourSystems.Count;
    }
    public override AbsztraktAllapot SzuperOperator(int i)
    {
        return RelayTravel(currentSystem.neighbourSystems[i]);
    }
    private AbsztraktAllapot RelayTravel(SolarSystem nextSystem)
    {
        SolarSystemAllapot SSA = new SolarSystemAllapot(nextSystem,endSystem,systems);

        if (!AllapotE())
            return null;

        return SSA;
    }

    public override string ToString()
    {
        return "(" + currentSystem.Name + ", " + endSystem.Name +  ")";
    }
    public override bool Equals(object obj)
    {
        SolarSystemAllapot szk = (SolarSystemAllapot)obj;
        return szk.currentSystem.Name == currentSystem.Name;
    }

    public override SolarSystem getCurrentSolarSystem()
    {
        return currentSystem;
    }
}
class Csucs
{
    //Mezők

    int melyseg;
    Csucs szulo;
    public int legutobbiOp;
    public AbsztraktAllapot allapot;

    public SolarSystem currentSolarSystem;

    //Propertik

    public int Melyseg { get { return melyseg; } }
    public Csucs Szulo { get { return szulo; } }

    //Konstruktor 

    public Csucs(AbsztraktAllapot k)
    {
        this.allapot = k;
        this.melyseg = 0;
        this.szulo = null;
        this.legutobbiOp = 0; // hívás után kell növelni
    }

    //Medódusok
    bool KorFigyeles(AbsztraktAllapot ujAllapot)
    {
        Csucs cs = this;
        while (cs != null)
        {
            if (cs.allapot.Equals(ujAllapot)) return true;
            cs = cs.szulo;
        }
        return false;
    }
    public Csucs KovetkezoGyermek()
    {
        AbsztraktAllapot kovAllapot = null;
        while (kovAllapot == null || KorFigyeles(kovAllapot))
        {
            if (legutobbiOp == allapot.OperatorokSzama()) break;
            kovAllapot = allapot.SzuperOperator(this.legutobbiOp);
            legutobbiOp++;
        }
        if (kovAllapot == null) return null;

        Csucs gyermek = new Csucs(kovAllapot);
        gyermek.allapot = kovAllapot;
        gyermek.melyseg = this.melyseg + 1;
        gyermek.szulo = this;
        gyermek.legutobbiOp = 0;
        return gyermek;
    }
    public bool TerminalisE()
    {
        return allapot.CelAllapotE();
    }
    public override string ToString()
    {
        return allapot + " " + melyseg;
    }
}
abstract class GrafKereso
{
    public abstract Csucs Keres(Csucs start);
    public abstract Csucs KeresCiklussal(Csucs start);
}
class BackTrack : GrafKereso
{
    int melysegiKorlat;
    public BackTrack(int mélységiKorlát)
    {
        this.melysegiKorlat = mélységiKorlát;
    }
    public override Csucs Keres(Csucs akt)
    {
        // ha akt terminális, akkor kész, visszadom őt
        if (akt.TerminalisE()) return akt;
        // ha alértem a mélységi korlátot, akkor visszalépés
        if (akt.Melyseg >= melysegiKorlat) return null;
        // elenkező esetben lépek egyet előre
        // azaz legyártom a gyermeket

        Csucs terminalis = null;
        while (terminalis == null)
        {
            Csucs gyermek = akt.KovetkezoGyermek();
            if (gyermek == null) return null; // ha reku, akkor ez a visszalépés
            terminalis = Keres(gyermek); //reku ameddig, level elemhez nem erunk 
        }
        return terminalis;
    }
    public override Csucs KeresCiklussal(Csucs start)
    {
        Csucs akt = start;
        while (akt != null)
        {
            if (akt.TerminalisE()) return akt;
            if (akt.Melyseg >= melysegiKorlat) akt = akt.Szulo;
            Csucs gyermek = akt.KovetkezoGyermek();
            if (gyermek == null)
                akt = akt.Szulo;
            else
                akt = gyermek;
        }
        return null;
    }
}
