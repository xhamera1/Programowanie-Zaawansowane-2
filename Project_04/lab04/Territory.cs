namespace Lab04
{
    public class Territory{
        public string Territoryid { get; set; }
        public string Territorydescription { get; set; }
        public string Regionid { get; set; }

        public Territory(string tid, string td, string rid)
        {
            Territoryid = tid;
            Territorydescription = td;
            Regionid = rid;
        }
        
        public override string ToString()
        {
            return $"{Territoryid}: {Territorydescription} (Region: {Regionid})";
        }
    }
}