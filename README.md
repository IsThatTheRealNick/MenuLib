# Menu Lib
A library for creating UI!

As REPOConfig gets updated, so will this library.

## For Developers - VERSION 2.0.0
You can reference the [REPOConfig GitHub](https://github.com/IsThatTheRealNick/REPOConfig).  
Official documentation will come later (sorry), but here's a super quick code snippet:

```cs
MenuAPI.AddElementToMainMenu(parent =>
{
	//`parent` in this scenario represents the MainMenu
	
	//Buttons
	var repoButton = MenuAPI.CreateREPOButton("A Button", () => Debug.Log("I was clicked!"), parent, Vector2.zero);
	
	//Labels
	var repoLabel = MenuAPI.CreateREPOLabel("A Label", parent, new Vector2(48.3f, 55.5f));
	
	//Toggles
	var repoToggle = MenuAPI.CreateREPOToggle("A Toggle", b => Debug.Log($"I was switched to: {b}"), parent, Vector2.zero, "Left Button Text", "Right Button Text", defaultValue: true);
	
	//Sliders
	//The precision argument/field is the number of decimals you want (0 for int)
	//The bar behavior argument/field is for the background bar visual, it doesn't affect functionality
	//The rest should be self-explanatory
	
	//Float Slider
	var repoFloatSlider = MenuAPI.CreateREPOSlider("Float Slider", "Description", f => Debug.Log($"New Float Value: {f}"), parent, Vector2.zero, -100f, 100f, 2, 50f, "prefix-", "-postfix", REPOSlider.BarBehavior.UpdateWithValue);

	//Int Slider (No precision argument)
	var repoIntSliderSlider = MenuAPI.CreateREPOSlider("Int Slider", "Description", i => Debug.Log($"New Int Value: {i}"), parent, Vector2.zero, -100, 100, 50, "prefix-", "-postfix", REPOSlider.BarBehavior.UpdateWithValue);
	
	//String Option Slider - Alternatively, you can use an int delegate -----------------> (int i) => Debug.Log($"New String Index Value: {i}")
	var repoStringSlider = MenuAPI.CreateREPOSlider("String Option Slider", "Description", (string s) => Debug.Log($"New String Value: {s}"), parent, ["Option A", "Option B", "Option C"], "a", Vector2.zero, "prefix-", "-postfix", REPOSlider.BarBehavior.UpdateWithValue);
	
	//Popup Page - These should be created and opened immediately (usually on a button press), trying to cache them will cause issues
	var repoPage = MenuAPI.CreateREPOPopupPage("Page Header", REPOPopupPage.PresetSide.Left, pageDimmerVisibility: true, spacing: 1.5f);
	
	//Popup Page Custom Position
	var repoPage = MenuAPI.CreateREPOPopupPage("Page Header", true, 1.5f, Vector2.zero);
	
	//Opens page exclusively, the previous page will be inactive
	repoPage.OpenPage(false);
	
	//Opens page inclusively, the previous page will still be active
	repoPage.OpenPage(true);
	
	//Closes page exclusively, only this page will close
	repoPage.ClosePage(false);
	
	//Closes this page + all pages added on top
	repoPage.ClosePage(true);
	
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
	var repoButton = MenuAPI.CreateREPOButton("A Button", () => Debug.Log("I was clicked!"), scrollView, Vector2.zero);

	var scrollViewElement = repoButton.repoScrollViewElement;
	
	//Sets space above this element when positioned
	scrollViewElement.topPadding = 50;
	
	//Sets space below this element when positioned, typically for the next element
	scrollViewElement.bottomPadding = 50;

	//To dynamically hide/show elements, you need to toggle this field
	scrollViewElement.visibility = false;
});
```
 