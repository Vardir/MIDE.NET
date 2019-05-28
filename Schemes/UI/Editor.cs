public void OpenTab(EditorTab tab, int order = -1)
public void CloseTab(string id)
public void CloseTab(EditorTab tab)

class EditorTab
{
	public string Id {get;set;}
	public ILayoutContents EditorContents {get;set;}
}