namespace Lab04
{
    public class Employee
    {
        public string Employeeid { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Title { get; set; }
        public string Titleofcourtesy { get; set; }
        public string Birthdate { get; set; }
        public string Hiredate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Postalcode { get; set; }
        public string Country { get; set; }
        public string Homephone { get; set; }
        public string Extension { get; set; }
        public string Photo { get; set; }
        public string Notes { get; set; }
        public string Reportsto { get; set; }
        public string Photopath { get; set; }
        
        
        public Employee(
            string employeeid, 
            string lastname, 
            string firstname, 
            string title, 
            string titleofcourtesy, 
            string birthdate, 
            string hiredate, 
            string address, 
            string city, 
            string region, 
            string postalcode, 
            string country, 
            string homephone, 
            string extension, 
            string photo, 
            string notes, 
            string reportsto, 
            string photopath)
        {
            Employeeid = employeeid;
            Lastname = lastname;
            Firstname = firstname;
            Title = title;
            Titleofcourtesy = titleofcourtesy;
            Birthdate = birthdate;
            Hiredate = hiredate;
            Address = address;
            City = city;
            Region = region;
            Postalcode = postalcode;
            Country = country;
            Homephone = homephone;
            Extension = extension;
            Photo = photo;
            Notes = notes;
            Reportsto = reportsto;
            Photopath = photopath;
        }
        
        public override string ToString()
        {
            return $"{Employeeid}: {Lastname}";
        }
    }
}