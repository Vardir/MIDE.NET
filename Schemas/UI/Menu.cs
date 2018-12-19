public void AddItem(string path, MenuItem item, int order = -1)
public void AddItem(string path, string category, MenuItem item, int order = -1)
public void RemoveItem(string path, string id)
public void RemoveItem(MenuItem item)

class MenuItem
{
	public string Id {get;set;}
}