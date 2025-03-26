namespace Lab04
{
    public class Region
    {
        public string Regionid { get; set; }
        public string Regiondescription { get; set; }
        
        public Region(string regionid, string regiondescription)
        {
            Regionid = regionid;
            Regiondescription = regiondescription;
        }
        
        
        public override string ToString()
        {
            return $"{Regionid}, {Regiondescription}";
        }
    }
}