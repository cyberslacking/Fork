#region Using directives

using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Permissions;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

using GMap.NET;

#endregion

namespace MapToolkit
{
    /// <summary>
    /// List of graphic objects
    /// </summary>
    [Serializable]
    public class MapObjects : ISerializable
    {
        #region Members
        private List<DrawObject> graphicsList;
        private const string mapID = "ID";
        private const string mapName = "Name";
        private const string mapDataFile = "DataFile";
        private const string mapAccessMode = "AccessMode";
        private const string mapZoom = "Zoom";
        private const string mapMinZoom = "MinZoom";
        private const string mapMaxZoom = "MaxZoom";
        private const string mapLocalPosition = "LocalPostion";
        private const string mapProviders = "MaxProviders";

        private const string entryCount = "Count";
        private const string entryType = "Type";
        private PropertyMap property;    //µÿÕº Ù–‘
        #endregion

        #region Properties
        public EHObjectChanged ehObjectChanged = null;
        public EHLableValueChanged ehLableValueChanged = null;

        public PropertyMap Property
        {
            get { return property; }
        }

        #endregion

        #region Constructor
        public MapObjects()
        {
            property = new PropertyMap();
            property.ehLableValueChanged += new EHLableValueChanged(OnPropertyValueChanged);
            graphicsList = new List<DrawObject>();
        }

        #endregion

        #region Serialization Support

        protected MapObjects(SerializationInfo info, StreamingContext context)
        {
            property = new PropertyMap();
            property.ehLableValueChanged += new EHLableValueChanged(OnPropertyValueChanged);
            property.ID = (long)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}",
                mapID),
                typeof(long));

            property.Name = (string)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}",
                mapName),
                typeof(string));

            property.AccessMode = (AccessMode)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}",
                mapAccessMode),
                typeof(AccessMode));

            property.MinZoom = (int)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}",
                mapMinZoom),
                typeof(int));

            property.MaxZoom = (int)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}",
                mapMaxZoom),
                typeof(int));

            property.Zoom = (int)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}",
                mapZoom),
                typeof(int));

            property.LocalPosition = (PointLatLng)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}",
                mapLocalPosition),
                typeof(PointLatLng));

            property.Providers = (MAP_PROVIDERS)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}",
                mapProviders),
                typeof(MAP_PROVIDERS));


            graphicsList = new List<DrawObject>();

            int n = info.GetInt32(entryCount);
            string typeName;
            DrawObject o;

            for (int i = 0; i < n; i++)
            {
                typeName = info.GetString(
                    String.Format(CultureInfo.InvariantCulture,
                        "{0}{1}",
                    entryType, i));

                o = (DrawObject)Assembly.GetExecutingAssembly().CreateInstance(
                    typeName);

                o.LoadFromStream(info, i);
                graphicsList.Add(o);

            }

        }

        /// <summary>
        /// Save object to serialization stream
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(mapID, property.ID);
            info.AddValue(mapName, property.Name);
            info.AddValue(mapAccessMode, property.AccessMode);
            info.AddValue(mapZoom, property.Zoom);
            info.AddValue(mapMinZoom, property.MinZoom);
            info.AddValue(mapMaxZoom, property.MaxZoom);
            info.AddValue(mapLocalPosition, property.LocalPosition);
            info.AddValue(mapProviders, property.Providers);
            info.AddValue(entryCount, graphicsList.Count);
            int i = 0;
            foreach (DrawObject o in graphicsList)
            {
                info.AddValue(
                    String.Format(CultureInfo.InvariantCulture,
                        "{0}{1}",
                        entryType, i),
                    o.GetType().FullName);
                o.SaveToStream(info, i);
                i++;
            }
        }

        #endregion

        #region Map property event
        private void OnPropertyValueChanged(string lable, object value)
        {
            ehLableValueChanged(lable,value);
        }
        #endregion

        #region Objects list functions

        public void Draw(Graphics g)
        {
            int n = graphicsList.Count;
            DrawObject o;

            // Enumerate list in reverse order to get first
            // object on the top of Z-order.
            for (int i = n - 1; i >= 0; i--)
            {
                o = graphicsList[i];
                //o.Draw(g);
                o.DrawTracker(g);
            }
        }

        /// <summary>
        /// Dump (for debugging)
        /// </summary>
        public void Dump()
        {
            foreach ( DrawObject o in graphicsList )
            {
                o.Dump();
            }
        }

        /// <summary>
        /// Clear all objects in the list
        /// </summary>
        /// <returns>
        /// true if at least one object is deleted
        /// </returns>
        public void Clear()
        {
            int n = graphicsList.Count;
            for (int i = n - 1; i >= 0; i--)
            {
                graphicsList[i].Clear();
            }
            graphicsList.Clear();
        }

        /// <summary>
        /// Count and this [nIndex] allow to read all graphics objects
        /// from GraphicsList in the loop.
        /// </summary>
        public int Count
        {
            get
            {
                return graphicsList.Count;
            }
        }

        public DrawObject this[int index]
        {
            get
            {
                if (index < 0 || index >= graphicsList.Count)
                    return null;

                return graphicsList[index];
            }
        }


        /// <summary>
        /// SelectedCount and GetSelectedObject allow to read
        /// selected objects in the loop
        /// </summary>
        public int SelectionCount
        {
            get
            {
                int n = 0;
                foreach (DrawObject o in Selection)
                {
                    n++;
                }
                return n;
            }
        }

        /// <summary>
        /// Returns INumerable object which may be used for enumeration
        /// of selected objects.
        /// 
        /// Note: returning IEnumerable<DrawObject> breaks CLS-compliance
        /// (assembly CLSCompliant = true is removed from AssemblyInfo.cs).
        /// To make this program CLS-compliant, replace 
        /// IEnumerable<DrawObject> with IEnumerable. This requires
        /// casting to object at runtime.
        /// </summary>
        /// <value></value>
        public IEnumerable<DrawObject> Selection
        {
            get
            {
                foreach (DrawObject o in graphicsList)
                {
                    if (o.Selected)
                    {
                        yield return o;
                    }
                }
            }
        }

        public void Add(DrawObject obj)
        {
            // insert to the top of z-order
            Insert(0, obj);
        }

        public void Remove(DrawObject obj)
        {
            if (ehObjectChanged != null)
            {
                ehObjectChanged(obj, OBJ_OPERATE.remove);
            }
            obj.Clear();
            graphicsList.Remove(obj);
        }

        public void Sort()
        {
            graphicsList.Sort(delegate(DrawObject x, DrawObject y)
            {
                return x.GetProperty().Name.CompareTo(y.GetProperty().Name);
            });
        }

        public DrawObject Find(int id)
        {
            return graphicsList.Find(x => x.GetProperty().ID == id);
        }

        /// <summary>
        /// Insert object to specified place.
        /// Used for Undo.
        /// </summary>
        public void Insert(int index, DrawObject o)
        {
            if (index >= 0 && o != null)
            {
                o.Show();
                graphicsList.Insert(index, o);
                if (ehObjectChanged != null)
                {
                    ehObjectChanged(o, OBJ_OPERATE.insert);
                }
            }
        }

        /// <summary>
        /// Replace object in specified place.
        /// Used for Undo.
        /// </summary>
        public void Replace(int index, DrawObject o)
        {
            if (index >= 0 && index < graphicsList.Count)
            {
                RemoveAt(index);
                Insert(index, o);
            }
        }

        /// <summary>
        /// Remove object by index.
        /// Used for Undo.
        /// </summary>
        public void RemoveAt(int index)
        {
            DrawObject o = graphicsList[index];
            o.Clear();
            if (ehObjectChanged != null)
            {
                ehObjectChanged(o, OBJ_OPERATE.remove);
            }
            graphicsList.RemoveAt(index);
        }

        /// <summary>
        /// Delete last added object from the list
        /// (used for Undo operation).
        /// </summary>
        public void DeleteLastAddedObject()
        {
            if ( graphicsList.Count > 0 )
            {
                RemoveAt(0);
            }
        }

        public void SelectInRectangle(Rectangle rectangle)
        {
            UnselectAll();

            foreach (DrawObject o in graphicsList)
            {
                if (o.IntersectsWith(rectangle))
                    o.Selected = true;
            }

        }

        public void UnselectAll()
        {
            foreach (DrawObject o in graphicsList)
            {
                o.Selected = false;
            }
        }

        public void SelectAll()
        {
            foreach (DrawObject o in graphicsList)
            {
                o.Selected = true;
            }
        }

        /// <summary>
        /// Delete selected items
        /// </summary>
        /// <returns>
        /// true if at least one object is deleted
        /// </returns>
        public bool DeleteSelection()
        {
            bool result = false;

            int n = graphicsList.Count;

            for (int i = n - 1; i >= 0; i--)
            {
                if (((DrawObject)graphicsList[i]).Selected)
                {
                    RemoveAt(i);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Move selected items to front (beginning of the list)
        /// </summary>
        /// <returns>
        /// true if at least one object is moved
        /// </returns>
        public bool MoveSelectionToFront()
        {
            int n;
            int i;
            List<DrawObject> tempList;

            tempList = new List<DrawObject>();
            n = graphicsList.Count;

            // Read source list in reverse order, add every selected item
            // to temporary list and remove it from source list
            for (i = n - 1; i >= 0; i--)
            {
                if ((graphicsList[i]).Selected)
                {
                    tempList.Add(graphicsList[i]);
                    graphicsList.RemoveAt(i);
                }
            }

            // Read temporary list in direct order and insert every item
            // to the beginning of the source list
            n = tempList.Count;

            for (i = 0; i < n; i++)
            {
                graphicsList.Insert(0, tempList[i]);
            }

            return (n > 0);
        }

        /// <summary>
        /// Move selected items to back (end of the list)
        /// </summary>
        /// <returns>
        /// true if at least one object is moved
        /// </returns>
        public bool MoveSelectionToBack()
        {
            int n;
            int i;
            List<DrawObject> tempList;

            tempList = new List<DrawObject>();
            n = graphicsList.Count;

            // Read source list in reverse order, add every selected item
            // to temporary list and remove it from source list
            for (i = n - 1; i >= 0; i--)
            {
                if ((graphicsList[i]).Selected)
                {
                    tempList.Add(graphicsList[i]);
                    graphicsList.RemoveAt(i);
                }
            }

            // Read temporary list in reverse order and add every item
            // to the end of the source list
            n = tempList.Count;

            for (i = n - 1; i >= 0; i--)
            {
                graphicsList.Add(tempList[i]);
            }

            return (n > 0);
        }

        #endregion
    }
}
