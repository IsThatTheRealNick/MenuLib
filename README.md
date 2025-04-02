# Menu Lib
A library for creating UI!

As REPOConfig gets updated, so will this library.

## For Developers - VERSION 2.x.x
You can reference the [REPOConfig GitHub](https://github.com/IsThatTheRealNick/REPOConfig).  
Official documentation will come later (sorry), but here's a super quick code snippet:

```cs
MenuAPI.AddElementToMainMenu(parent =>
{
	//`parent` in this scenario represents the MainMenu
	
	//Buttons
	var repoButton = MenuAPI.CreateREPOButton("A Button", () => Debug.Log("I was clicked!"), parent, localPosition: Vector2.zero);
	
	//Labels
	var repoLabel = MenuAPI.CreateREPOLabel("A Label", parent, localPosition: new Vector2(48.3f, 55.5f));
	
	//Toggles
	var repoToggle = MenuAPI.CreateREPOToggle("A Toggle", b => Debug.Log($"I was switched to: {b}"), parent, Vector2.zero, "Left Button Text", "Right Button Text", defaultValue: true);
	
	//Avatar Previews
	var repoAvatarPreview = MenuAPI.CreateREPOAvatarPreview(parent, new Vector2(48.3f, 55.5f), enableBackgroundImage: true, backgroundImageColor: Color.white);
	
	//Sliders
	//The precision argument/field is the number of decimals you want (0 = integers, 1 = 0.1, 2 = 0.01, etc.)
	//The bar behavior argument/field is for the background bar visual, it doesn't affect functionality
	//The rest should be self-explanatory
	
	//Float Slider
	var repoFloatSlider = MenuAPI.CreateREPOSlider("Float Slider", "Description", f => Debug.Log($"New Float Value: {f}"), parent, localPosition: Vector2.zero, min: -100f, max: 100f, precision: 2, defaultValue: 50f, "prefix-", "-postfix", REPOSlider.BarBehavior.UpdateWithValue);

	//Int Slider (No precision argument)
	var repoIntSliderSlider = MenuAPI.CreateREPOSlider("Int Slider", "Description", i => Debug.Log($"New Int Value: {i}"), parent, localPosition: Vector2.zero, min: -100, max: 100, defaultValue: 50, "prefix-", "-postfix", REPOSlider.BarBehavior.UpdateWithValue);
	
	//String Option Slider - Alternatively, you can use an int delegate -----------------> (int i) => Debug.Log($"New String Index Value: {i}")
	var repoStringSlider = MenuAPI.CreateREPOSlider("String Option Slider", "Description", (string s) => Debug.Log($"New String Value: {s}"), parent, stringOptions: ["Option A", "Option B", "Option C"], defaultOption: "a", localPosition: Vector2.zero, "prefix-", "-postfix", REPOSlider.BarBehavior.UpdateWithValue);
	
	//Popup Page
	//If caching is disabled then the page should be created on a button's press
	//If caching is enabled then you should assign it to a field and only create the page if the fields null, otherwise menus will duplicate over time
	var repoPage = MenuAPI.CreateREPOPopupPage("Page Header", REPOPopupPage.PresetSide.Left, shouldCachePage: false, pageDimmerVisibility: true, spacing: 1.5f);
	
	//Popup Page Custom Position
	var repoPage = MenuAPI.CreateREPOPopupPage("Page Header", shouldCachePage: false, pageDimmerVisibility: true, spacing: 1.5f, localPosition: Vector2.zero);
	
	//Opens the page
	//openOnTop:
	//If true, the previous page will not be set to inactive
	//If false, the previous page will be set to inactive
	repoPage.OpenPage(openOnTop: false);
	
	//Closes this page
	//closePagesAddedOnTop:
	//If true, all pages added on top will close too
	//If false, only this page will close
	repoPage.ClosePage(closePagesAddedOnTop: false);
	
	//Sets the padding for the scroll box mask
	repoPage.maskPadding = new Padding(left: 0, top: 0, right: 0, bottom: 0);
	
	//Adds an element to the page
	repoPage.AddElement(parent =>
	{
		//Create element, parent it using `parent`
	});
	
	//Adds an element to the page's scroll box
	repoPage.AddElementToScrollView(scrollView =>
	{
		//Create element, parent it using `scrollView`
		//Setting the Y position of an element in here is useless, it will be overwritten
		//Additionally, this delegate requires a RectTransform to be returned:
		
		//return newlyCreatedElement.rectTransform;
	});
	
	//Each element has access to its scroll view element, it will be null if it wasn't parented to a scroll box
	var repoButton = MenuAPI.CreateREPOButton("A Button", () => Debug.Log("I was clicked!"), parent: scrollView, localPosition: Vector2.zero);

	var scrollViewElement = repoButton.repoScrollViewElement;
	
	//Sets space above this element when positioned
	scrollViewElement.topPadding = 50;
	
	//Sets space below this element when positioned, typically for the next element
	scrollViewElement.bottomPadding = 50;

	//To dynamically hide/show elements, you need to toggle this field
	scrollViewElement.visibility = false;
});
```
 