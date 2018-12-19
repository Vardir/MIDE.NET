public void OpenTab(ViewerTab tab, int order = -1)
public void CloseTab(string id)
public void CloseTab(ViewerTab tab)

class ViewerTab
{
	public string Id {get;set;}
	public ILayoutContents ViewerContents {get;set;}
}