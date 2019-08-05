using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;


using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace MapToolkit
{
    public partial class MapControl : GMapControl
    {
        #region Constructor
        public MapControl(Form owner)
        {
            InitializeComponent();
            this.owner = owner;
            InitMap();
            InitTool();
            InitDoc();

            
            Global.control = this;
        }

        public MapControl(Form owner, ToolStrip strip)
        {
            InitializeComponent();
            this.owner = owner;
            InitMap();
            InitTool();
            InitDoc();
            Global.control = this;

            // Create undo manager            
            undoManager = new UndoManager(objects);

            this.toolStripEdit = strip;
            // Select active tool
            this.toolStripButtonPointer = toolStripEdit.Items[0];
            this.toolStripButtonCamera = toolStripEdit.Items[10];
            this.toolStripButtonDefence = toolStripEdit.Items[9];
            this.toolStripButtonMarker = toolStripEdit.Items[3];
            this.toolStripButtonMarker2 = toolStripEdit.Items[4];
            this.toolStripButtonMarker3 = toolStripEdit.Items[5];
            this.toolStripButtonMarker4 = toolStripEdit.Items[6];
            this.toolStripButtonEllipse = toolStripEdit.Items[12];
            this.toolStripButtonPolygon = toolStripEdit.Items[13];
            this.toolStripButtonText = toolStripEdit.Items[14];
            this.toolStripButtonImage = toolStripEdit.Items[15];
            this.toolStripButtonDelete = toolStripEdit.Items[18];
            this.toolStripButtonUndo = toolStripEdit.Items[16];
            this.toolStripButtonRedo = toolStripEdit.Items[17];
            toolStripTextBoxKey = toolStripEdit.Items[20];
            toolStripButtonSearch = toolStripEdit.Items[21];
            toolStripButtonModel = toolStripEdit.Items[8];

            this.toolStripButtonPointer.Click += new System.EventHandler(this.toolStripButtonPointer_Click);
            this.toolStripButtonCamera.Click += new System.EventHandler(this.toolStripButtonCamera_Click);
            this.toolStripButtonDefence.Click += new System.EventHandler(this.toolStripButtonDefence_Click);
            this.toolStripButtonPolygon.Click += new System.EventHandler(this.toolStripButtonPolygon_Click);
            this.toolStripButtonEllipse.Click += new System.EventHandler(this.toolStripButtonEllipse_Click);
            this.toolStripButtonText.Click += new System.EventHandler(this.toolStripButtonText_Click);
            this.toolStripButtonImage.Click += new System.EventHandler(this.toolStripButtonImage_Click);
            this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
            this.toolStripButtonRedo.Click += new System.EventHandler(this.toolStripButtonRedo_Click);
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            this.toolStripButtonSearch.Click += new System.EventHandler(this.toolStripButtonSearch_Click);
            this.toolStripButtonMarker.Click += new System.EventHandler(this.toolStripButtonMarker_Click);
            this.toolStripButtonMarker2.Click += new System.EventHandler(this.toolStripButtonMarker2_Click);
            this.toolStripButtonMarker3.Click += new System.EventHandler(this.toolStripButtonMarker3_Click);
            this.toolStripButtonMarker4.Click += new System.EventHandler(this.toolStripButtonMarker3_Click);
            this.toolStripButtonModel.Click += new System.EventHandler(this.toolStripButtonModel_Click);
        }

        #endregion Constructor

        private ToolStrip toolStripEdit;
        private ToolStripItem toolStripButtonPointer;
        private ToolStripItem toolStripButtonCamera;
        private ToolStripItem toolStripButtonDefence;
        private ToolStripItem toolStripButtonMarker;
        private ToolStripItem toolStripButtonMarker2;
        private ToolStripItem toolStripButtonMarker3;
        private ToolStripItem toolStripButtonMarker4;
        private ToolStripItem toolStripButtonEllipse;
        private ToolStripItem toolStripButtonPolygon;
        private ToolStripItem toolStripButtonText;
        private ToolStripItem toolStripButtonImage;
        private ToolStripItem toolStripButtonDelete;
        private ToolStripItem toolStripButtonUndo;
        private ToolStripItem toolStripButtonRedo;
        private ToolStripItem toolStripTextBoxKey;
        private ToolStripItem toolStripButtonSearch;
        private ToolStripItem toolStripButtonModel;



        #region Private Members

        private List<DrawObject> graphicsList;

        // (instances of DrawObject-derived classes)
        private Form owner;
        private TOOL_TYPE activeTool;      // active drawing tool
        private MARKER_TYPE markerType = MARKER_TYPE.Green;
        private Tool[] tools;                 // array of tools
        private bool selected;

        private Point ptScreen;    //
        private bool wasMove;

        // Information about owner form
        private UndoManager undoManager;
        //
        private MAP_MODE mode = MAP_MODE.none;

        private volatile bool isShowWarningMarker = true;

        #endregion


        private bool isLeftButtonDown;

        #region 测距

        //路径图层,实线
        public GMapOverlay routeOverlay = new GMapOverlay("routeOverlay");
        //临时路径,虚线
        public GMapOverlay tempOverlay = new GMapOverlay("tempOverlay");
        //标注图层
        public GMapOverlay labelOverlay = new GMapOverlay("labelOverlay");
        //正在画的路径
        private GMapRoute drawingRoute = null;
        ////当前路径
        //private GMapRoute currentRoute;
        //路径的点集
        private List<PointLatLng> drawingPoints = new List<PointLatLng>();

        #endregion

        #region 圆圈 传感器分布

        public GMapOverlay sensorCircleOverlay = new GMapOverlay("sensorCircle");

        #endregion

        #region Properties

        public ConfigMap config = null;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (value)
                {
                    ObjectSelected(config);
                }
                selected = value;
            }
        }

        /// <summary>
        /// Active drawing tool.
        /// </summary>
        public TOOL_TYPE ActiveTool
        {
            get
            {
                return activeTool;
            }
            set
            {
                activeTool = value;
            }
        }

        public MARKER_TYPE MarkerType
        {
            get
            {
                return this.markerType;
            }
        }

        /// <summary>
        /// List of graphics tools.
        /// </summary>
        public Tool[] Tools
        {
            get
            {
                return tools;
            }
            set
            {
                tools = value;
            }
        }

        /// <summary>
        /// Return True if Undo operation is possible
        /// </summary>
        public bool CanUndo
        {
            get
            {
                if (undoManager != null)
                {
                    return undoManager.CanUndo;
                }
                return false;
            }
        }

        /// <summary>
        /// Return True if Redo operation is possible
        /// </summary>
        public bool CanRedo
        {
            get
            {
                if (undoManager != null)
                {
                    return undoManager.CanRedo;
                }

                return false;
            }
        }

        #endregion

        #region Public Members
        public event EHZoomChanged ehZoomChanged;
        public event EHObjectSelected ehObjectSelected;
        public event EHPropertyChanged ehPropertyChanged;
        public event EHMouseRightClick ehMouseRightClick;
        public event EHDrawPolylineMouseEnter ehDrawPolylineMouseEnter;
        public event EHDrawPolylineMouseClick ehDrawPolylineMouseClick;
        public event EHGMapMarkerMouseClick ehGMapMarkerMouseClick;

        #endregion

        #region Private Functions
        private void InitDoc()
        {
            using (var io = System.IO.File.OpenRead(Global.configFile))
            {
                try
                {
                    config = ProtoBuf.Serializer.Deserialize<ConfigMap>(io);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("初始化配置文件失败：{0}", ex.Message));
                }
            }
            /*
            ClearHistory();
            Refresh();
            */
        }

        private void InitTool()
        {
            // create array of drawing tools
            tools = new Tool[(int)TOOL_TYPE.numberOfObjects];
            tools[(int)TOOL_TYPE.pointer] = new ToolPointer();
            tools[(int)TOOL_TYPE.rectangle] = new ToolRectangle();
            tools[(int)TOOL_TYPE.ellipse] = new ToolEllipse();
            tools[(int)TOOL_TYPE.marker] = new ToolMarker();
            tools[(int)TOOL_TYPE.defence] = new ToolDefence();
            tools[(int)TOOL_TYPE.camera] = new ToolCamera();
            tools[(int)TOOL_TYPE.polygon] = new ToolPolygon();
            tools[(int)TOOL_TYPE.text] = new ToolText();
            tools[(int)TOOL_TYPE.image] = new ToolImage();
            tools[(int)TOOL_TYPE.model] = new ToolModel();
            // set default tool
            activeTool = TOOL_TYPE.pointer;

            // Submit to Idle event to set controls state at idle time
            Application.Idle += delegate (object o, EventArgs a)
            {
                SetMapStateOfControls();
            };
        }

        /// <summary>
        /// 初始化MAP配置文件
        /// </summary>
        /// <returns></returns>
        private void InitMap()
        {       
            ShowCenter = false;
            Dock = DockStyle.Fill;
            DragButton = MouseButtons.Left;
            GMapOverlay Objects = new GMapOverlay("Objects");
            Overlays.Add(Objects);
        }

        /// <summary>
        /// 根据枚举配置服务商
        /// </summary>
        private void SetMapProvider(MAP_PROVIDERS provider)
        {
            switch (provider)
            {
                case MAP_PROVIDERS.GoogleChinaMap:
                    MapProvider = GMapProviders.GoogleChinaMap;
                    break;
                case MAP_PROVIDERS.GoogleTerrainMap:
                    MapProvider = GMapProviders.GoogleTerrainMap;
                    break;
                case MAP_PROVIDERS.GoogleChinaSatelliteMap:
                    MapProvider = GMapProviders.GoogleChinaSatelliteMap;
                    break;
                case MAP_PROVIDERS.GoogleChinaHybridMap:
                    MapProvider = GMapProviders.GoogleChinaHybridMap;
                    break;
                case MAP_PROVIDERS.BingMap:
                    MapProvider = GMapProviders.BingMap;
                    break;
                case MAP_PROVIDERS.BingSatelliteMap:
                    MapProvider = GMapProviders.BingSatelliteMap;
                    break;
                case MAP_PROVIDERS.OpenStreetMap:
                    MapProvider = GMapProviders.OpenStreetMap;
                    break;
                default:
                    MapProvider = GMapProviders.GoogleChinaMap;
                    break;
            }
        }

        /// <summary>
        /// Right-click handler
        /// </summary>
        /// <param name="e"></param>
        private void OnContextMenu(MouseEventArgs e)
        {
            // Change current selection if necessary

            Point point = new Point(e.X, e.Y);
            DrawObject obj = null;
            foreach(var o in graphicsList )
            {
                int t = o.HitTest(point);
                if (t == 0)
                {
                    obj = o;
                    break;
                }
                else if (t > 0)
                {

                }
            }

            if (obj != null)
            {
                if (!obj.Selected)
                    UnselectAll();

                // Select clicked object
                obj.Selected = true;
            }

            SetStateOfControls();

            Refresh();      // in the case selection was changed
            ptScreen = point;
            contextMenuStripOperate.Show(PointToScreen(point));

            return;
        }

        /// <summary>
        /// 根据图层显示\隐藏对象
        /// </summary>
        private void ObjectsVisible(int zoom)
        {
            foreach(var o in graphicsList)
            {
                o.IsZoomVisible(zoom);
                o.Normalize();
            }
        }

        #endregion

        #region Public Functions (EX)

        #region Method
        public void Save()
        {
            using (var io = System.IO.File.Create(Global.configFile))
            {
                ProtoBuf.Serializer.Serialize(io, config);
            }
        }


        public void UnselectAll()
        {
            foreach (var o in graphicsList)
            {
                o.Selected = false;
            }
        }

        /// <summary>
        /// 中心显示
        /// </summary>
        /// <param name="id"></param>
        public void SetCenter()
        {
            Position = config.localPosition;
            Zoom = config.defaultZoom;
        }

        /// <summary>
        /// 对象中心显示
        /// </summary>
        /// <param name="id"></param>
        public void SetObjectCenter(DrawObject o)
        {
            UnselectAll();
            if (mode == MAP_MODE.edit)
            {
                o.Selected = true;
            }
            Position = o.GetProperty().LocalPosition;
            Zoom = o.GetProperty().DefaultZoom;
            ObjectSelected(o);
        }

        /// <summary>
        /// 删除选中对象
        /// </summary>
        public void DeleteSelected()
        {
            CommandDelete command = new CommandDelete(Objects);
            if (DeleteSelection())
            {
                AddCommandToHistory(command);
                SetDirty();
            }
        }

        /// <summary>
        /// 删除所有对象
        /// </summary>
        public void DeleteAll()
        {
            CommandDeleteAll command = new CommandDeleteAll(graphicsList);
            AddCommandToHistory(command);
            graphicsList.Clear();
            SetDirty();
        }

        public bool SearchByKeywords(string key)
        {
            try
            {
                GeoCoderStatusCode code = this.SetPositionByKeywords(key);
                if (code == GeoCoderStatusCode.G_GEO_SUCCESS)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 下载地图数据
        /// </summary>
        public void Prefetch()
        {
            RectLatLng area = SelectedArea;
            if (!area.IsEmpty)
            {
                for (int i = (int)Zoom; i <= MaxZoom; i++)
                {
                    DialogResult res = MessageBox.Show("Ready ripp at Zoom = " + i + " ?", "GMap.NET", MessageBoxButtons.YesNoCancel);

                    if (res == DialogResult.Yes)
                    {
                        using (TilePrefetcher obj = new TilePrefetcher())
                        {
                            obj.Overlay = Overlays[0]; // set overlay if you want to see cache progress on the map
                            obj.Shuffle = Manager.Mode != AccessMode.CacheOnly;
                            obj.Owner = owner;
                            obj.ShowCompleteMessage = true;
                            obj.Start(area, i, MapProvider, Manager.Mode == AccessMode.CacheOnly ? 0 : 100,
                                Manager.Mode == AccessMode.CacheOnly ? 0 : 1);
                        }
                    }
                    else if (res == DialogResult.No)
                    {
                        continue;
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "Wutos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// 设置模式
        /// </summary>
        public void SetEditMode(MAP_MODE m)
        {
            if (m == MAP_MODE.none)
            {
                UnselectAll();
                config.defaultZoom = (int)Zoom;
                config.localPosition = Position;
                DragButton = System.Windows.Forms.MouseButtons.Left;
                toolStripEdit.Visible = false;
                docManager.SaveDocument(DocManager.SaveType.Save);
                ShowCenter = false;
            }
            else
            {
                DragButton = System.Windows.Forms.MouseButtons.Right;
                toolStripEdit.Visible = true;
                ShowCenter = true;
            }
            mode = m;
        }

        public MAP_MODE GetEditMode()
        {
            return mode;
        }

        /// <summary>
        /// Set dirty flag (file is changed after last save operation)
        /// </summary>
        public void SetDirty()
        {
            docManager.Dirty = true;
        }

        #endregion

        #endregion

        #region Public Functions (IN)
        /// <summary>
        /// Add command to history.
        /// </summary>
        public void AddCommandToHistory(Command command)
        {
            undoManager.AddCommandToHistory(command);
        }

        /// <summary>
        /// Clear Undo history.
        /// </summary>
        public void ClearHistory()
        {
            undoManager.ClearHistory();
        }

        public void ObjectSelected(ConfigBase p)
        {
            if (ehObjectSelected != null)
            {
                ehObjectSelected(p);
            }
        }

        public void PropertyChanged(ConfigBase p)
        {
            if (ehPropertyChanged != null)
            {
                ehPropertyChanged(p);
            }
        }

        public void Add(DrawObject o)
        {
            UnselectAll();
            o.Selected = true;
            graphicsList.Add(o);
        }

        public void Remove(DrawObject o)
        {
            graphicsList.Remove(o);
        }

        #endregion

        #region Event Handlers

        #region DocManager Event Handlers
        /// <summary>
        /// Load document from the stream supplied by DocManager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void docManager_LoadEvent(object sender, SerializationEventArgs e)
        {
            // DocManager asks to load document from supplied stream
            try
            {
                if (objects != null)
                {
                    objects.Clear();
                }
                objects = (MapObjects)e.Formatter.Deserialize(e.SerializationStream);
                undoManager = new UndoManager(objects);

                SetMapProvider(objects.Property.Providers);

                Position = objects.Property.LocalPosition;
                MaxZoom = objects.Property.MaxZoom;
                MinZoom = objects.Property.MinZoom;
                CacheLocation = AppDomain.CurrentDomain.BaseDirectory + @"MapData";
                Zoom = objects.Property.Zoom;
                Manager.Mode = objects.Property.AccessMode;
                objects.ehLableValueChanged += new EHLableValueChanged(OnLableValueChanged);
                objects.Property.IsLoad = true;

                if (objects.Property.AccessMode != AccessMode.ServerOnly)
                {
                    MapManagerLoader.Instance.Load(objects.Property.MapFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loading map configuration file failed，Error：" + ex.Message);
            }
        }


        /// <summary>
        /// Save document to stream supplied by DocManager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void docManager_SaveEvent(object sender, SerializationEventArgs e)
        {
            // DocManager asks to save document to supplied stream
            try
            {
                MapManagerLoader.Instance.Save(objects.Property.MapFile);
                e.Formatter.Serialize(e.SerializationStream, objects);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save map configuration file failed，Error：" + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 属性变更
        /// </summary>
        private void OnLableValueChanged(string lable, object value)
        {
            switch (lable)
            {
                case "MaxZoom":
                    MaxZoom = (Int32)value;
                    break;
                case "MinZoom":
                    MinZoom = (Int32)value;
                    break;
                case "MapFile":
                    MapManagerLoader.Instance.Load((string)value);
                    ReloadMap();
                    break;
                case "AccessMode":
                    Manager.Mode = (AccessMode)value;
                    break;
                case "Providers":
                    SetMapProvider((MAP_PROVIDERS)value);
                    ReloadMap();
                    break;
            }
            PropertyChanged(objects.Property);
        }

        /// <summary>
        /// 多选属性变更
        /// </summary>
        private void OnPropertyObjectsChanged(string lable, object value)
        {
            foreach (var o in objects.Selection)
            {
                switch (lable)
                {
                    case "Name":
                        o.GetProperty().Name = (string)value;
                        break;
                    case "DefaultZoom":
                        o.GetProperty().DefaultZoom = (Int32)value;
                        break;
                    case "MinZoom":
                        o.GetProperty().MinZoom = (Int32)value;
                        break;
                    case "MaxZoom":
                        o.GetProperty().MaxZoom = (Int32)value;
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawArea_Paint(object sender, PaintEventArgs e)
        {
            //SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            //e.Graphics.FillRectangle(brush,this.ClientRectangle);

            if (objects != null)
            {
                objects.Draw(e.Graphics);
            }

            ////DrawNetSelection(e.Graphics);

            //brush.Dispose();
        }


        /// <summary>
        /// Mouse down.
        /// Left button down event is passed to active tool.
        /// Right button down event is handled in this class.
        /// </summary>
        private void DrawArea_MouseDown(object sender, MouseEventArgs e)
        {
            wasMove = false;
            if (e.Button == MouseButtons.Left)
            {
                isLeftButtonDown = true;

                #region MyRegion

                //if (isMesuring)
                //{
                //    drawingPoints.Add(this.FromLocalToLatLng(e.X, e.Y));
                //    if (drawingRoute == null)
                //    {
                //        drawingRoute = new GMapRoute(drawingPoints, "routeForMeasure");
                //        drawingRoute.Stroke = new Pen(Color.Coral, 2);

                //        drawingRoute.IsHitTestVisible = true;
                //        routeOverlay.Routes.Add(drawingRoute);

                //        PointLatLng startPt = drawingPoints[0];
                //        GMapMarker startMarker = new GMarkerGoogle(startPt, GMarkerGoogleType.orange_small);
                //        startMarker.ToolTipMode = MarkerTooltipMode.Always;
                //        startMarker.ToolTipText = string.Format("{0}", "起点", new Font("Arial", 3));
                //        labelOverlay.Markers.Add(startMarker);
                //    }
                //    else
                //    {
                //        drawingRoute.Points.Clear();
                //        drawingRoute.Points.AddRange(drawingPoints);
                //        if (routeOverlay.Polygons.Count == 0)
                //        {
                //            routeOverlay.Routes.Add(drawingRoute);
                //        }
                //        else
                //        {
                //            UpdateRouteLocalPosition(drawingRoute);
                //        }

                //        PointLatLng p = drawingPoints[drawingPoints.Count - 1];
                //        GMapMarker m1 = new GMarkerGoogle(p, GMarkerGoogleType.orange_small);

                //        double totalDis = GetTotalDistance(drawingPoints);
                //        string disInfo = GetDistanceInfo(totalDis);

                //        m1.ToolTipMode = MarkerTooltipMode.Always;
                //        m1.ToolTipText = string.Format("{0}", disInfo, new Font("Arial", 3));
                //        labelOverlay.Markers.Add(m1);
                //    }
                //}
                //else if (mode == MAP_MODE.edit)
                //{
                //    tools[(int)activeTool].OnMouseDown(this, e, (int)Zoom);
                //}

                #endregion

                if (mode == MAP_MODE.edit)
                {
                    tools[(int)activeTool].OnMouseDown(this, e, (int)Zoom);
                }
                if (isMesuring)
                {
                    drawingPoints.Add(this.FromLocalToLatLng(e.X, e.Y));
                    if (drawingRoute == null)
                    {
                        drawingRoute = new GMapRoute(drawingPoints, "routeForMeasure");
                        drawingRoute.Stroke = new Pen(Color.Coral, 2);

                        drawingRoute.IsHitTestVisible = true;
                        routeOverlay.Routes.Add(drawingRoute);

                        PointLatLng startPt = drawingPoints[0];
                        GMapMarker startMarker = new GMarkerGoogle(startPt, GMarkerGoogleType.orange_small);
                        startMarker.ToolTipMode = MarkerTooltipMode.Always;
                        startMarker.ToolTipText = string.Format("{0}", "起点", new Font("Arial", 3));
                        labelOverlay.Markers.Add(startMarker);
                    }
                    else
                    {
                        drawingRoute.Points.Clear();
                        drawingRoute.Points.AddRange(drawingPoints);
                        if (routeOverlay.Polygons.Count == 0)
                        {
                            routeOverlay.Routes.Add(drawingRoute);
                        }
                        else
                        {
                            UpdateRouteLocalPosition(drawingRoute);
                        }

                        PointLatLng p = drawingPoints[drawingPoints.Count - 1];
                        GMapMarker m1 = new GMarkerGoogle(p, GMarkerGoogleType.orange_small);

                        double totalDis = GetTotalDistance(drawingPoints);
                        string disInfo = GetDistanceInfo(totalDis);

                        m1.ToolTipMode = MarkerTooltipMode.Always;
                        m1.ToolTipText = string.Format("{0}", disInfo, new Font("Arial", 3));
                        labelOverlay.Markers.Add(m1);
                    }
                }
            }

        }

        /// <summary>
        /// Mouse move.
        /// Moving without button pressed or with left button pressed
        /// is passed to active tool.
        /// </summary>
        private void DrawArea_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.None)
            {

                if (mode == MAP_MODE.edit)
                {
                    tools[(int)activeTool].OnMouseMove(this, e);
                }

                if (isMesuring)
                {
                    if (drawingPoints.Count > 0)
                    {
                        PointLatLng ptCurrent = this.FromLocalToLatLng(e.X, e.Y);
                        PointLatLng prePoint = drawingPoints[drawingPoints.Count - 1];

                        GMapRoute tempRoute = new GMapRoute(new List<PointLatLng>() { prePoint, ptCurrent }, "tempRoute");

                        tempRoute.Stroke = new Pen(Color.BurlyWood, 2);

                        tempRoute.IsHitTestVisible = true;

                        if (tempOverlay.Polygons.Count == 0)
                        {
                            if (tempOverlay.Routes.Count != 0)
                            {
                                tempOverlay.Routes.Clear();
                            }
                            tempOverlay.Routes.Add(tempRoute);
                        }
                        else
                        {
                            UpdateRouteLocalPosition(tempRoute);
                        }
                        Refresh();
                    }
                }

            }
            else if (e.Button == MouseButtons.Right && this.Cursor == Cursors.SizeAll)
            {
                wasMove = true;
            }
        }

        /// <summary>
        /// Mouse up event.
        /// Left button up event is passed to active tool.
        /// </summary>
        private void DrawArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (mode != MAP_MODE.edit)
                {
                    return;
                }

                tools[(int)activeTool].OnMouseUp(this, e);
            }
            else if (e.Button == MouseButtons.Right && !wasMove)
            {
                if (mode == MAP_MODE.none)
                {
                    ehMouseRightClick(e.X, e.Y);
                }
                else
                {
                    OnContextMenu(e);
                }
            }
        }

        private void DrawArea_OnMapDrag()
        {
            objects.Property.LocalPosition = Position;
            ObjectSelected(objects.Property);
        }

        private void DrawArea_OnMapZoomChanged()
        {
            ObjectsVisible((int)Zoom);
            objects.Property.Zoom = (int)Zoom;
            if (ehZoomChanged != null)
            {
                ehZoomChanged((int)Zoom);
            }
        }

        private void DrawArea_OnRouteEnter(GMapRoute item)
        {
            int n = Objects.Count;
            for (int i = 0; i < n; i++)
            {
                Objects[i].IsHit = false;
                if (Objects[i] is DrawPolyline)
                {
                    DrawPolyline o = Objects[i] as DrawPolyline;
                    if (o.route == item)
                    {
                        o.IsHit = true;

                        if (this.ehDrawPolylineMouseEnter != null)
                        {
                            Point p = this.PointToClient(MousePosition);
                            this.ehDrawPolylineMouseEnter(o, p.X, p.Y);
                        }
                    }
                }
            }
        }

        private void DrawArea_OnRouteDoubleClick(GMapRoute item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            int n = Objects.Count;

            for (int i = 0; i < n; i++)
            {
                if (Objects[i] is DrawPolyline)
                {
                    DrawPolyline o = Objects[i] as DrawPolyline;

                    if (o.route == item)
                    {
                        if (this.ehDrawPolylineMouseClick != null)
                        {
                            Point p = this.PointToClient(MousePosition);
                            this.ehDrawPolylineMouseClick(o, p.X, p.Y);
                        }
                    }
                }
            }
        }

        private void DrawArea_OnRouteLeave(GMapRoute item)
        {
            this.HideToolTip();
            int n = Objects.Count;
            for (int i = 0; i < n; i++)
            {
                if (Objects[i] is DrawPolyline)
                {
                    DrawPolyline o = Objects[i] as DrawPolyline;
                    if (o.route == item)
                    {
                        o.IsHit = false;
                    }
                }
            }
        }

        private void DrawArea_OnMarkerEnter(GMapMarker item)
        {
            int n = Objects.Count;
            for (int i = 0; i < n; i++)
            {
                Objects[i].IsHit = false;
                switch (Objects[i].GetType().Name)
                {
                    case "DrawMarker":
                        DrawMarker marker = Objects[i] as DrawMarker;
                        if (marker.marker == item)
                        {
                            marker.IsHit = true;
                        }
                        break;
                    case "DrawEllipse":
                        DrawEllipse ellipse = Objects[i] as DrawEllipse;
                        if (ellipse.marker == item)
                        {
                            ellipse.IsHit = true;
                        }
                        break;
                    case "DrawText":
                        DrawText text = Objects[i] as DrawText;
                        if (text.marker == item)
                        {
                            text.IsHit = true;
                        }
                        break;
                    case "DrawImage":
                        DrawImage image = Objects[i] as DrawImage;
                        if (image.marker == item)
                        {
                            image.IsHit = true;
                        }
                        break;
                        /*
                    case "DrawPolyline":
                        DrawPolyline line = Objects[i] as DrawPolyline;
                        foreach (KeyValuePair<string, LinkObject> node in line.mapLink)
                        {
                            if (node.Value.marker.marker == item)
                            {
                                node.Value.marker.IsHit = true;
                            }
                        }
                        break;
                        */
                }
            }
        }

        private void DrawArea_OnMarkerLeave(GMapMarker item)
        {
            int n = Objects.Count;
            for (int i = 0; i < n; i++)
            {
                switch (Objects[i].GetType().Name)
                {
                    case "DrawMarker":
                        DrawMarker marker = Objects[i] as DrawMarker;
                        if (marker.marker == item)
                        {
                            marker.IsHit = false;
                        }
                        break;
                    case "DrawEllipse":
                        DrawEllipse ellipse = Objects[i] as DrawEllipse;
                        if (ellipse.marker == item)
                        {
                            ellipse.IsHit = false;
                        }
                        break;
                    case "DrawText":
                        DrawText text = Objects[i] as DrawText;
                        if (text.marker == item)
                        {
                            text.IsHit = false;
                        }
                        break;
                    case "DrawImage":
                        DrawImage image = Objects[i] as DrawImage;
                        if (image.marker == item)
                        {
                            image.IsHit = false;
                        }
                        break;
                        /*
                    case "DrawPolyline":
                        DrawPolyline line = Objects[i] as DrawPolyline;
                        foreach (KeyValuePair<string, LinkObject> node in line.mapLink)
                        {
                            if (node.Value.marker.marker == item)
                            {
                                node.Value.marker.IsHit = false;
                            }
                        }
                        break;
                         */
                }
            }
        }

        private void DrawArea_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            int n = Objects.Count;
            for (int i = 0; i < n; i++)
            {
                if (Objects[i].GetType().Name == "DrawPolyline")
                {
                    DrawPolyline line = Objects[i] as DrawPolyline;

                    if (line == null || line.MapMarker == null)
                    {
                        return;
                    }

                    object obj = null;

                    foreach (KeyValuePair<object, GMapMarker> node in line.MapMarker)
                    {
                        if (node.Value == item)
                        {
                            obj = node.Key;
                            break;
                        }
                    }

                    if (this.ehGMapMarkerMouseClick != null && obj != null)
                    {
                        this.ehGMapMarkerMouseClick(line, obj, item, e.Button);
                        break;
                    }
                }
            }
        }

        private void DrawArea_OnPolygonEnter(GMapPolygon item)
        {
            int n = Objects.Count;
            for (int i = 0; i < n; i++)
            {
                Objects[i].IsHit = false;
                if (Objects[i] is DrawPolygon)
                {
                    DrawPolygon o = Objects[i] as DrawPolygon;
                    if (o.polygon == item)
                    {
                        o.IsHit = true;
                    }
                }
            }
        }

        private void DrawArea_OnPolygonLeave(GMapPolygon item)
        {
            int n = Objects.Count;
            for (int i = 0; i < n; i++)
            {
                if (Objects[i] is DrawPolygon)
                {
                    DrawPolygon o = Objects[i] as DrawPolygon;
                    if (o.polygon == item)
                    {
                        o.IsHit = false;
                    }
                }
            }
        }
        #endregion Event Handlers

        #region Menu Handlers
        private void SetStateOfControls()
        {
            bool selectedObjects = (objects.SelectionCount > 0);
            bool canAddTracker = false;
            bool canDelTracker = false;
            if (objects.SelectionCount == 1)
            {
                foreach (DrawObject o in objects.Selection)
                {
                    switch (o.GetType().Name)
                    {
                        case "DrawPolyline":
                        case "DrawPolygon":
                            if (o.Tracker > 0)
                            {
                                canDelTracker = true;
                            }
                            else
                            {
                                canAddTracker = true;
                            }
                            break;
                    }
                }
            }

            ToolStripMenuItemSelectAll.Enabled = (objects.Count > 0);
            ToolStripMenuItemDelete.Enabled = selectedObjects;
            ToolStripMenuItemFront.Enabled = selectedObjects;
            ToolStripMenuItemBack.Enabled = selectedObjects;
            ToolStripMenuItemAddTracker.Enabled = canAddTracker;
            ToolStripMenuItemDeleteTracker.Enabled = canDelTracker;
        }

        private void ToolStripMenuItemSelectAll_Click(object sender, EventArgs e)
        {
            objects.SelectAll();
            Refresh();
        }

        private void ToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            CommandDelete command = new CommandDelete(objects);
            if (objects.DeleteSelection())
            {
                AddCommandToHistory(command);
                SetDirty();
                Refresh();
            }
        }

        private void ToolStripMenuItemAddTracker_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                int n = objects[i].HitTest(ptScreen);
                if (n == 0)
                {
                    objects[i].InsertPoint(ptScreen);
                    SetDirty();
                    Refresh();
                    break;
                }
            }
        }

        private void ToolStripMenuItemDeleteTracker_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                int n = objects[i].HitTest(ptScreen);
                if (n > 0)
                {
                    objects[i].DeletePoint(n - 1);
                    SetDirty();
                    Refresh();
                    break;
                }
            }
        }

        private void ToolStripMenuItemFront_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].Selected)
                {
                    switch (objects[i].GetType().Name)
                    {
                        case "DrawPolyline":
                            DrawPolyline polyline = Objects[i] as DrawPolyline;
                            polyline.Overlay = TransformOverlayFront(polyline.route);
                            break;
                        case "DrawPolygon":
                            DrawPolygon polygon = Objects[i] as DrawPolygon;
                            polygon.Overlay = TransformOverlayFront(polygon.polygon);
                            break;
                        case "DrawMarker":
                            DrawMarker marker = Objects[i] as DrawMarker;
                            marker.Overlay = TransformOverlayFront(marker.marker);
                            break;
                        case "DrawEllipse":
                            DrawEllipse ellipse = Objects[i] as DrawEllipse;
                            ellipse.Overlay = TransformOverlayFront(ellipse.marker);
                            break;
                        case "DrawText":
                            DrawText text = Objects[i] as DrawText;
                            text.Overlay = TransformOverlayFront(text.marker);
                            break;
                        case "DrawImage":
                            DrawImage image = Objects[i] as DrawImage;
                            image.Overlay = TransformOverlayFront(image.marker);
                            break;
                    }
                    SetDirty();
                    Refresh();
                }
            }
            CheckOverlay();
        }

        private void ToolStripMenuItemBack_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].Selected)
                {
                    switch (objects[i].GetType().Name)
                    {
                        case "DrawPolyline":
                            DrawPolyline polyline = Objects[i] as DrawPolyline;
                            TransformOverlayBack(polyline.route);
                            break;
                        case "DrawPolygon":
                            DrawPolygon polygon = Objects[i] as DrawPolygon;
                            TransformOverlayBack(polygon.polygon);
                            break;
                        case "DrawMarker":
                            DrawMarker marker = Objects[i] as DrawMarker;
                            TransformOverlayBack(marker.marker);
                            break;
                        case "DrawEllipse":
                            DrawEllipse ellipse = Objects[i] as DrawEllipse;
                            TransformOverlayBack(ellipse.marker);
                            break;
                        case "DrawText":
                            DrawText text = Objects[i] as DrawText;
                            TransformOverlayBack(text.marker);
                            break;
                        case "DrawImage":
                            DrawImage image = Objects[i] as DrawImage;
                            TransformOverlayBack(image.marker);
                            break;
                    }
                    SetDirty();
                    Refresh();
                }
            }
            CheckOverlay();
        }

        private void CheckOverlay()
        {
            int count = 0, n = Overlays.Count;
            List<GMapOverlay> listRemove = new List<GMapOverlay>();
            for (int i = 0; i < n; i++)
            {
                count = Overlays[i].Markers.Count + Overlays[i].Routes.Count + Overlays[i].Polygons.Count;
                if (count == 0)
                {
                    listRemove.Add(Overlays[i]);
                }
            }
            foreach (GMapOverlay overlay in listRemove)
            {
                Overlays.Remove(overlay);
            }
        }

        private int TransformOverlayFront(GMapMarker marker)
        {
            int currentOverlay = 0;
            int n = Overlays.Count;
            List<GMapOverlay> listAdd = new List<GMapOverlay>();
            for (int i = 0; i < n; i++)
            {
                if (Overlays[i].Markers.Contains(marker))
                {
                    Overlays[i].Markers.Remove(marker);
                    if (i + 1 < Overlays.Count)
                    {
                        Overlays[i + 1].Markers.Add(marker);
                    }
                    else
                    {
                        Overlays[i].Markers.Remove(marker);
                        GMapOverlay Objects = new GMapOverlay("Objects");
                        Objects.Markers.Add(marker);
                        listAdd.Add(Objects);
                    }
                    currentOverlay = i + 1;
                }
            }

            foreach (GMapOverlay overlay in listAdd)
            {
                Overlays.Add(overlay);
            }
            return currentOverlay;
        }

        private int TransformOverlayFront(GMapRoute route)
        {
            int currentOverlay = 0;
            int n = Overlays.Count;
            List<GMapOverlay> listAdd = new List<GMapOverlay>();
            for (int i = 0; i < n; i++)
            {
                if (Overlays[i].Routes.Contains(route))
                {
                    Overlays[i].Routes.Remove(route);
                    if (i + 1 < Overlays.Count)
                    {
                        Overlays[i + 1].Routes.Add(route);
                    }
                    else
                    {
                        Overlays[i].Routes.Remove(route);
                        GMapOverlay Objects = new GMapOverlay("Objects");
                        Objects.Routes.Add(route);
                        listAdd.Add(Objects);
                    }
                    currentOverlay = i + 1;
                }
            }

            foreach (GMapOverlay overlay in listAdd)
            {
                Overlays.Add(overlay);
            }
            return currentOverlay;
        }

        private int TransformOverlayFront(GMapPolygon polygon)
        {
            int currentOverlay = 0;
            int n = Overlays.Count;
            List<GMapOverlay> listAdd = new List<GMapOverlay>();
            for (int i = 0; i < n; i++)
            {
                if (Overlays[i].Polygons.Contains(polygon))
                {
                    Overlays[i].Polygons.Remove(polygon);
                    if (i + 1 < Overlays.Count)
                    {
                        Overlays[i + 1].Polygons.Add(polygon);
                    }
                    else
                    {
                        Overlays[i].Polygons.Remove(polygon);
                        GMapOverlay Objects = new GMapOverlay("Objects");
                        Objects.Polygons.Add(polygon);
                        listAdd.Add(Objects);
                    }
                    currentOverlay = i + 1;
                }
            }

            foreach (GMapOverlay overlay in listAdd)
            {
                Overlays.Add(overlay);
            }
            return currentOverlay;
        }

        private int TransformOverlayBack(GMapMarker marker)
        {
            int currentOverlay = 0;
            int n = Overlays.Count;
            List<GMapOverlay> listAdd = new List<GMapOverlay>();
            for (int i = 0; i < n; i++)
            {
                if (Overlays[i].Markers.Contains(marker))
                {
                    if (i > 0)
                    {
                        Overlays[i].Markers.Remove(marker);
                        Overlays[i - 1].Markers.Add(marker);
                        currentOverlay = i - 1;
                    }
                    else
                    {
                        Overlays[i].Markers.Remove(marker);
                        GMapOverlay Objects = new GMapOverlay("Objects");
                        Objects.Markers.Add(marker);
                        listAdd.Add(Objects);
                    }
                }
            }
            foreach (GMapOverlay overlay in listAdd)
            {
                Overlays.Insert(0, overlay);
            }
            return currentOverlay;
        }

        private int TransformOverlayBack(GMapRoute route)
        {
            int currentOverlay = 0;
            int n = Overlays.Count;
            List<GMapOverlay> listAdd = new List<GMapOverlay>();
            for (int i = 0; i < n; i++)
            {
                if (Overlays[i].Routes.Contains(route))
                {
                    if (i > 0)
                    {
                        Overlays[i].Routes.Remove(route);
                        Overlays[i - 1].Routes.Add(route);
                        currentOverlay = i - 1;
                    }
                    else
                    {
                        Overlays[i].Routes.Remove(route);
                        GMapOverlay Objects = new GMapOverlay("Objects");
                        Objects.Routes.Add(route);
                        listAdd.Add(Objects);
                    }
                }
            }
            foreach (GMapOverlay overlay in listAdd)
            {
                Overlays.Insert(0, overlay);
            }
            return currentOverlay;
        }

        private int TransformOverlayBack(GMapPolygon polygon)
        {
            int currentOverlay = 0;
            int n = Overlays.Count;
            List<GMapOverlay> listAdd = new List<GMapOverlay>();
            for (int i = 0; i < n; i++)
            {
                if (Overlays[i].Polygons.Contains(polygon))
                {
                    if (i > 0)
                    {
                        Overlays[i].Polygons.Remove(polygon);
                        Overlays[i - 1].Polygons.Add(polygon);
                        currentOverlay = i - 1;
                    }
                    else
                    {
                        Overlays[i].Polygons.Remove(polygon);
                        GMapOverlay Objects = new GMapOverlay("Objects");
                        Objects.Polygons.Add(polygon);
                        listAdd.Add(Objects);
                    }
                }
            }
            foreach (GMapOverlay overlay in listAdd)
            {
                Overlays.Insert(0, overlay);
            }
            return currentOverlay;
        }

        private void ToolStripMenuItemPaste_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripMenuItemCopy_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Tool Handlers
        public void SetMapStateOfControls()
        {
            // Select active tool
            ((ToolStripButton)toolStripButtonPointer).Checked = (ActiveTool == TOOL_TYPE.pointer);
            ((ToolStripButton)toolStripButtonCamera).Checked = (ActiveTool == TOOL_TYPE.camera);
            ((ToolStripButton)toolStripButtonDefence).Checked = (ActiveTool == TOOL_TYPE.defence);
            ((ToolStripButton)toolStripButtonMarker).Checked = (ActiveTool == TOOL_TYPE.marker && MarkerType == MARKER_TYPE.Green);
            ((ToolStripButton)toolStripButtonMarker2).Checked = (ActiveTool == TOOL_TYPE.marker && MarkerType == MARKER_TYPE.Yellow);
            ((ToolStripButton)toolStripButtonMarker3).Checked = (ActiveTool == TOOL_TYPE.marker && MarkerType == MARKER_TYPE.Red);
            ((ToolStripButton)toolStripButtonMarker4).Checked = (ActiveTool == TOOL_TYPE.marker && MarkerType == MARKER_TYPE.Gray);
            ((ToolStripButton)toolStripButtonEllipse).Checked = (ActiveTool == TOOL_TYPE.ellipse);
            ((ToolStripButton)toolStripButtonPolygon).Checked = (ActiveTool == TOOL_TYPE.polygon);
            ((ToolStripButton)toolStripButtonText).Checked = (ActiveTool == TOOL_TYPE.text);
            ((ToolStripButton)toolStripButtonImage).Checked = (ActiveTool == TOOL_TYPE.image);

            bool selectedObjects = (objects.SelectionCount > 0);

            // File operations
            toolStripButtonDelete.Enabled = selectedObjects;

            // Undo, Redo
            toolStripButtonUndo.Enabled = CanUndo;
            toolStripButtonRedo.Enabled = CanRedo;


        }

        private void toolStripButtonPointer_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.pointer;
        }

        private void toolStripButtonStation_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.marker;
            markerType = MARKER_TYPE.Green;
        }

        private void toolStripButtonStation2_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.marker;
            markerType = MARKER_TYPE.Yellow;
        }

        private void toolStripButtonStation3_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.marker;
            markerType = MARKER_TYPE.Red;
        }

        private void toolStripButtonStation4_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.marker;
            markerType = MARKER_TYPE.Gray;
        }

        private void toolStripButtonDefence_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.defence;
        }

        private void toolStripButtonCamera_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.camera;
        }

        private void toolStripButtonPolygon_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.polygon;
        }

        private void toolStripButtonEllipse_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.ellipse;
        }

        private void toolStripButtonRectangle_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.rectangle;
        }

        private void toolStripButtonText_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.text;
        }

        private void toolStripButtonImage_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.image;
        }

        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            undoManager.Undo();
        }

        private void toolStripButtonRedo_Click(object sender, EventArgs e)
        {
            undoManager.Redo();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        private void toolStripButtonDownload_Click(object sender, EventArgs e)
        {
            Prefetch();
        }

        private static bool IsFloat(string str)
        {
            string regextext = @"^(-?\d+)(\.\d+)?$";
            Regex regex = new Regex(regextext, RegexOptions.None);
            return regex.IsMatch(str.Trim());
        }

        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            string[] strLaglng = toolStripTextBoxKey.Text.Split(',');
            if (strLaglng.Length == 2)
            {
                if (IsFloat(strLaglng[0]) && IsFloat(strLaglng[1]))
                {
                    double[] laglng = new double[2];
                    GpsCorrect.transform(double.Parse(strLaglng[0]), double.Parse(strLaglng[1]), laglng);
                    Position = new PointLatLng(laglng[0], laglng[1]);
                }
                else
                {
                    MessageBox.Show("经纬度格式不正确!");
                }
            }
            else
            {
                if (!SearchByKeywords(toolStripTextBoxKey.Text))
                {
                    MessageBox.Show("查找失败!");
                }
            }
        }
        #endregion

        private void MapControl_Load(object sender, EventArgs e)
        {
            ObjectsVisible((int)Zoom);
        }


        #region Keyboard Functions
        protected override bool ProcessDialogKey(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Delete:
                    DeleteSelected();
                    break;
                case Keys.Up:
                    Position = new PointLatLng(Position.Lat + 0.0005, Position.Lng);
                    break;
                case Keys.Down:
                    Position = new PointLatLng(Position.Lat - 0.0005, Position.Lng);
                    break;
                case Keys.Right:
                    Position = new PointLatLng(Position.Lat, Position.Lng + 0.0005);
                    break;
                case Keys.Left:
                    Position = new PointLatLng(Position.Lat, Position.Lng - 0.0005);
                    break;
            }
            return true;
        }
        #endregion Keyboard Functions

        private void toolStripTextBoxKey_TextChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(toolStripTextBoxKey.Text);
        }

        public void ShowToolTip(string message, int x, int y)
        {           
            toolTip.Show(message, this, x, y + 20);
        }

        public void HideToolTip()
        {           
            toolTip.Hide(this);
        }


        /// <summary>
        /// 是否处于测距状态
        /// </summary>
        private bool isMesuring;

        /// <summary>
        /// 测距
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsCalcDistance_Click(object sender, EventArgs e)
        {
            isMesuring = true;

            routeOverlay.Routes.Clear();
            labelOverlay.Markers.Clear();
            tempOverlay.Routes.Clear();
        }


        #region 自定义测距方法，不启用

        public static double EARTH_RADIUS = 6371.004;

        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat1">坐标1经度</param>
        /// <param name="lng1">坐标1纬度</param>
        /// <param name="lat2">坐标2经度</param>
        /// <param name="lng2">坐标2纬度</param>
        /// <returns>单位:米</returns>
        public static double getDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000d) / 10000d;
            s = s * 1000;
            return s;
        }

        public static double GetPointsDistance(PointLatLng ptFrom, PointLatLng ptTo)
        {
            return getDistance(ptFrom.Lat, ptFrom.Lng, ptTo.Lat, ptTo.Lng);
        }

        /// <summary>
        /// 根据路径
        /// </summary>
        /// <returns>
        /// 距离，单位米
        /// </returns>
        private double GetTotalDistance(List<PointLatLng> points)
        {
            double d = 0;
            if (points.Count >= 2)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (points.Count != i + 1)
                    {
                        d += getDistance(points[i].Lat, points[i].Lng, points[i + 1].Lat, points[i + 1].Lng);
                    }
                }
            }
            return d;
        }

        /// <summary>
        /// 长度显示文本
        /// </summary>
        /// <param name="distance">单位:米</param>
        /// <returns></returns>
        private string GetDistanceInfo(double distance)
        {
            string distanceInfo = "";
            if (distance > 1000)
            {
                double displayDistance = Math.Round(distance / 1000f, 2);
                distanceInfo = displayDistance.ToString() + "公里";
            }
            else
            {
                distanceInfo = Math.Round(distance, 0).ToString() + "米";
            }
            return distanceInfo;
        }

        /// <summary>
        /// 将角度换算为弧度。
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns>弧度</returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double ConvertRadiansToDegrees(double radian)
        {
            return radian * 180.0 / Math.PI;
        }

        #endregion

        private void BtnClearMeasure_Click(object sender, EventArgs e)
        {
            try
            {
                StopMeasure();
            }
            catch (Exception exception)
            {

            }

        }

        private void MapControl_MouseHover(object sender, EventArgs e)
        {

        }

        public void StartMeasure()
        {
            isMesuring = true;
/*            routeOverlay.Routes.Clear();
            labelOverlay.Markers.Clear();
            tempOverlay.Routes.Clear();*/
        }

        public void StopMeasure()
        {
            routeOverlay.Routes.Clear();
            labelOverlay.Markers.Clear();
            tempOverlay.Routes.Clear();
            drawingPoints.Clear();
            isMesuring = false;
        }

        public void ShowWarningMarker(List<long> sensorIds)
        {
            this.isShowWarningMarker = true;

            foreach (long sensorId in sensorIds)
            {
                int n = Objects.Count;

                for (int i = 0; i < n; i++)
                {
                    Objects[i].IsHit = false;

                    if (Objects[i] is DrawPolyline)
                    {
                        DrawPolyline o = Objects[i] as DrawPolyline;

                        if (o != null)
                        {
                            o.ShowCurrentMarker(sensorId);
                        }
                    }
                }
            }
        }

        public void HideWarningMarker(List<long> sensorIds)
        {
            this.isShowWarningMarker = false;

            foreach (long sensorId in sensorIds)
            {
                int n = Objects.Count;

                for (int i = 0; i < n; i++)
                {
                    Objects[i].IsHit = false;

                    if (Objects[i] is DrawPolyline)
                    {
                        DrawPolyline o = Objects[i] as DrawPolyline;

                        if (o != null)
                        {
                            o.HideCurrentMarker(sensorId);
                        }
                    }
                }
            }
        }

        private void toolStripButtonMarker_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.marker;
            markerType = MARKER_TYPE.Green;
        }

        private void toolStripButtonMarker2_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.marker;
            markerType = MARKER_TYPE.Yellow;
        }

        private void toolStripButtonMarker3_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.marker;
            markerType = MARKER_TYPE.Red;
        }

        private void toolStripButtonModel_Click(object sender, EventArgs e)
        {
            ActiveTool = TOOL_TYPE.model;
        }

        private void MapControl_DoubleClick(object sender, EventArgs e)
        {
            if (isMesuring)
            {
                if (drawingRoute != null)
                {
                    routeOverlay.Routes.Add(drawingRoute);
                    drawingRoute = null;
                }

                //总共的长度
                double totalDistance = GetTotalDistance(drawingPoints);

                PointLatLng p = drawingPoints[drawingPoints.Count - 1];
                GMapMarker m1 = new GMarkerGoogle(p, GMarkerGoogleType.orange_small);

                m1.ToolTipText = string.Format("总长:{0}", GetDistanceInfo(totalDistance), new Font("Arial", 3));
                m1.ToolTipMode = MarkerTooltipMode.Always;
                labelOverlay.Markers.Add(m1);
                tempOverlay.Routes.Clear();

                drawingPoints.Clear();
                isMesuring = false;
            }
        }
    }
}
