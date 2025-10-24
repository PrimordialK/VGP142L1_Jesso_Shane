using System.Collections.Generic;
using UnityEngine;




public class MenuController : MonoBehaviour
{
    public BaseMenu[] allMenus;

    public MenuStates initState = MenuStates.MainMenu;

    public BaseMenu currentMenu => _currentMenu;
    private BaseMenu _currentMenu;

    Dictionary<MenuStates, BaseMenu> menuDictionary = new Dictionary<MenuStates, BaseMenu>();
    Stack<MenuStates> menuStack = new Stack<MenuStates>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (allMenus.Length <= 0)
        {
            allMenus = GetComponentsInChildren<BaseMenu>(true);
        }

        foreach (BaseMenu menu in allMenus)
        {
            if (menu == null) continue;
            menu.Init(this);

            if (menuDictionary.ContainsKey(menu.state)) continue;

            menuDictionary.Add(menu.state, menu);

        }
        JumpTo(initState);
    }

    // Update is called once per frame

    public void JumpBack()
    {
        if (menuStack.Count <= 0) return;

        menuStack.Pop();
        JumpTo(menuStack.Peek(), true);

    }
    public void JumpTo(MenuStates newState, bool fromJumpBack = false)
    {
        if (!menuDictionary.ContainsKey(newState))
        {
            Debug.LogWarning($"No menu found for state {newState}");
            return;
        }

        // do nothing if we are already in the requested menu
        if (_currentMenu == menuDictionary[newState]) return;


        if (_currentMenu != null)
        {
            _currentMenu.Exit();
            _currentMenu.gameObject.SetActive(false);
        }
        _currentMenu = menuDictionary[newState];
        _currentMenu.gameObject.SetActive(true);
        _currentMenu.Enter();

        if (!fromJumpBack)
        {
            if (menuStack.Contains(newState))
            {
                menuStack.Pop();
                return;
            }

        }
        menuStack.Push(newState);

    }
}
