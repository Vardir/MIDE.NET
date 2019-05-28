public void OpenTab(OutputTab tab, int order = -1)
public void CloseTab(string id)
public void CloseTab(OutputTab tab)

class OutputTab
{
	public string Id {get;set;}
	public IOutputContents {get;set;}
}