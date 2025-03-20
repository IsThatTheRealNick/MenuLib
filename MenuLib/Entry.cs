using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace MenuLib
{
    [BepInPlugin("nickklmao.menulib", MOD_NAME, "1.0.5")]
    internal sealed class Entry : BaseUnityPlugin
    {
        private const string MOD_NAME = "Menu Lib";
        
        internal static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(MOD_NAME);
        
        private static void MenuPageMain_StartHook(Action<MenuPageMain> orig, MenuPageMain self)
        {
            orig.Invoke(self);
            MenuAPI.mainMenuBuilderDelegates?.Invoke(self.transform);
        }
        
        private static void MenuPageEsc_StartHook(Action<MenuPageEsc> orig, MenuPageEsc self)
        {
            orig.Invoke(self);
			MenuAPI.escapeMenuBuilderDelegates?.Invoke(self.transform);
        }

        private static void SemiFunc_UIMouseHoverILHook(ILContext il)
        {
            var cursor = new ILCursor(il);

            cursor.GotoNext(instruction => instruction.MatchBrfalse(out var label) && label.Target.OpCode == OpCodes.Ldarg_1);

            cursor.Index += 2;
            cursor.RemoveRange(27);

            cursor.Emit(OpCodes.Ldloc_0);
            cursor.EmitDelegate((MenuScrollBox menuScrollBox, Vector2 vector) =>
            {
                var mask = (RectTransform) menuScrollBox.scroller.parent;

                var bottom = mask.position.y;
                var top = bottom + mask.sizeDelta.y;

                return vector.y > bottom && vector.y < top;
            });

            var jumpToLabel = cursor.DefineLabel();

            cursor.Emit(OpCodes.Brtrue_S, jumpToLabel);
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret);

            cursor.MarkLabel(jumpToLabel);
        }
        
        private static void MenuScrollBox_StartILHook(ILContext il)
        {
            var cursor = new ILCursor(il);

            cursor.GotoNext(instruction => instruction.MatchLdfld<MenuScrollBox>("scrollHandle"));

            cursor.Index -= 2;

            cursor.RemoveRange(97);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate((MenuScrollBox menuScrollBox) =>
            {
                var mask = (RectTransform) menuScrollBox.scroller.parent;
                
                var minY = float.MaxValue;
                var maxY = float.MinValue;

                for (var i = 0; i < menuScrollBox.scroller.childCount; i++)
                {
                    if (i <= 2)
                        continue;
                
                    var child = menuScrollBox.scroller.GetChild(i) as RectTransform;
                
                    var corners = new Vector3[4];
                    child!.GetWorldCorners(corners);
                
                    for (var j = 0; j < 4; j++)
                        corners[j] = menuScrollBox.scroller.InverseTransformPoint(corners[j]);
                
                    var childMinY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
                    var childMaxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        
                    minY = Mathf.Min(minY, childMinY);
                    maxY = Mathf.Max(maxY, childMaxY);
                }

                var height = Math.Abs(maxY - minY);
            
                AccessTools.Field(typeof(MenuScrollBox), "scrollHeight").SetValue(menuScrollBox, height);
                AccessTools.Field(typeof(MenuScrollBox), "scrollerStartPosition").SetValue(menuScrollBox, height + 42f);
                AccessTools.Field(typeof(MenuScrollBox), "scrollerEndPosition").SetValue(menuScrollBox, menuScrollBox.scroller.localPosition.y);

                if (height < mask.sizeDelta.y)
                    menuScrollBox.scrollBar.SetActive(false);
                else
                {
                    var parentPage = AccessTools.Field(typeof(MenuScrollBox), "parentPage").GetValue(menuScrollBox);
                    
                    menuScrollBox.scrollBar.SetActive(true);
                
                    var scrollBoxesFieldInfo = AccessTools.Field(typeof(MenuPage), "scrollBoxes"); 
                    scrollBoxesFieldInfo.SetValue(parentPage, (int) scrollBoxesFieldInfo.GetValue(parentPage) + 1);
                }
            });
        }
        
        private void Awake()
        {
            logger.LogDebug("Hooking `MenuPageMain.Start`");
            new Hook(AccessTools.Method(typeof(MenuPageMain), "Start"), MenuPageMain_StartHook);
            
            logger.LogDebug("Hooking `MenuPageEsc.Start`");
            new Hook(AccessTools.Method(typeof(MenuPageEsc), "Start"), MenuPageEsc_StartHook);
            
            logger.LogDebug("Hooking `SemiFunc.UIMouseHover`");
            new ILHook(AccessTools.Method(typeof(SemiFunc), "UIMouseHover"), SemiFunc_UIMouseHoverILHook);
            
            //Requires a rewrite
            /*logger.LogDebug("Hooking `MenuScrollBox.Start`");
            new ILHook(AccessTools.Method(typeof(MenuScrollBox), "Start"), MenuScrollBox_StartILHook);*/
        }
    }
}