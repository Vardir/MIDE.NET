public void OpenTab(ExplorerTab tab, int order = -1)
public void CloseTab(string id)
public void CloseTab(OutputTab tab)

class ExplorerTab
{
	public string Id {get;set;}
	public ILayoutComponent Child {get;set;}
}