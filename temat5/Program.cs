using System.Globalization;

namespace Lab04
{
    class Program
    {
        public static void Main(string[] args)
        {
            // ZADANIE 1
            DataBaseHandler<Region> dataBaseHandlerRegion = new DataBaseHandler<Region>();
            string regionsFilePath = "DataBase/regions.csv";
            List<Region> regionsList = dataBaseHandlerRegion.GetListOfObjects(regionsFilePath, x => new Region(x[0], x[1]));
            
            DataBaseHandler<Territory> dataBaseHandlerTerritory = new DataBaseHandler<Territory>();
            string territoriesFilePath = "DataBase/territories.csv";
            List<Territory> territoryList = dataBaseHandlerTerritory.GetListOfObjects(territoriesFilePath, x => new Territory(x[0], x[1], x[2]));
            
            DataBaseHandler<EmployeeTerritory> dataBaseHandlerEmployeeTerritory = new DataBaseHandler<EmployeeTerritory>();
            string employeeTerritoriesFilePath = "DataBase/employee_territories.csv";
            List<EmployeeTerritory> employeeTerritoryList = dataBaseHandlerEmployeeTerritory.GetListOfObjects(employeeTerritoriesFilePath, x => new EmployeeTerritory(x[0], x[1]));

            DataBaseHandler<Employee> dataBaseHandlerEmployee = new DataBaseHandler<Employee>();
            string employeeFilePath = "DataBase/employees.csv";
            List<Employee> employeeList = dataBaseHandlerEmployee.GetListOfObjects(employeeFilePath, x => new Employee(x[0], x[1], x[2], x[3], x[4], x[5]
                , x[6], x[7], x[8], x[9], x[10], x[11], x[12], x[13], x[14], x[15], x[16], x[17]));

            
            // regions: 
            int x = 0;
            foreach (var region in regionsList)
            {
                Console.WriteLine(region);
                if (x == 3)
                {
                    break;
                }
                x++;
            }
            
            
            // territories: 
            Console.WriteLine();
            x = 0;
            foreach (var territory in territoryList)
            {
                Console.WriteLine(territory);
                if (x == 3)
                {
                    break;
                }
                x++;
            }
            
            
            // employee_territories: 
            Console.WriteLine();
            x = 0;
            foreach (var et in employeeTerritoryList)
            {
                Console.WriteLine(et);
                if (x == 3)
                {
                    break;
                }
                x++;
            }
            
            // employee: 
            Console.WriteLine();
            x = 0;
            foreach (var emp in employeeList)
            {
                Console.WriteLine(emp);
                if (x == 3)
                {
                    break;
                }
                x++;
            }
            
            
            // ZADANIE 2
            Console.WriteLine();
            Console.WriteLine("########################### ZADANIE 2 ##########################");
            Console.WriteLine();
            var lastNamesAllEmployee = 
                from e in employeeList
                select new { e.Lastname };

            foreach (var e in lastNamesAllEmployee)
            {
                Console.WriteLine(e.Lastname);
            }


            // ZADANIE 3
            Console.WriteLine();
            Console.WriteLine("########################### ZADANIE 3 ##########################");
            Console.WriteLine();
            var regionsAndTerritoryEmp =
                from e in employeeList
                join et in employeeTerritoryList on e.Employeeid equals et.Employeeid
                join t in territoryList on et.Territoryid equals t.Territoryid
                join r in regionsList on t.Regionid equals r.Regionid
                select new
                {
                    e.Lastname,
                    r.Regiondescription,
                    t.Territorydescription
                };

            foreach (var regAndTer in regionsAndTerritoryEmp)
            {
                Console.WriteLine(regAndTer.Lastname + " " + regAndTer.Regiondescription + " " +
                                  regAndTer.Territorydescription);
            }
            
            
            // ZADANIE 4
            Console.WriteLine();
            Console.WriteLine("########################### ZADANIE 4 ##########################");
            Console.WriteLine();
            var regionsEmployees =
                from e in employeeList
                join et in employeeTerritoryList on e.Employeeid equals et.Employeeid
                join t in territoryList on et.Territoryid equals t.Territoryid
                join r in regionsList on t.Regionid equals r.Regionid
                group e by r.Regiondescription
                into regionGroup
                select new
                {
                    RegionDescription = regionGroup.Key,
                    EmployeeLastnames = regionGroup.Select(emp => emp.Lastname).ToList()
                };

            foreach (var region in regionsEmployees)
			{
    			Console.WriteLine($"Region: {region.RegionDescription}");
    			foreach (var lastname in region.EmployeeLastnames)
    			{
       					Console.WriteLine($"   {lastname}");
    			}
			
			}

			// ZADANIE 5
            Console.WriteLine();
            Console.WriteLine("########################### ZADANIE 5 ##########################");
            Console.WriteLine();

            DataBaseHandler<Order> dataBaseHandlerOrder = new DataBaseHandler<Order>();
            string orderFilePath = "DataBase/orders.csv";
            List<Order> orderList = dataBaseHandlerOrder.GetListOfObjects(orderFilePath, x => new Order(x[0], x[1], x[2], x[3], x[4], x[5]
                , x[6], x[7], x[8], x[9], x[10], x[11], x[12], x[13]));


            DataBaseHandler<OrderDetail> dataBaseHandlerOrderDetail = new DataBaseHandler<OrderDetail>();
            string orderDetailFilePath = "DataBase/orders_details.csv";
            List<OrderDetail> orderDetailList = dataBaseHandlerOrderDetail.GetListOfObjects(orderDetailFilePath, x => new OrderDetail(x[0], x[1], x[2], x[3], x[4]));


            // orders:
            Console.WriteLine();
            x = 0;
            foreach (var order in orderList)
            {
                Console.WriteLine(order);
                if (x == 3)
                {
                    break;
                }
                x++;
            }
            
            
            // orders_details: 
            Console.WriteLine();
            x = 0;
            foreach (var od in orderDetailList)
            {
                Console.WriteLine(od);
                if (x == 3)
                {
                    break;
                }
                x++;
            }
            
            var ordersValues = 
				from o in orderList
				join od in orderDetailList on o.Orderid equals od.Orderid
				group od by o.Orderid into orderGroup
				select new {
					OrderId = orderGroup.Key,
					Value = orderGroup.Sum(od => Decimal.Parse(od.Unitprice, CultureInfo.InvariantCulture) * Decimal.Parse(od.Quantity, CultureInfo.InvariantCulture) 
								* (1 - Decimal.Parse(od.Discount, CultureInfo.InvariantCulture))),
					Employeeid = orderList.First(o => o.Orderid == orderGroup.Key).Employeeid
				};

				
			var ordersStats = 
				from ov in ordersValues
				join e in employeeList on ov.Employeeid equals e.Employeeid
				group ov by new { e.Employeeid, e.Lastname} into g
				select new {
					Employee = g.Key.Lastname,
					OrderCount = g.Count(),
					AverageOrderValue = Math.Round(g.Average(x => x.Value), 2),
					MaxOrderValue = Math.Round(g.Max(x => x.Value), 2)
				};
 

			Console.WriteLine();
            foreach (var stat in ordersStats)
            {
				Console.WriteLine($"{stat.Employee}: Count = {stat.OrderCount}, Average = {stat.AverageOrderValue}, Max = {stat.MaxOrderValue}");
            }

            
            
            


			
	

	
           
        }
    }
}

