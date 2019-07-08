using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

/// <summary>
/// These static methods fix bugs related to the AjaxControlToolkit's ComboBox.
/// </summary>
public class ComboBoxFixer
{
/// <summary>
/// Use this when adding ACT Comboboxes that may be initially hidden when loaded.
/// It registers all comboboxes into a client-side array. If the user writes
/// scripts that make the ACT Comboboxes visible, they should call
/// ActComboBoxMadeVisible_All(), a client-side method.
/// </summary>
   public static void RegisterComboBox(AjaxControlToolkit.ComboBox pComboBox)
   {
      ClientScriptManager vClientScript = pComboBox.Page.ClientScript;
      if (!vClientScript.IsClientScriptBlockRegistered("ActComboBoxFixer"))
      {
         string vScript =
            "function ActComboBoxMadeVisible_All()\r\n" +
            "{\r\n" +
               "   if (window.gActComboboxes)\r\n" +
               "      for (var vI = 0; vI < window.gActComboboxes.length; vI++)\r\n" +
               "      {\r\n" +
               "         var vCB = $find(window.gActComboboxes[vI]);\r\n" +
               "         if (vCB)\r\n" +
               "         {\r\n" +
               "            ActComboBoxMadeVisible(vCB);\r\n" +
               "         }\r\n" +
               "      }\r\n" +  
            "}\r\n" +
            "function ActComboBoxMadeVisible(pCB)\r\n" +
            "{\r\n" +
               "  if (pCB && !pCB._optionListItemHeight)\r\n" +
               "  {\r\n" +
               "     var vBtn = pCB.get_buttonControl();\r\n" +
               "     vBtn.style.width = '';\r\n" +
               "     vBtn.style.height = '';\r\n" +
               "     pCB.initializeButton();\r\n" +
               "     pCB._optionListHeight = null;\r\n" +
               "     pCB._optionListWidth = null;\r\n" +
               "     pCB._optionListItemHeight = 21;\r\n" +
               "     pCB._getOptionListWidth();\r\n" +
               "     pCB._getOptionListHeight();\r\n" +
               "  }\r\n" +
            "}\r\n";
         vClientScript.RegisterClientScriptBlock(typeof(Page), "ActComboBoxFixer", vScript, true);
      }
      pComboBox.Page.ClientScript.RegisterArrayDeclaration("gActComboboxes", "'" + pComboBox.ClientID + "'");
   }

/// <summary>
/// Helps ModalDialogExtenders work with AjaxControlToolkit.ComboBox. Call for each ModalDialogExtender
/// that may have ComboBoxes. If you have more than one and they may appear simulateously, add them
/// in a specific order where the topmost one is added before those below it.
/// </summary>
/// <remarks>
/// <para>Requires each ComboBox is passed to ComboBoxFixer.RegisterComboBox.</para>
/// </remarks>
/// <param name="pModalExtender"></param>
   public static void RegisterModalPopupExtender(AjaxControlToolkit.ModalPopupExtender pModalExtender)
   {
      ClientScriptManager vClientScript = pModalExtender.Page.ClientScript;
      vClientScript.RegisterArrayDeclaration("gACTModalDEIDs", "'" + pModalExtender.ClientID + "'");

      if (!vClientScript.IsClientScriptBlockRegistered("ActComboBoxInMDE"))
      {
// The basic idea: Replace AjaxControlToolkit._popupShown with ActComboBoxInMDE_PopupShown.
// ActComboBoxInMDE_PopupShown is a clone of _popupShown, but inserts code to change x,y
// when a ModalDialogExtender is visible. 
// MDEs are registerd in the client-side array gACTModalDEIDs.
// This allows multiple MDEs and will only evaluate the first whose content element (called _foregroundElement)
// is visible. So if there are nested MDEs, register the topmost one first and bottommost last.
         string vScript =
         "function ActComboBoxInMDE_Init()\r\n" +
         "{\r\n" +
         "   if (window.gActComboboxes)\r\n" +
         "      for (var vI = 0; vI < gActComboboxes.length; vI++)\r\n" +
         "      {\r\n" +
         "         var vCB = $find(gActComboboxes[vI]);\r\n" +
         "         if (vCB.InitedMDE) continue;\r\n" +
         "         vCB._popupShown = ActComboBoxInMDE_PopupShown;\r\n" +
         "         vCB._popupShownHandlerFix = Function.createDelegate(vCB, ActComboBoxInMDE_PopupShown);\r\n" +
         "         vCB._popupBehavior.add_shown(vCB._popupShownHandlerFix);\r\n" +
         "         vCB.InitedMDE = 1;\r\n" +
         "      }\r\n" +
         "}\r\n" +
         "function ActComboBoxInMDE_PopupShown() {\r\n" +
         "\r\n" +
         "   this.get_optionListControl().style.display = 'block';\r\n" +
         "\r\n" +
         "   // check and enforce correct positioning.\r\n" +
         "   var tableBounds = Sys.UI.DomElement.getBounds(this.get_comboTableControl());\r\n" +
         "   var listBounds = Sys.UI.DomElement.getBounds(this.get_optionListControl());\r\n" +
         "   var textBoxBounds = Sys.UI.DomElement.getBounds(this.get_textBoxControl());\r\n" +
         "   var y = listBounds.y;\r\n" +
         "   var x;\r\n" +
         "\r\n" +
         "   if (this._popupBehavior.get_positioningMode() === AjaxControlToolkit.PositioningMode.BottomLeft\r\n" +
         "      || this._popupBehavior.get_positioningMode() === AjaxControlToolkit.PositioningMode.TopLeft) {\r\n" +
         "      x = textBoxBounds.x;\r\n" +
         "   }\r\n" +
         "   else if (this._popupBehavior.get_positioningMode() === AjaxControlToolkit.PositioningMode.BottomRight\r\n" +
         "      || this._popupBehavior.get_positioningMode() === AjaxControlToolkit.PositioningMode.TopRight) {\r\n" +
         "      x = textBoxBounds.x - (listBounds.width - textBoxBounds.width);\r\n" +
         "   }\r\n" +
         "\r\n" +
         "   if (window.gACTModalDEIDs)\r\n" +
         "      for (var vI = 0; vI < gACTModalDEIDs.length; vI++)\r\n" +
         "      {\r\n" +
         "         var vMDE = $find(gACTModalDEIDs[vI]);\r\n" +
         "         if (vMDE._foregroundElement.style.display == '')\r\n" +
         "         {\r\n" +
         "            var vMDBounds = Sys.UI.DomElement.getBounds(vMDE._foregroundElement);\r\n" +
         "            x = x - vMDBounds.x;\r\n" +
         "            y = (textBoxBounds.y + textBoxBounds.height) - vMDBounds.y;\r\n" +
         "            break;\r\n" +
         "         }\r\n" +
         "      }\r\n" +
         "   Sys.UI.DomElement.setLocation(this.get_optionListControl(), x, y);\r\n" +
         "\r\n" +
         "   // enforce default scroll\r\n" +
         "   this._ensureHighlightedIndex();\r\n" +
         "   this._ensureScrollTop();\r\n" +
         "\r\n" +
         "   // show the option list\r\n" +
         "   this.get_optionListControl().style.visibility = 'visible';\r\n" +
         "\r\n" +
         "}\r\n";
         vClientScript.RegisterClientScriptBlock(typeof(Page), "ActComboBoxInMDE", vScript, true);

         vClientScript.RegisterStartupScript(typeof(Page), "ActComboBoxInMDEInit",
            "Sys.Application.add_load(ActComboBoxInMDE_Init);\r\n", true);

         // on MDE popup, also fix comboboxes
         vScript =
         "function ActComboBoxInMDE_MDEPopupInit()\r\n" +
         "{\r\n" +
         "   if (window.gACTModalDEIDs)\r\n" +
         "      for (var vI = 0; vI < gACTModalDEIDs.length; vI++)\r\n" +
         "      {\r\n" +
         "         var vMD = $find(gACTModalDEIDs[vI]);\r\n" +
         "         vMD.add_shown(ActComboBoxInMDE_MDEPopupShown);\r\n" +
         "      }\r\n" +
         "}\r\n" +
         "function ActComboBoxInMDE_MDEPopupShown(sender, args)\r\n" +
         "{\r\n" +
         "   if (window.ActComboBoxMadeVisible_All)\r\n" +
         "      ActComboBoxMadeVisible_All();\r\n" +
         "}\r\n";
         vClientScript.RegisterClientScriptBlock(typeof(Page), "ActComboBoxInMDE_MDEPopupInitBlock", vScript, true);
         vClientScript.RegisterStartupScript(typeof(Page), "ActComboBoxInMDE_MDEPopupInit",
            "Sys.Application.add_load(ActComboBoxInMDE_MDEPopupInit);\r\n", true);

      } // if !ClientScript

   }  // RegisterModalPopupExtender
}