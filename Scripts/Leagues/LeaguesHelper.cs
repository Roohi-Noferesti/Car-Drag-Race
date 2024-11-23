using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//moeen: we have this class in both front and backend

  public static  class LeaguesHelper
    {
        public struct LeagueEntity
        {
            public int Index;
            public string ID;
            public int AscendingCount;// how many will fall from this league to lower level league
            public int DescendingCount;// how many will go to higher level league
            private string Description;// just everything that explains the league well


            public LeagueEntity(int index, string id, int ascendingCount,int descendingCount, string description)
            {
                Index = index;
                ID = id;
                AscendingCount = ascendingCount;
                DescendingCount = descendingCount;
                Description = description;
            }
        }

        public static List<LeagueEntity> Leagues= new List<LeagueEntity>()
        {
            new LeagueEntity(0,"Weekly",500,0,"its the previous weekly league that had all players. we have it to handle old versions and previous users"),
            new LeagueEntity(1,"bronze3",400,0,""),
            new LeagueEntity(2,"bronze2",50,50,""),
            new LeagueEntity(3,"bronze1",50,50,""),
            new LeagueEntity(4,"silver3",50,50,""),
            new LeagueEntity(5,"silver2",50,50,""),
            new LeagueEntity(1,"silver1",50,50,""),
            new LeagueEntity(1,"gold3",50,50,""),
            new LeagueEntity(1,"gold2",50,50,""),
            new LeagueEntity(1,"gold1",50,50,""),
            new LeagueEntity(1,"diamond3",50,10,""),
            new LeagueEntity(1,"diamond2",5,10,""),
            new LeagueEntity(1,"diamond1",5,10,""),
            new LeagueEntity(1,"heroes",0,5,"")

        };


    }

