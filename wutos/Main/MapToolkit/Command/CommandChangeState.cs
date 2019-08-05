using System;
using System.Collections.Generic;
using System.Text;

namespace MapToolkit
{
    /// <summary>
    /// Changing state of existing objects:
    /// move, resize, change properties.
    /// </summary>
    class CommandChangeState : Command
    {
        // Selected object(s) before operation
        public List<DrawObject> listBefore;

        // Selected object(s) after operation
        public List<DrawObject> listAfter;
        

        // Create this command BEFORE operation.
        public CommandChangeState(MapObjects graphicsList)
        {
            // Keep objects state before operation.
            FillList(graphicsList, ref listBefore);
        }

        // Call this function AFTER operation.
        public void NewState(MapObjects graphicsList)
        {
            // Keep objects state after operation.
            FillList(graphicsList, ref listAfter);
        }

        public override void Undo(MapObjects list)
        {
            // Replace all objects in the list with objects from listBefore
            ReplaceObjects(list, listBefore);
        }

        public override void Redo(MapObjects list)
        {
            // Replace all objects in the list with objects from listAfter
            ReplaceObjects(list, listAfter);
        }

        // Replace objects in graphicsList with objects from list
        private void ReplaceObjects(MapObjects graphicsList, List<DrawObject> list)
        {
            for ( int i = 0; i < graphicsList.Count; i++ )
            {
                DrawObject replacement = null;

                foreach(DrawObject o in list)
                {
                    if (o.GetProperty().ID == graphicsList[i].GetProperty().ID)
                    {
                        replacement = o;
                        break;
                    }
                }

                if ( replacement != null )
                {
                    graphicsList.Replace(i, replacement);
                }
            }
        }

        // Fill list from selection
        private void FillList(MapObjects graphicsList, ref List<DrawObject> listToFill)
        {
            listToFill = new List<DrawObject>();

            foreach (DrawObject o in graphicsList.Selection)
            {
                listToFill.Add(o.Clone());
            }
        }
    }
}
