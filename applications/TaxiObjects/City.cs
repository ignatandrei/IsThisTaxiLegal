using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxiObjects
{
    public class City
    {
        static List<City> cities=new List<City>();
        private City()
        {

        }
        public string Name { get; private set; }

        public static City FromName(string name)
        {
            var City = cities.FirstOrDefault(it =>string.Compare(it.Name , name,StringComparison.InvariantCultureIgnoreCase)==0);
            if(City == null)
            {
                City = new City();
                City.Name = name;
                cities.Add(City);
            }
            return City;
        }
    }
}
