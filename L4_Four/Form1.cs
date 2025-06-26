using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph; // Для Vertex
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace L4_Four
{
    public partial class Form1 : Form
    {
        // --- Data Members ---
        DataTable CANdataTable;
        DataTable dataTable; // Processed data for plotting (TickStamp, Param1, Param2, ...)

        // --- OpenGL related members ---
        float rotationX = 20.0f, rotationY = -30.0f;
        float cameraDistance = 5.0f;
        Point lastMousePos;
        bool isMouseDown = false;

        // Data for 3D plot
        List<Point3F> active3DDataPoints = new List<Point3F>();
        string selectedXParam, selectedYParam, selectedZParam;

        // Textures for cube faces
        Dictionary<string, uint> projectionTextures = new Dictionary<string, uint>();

        // Parameters for face plots
        string selectedTimePlotParamFace;
        string selectedOutlierPlotParamFace;

        // Constants
        const int TEXTURE_SIZE = 256;
        const float CUBE_HALF_SIZE = 1.0f;
        const float FACE_OFFSET = 0.15f; // Уменьшил немного, чтобы было ближе

        // --- НОВЫЕ ПОЛЯ ДЛЯ ГИСТОГРАММЫ ПОВЕРХНОСТИ ---
        const int HISTOGRAM_GRID_SIZE = 20; // Размер сетки для гистограммы
        double[,] histogramDataXY;          // Данные для гистограммы XY
        double maxHistogramValueXY;         // Максимальное значение в histogramDataXY
        const float HISTOGRAM_BAR_MAX_HEIGHT = 0.5f; // Максимальная высота столбца гистограммы в мировых координатах
        const float HISTOGRAM_SURFACE_OFFSET_Z = 0.3f; // Смещение гистограммы от верхней грани куба

        // Enum for face plot types
        public enum FacePlotType
        {
            None,
            Projection,
            OutlierPlot,
            TimePlot
        }

        public struct CANDumpData
        {
            public UInt32 TickStamp;
            public byte Prefix;
            public byte Format;
            public byte Dest;
            public byte Source;
            public byte DLC;
            public byte b1, b2, b3, b4, b5, b6, b7, b8;

            public CANDumpData(uint ts, byte p, byte f, byte d, byte s, byte dl, byte B1, byte B2, byte B3, byte B4, byte B5, byte B6, byte B7, byte B8)
            {
                TickStamp = ts; Prefix = p; Format = f; Dest = d; Source = s; DLC = dl;
                b1 = B1; b2 = B2; b3 = B3; b4 = B4; b5 = B5; b6 = B6; b7 = B7; b8 = B8;
            }
        }

        public struct Point3F
        {
            public float X, Y, Z;
            public bool IsValid;
            public Point3F(float x, float y, float z, bool isValid = true)
            {
                X = x; Y = y; Z = z;
                IsValid = isValid;
            }
        }

        public Form1()
        {
            InitializeComponent();
            InitializeFaceTypeComboBoxes();

            // --- ИНИЦИАЛИЗАЦИЯ ДАННЫХ ГИСТОГРАММЫ ---
            histogramDataXY = new double[HISTOGRAM_GRID_SIZE, HISTOGRAM_GRID_SIZE];

            // Subscribe to events
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);

            this.cmbXAxis.SelectedIndexChanged += new System.EventHandler(this.cmbMainOrFaceParams_Changed);
            this.cmbYAxis.SelectedIndexChanged += new System.EventHandler(this.cmbMainOrFaceParams_Changed);
            this.cmbZAxis.SelectedIndexChanged += new System.EventHandler(this.cmbMainOrFaceParams_Changed);

            this.cmbFaceTypeX.SelectedIndexChanged += new System.EventHandler(this.cmbMainOrFaceParams_Changed);
            this.cmbFaceTypeY.SelectedIndexChanged += new System.EventHandler(this.cmbMainOrFaceParams_Changed);
            this.cmbFaceTypeZ.SelectedIndexChanged += new System.EventHandler(this.cmbMainOrFaceParams_Changed);

            if (this.cmbTimePlotParamFace != null)
                this.cmbTimePlotParamFace.SelectedIndexChanged += new System.EventHandler(this.cmbMainOrFaceParams_Changed);
            if (this.cmbOutlierParamFace != null)
                this.cmbOutlierParamFace.SelectedIndexChanged += new System.EventHandler(this.cmbMainOrFaceParams_Changed);

            this.chkShowCube.CheckedChanged += new System.EventHandler(this.chkShowCube_CheckedChanged);

            // --- ДОБАВЬ ЭТУ СТРОКУ, ЕСЛИ У ТЕБЯ ЕСТЬ CheckBox с именем chkShowHistogramSurface ---
            // Убедись, что он добавлен в дизайнере формы!
            if (this.Controls.ContainsKey("chkShowHistogramSurface"))
            {
                (this.Controls["chkShowHistogramSurface"] as CheckBox).CheckedChanged += chkShowHistogramSurface_CheckedChanged;
            }
            else
            {
                // Если чекбокса нет, можно создать его программно или вывести сообщение
                // Для простоты, сейчас предположим, что он есть.
                // MessageBox.Show("CheckBox 'chkShowHistogramSurface' не найден на форме.");
            }
        }

        // --- ОБРАБОТЧИК ДЛЯ НОВОГО CheckBox ---
        private void chkShowHistogramSurface_CheckedChanged(object sender, EventArgs e)
        {
            sharpGLControl1.Invalidate(); // Перерисовать сцену
        }


        private void InitializeFaceTypeComboBoxes()
        {
            var plotTypes = Enum.GetValues(typeof(FacePlotType)).Cast<FacePlotType>().ToList();
            Action<ComboBox> setupCb = (cb) => {
                if (cb != null) { cb.DataSource = plotTypes.ToList(); cb.SelectedItem = FacePlotType.Projection; }
            };
            setupCb(cmbFaceTypeX);
            setupCb(cmbFaceTypeY);
            setupCb(cmbFaceTypeZ);
        }

        private void sharpGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {
            OpenGL gl = sharpGLControl1.OpenGL;
            gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_TEXTURE_2D); // Уже было, оставляем
            gl.Enable(OpenGL.GL_BLEND); // Для полупрозрачности гистограммы
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
        }

        private void sharpGLControl1_Resize(object sender, EventArgs e)
        {
            OpenGL gl = sharpGLControl1.OpenGL;
            gl.Viewport(0, 0, sharpGLControl1.Width, sharpGLControl1.Height);
            SetProjection(gl);
        }

        private void SetProjection(OpenGL gl)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0f, (double)sharpGLControl1.Width / (double)sharpGLControl1.Height, 0.1f, 100.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void sharpGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
            OpenGL gl = sharpGLControl1.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -cameraDistance);
            gl.Rotate(rotationX, 1.0f, 0.0f, 0.0f);
            gl.Rotate(rotationY, 0.0f, 1.0f, 0.0f);

            DrawAxes(gl);
            if (chkShowCube.Checked)
            {
                DrawBoundingWireCube(gl);
            }
            Draw3DLinePlot(gl);
            DrawCustomizableProjectionPlanes(gl);

            // --- ОТРИСОВКА ГИСТОГРАММЫ ПОВЕРХНОСТИ ---
            // Проверяем, есть ли чекбокс и отмечен ли он
            CheckBox chkHist = this.Controls.Find("chkShowHistogramSurface", true).FirstOrDefault() as CheckBox;
            if (chkHist != null && chkHist.Checked)
            {
                gl.PushMatrix();
                // Смещаем гистограмму так, чтобы ее основание было чуть выше верхней грани куба (по Z)
                gl.Translate(0, 0, CUBE_HALF_SIZE + HISTOGRAM_SURFACE_OFFSET_Z);
                DrawHistogramSurface(gl); // Используем твой метод
                gl.PopMatrix();
            }
            // -------------------------------------------

            gl.Flush();
        }

        private float NormalizeValue(byte val)
        {
            // Нормализация к диапазону [-CUBE_HALF_SIZE, CUBE_HALF_SIZE]
            return ((float)val / 255.0f) * (2.0f * CUBE_HALF_SIZE) - CUBE_HALF_SIZE;
        }

        // --- НОВЫЙ МЕТОД: ПОДГОТОВКА ДАННЫХ ДЛЯ ГИСТОГРАММЫ XY ---
        private void PrepareHistogramDataXY()
        {
            // Очистка предыдущих данных
            for (int i = 0; i < HISTOGRAM_GRID_SIZE; i++)
            {
                for (int j = 0; j < HISTOGRAM_GRID_SIZE; j++)
                {
                    histogramDataXY[i, j] = 0;
                }
            }
            maxHistogramValueXY = 0;

            if (active3DDataPoints == null || !active3DDataPoints.Any()) return;

            float halfCubeSize = CUBE_HALF_SIZE; // Используем CUBE_HALF_SIZE, так как точки уже нормализованы к этому диапазону

            foreach (var point in active3DDataPoints)
            {
                if (!point.IsValid) continue;

                // Проецируем точку (X, Y) на сетку гистограммы
                // Координаты точек active3DDataPoints уже в диапазоне [-CUBE_HALF_SIZE, CUBE_HALF_SIZE]
                // Нам нужно отобразить их в индексы сетки [0, HISTOGRAM_GRID_SIZE-1]
                // Сначала переводим в диапазон [0, 2 * CUBE_HALF_SIZE]
                float mappedX = point.X + halfCubeSize;
                float mappedY = point.Y + halfCubeSize;

                // Затем в диапазон [0, 1]
                float normalizedGridX = mappedX / (2.0f * halfCubeSize);
                float normalizedGridY = mappedY / (2.0f * halfCubeSize);

                // И наконец в индексы сетки
                int gridX = (int)(normalizedGridX * HISTOGRAM_GRID_SIZE);
                int gridY = (int)(normalizedGridY * HISTOGRAM_GRID_SIZE);

                // Ограничиваем индексами, чтобы не выйти за пределы массива
                gridX = Math.Max(0, Math.Min(gridX, HISTOGRAM_GRID_SIZE - 1));
                gridY = Math.Max(0, Math.Min(gridY, HISTOGRAM_GRID_SIZE - 1));

                histogramDataXY[gridX, gridY]++;
            }

            // Находим максимальное значение для нормализации высот
            for (int i = 0; i < HISTOGRAM_GRID_SIZE; i++)
            {
                for (int j = 0; j < HISTOGRAM_GRID_SIZE; j++)
                {
                    if (histogramDataXY[i, j] > maxHistogramValueXY)
                    {
                        maxHistogramValueXY = histogramDataXY[i, j];
                    }
                }
            }
        }


        private void PrepareDataAndTextures()
        {
            OpenGL gl = sharpGLControl1.OpenGL;
            active3DDataPoints.Clear();

            foreach (var pair in projectionTextures)
            {
                if (pair.Value != 0) gl.DeleteTextures(1, new[] { pair.Value });
            }
            projectionTextures.Clear();

            if (dataTable == null || cmbXAxis.SelectedItem == null || cmbYAxis.SelectedItem == null || cmbZAxis.SelectedItem == null)
            {
                sharpGLControl1.Invalidate();
                return;
            }

            selectedXParam = cmbXAxis.SelectedItem.ToString();
            selectedYParam = cmbYAxis.SelectedItem.ToString();
            selectedZParam = cmbZAxis.SelectedItem.ToString();

            if (!dataTable.Columns.Contains(selectedXParam) ||
                !dataTable.Columns.Contains(selectedYParam) ||
                !dataTable.Columns.Contains(selectedZParam))
            {
                sharpGLControl1.Invalidate();
                return;
            }

            foreach (DataRow row in dataTable.Rows)
            {
                byte rawX = 0, rawY = 0, rawZ = 0;
                bool xValid = false, yValid = false, zValid = false;
                if (row[selectedXParam] != DBNull.Value) { rawX = (byte)row[selectedXParam]; xValid = true; }
                if (row[selectedYParam] != DBNull.Value) { rawY = (byte)row[selectedYParam]; yValid = true; }
                if (row[selectedZParam] != DBNull.Value) { rawZ = (byte)row[selectedZParam]; zValid = true; }

                bool pointIsValid = xValid && yValid && zValid;
                float normX = NormalizeValue(rawX);
                float normY = NormalizeValue(rawY);
                float normZ = NormalizeValue(rawZ);

                active3DDataPoints.Add(new Point3F(normX, normY, normZ, pointIsValid));
            }


            // --- ВЫЗОВ ПОДГОТОВКИ ДАННЫХ ДЛЯ ГИСТОГРАММЫ ---
            PrepareHistogramDataXY();
            // -----------------------------------------------

            if (!active3DDataPoints.Any())
            {
                sharpGLControl1.Invalidate();
                return;
            }

            selectedTimePlotParamFace = cmbTimePlotParamFace?.SelectedItem?.ToString();
            selectedOutlierPlotParamFace = cmbOutlierParamFace?.SelectedItem?.ToString();

            List<PointF> xyPoints = active3DDataPoints.Where(p => p.IsValid).Select(p => new PointF(p.X, p.Y)).ToList();
            List<PointF> xzPoints = active3DDataPoints.Where(p => p.IsValid).Select(p => new PointF(p.X, p.Z)).ToList();
            List<PointF> yzPoints = active3DDataPoints.Where(p => p.IsValid).Select(p => new PointF(p.Y, p.Z)).ToList();

            FacePlotType faceTypeX = cmbFaceTypeX?.SelectedItem != null ? (FacePlotType)cmbFaceTypeX.SelectedItem : FacePlotType.None;
            ProcessFacePairTextures(gl, faceTypeX, "PX", "NX",
                () => CreateScatterPlotTexture(gl, yzPoints, selectedYParam, selectedZParam, "YZ Proj."),
                selectedOutlierPlotParamFace, selectedTimePlotParamFace);

            FacePlotType faceTypeY = cmbFaceTypeY?.SelectedItem != null ? (FacePlotType)cmbFaceTypeY.SelectedItem : FacePlotType.None;
            ProcessFacePairTextures(gl, faceTypeY, "PY", "NY",
                () => CreateScatterPlotTexture(gl, xzPoints, selectedXParam, selectedZParam, "XZ Proj."),
                selectedOutlierPlotParamFace, selectedTimePlotParamFace);

            FacePlotType faceTypeZ = cmbFaceTypeZ?.SelectedItem != null ? (FacePlotType)cmbFaceTypeZ.SelectedItem : FacePlotType.None;
            ProcessFacePairTextures(gl, faceTypeZ, "PZ", "NZ",
                () => CreateScatterPlotTexture(gl, xyPoints, selectedXParam, selectedYParam, "XY Proj."),
                selectedOutlierPlotParamFace, selectedTimePlotParamFace);

            sharpGLControl1.Invalidate();
        }

        private void ProcessFacePairTextures(OpenGL gl, FacePlotType type, string keyPositive, string keyNegative,
                                     Func<uint> projectionTextureFunc,
                                     string outlierParam, string timeParam)
        {
            uint textureId = 0;
            switch (type)
            {
                case FacePlotType.None: break;
                case FacePlotType.Projection:
                    if (active3DDataPoints.Any() && !string.IsNullOrEmpty(selectedXParam) && !string.IsNullOrEmpty(selectedYParam) && !string.IsNullOrEmpty(selectedZParam))
                        textureId = projectionTextureFunc();
                    break;
                case FacePlotType.OutlierPlot:
                    if (!string.IsNullOrEmpty(outlierParam) && dataTable != null && dataTable.Columns.Contains(outlierParam))
                        textureId = CreateOutlierPlotTexture(gl, outlierParam, $"Dist: '{outlierParam}'");
                    break;
                case FacePlotType.TimePlot:
                    if (!string.IsNullOrEmpty(timeParam) && dataTable != null && dataTable.Columns.Contains(timeParam))
                        textureId = CreateTimePlotTexture(gl, timeParam, $"'{timeParam}' vs Time");
                    break;
            }
            if (textureId != 0) { projectionTextures[keyPositive] = textureId; projectionTextures[keyNegative] = textureId; }
        }

        // --- ТВОЙ МЕТОД ДЛЯ ОТРИСОВКИ ГИСТОГРАММЫ ПОВЕРХНОСТИ (адаптированный) ---
        // Использует поля класса: histogramDataXY, maxHistogramValueXY, HISTOGRAM_GRID_SIZE, HISTOGRAM_BAR_MAX_HEIGHT
        private void DrawHistogramSurface(OpenGL gl)
        {
            if (maxHistogramValueXY <= 0) return; // Нечего рисовать

            // Размер ячейки сетки в плоскости XY гистограммы.
            // Гистограмма будет занимать пространство от -CUBE_HALF_SIZE до +CUBE_HALF_SIZE по X и Y.
            float totalWidth = 2.0f * CUBE_HALF_SIZE;
            float cellSize = totalWidth / HISTOGRAM_GRID_SIZE;

            // Отрисовка с использованием Quads
            gl.Begin(BeginMode.Quads);

            for (int i = 0; i < HISTOGRAM_GRID_SIZE - 1; i++) // Цикл до GridSize-1 для quads
            {
                for (int j = 0; j < HISTOGRAM_GRID_SIZE - 1; j++) // Цикл до GridSize-1 для quads
                {
                    // Центр первой ячейки (i,j)
                    // X и Y координаты от -CUBE_HALF_SIZE до +CUBE_HALF_SIZE
                    float x_base = -CUBE_HALF_SIZE + i * cellSize;
                    float y_base = -CUBE_HALF_SIZE + j * cellSize;

                    // Координаты для 4 углов quad'а (основания на z=0 относительно текущей трансформации)
                    float x0 = x_base;
                    float y0 = y_base;
                    float z0_norm = (float)(histogramDataXY[i, j] / maxHistogramValueXY);
                    float z0 = z0_norm * HISTOGRAM_BAR_MAX_HEIGHT;

                    float x1 = x_base + cellSize;
                    float y1 = y_base;
                    float z1_norm = (float)(histogramDataXY[i + 1, j] / maxHistogramValueXY);
                    float z1 = z1_norm * HISTOGRAM_BAR_MAX_HEIGHT;

                    float x2 = x_base + cellSize;
                    float y2 = y_base + cellSize;
                    float z2_norm = (float)(histogramDataXY[i + 1, j + 1] / maxHistogramValueXY);
                    float z2 = z2_norm * HISTOGRAM_BAR_MAX_HEIGHT;

                    float x3 = x_base;
                    float y3 = y_base + cellSize;
                    float z3_norm = (float)(histogramDataXY[i, j + 1] / maxHistogramValueXY);
                    float z3 = z3_norm * HISTOGRAM_BAR_MAX_HEIGHT;

                    // Задание цвета на основе средней нормализованной высоты для грани
                    // Можно использовать z0, или среднее из z0,z1,z2,z3 для более плавного цвета
                    float avg_z_norm = (z0_norm + z1_norm + z2_norm + z3_norm) / 4.0f;
                    gl.Color(avg_z_norm, 0.5f * (1 - avg_z_norm), 1.0f - avg_z_norm, 0.7f); // 0.7f Alpha

                    // Определение вершин quad'а (по часовой стрелке или против, главное консистентно)
                    gl.Vertex(x0, y0, z0);
                    gl.Vertex(x1, y1, z1);
                    gl.Vertex(x2, y2, z2);
                    gl.Vertex(x3, y3, z3);
                }
            }
            gl.End();

            // Опционально: Отрисовка контура для ясности
            gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            gl.Color(0.9f, 0.9f, 0.9f, 0.5f); // Белые/серые линии с полупрозрачностью
            gl.LineWidth(1.0f);
            gl.Begin(BeginMode.Quads);
            for (int i = 0; i < HISTOGRAM_GRID_SIZE - 1; i++)
            {
                for (int j = 0; j < HISTOGRAM_GRID_SIZE - 1; j++)
                {
                    float x_base = -CUBE_HALF_SIZE + i * cellSize;
                    float y_base = -CUBE_HALF_SIZE + j * cellSize;

                    float x0 = x_base;
                    float y0 = y_base;
                    float z0 = (float)(histogramDataXY[i, j] / maxHistogramValueXY) * HISTOGRAM_BAR_MAX_HEIGHT;

                    float x1 = x_base + cellSize;
                    float y1 = y_base;
                    float z1 = (float)(histogramDataXY[i + 1, j] / maxHistogramValueXY) * HISTOGRAM_BAR_MAX_HEIGHT;

                    float x2 = x_base + cellSize;
                    float y2 = y_base + cellSize;
                    float z2 = (float)(histogramDataXY[i + 1, j + 1] / maxHistogramValueXY) * HISTOGRAM_BAR_MAX_HEIGHT;

                    float x3 = x_base;
                    float y3 = y_base + cellSize;
                    float z3 = (float)(histogramDataXY[i, j + 1] / maxHistogramValueXY) * HISTOGRAM_BAR_MAX_HEIGHT;

                    gl.Vertex(x0, y0, z0);
                    gl.Vertex(x1, y1, z1);
                    gl.Vertex(x2, y2, z2);
                    gl.Vertex(x3, y3, z3);
                }
            }
            gl.End();
            gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL); // Сброс в режим заливки
            gl.LineWidth(1.0f); // Сброс толщины линии
        }


        // --- Texture Creation Methods (без изменений, просто для полноты) ---
        private uint CreateScatterPlotTexture(OpenGL gl, List<PointF> points, string xLabel, string yLabel, string title)
        {
            Bitmap bmp = new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(180, 240, 240, 240));
                Font font = new Font("Arial", 10);
                Brush dataBrush = Brushes.DarkBlue;
                float textHeight = font.GetHeight(g);
                g.DrawString(title, font, Brushes.Black, 5, 5);
                if (!string.IsNullOrEmpty(xLabel)) g.DrawString($"X: {xLabel}", font, Brushes.Red, 5, TEXTURE_SIZE - 2 * textHeight - 5);
                if (!string.IsNullOrEmpty(yLabel)) g.DrawString($"Y: {yLabel}", font, Brushes.Green, 5, TEXTURE_SIZE - textHeight - 5);
                float pointRadius = 1.5f;
                if (points != null && points.Any())
                {
                    foreach (var p in points)
                    {
                        float tx = (p.X + CUBE_HALF_SIZE) / (2 * CUBE_HALF_SIZE) * TEXTURE_SIZE;
                        float ty = (p.Y + CUBE_HALF_SIZE) / (2 * CUBE_HALF_SIZE) * TEXTURE_SIZE;
                        g.FillEllipse(dataBrush, tx - pointRadius, ty - pointRadius, 2 * pointRadius, 2 * pointRadius);
                    }
                }
            }
            return CreateTextureFromBitmap(gl, bmp);
        }

        private uint CreateTimePlotTexture(OpenGL gl, string paramName, string title)
        {
            if (dataTable == null || string.IsNullOrEmpty(paramName) || !dataTable.Columns.Contains(paramName) || !dataTable.Columns.Contains("TickStamp"))
                return CreateTextureFromBitmap(gl, new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE));
            List<PointF> timeSeriesPoints = new List<PointF>();
            UInt32 minTick = UInt32.MaxValue, maxTick = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                if (row[paramName] != DBNull.Value && row["TickStamp"] != DBNull.Value)
                {
                    UInt32 tick = (UInt32)row["TickStamp"];
                    byte val = (byte)row[paramName];
                    timeSeriesPoints.Add(new PointF(tick, val));
                    if (tick < minTick) minTick = tick;
                    if (tick > maxTick) maxTick = tick;
                }
            }

            if (timeSeriesPoints.Count < 2) return CreateTextureFromBitmap(gl, new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE));
            Bitmap bmp = new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.FromArgb(180, 220, 220, 250));
                Font axisFont = new Font("Arial", 8);
                Font titleFont = new Font("Arial", 10, FontStyle.Bold);
                Pen linePen = new Pen(Color.DarkSlateBlue, 1.5f);
                Pen gridPen = new Pen(Color.FromArgb(100, 128, 128, 128), 0.5f);

                float padding = 15;
                float headerHeight = g.MeasureString(title, titleFont).Height + 5;
                float footerHeight = g.MeasureString("Time", axisFont).Height + 5;
                float yAxisLabelWidth = g.MeasureString("255", axisFont).Width + 5;

                RectangleF plotArea = new RectangleF(
                    padding + yAxisLabelWidth,
                    padding + headerHeight,
                    TEXTURE_SIZE - (2 * padding) - yAxisLabelWidth - 5,
                    TEXTURE_SIZE - (2 * padding) - headerHeight - footerHeight - 5
                );

                if (plotArea.Width <= 0 || plotArea.Height <= 0) return CreateTextureFromBitmap(gl, bmp);

                SizeF titleSize = g.MeasureString(title, titleFont);
                g.DrawString(title, titleFont, Brushes.Black, plotArea.X + Math.Max(0, (plotArea.Width - titleSize.Width) / 2), padding);

                g.DrawString(paramName, axisFont, Brushes.DarkRed, padding + yAxisLabelWidth, plotArea.Bottom + 2);
                g.DrawString("Time", axisFont, Brushes.DarkGreen, plotArea.Right - g.MeasureString("Time", axisFont).Width, plotArea.Bottom + 2);

                g.DrawRectangle(Pens.DimGray, plotArea.X, plotArea.Y, plotArea.Width, plotArea.Height);

                for (int i = 0; i <= 5; i++)
                {
                    float yVal = i * (255.0f / 5.0f);
                    float yPos = plotArea.Bottom - (yVal / 255.0f) * plotArea.Height;
                    g.DrawLine(gridPen, plotArea.Left, yPos, plotArea.Right, yPos);
                    g.DrawString(Math.Round(yVal).ToString(), axisFont, Brushes.Black, padding, yPos - axisFont.Height / 2);
                }

                List<PointF> screenPoints = new List<PointF>();
                float tickRange = (maxTick - minTick);
                if (tickRange == 0) tickRange = 1;

                foreach (var p in timeSeriesPoints.OrderBy(pt => pt.X))
                {
                    float tx = plotArea.X + ((p.X - minTick) / tickRange) * plotArea.Width;
                    float ty = plotArea.Bottom - ((p.Y / 255.0f) * plotArea.Height);
                    screenPoints.Add(new PointF(tx, ty));
                }

                if (screenPoints.Count > 1)
                {
                    g.DrawLines(linePen, screenPoints.ToArray());
                }
            }
            return CreateTextureFromBitmap(gl, bmp);
        }

        public struct OhlcData
        {
            public byte Open;
            public byte High;
            public byte Low;
            public byte Close;
            public uint StartTick; // Для возможного использования в будущем
            public uint EndTick;   // Для возможного использования в будущем

            public OhlcData(byte open, byte high, byte low, byte close, uint startTick, uint endTick)
            {
                Open = open;
                High = high;
                Low = low;
                Close = close;
                StartTick = startTick;
                EndTick = endTick;
            }
        }

        private uint CreateOutlierPlotTexture(OpenGL gl, string paramName, string title) // Фактически будет Candlestick
        {
            if (dataTable == null || string.IsNullOrEmpty(paramName) || !dataTable.Columns.Contains(paramName))
                return CreateTextureFromBitmap(gl, new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE));

            // 1. Сбор временного ряда для выбранного параметра
            List<Tuple<uint, byte>> timeSeries = new List<Tuple<uint, byte>>();
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["TickStamp"] != DBNull.Value && row[paramName] != DBNull.Value)
                {
                    timeSeries.Add(new Tuple<uint, byte>((uint)row["TickStamp"], (byte)row[paramName]));
                }
            }

            if (timeSeries.Count < 2) // Нужно хотя бы 2 точки для какой-то динамики
                return CreateTextureFromBitmap(gl, new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE));

            // Сортируем по времени (TickStamp)
            timeSeries.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            // 2. Агрегация в OHLC данные
            List<OhlcData> ohlcValues = new List<OhlcData>();
            const int TARGET_CANDLES_TO_DISPLAY = 30; // Желаемое количество свечей на графике
            int pointsPerCandle = Math.Max(1, timeSeries.Count / TARGET_CANDLES_TO_DISPLAY);
            // Если точек мало, каждая точка может стать почти своей свечой (pointsPerCandle=1)
            // Если точек очень много, одна свеча будет агрегировать больше точек.

            for (int i = 0; i < timeSeries.Count; i += pointsPerCandle)
            {
                List<Tuple<uint, byte>> currentBucket = timeSeries.Skip(i).Take(pointsPerCandle).ToList();
                if (!currentBucket.Any()) continue;

                byte open = currentBucket.First().Item2;
                byte close = currentBucket.Last().Item2;
                byte high = currentBucket.Max(p => p.Item2);
                byte low = currentBucket.Min(p => p.Item2);
                uint startTick = currentBucket.First().Item1;
                uint endTick = currentBucket.Last().Item1;

                ohlcValues.Add(new OhlcData(open, high, low, close, startTick, endTick));
            }

            if (!ohlcValues.Any())
                return CreateTextureFromBitmap(gl, new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE));

            // --- Начало рисования ---
            Bitmap bmp = new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.FromArgb(255, 245, 245, 245)); // Светло-серый фон
                Font axisFont = new Font("Arial", 8);
                Font titleFont = new Font("Arial", 10, FontStyle.Bold);
                Pen wickPen = new Pen(Color.Black, 0.8f);
                Brush greenBrush = Brushes.Green;
                Brush redBrush = Brushes.Red;

                float topPadding = 20;
                float bottomPadding = 20;
                float leftPadding = 30;
                float rightPadding = 10;

                RectangleF plotArea = new RectangleF(
                    leftPadding, topPadding,
                    TEXTURE_SIZE - leftPadding - rightPadding,
                    TEXTURE_SIZE - topPadding - bottomPadding
                );

                if (plotArea.Width <= 0 || plotArea.Height <= 0)
                {
                    bmp.Dispose();
                    return CreateTextureFromBitmap(gl, new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE));
                }

                // --- Функция для преобразования значения в Y-координату ---
                Func<byte, float> valToY = (val) => plotArea.Bottom - (float)((val / 255.0) * plotArea.Height);

                // --- Заголовок ---
                string chartTitle = $"'{paramName}' Candlestick"; // Новый заголовок
                SizeF titleSize = g.MeasureString(chartTitle, titleFont);
                g.DrawString(chartTitle, titleFont, Brushes.Black,
                             plotArea.X + (plotArea.Width - titleSize.Width) / 2,
                             topPadding / 2 - titleSize.Height / 2 + 2);

                // --- Ось Y и метки ---
                g.DrawLine(Pens.DarkGray, plotArea.Left, plotArea.Top, plotArea.Left, plotArea.Bottom);
                int numGridLinesY = 5;
                for (int i = 0; i <= numGridLinesY; i++)
                {
                    byte val = (byte)Math.Round(i * (255.0 / numGridLinesY));
                    float yPos = valToY(val);
                    g.DrawLine(Pens.LightGray, plotArea.Left, yPos, plotArea.Right, yPos);
                    string label = val.ToString();
                    SizeF labelSize = g.MeasureString(label, axisFont);
                    g.DrawString(label, axisFont, Brushes.Black, plotArea.Left - labelSize.Width - 3, yPos - labelSize.Height / 2);
                }

                // --- Рисование свечей ---
                float candleTotalWidth = plotArea.Width / Math.Max(1, ohlcValues.Count); // Ширина, выделенная под одну свечу (включая возможное пространство)
                float candleBodyWidth = candleTotalWidth * 0.7f; // Фактическая ширина тела свечи
                float candleSpacing = candleTotalWidth - candleBodyWidth; // Пространство между свечами (если есть)

                for (int i = 0; i < ohlcValues.Count; i++)
                {
                    OhlcData ohlc = ohlcValues[i];
                    float xCandleCenter = plotArea.X + i * candleTotalWidth + candleTotalWidth / 2;

                    float yHigh = valToY(ohlc.High);
                    float yLow = valToY(ohlc.Low);
                    float yOpen = valToY(ohlc.Open);
                    float yClose = valToY(ohlc.Close);

                    // Рисуем тень/фитиль (вертикальная линия от High до Low)
                    g.DrawLine(wickPen, xCandleCenter, yHigh, xCandleCenter, yLow);

                    // Рисуем тело свечи
                    Brush bodyBrush = ohlc.Close >= ohlc.Open ? greenBrush : redBrush;
                    float bodyTopY = Math.Min(yOpen, yClose);
                    float bodyBottomY = Math.Max(yOpen, yClose);
                    float bodyHeight = bodyBottomY - bodyTopY;

                    if (bodyHeight < 1) bodyHeight = 1; // Чтобы свеча была видна, даже если Open == Close

                    g.FillRectangle(bodyBrush,
                                    xCandleCenter - candleBodyWidth / 2,
                                    bodyTopY,
                                    candleBodyWidth,
                                    bodyHeight);
                    // Опционально: контур тела свечи, если хотите
                    // g.DrawRectangle(Pens.Black, xCandleCenter - candleBodyWidth / 2, bodyTopY, candleBodyWidth, bodyHeight);

                }
                // --- Подпись оси X (Время/Интервалы) ---
                string xAxisLabel = "Time Intervals";
                SizeF xAxisLabelSize = g.MeasureString(xAxisLabel, axisFont);
                g.DrawString(xAxisLabel, axisFont, Brushes.Black,
                             plotArea.X + (plotArea.Width - xAxisLabelSize.Width) / 2,
                             plotArea.Bottom + 3);
            }
            return CreateTextureFromBitmap(gl, bmp);
        }

        // Вспомогательный метод для вычисления перцентилей
        // (Нужно добавить его в ваш класс Form1)
        private double GetPercentile(List<byte> sortedSequence, double percentile)
        {
            if (sortedSequence == null || sortedSequence.Count == 0)
                return 0; // Или выбросить исключение

            int N = sortedSequence.Count;
            double n = (N - 1) * (percentile / 100.0) + 1;

            if (n == 1.0) return sortedSequence[0];
            else if (n == N) return sortedSequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sortedSequence[k - 1] + d * (sortedSequence[k] - sortedSequence[k - 1]);
            }
        }

        private uint CreateTextureFromBitmap(OpenGL gl, Bitmap bitmap)
        {
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            uint[] textureIds = new uint[1];
            gl.GenTextures(1, textureIds);
            uint textureId = textureIds[0];
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0,
                          OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bmpData.Scan0);
            bitmap.UnlockBits(bmpData);
            bitmap.Dispose();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            return textureId;
        }

        // --- Drawing Methods (остальные без изменений) ---
        private void DrawAxes(OpenGL gl)
        {
            float axisLength = CUBE_HALF_SIZE * 1.2f;
            gl.LineWidth(2.0f);
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(1.0f, 0.0f, 0.0f); gl.Vertex(-axisLength, 0.0f, 0.0f); gl.Vertex(axisLength, 0.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f); gl.Vertex(0.0f, -axisLength, 0.0f); gl.Vertex(0.0f, axisLength, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f); gl.Vertex(0.0f, 0.0f, -axisLength); gl.Vertex(0.0f, 0.0f, axisLength);
            gl.End();
            gl.LineWidth(1.0f);
        }

        private void DrawBoundingWireCube(OpenGL gl)
        {
            gl.Color(0.7f, 0.7f, 0.7f, 0.5f);
            gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            float s = CUBE_HALF_SIZE;
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(s, s, s); gl.Vertex(-s, s, s); gl.Vertex(-s, -s, s); gl.Vertex(s, -s, s);
            gl.Vertex(s, s, -s); gl.Vertex(s, -s, -s); gl.Vertex(-s, -s, -s); gl.Vertex(-s, s, -s);
            gl.Vertex(s, s, s); gl.Vertex(s, s, -s); gl.Vertex(-s, s, -s); gl.Vertex(-s, s, s);
            gl.Vertex(s, -s, s); gl.Vertex(-s, -s, s); gl.Vertex(-s, -s, -s); gl.Vertex(s, -s, -s);
            gl.Vertex(s, s, s); gl.Vertex(s, -s, s); gl.Vertex(s, -s, -s); gl.Vertex(s, s, -s);
            gl.Vertex(-s, s, s); gl.Vertex(-s, s, -s); gl.Vertex(-s, -s, -s); gl.Vertex(-s, -s, s);
            gl.End();
            gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
        }

        private void Draw3DLinePlot(OpenGL gl)
        {
            if (active3DDataPoints.Count < 2) return;
            gl.Color(1.0f, 1.0f, 0.0f); // Yellow
            gl.LineWidth(1.5f);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            foreach (var point in active3DDataPoints)
            {
                if (point.IsValid)
                {
                    gl.Vertex(point.X, point.Y, point.Z);
                }
                else
                {
                    gl.End(); gl.Begin(OpenGL.GL_LINE_STRIP);
                }
            }
            gl.End();
            gl.LineWidth(1.0f);
        }

        private void DrawCustomizableProjectionPlanes(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            float s = CUBE_HALF_SIZE; // 's' используется внутри лямбда-выражений drawQuadAction
            double[] mv_array = new double[16];
            gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, mv_array);

            // YZ Plot на PX грани (положительная X)
            DrawSingleFace(gl, mv_array, new Vertex(1, 0, 0), "PX", s, () => {
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(s, -s, -s);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(s, s, -s);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(s, s, s);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(s, -s, s);
            }, FACE_OFFSET); // <--- Передаем FACE_OFFSET

            // YZ Plot на NX грани (отрицательная X)
            DrawSingleFace(gl, mv_array, new Vertex(-1, 0, 0), "NX", s, () => {
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-s, -s, -s);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-s, s, -s);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-s, s, s);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-s, -s, s);
            }, FACE_OFFSET); // <--- Передаем FACE_OFFSET

            // XZ Proj. на PY грани (верхняя, положительная Y)
            DrawSingleFace(gl, mv_array, new Vertex(0, 1, 0), "PY", s, () => {
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-s, s, -s);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(s, s, -s);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(s, s, s);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-s, s, s);
            }, FACE_OFFSET); // <--- Передаем FACE_OFFSET

            // XZ Proj. на NY грани (нижняя, отрицательная Y)
            DrawSingleFace(gl, mv_array, new Vertex(0, -1, 0), "NY", s, () => {
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(s, -s, -s);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-s, -s, -s);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-s, -s, s);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(s, -s, s);
            }, FACE_OFFSET); // <--- Передаем FACE_OFFSET

            // XY Proj. на PZ грани (передняя, положительная Z)
            DrawSingleFace(gl, mv_array, new Vertex(0, 0, 1), "PZ", s, () => {
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-s, -s, s);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(s, -s, s);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(s, s, s);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-s, s, s);
            }, FACE_OFFSET); // <--- Передаем FACE_OFFSET

            // XY Proj. на NZ грани (задняя, отрицательная Z)
            DrawSingleFace(gl, mv_array, new Vertex(0, 0, -1), "NZ", s, () => {
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-s, -s, -s);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(s, -s, -s);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(s, s, -s);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-s, s, -s);
            }, FACE_OFFSET); // <--- Передаем FACE_OFFSET

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0); // Сброс привязки текстуры
            gl.Disable(OpenGL.GL_BLEND);
        }


        private void DrawSingleFace(OpenGL gl, double[] mvMatrix, Vertex localNormal, string textureKey, float size, Action drawQuadAction, float offset)
        {
            if (!projectionTextures.TryGetValue(textureKey, out uint textureId) || textureId == 0) return;

            float nx_model = localNormal.X;
            float ny_model = localNormal.Y;
            float nz_model = localNormal.Z;

            float viewNormalZ = (float)(mvMatrix[2] * nx_model + mvMatrix[6] * ny_model + mvMatrix[10] * nz_model);

            if (viewNormalZ > 0.001f) return; // Отсечение задних граней

            gl.Color(1.0f, 1.0f, 1.0f, 0.7f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

            gl.PushMatrix();
            gl.Translate(localNormal.X * offset, localNormal.Y * offset, localNormal.Z * offset);
            gl.Begin(OpenGL.GL_QUADS);
            drawQuadAction();
            gl.End();
            gl.PopMatrix();
        }

        // --- Mouse Controls (без изменений) ---
        private void sharpGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                lastMousePos = e.Location;
            }
        }

        private void sharpGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                float dx = e.X - lastMousePos.X;
                float dy = e.Y - lastMousePos.Y;

                rotationY += dx * 0.5f;
                rotationX += dy * 0.5f;

                lastMousePos = e.Location;
                sharpGLControl1.Invalidate();
            }
        }

        private void sharpGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void sharpGLControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            cameraDistance -= e.Delta * 0.005f;
            if (cameraDistance < 1.0f) cameraDistance = 1.0f;
            if (cameraDistance > 20.0f) cameraDistance = 20.0f;
            sharpGLControl1.Invalidate();
        }

        // --- UI Event Handlers (остальные без изменений) ---
        private void btnLoadData_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            List<CANDumpData> frames = new List<CANDumpData>();
            try
            {
                using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);

                    byte[] frameBuffer = new byte[17];
                    int counter = 0;

                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (i + 1 < bytes.Length && bytes[i] == 255 && bytes[i + 1] == 255)
                        {
                            if (counter == 17)
                            {
                                UInt32 time = BitConverter.ToUInt32(frameBuffer, 0);
                                frames.Add(new CANDumpData(
                                    time, frameBuffer[4], frameBuffer[5], frameBuffer[6], frameBuffer[7],
                                    frameBuffer[8], frameBuffer[9], frameBuffer[10], frameBuffer[11],
                                    frameBuffer[12], frameBuffer[13], frameBuffer[14], frameBuffer[15], frameBuffer[16]
                                ));
                            }
                            counter = 0;
                            i++;
                        }
                        else
                        {
                            if (counter < 17)
                            {
                                frameBuffer[counter] = bytes[i];
                                counter++;
                            }
                        }
                    }
                }
                CANdataTable = dataTableFromCAN(frames);
                dataGridView1.DataSource = CANdataTable;
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);

                FillTAble();
                UpdateParameterComboBoxes();
                PrepareDataAndTextures(); // Это вызовет и PrepareHistogramDataXY()
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading or processing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillTAble()
        {
            dataTable = new DataTable();
            dataTable.Columns.Add("TickStamp", typeof(UInt32));

            if (CANdataTable == null || CANdataTable.Rows.Count == 0) return;

            var uniqueTickStamps = CANdataTable.AsEnumerable()
                                           .Select(row => row.Field<UInt32>("TickStamp"))
                                           .Distinct()
                                           .OrderBy(t => t);

            foreach (UInt32 tickStamp in uniqueTickStamps)
            {
                DataRow[] rowsForTick = CANdataTable.Select($"TickStamp = {tickStamp}");
                DataRow newRow = dataTable.NewRow();
                newRow["TickStamp"] = tickStamp;

                foreach (DataRow canRow in rowsForTick)
                {
                    string paramName = $"{canRow["Source"]}=>{canRow["Dest"]}";
                    if (!dataTable.Columns.Contains(paramName))
                    {
                        dataTable.Columns.Add(paramName, typeof(byte));
                    }
                    if (canRow["b1"] != DBNull.Value)
                    {
                        newRow[paramName] = (byte)canRow["b1"];
                    }
                    else
                    {
                        newRow[paramName] = DBNull.Value;
                    }
                }
                dataTable.Rows.Add(newRow);
            }
            dataGridViewRaw.DataSource = dataTable;
            dataGridViewRaw.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }

        private DataTable dataTableFromCAN(List<CANDumpData> frames)
        {
            DataTable table = new DataTable();
            table.Columns.Add("TickStamp", typeof(UInt32));
            table.Columns.Add("Prefix", typeof(byte));
            table.Columns.Add("Format", typeof(byte));
            table.Columns.Add("Dest", typeof(byte));
            table.Columns.Add("Source", typeof(byte));
            table.Columns.Add("DLC", typeof(byte));
            table.Columns.Add("b1", typeof(byte)); table.Columns.Add("b2", typeof(byte));
            table.Columns.Add("b3", typeof(byte)); table.Columns.Add("b4", typeof(byte));
            table.Columns.Add("b5", typeof(byte)); table.Columns.Add("b6", typeof(byte));
            table.Columns.Add("b7", typeof(byte)); table.Columns.Add("b8", typeof(byte));

            foreach (CANDumpData data in frames)
            {
                table.Rows.Add(data.TickStamp, data.Prefix, data.Format, data.Dest, data.Source, data.DLC,
                               data.b1, data.b2, data.b3, data.b4, data.b5, data.b6, data.b7, data.b8);
            }
            return table;
        }


        private void UpdateParameterComboBoxes()
        {
            string oldX = cmbXAxis.SelectedItem?.ToString();
            string oldY = cmbYAxis.SelectedItem?.ToString();
            string oldZ = cmbZAxis.SelectedItem?.ToString();
            string oldTimeFace = cmbTimePlotParamFace?.SelectedItem?.ToString();
            string oldOutlierFace = cmbOutlierParamFace?.SelectedItem?.ToString();

            cmbXAxis.Items.Clear();
            cmbYAxis.Items.Clear();
            cmbZAxis.Items.Clear();
            if (cmbTimePlotParamFace != null) cmbTimePlotParamFace.Items.Clear();
            if (cmbOutlierParamFace != null) cmbOutlierParamFace.Items.Clear();

            if (dataTable != null)
            {
                var paramNames = dataTable.Columns.Cast<DataColumn>()
                                          .Where(c => c.ColumnName != "TickStamp" && c.DataType == typeof(byte))
                                          .Select(c => c.ColumnName)
                                          .OrderBy(name => name)
                                          .ToList();
                foreach (string name in paramNames)
                {
                    cmbXAxis.Items.Add(name);
                    cmbYAxis.Items.Add(name);
                    cmbZAxis.Items.Add(name);
                    if (cmbTimePlotParamFace != null) cmbTimePlotParamFace.Items.Add(name);
                    if (cmbOutlierParamFace != null) cmbOutlierParamFace.Items.Add(name);
                }

                SetComboBoxSelection(cmbXAxis, oldX, 0);
                SetComboBoxSelection(cmbYAxis, oldY, Math.Min(1, cmbYAxis.Items.Count - 1));
                SetComboBoxSelection(cmbZAxis, oldZ, Math.Min(2, cmbZAxis.Items.Count - 1));
                if (cmbTimePlotParamFace != null) SetComboBoxSelection(cmbTimePlotParamFace, oldTimeFace, 0);
                if (cmbOutlierParamFace != null) SetComboBoxSelection(cmbOutlierParamFace, oldOutlierFace, 0);
            }
        }

        private void SetComboBoxSelection(ComboBox cmb, string valueToSet, int defaultIndexIfNotFound)
        {
            if (cmb == null || cmb.Items.Count == 0) return;

            int index = -1;
            if (!string.IsNullOrEmpty(valueToSet))
            {
                index = cmb.FindStringExact(valueToSet);
            }

            if (index != -1)
            {
                cmb.SelectedIndex = index;
            }
            else if (defaultIndexIfNotFound >= 0 && cmb.Items.Count > defaultIndexIfNotFound)
            {
                cmb.SelectedIndex = defaultIndexIfNotFound;
            }
            else if (cmb.Items.Count > 0)
            {
                cmb.SelectedIndex = 0;
            }
        }

        private void cmbMainOrFaceParams_Changed(object sender, EventArgs e)
        {
            PrepareDataAndTextures();
        }

        private void chkShowCube_CheckedChanged(object sender, EventArgs e)
        {
            sharpGLControl1.Invalidate();
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap bmp = new Bitmap(sharpGLControl1.Width, sharpGLControl1.Height);
                    OpenGL gl = sharpGLControl1.OpenGL;

                    sharpGLControl1.DoRender();

                    gl.ReadBuffer(OpenGL.GL_FRONT);
                    BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                    ImageLockMode.WriteOnly,
                                                    PixelFormat.Format24bppRgb);

                    gl.ReadPixels(0, 0, bmp.Width, bmp.Height,
                                  OpenGL.GL_BGR,
                                  OpenGL.GL_UNSIGNED_BYTE,
                                  bmpData.Scan0);
                    bmp.UnlockBits(bmpData);

                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    bmp.Save(saveFileDialog1.FileName);
                    MessageBox.Show("Image saved to " + saveFileDialog1.FileName, "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bmp.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving image: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}