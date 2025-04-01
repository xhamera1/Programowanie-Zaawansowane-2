namespace Lab04
{
    public class EmployeeTerritory
    {
        public string Employeeid { get; set; }
        public string Territoryid { get; set; }


        public EmployeeTerritory(string employeeid, string territoryid)
        {
	        Employeeid = employeeid;
	        Territoryid = territoryid;
	        
        }
        
        public override string ToString()
        {
	        return $"Employee: {Employeeid}, Territory: {Territoryid}";
        }

		
		

	
		
    }


}