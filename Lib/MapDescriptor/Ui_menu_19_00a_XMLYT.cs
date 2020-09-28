using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace FSEditor.MapDescriptor
{
    public class Ui_menu_19_00a_XMLYT
    {

        public static string constructMapIconTplName(string mapIcon)
        {
            if (Path.GetFileNameWithoutExtension(mapIcon) == "bghatena")
            {
                throw new ArgumentException("The map icon name cannot be bghatena");
            }
            return "ui_menu007_" + Path.GetFileNameWithoutExtension(mapIcon) + ".tpl";
        }

        public static bool injectMapIcons(string xmlytFile, Dictionary<string, string> mapIconToTplName)
        {
            // check map icon tpl names for validity
            foreach (String tpl in mapIconToTplName.Values)
            {
                if (!tpl.StartsWith("ui_menu007_") || !tpl.EndsWith(".tpl") || tpl == "ui_menu007_bghatena.tpl")
                {
                    throw new ArgumentException("The list of map icon tpls must follow the format ui_menu007_<XYZ>.tpl");
                }
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlytFile);
            // --- write the txl1 section for all the references to .tpl files ---
            XmlNode tplEntriesNode = doc.SelectSingleNode("/xmlyt/tag[@type='txl1']/entries");
            XmlNodeList tplEntryNameList = doc.SelectNodes("/xmlyt/tag[@type='txl1']/entries/name");
            // delete all map icon entries
            foreach (XmlNode tplEntryName in tplEntryNameList)
            {
                if (tplEntryName.InnerText.StartsWith("ui_menu007_") && tplEntryName.InnerText != "ui_menu007_bghatena.tpl")
                {
                    tplEntriesNode.RemoveChild(tplEntryName);
                }
            }
            // create new map icon entries
            foreach (string tplName in mapIconToTplName.Values)
            {
                XmlNode newElem = doc.CreateNode("element", "name", "");
                newElem.InnerText = tplName;
                tplEntriesNode.AppendChild(newElem);
            }
            // --- write the mat1 section for all the references to .tpl files ---
            // delete all map icon material entries
            XmlNodeList materialNodesTexture = doc.SelectNodes("/xmlyt/tag[@type='mat1']/entries/texture");
            // get the first material entry which is a map icon material to serve as a template
            XmlNode templateMaterialNode = null;
            var nonMapIconMatNames = new List<string>();
            foreach (XmlNode materialNodeTexture in materialNodesTexture)
            {
                var materialNode = materialNodeTexture.ParentNode;
                var tplName = materialNodeTexture.Attributes.GetNamedItem("name").InnerText;
                if (tplName.StartsWith("ui_menu007_") && tplName != "ui_menu007_bghatena.tpl")
                {
                    if (templateMaterialNode == null)
                    {
                        templateMaterialNode = materialNodeTexture.ParentNode.Clone();
                    }
                    // delete this material entry
                    materialNode.ParentNode.RemoveChild(materialNode);
                }
                else
                {
                    // remember which material entries are not map icons
                    nonMapIconMatNames.Add(materialNode.Attributes.GetNamedItem("name").InnerText);
                }
            }
            // now build the new map icon materials again
            XmlNode matEntriesNode = doc.SelectSingleNode("/xmlyt/tag[@type='mat1']");
            foreach (var entry in mapIconToTplName)
            {
                string mapIcon = entry.Key;
                string tpl = entry.Value;
                XmlNode newElem = templateMaterialNode.Clone();
                newElem.Attributes.GetNamedItem("name").InnerText = mapIcon;
                newElem.SelectSingleNode("texture").Attributes.GetNamedItem("name").InnerText = tpl;
                matEntriesNode.PrependChild(newElem);
            }
            // --- write the pic1 section for all the layout settings ---
            // delete all pic tags
            XmlNodeList picNodesMaterial = doc.SelectNodes("/xmlyt/tag[@type='pic1']/material");
            // there are different pic tags, some of them are map icons others are decoration like the yellow selection mark
            // get the first pic tag which is a map icon to serve as a template
            XmlNode templatePicNode = null;
            // remember the position in the xml file where to insert the new map icons
            XmlNode insertionPoint = null;
            foreach (XmlNode picNodeMaterial in picNodesMaterial)
            {
                var picNode = picNodeMaterial.ParentNode;
                var matName = picNodeMaterial.Attributes.GetNamedItem("name").InnerText;
                // if the mat name is not any of those which is not a map icon then it must be a map icon... confusing double negative
                if (!nonMapIconMatNames.Contains(matName))
                {
                    if(insertionPoint == null)
                    {
                        insertionPoint = picNode.PreviousSibling;
                    }
                    if (templatePicNode == null)
                    {
                        templatePicNode = picNode.Clone();
                    }
                    // delete this map icon pic tag
                    picNode.ParentNode.RemoveChild(picNode);
                }
            }
            // now build the new map icon pics again
            foreach (var entry in mapIconToTplName)
            {
                string mapIcon = entry.Key;
                XmlNode newElem = templatePicNode.Clone();
                newElem.Attributes.GetNamedItem("name").InnerText = mapIcon;
                newElem.SelectSingleNode("material").Attributes.GetNamedItem("name").InnerText = mapIcon;
                insertionPoint.AppendChild(newElem);
            }
            doc.Save(xmlytFile);
            return true;
        }
    }
}
