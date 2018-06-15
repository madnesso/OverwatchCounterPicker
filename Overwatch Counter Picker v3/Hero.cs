using System.Drawing;

namespace Overwatch_Counter_Picker_v3
{
    class Hero
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string HeroCounters { get; set; }
        public string CounterHeroes { get; set; }
        public int CounterValue { get; set; }
        public Image Icon { get; set; }

        public Hero(string name, string group, string herocounters, string counterheroes, Image icon, int countervalue)
        {
            Name = name;
            Group = group;
            HeroCounters = herocounters;
            CounterHeroes = counterheroes;
            CounterValue = countervalue;
            Icon = icon;
        }
    }
}