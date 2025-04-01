namespace Lab04{
    public class OrderDetail{
        public string Orderid { get; set; }
        public string Productid { get; set; }
        public string Unitprice { get; set; }
        public string Quantity { get; set; }
        public string Discount { get; set; }

        public OrderDetail(string orderid, string productid, string unitprice, string quantity, string discount)
        {
            Orderid = orderid;
            Productid = productid;
            Unitprice = unitprice;
            Quantity = quantity;
            Discount = discount;
        }

        public override string ToString()
        {
            return $"Orderid: {Orderid}, Productid: {Productid}, Unitprice: {Unitprice}, Quantity: {Quantity}, Discount: {Discount}";
        }
    }
}