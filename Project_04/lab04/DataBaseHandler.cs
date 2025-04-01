namespace Lab04
{
    public class DataBaseHandler<T>
    {
	    
        public List<T> GetListOfObjects(string path, Func<string[], T> generate)
        {
            List<T> list = new List<T>();

            using (StreamReader sr = new StreamReader(path))
            {
	            sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null) {
					if (string.IsNullOrWhiteSpace(line)) {
						continue;
					}
					string[] fields = line.Split(',');

                	T item = generate(fields);
                	list.Add(item);
				}
            }
            
			return list;
        }
    }
}