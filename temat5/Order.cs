namespace Lab04
{
    public class Order
    {
        public string Orderid { get; set; }
        public string Customerid { get; set; }
        public string Employeeid { get; set; }
        public string Orderdate { get; set; }
        public string Requireddate { get; set; }
        public string Shippeddate { get; set; }
        public string Shipvia { get; set; }
        public string Freight { get; set; }
        public string Shipname { get; set; }
        public string Shipaddress { get; set; }
        public string Shipcity { get; set; }
        public string Shipregion { get; set; }
        public string Shippostalcode { get; set; }
        public string Shipcountry { get; set; }

        public Order(string orderid, string customerid, string employeeid, string orderdate,
                     string requireddate, string shippeddate, string shipvia, string freight,
                     string shipname, string shipaddress, string shipcity, string shipregion,
                     string shippostalcode, string shipcountry)
        {
            Orderid = orderid;
            Customerid = customerid;
            Employeeid = employeeid;
            Orderdate = orderdate;
            Requireddate = requireddate;
            Shippeddate = shippeddate;
            Shipvia = shipvia;
            Freight = freight;
            Shipname = shipname;
            Shipaddress = shipaddress;
            Shipcity = shipcity;
            Shipregion = shipregion;
            Shippostalcode = shippostalcode;
            Shipcountry = shipcountry;
        }

        public override string ToString()
        {
            return $"Orderid: {Orderid}, Customerid: {Customerid}, Employeeid: {Employeeid} ... ";
        }

    }
}