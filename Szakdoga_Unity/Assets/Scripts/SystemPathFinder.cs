using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Syste
{
    class SystemPathFinder : MonoBehaviour
    {
        static Game game;
        static List<SolarSystem> Systems = new List<SolarSystem>();
        void Start()
        {
            game = GameObject.Find("Game").GetComponent<Game>();
            AbsztraktAllapot SystemsAllapot = new SolarSystemAllapot(game.currentSolarSystem, game.Systems[3], game.Systems);
            GrafKereso Gk = new BackTrack(5);
            Csucs celcsucs = Gk.Keres(new Csucs(SystemsAllapot));
            if (celcsucs == null)
            {
                Debug.Log("Nem lehet így megoldani!");
            }
            else
            {
                Csucs cs = celcsucs;
                while (cs != null)
                {
                    cs = cs.Szulo;
                }
            }

        }

    }

    abstract class AbsztraktAllapot
    {
        public abstract bool AllapotE();
        public abstract bool CelAllapotE();
        public abstract int OperatorokSzama();
        public abstract AbsztraktAllapot SzuperOperator(int i);
    }
    class SolarSystemAllapot : AbsztraktAllapot
    {
        List<SolarSystem> systems = new List<SolarSystem>();
        SolarSystem currentSolarSystem;
        SolarSystem targetSolarSystem;
        public SolarSystemAllapot(SolarSystem startSolarSystem, SolarSystem targetSolarSystem, List<SolarSystem> systems)
        {
            currentSolarSystem = startSolarSystem;
            this.targetSolarSystem = targetSolarSystem;
            this.systems = systems;
        }
        public override bool AllapotE()
        {
            return (systems.Contains(currentSolarSystem));
        }
        public override bool CelAllapotE()
        {
            return currentSolarSystem.Name == targetSolarSystem.Name;
        }
        public override int OperatorokSzama()
        {
            return currentSolarSystem.neighbourSystems.Count;
        }
        public override AbsztraktAllapot SzuperOperator(int i)
        {
            return RelayTravel(currentSolarSystem.neighbourSystems[i]);
        }
        private AbsztraktAllapot RelayTravel(SolarSystem neighbourSystem)
        {

            SolarSystemAllapot SA = new SolarSystemAllapot(neighbourSystem, targetSolarSystem, systems);

            if (!AllapotE())
                return null;

            return SA;
        }
        private List<SolarSystem> CopyList(List<SolarSystem> systems)
        {
            List<SolarSystem> L = new List<SolarSystem>();
            for (int i = 0; i < systems.Count; i++)
            {
                L.Add(systems[i]);
            }
            return L;
        }
        public override string ToString()
        {
            return "(" + currentSolarSystem.Name + ")";
        }
        public override bool Equals(object obj)
        {
            SolarSystemAllapot SSA = (SolarSystemAllapot)obj;
            return SSA.currentSolarSystem.Name == currentSolarSystem.Name;
        }
    }
    class Csucs
    {
        //Mezők

        int melyseg;
        Csucs szulo;
        public int legutobbiOp;
        public AbsztraktAllapot allapot;

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
            if (akt.TerminalisE()) return akt;
            if (akt.Melyseg >= melysegiKorlat) return null;

            Csucs terminalis = null;
            while (terminalis == null)
            {
                Csucs gyermek = akt.KovetkezoGyermek();
                if (gyermek == null) return null;
                terminalis = Keres(gyermek);
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
}
