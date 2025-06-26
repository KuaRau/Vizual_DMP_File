namespace L4_Four
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnLoadData = new System.Windows.Forms.Button();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.cmbXAxis = new System.Windows.Forms.ComboBox();
            this.cmbYAxis = new System.Windows.Forms.ComboBox();
            this.cmbZAxis = new System.Windows.Forms.ComboBox();
            this.lblXAxis = new System.Windows.Forms.Label();
            this.lblYAxis = new System.Windows.Forms.Label();
            this.lblZAxis = new System.Windows.Forms.Label();
            this.chkShowCube = new System.Windows.Forms.CheckBox();
            this.splitContainerRight = new System.Windows.Forms.SplitContainer();
            this.sharpGLControl1 = new SharpGL.OpenGLControl();
            this.groupBoxControls = new System.Windows.Forms.GroupBox();
            this.chkShowHistogramSurface = new System.Windows.Forms.CheckBox();
            this.lblOutlierParamFace = new System.Windows.Forms.Label();
            this.cmbOutlierParamFace = new System.Windows.Forms.ComboBox();
            this.lblTimePlotParamFace = new System.Windows.Forms.Label();
            this.cmbTimePlotParamFace = new System.Windows.Forms.ComboBox();
            this.lblFaceTypeZ = new System.Windows.Forms.Label();
            this.cmbFaceTypeZ = new System.Windows.Forms.ComboBox();
            this.lblFaceTypeY = new System.Windows.Forms.Label();
            this.cmbFaceTypeY = new System.Windows.Forms.ComboBox();
            this.lblFaceTypeX = new System.Windows.Forms.Label();
            this.cmbFaceTypeX = new System.Windows.Forms.ComboBox();
            this.splitContainerDataTables = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblRawCanData = new System.Windows.Forms.Label();
            this.dataGridViewRaw = new System.Windows.Forms.DataGridView();
            this.lblProcessedData = new System.Windows.Forms.Label();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).BeginInit();
            this.splitContainerRight.Panel1.SuspendLayout();
            this.splitContainerRight.Panel2.SuspendLayout();
            this.splitContainerRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sharpGLControl1)).BeginInit();
            this.groupBoxControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDataTables)).BeginInit();
            this.splitContainerDataTables.Panel1.SuspendLayout();
            this.splitContainerDataTables.Panel2.SuspendLayout();
            this.splitContainerDataTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Dump Files (*.dmp)|*.dmp|All Files (*.*)|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|Bitmap Image (*.bmp)|*.bmp";
            // 
            // btnLoadData
            // 
            this.btnLoadData.Location = new System.Drawing.Point(12, 12);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(100, 23);
            this.btnLoadData.TabIndex = 1;
            this.btnLoadData.Text = "Load Data";
            this.btnLoadData.UseVisualStyleBackColor = true;
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.Location = new System.Drawing.Point(118, 12);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(100, 23);
            this.btnSaveImage.TabIndex = 2;
            this.btnSaveImage.Text = "Save Image";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            // 
            // cmbXAxis
            // 
            this.cmbXAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbXAxis.FormattingEnabled = true;
            this.cmbXAxis.Location = new System.Drawing.Point(70, 19);
            this.cmbXAxis.Name = "cmbXAxis";
            this.cmbXAxis.Size = new System.Drawing.Size(121, 21);
            this.cmbXAxis.TabIndex = 0;
            // 
            // cmbYAxis
            // 
            this.cmbYAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYAxis.FormattingEnabled = true;
            this.cmbYAxis.Location = new System.Drawing.Point(70, 46);
            this.cmbYAxis.Name = "cmbYAxis";
            this.cmbYAxis.Size = new System.Drawing.Size(121, 21);
            this.cmbYAxis.TabIndex = 2;
            // 
            // cmbZAxis
            // 
            this.cmbZAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZAxis.FormattingEnabled = true;
            this.cmbZAxis.Location = new System.Drawing.Point(70, 73);
            this.cmbZAxis.Name = "cmbZAxis";
            this.cmbZAxis.Size = new System.Drawing.Size(121, 21);
            this.cmbZAxis.TabIndex = 4;
            // 
            // lblXAxis
            // 
            this.lblXAxis.AutoSize = true;
            this.lblXAxis.Location = new System.Drawing.Point(6, 22);
            this.lblXAxis.Name = "lblXAxis";
            this.lblXAxis.Size = new System.Drawing.Size(56, 13);
            this.lblXAxis.TabIndex = 8;
            this.lblXAxis.Text = "3D X-Axis:";
            // 
            // lblYAxis
            // 
            this.lblYAxis.AutoSize = true;
            this.lblYAxis.Location = new System.Drawing.Point(6, 49);
            this.lblYAxis.Name = "lblYAxis";
            this.lblYAxis.Size = new System.Drawing.Size(56, 13);
            this.lblYAxis.TabIndex = 9;
            this.lblYAxis.Text = "3D Y-Axis:";
            // 
            // lblZAxis
            // 
            this.lblZAxis.AutoSize = true;
            this.lblZAxis.Location = new System.Drawing.Point(6, 76);
            this.lblZAxis.Name = "lblZAxis";
            this.lblZAxis.Size = new System.Drawing.Size(56, 13);
            this.lblZAxis.TabIndex = 10;
            this.lblZAxis.Text = "3D Z-Axis:";
            // 
            // chkShowCube
            // 
            this.chkShowCube.AutoSize = true;
            this.chkShowCube.Checked = true;
            this.chkShowCube.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowCube.Location = new System.Drawing.Point(9, 100);
            this.chkShowCube.Name = "chkShowCube";
            this.chkShowCube.Size = new System.Drawing.Size(117, 17);
            this.chkShowCube.TabIndex = 6;
            this.chkShowCube.Text = "Show Cube Outline";
            this.chkShowCube.UseVisualStyleBackColor = true;
            // 
            // splitContainerRight
            // 
            this.splitContainerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRight.Name = "splitContainerRight";
            this.splitContainerRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerRight.Panel1
            // 
            this.splitContainerRight.Panel1.Controls.Add(this.sharpGLControl1);
            // 
            // splitContainerRight.Panel2
            // 
            this.splitContainerRight.Panel2.Controls.Add(this.groupBoxControls);
            this.splitContainerRight.Size = new System.Drawing.Size(643, 531);
            this.splitContainerRight.SplitterDistance = 338;
            this.splitContainerRight.SplitterWidth = 3;
            this.splitContainerRight.TabIndex = 0;
            // 
            // sharpGLControl1
            // 
            this.sharpGLControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sharpGLControl1.DrawFPS = false;
            this.sharpGLControl1.FrameRate = 60;
            this.sharpGLControl1.Location = new System.Drawing.Point(0, 0);
            this.sharpGLControl1.Name = "sharpGLControl1";
            this.sharpGLControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.sharpGLControl1.RenderContextType = SharpGL.RenderContextType.FBO;
            this.sharpGLControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.sharpGLControl1.Size = new System.Drawing.Size(643, 338);
            this.sharpGLControl1.TabIndex = 0;
            this.sharpGLControl1.OpenGLInitialized += new System.EventHandler(this.sharpGLControl1_OpenGLInitialized);
            this.sharpGLControl1.OpenGLDraw += new SharpGL.RenderEventHandler(this.sharpGLControl1_OpenGLDraw);
            this.sharpGLControl1.Resized += new System.EventHandler(this.sharpGLControl1_Resize);
            this.sharpGLControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sharpGLControl1_MouseDown);
            this.sharpGLControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.sharpGLControl1_MouseMove);
            this.sharpGLControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sharpGLControl1_MouseUp);
            this.sharpGLControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.sharpGLControl1_MouseWheel);
            // 
            // groupBoxControls
            // 
            this.groupBoxControls.Controls.Add(this.chkShowHistogramSurface);
            this.groupBoxControls.Controls.Add(this.lblOutlierParamFace);
            this.groupBoxControls.Controls.Add(this.cmbOutlierParamFace);
            this.groupBoxControls.Controls.Add(this.lblTimePlotParamFace);
            this.groupBoxControls.Controls.Add(this.cmbTimePlotParamFace);
            this.groupBoxControls.Controls.Add(this.lblFaceTypeZ);
            this.groupBoxControls.Controls.Add(this.cmbFaceTypeZ);
            this.groupBoxControls.Controls.Add(this.lblFaceTypeY);
            this.groupBoxControls.Controls.Add(this.cmbFaceTypeY);
            this.groupBoxControls.Controls.Add(this.lblFaceTypeX);
            this.groupBoxControls.Controls.Add(this.cmbFaceTypeX);
            this.groupBoxControls.Controls.Add(this.cmbYAxis);
            this.groupBoxControls.Controls.Add(this.cmbXAxis);
            this.groupBoxControls.Controls.Add(this.lblZAxis);
            this.groupBoxControls.Controls.Add(this.lblXAxis);
            this.groupBoxControls.Controls.Add(this.lblYAxis);
            this.groupBoxControls.Controls.Add(this.cmbZAxis);
            this.groupBoxControls.Controls.Add(this.chkShowCube);
            this.groupBoxControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxControls.Location = new System.Drawing.Point(0, 0);
            this.groupBoxControls.Name = "groupBoxControls";
            this.groupBoxControls.Size = new System.Drawing.Size(643, 190);
            this.groupBoxControls.TabIndex = 12;
            this.groupBoxControls.TabStop = false;
            this.groupBoxControls.Text = "Display Options";
            // 
            // chkShowHistogramSurface
            // 
            this.chkShowHistogramSurface.AutoSize = true;
            this.chkShowHistogramSurface.Location = new System.Drawing.Point(220, 20);
            this.chkShowHistogramSurface.Name = "chkShowHistogramSurface";
            this.chkShowHistogramSurface.Size = new System.Drawing.Size(80, 17);
            this.chkShowHistogramSurface.TabIndex = 23;
            this.chkShowHistogramSurface.Text = "checkBox1";
            this.chkShowHistogramSurface.UseVisualStyleBackColor = true;
            // 
            // lblOutlierParamFace
            // 
            this.lblOutlierParamFace.AutoSize = true;
            this.lblOutlierParamFace.Location = new System.Drawing.Point(219, 76);
            this.lblOutlierParamFace.Name = "lblOutlierParamFace";
            this.lblOutlierParamFace.Size = new System.Drawing.Size(100, 13);
            this.lblOutlierParamFace.TabIndex = 22;
            this.lblOutlierParamFace.Text = "Face Outlier Param:";
            // 
            // cmbOutlierParamFace
            // 
            this.cmbOutlierParamFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutlierParamFace.FormattingEnabled = true;
            this.cmbOutlierParamFace.Location = new System.Drawing.Point(323, 73);
            this.cmbOutlierParamFace.Name = "cmbOutlierParamFace";
            this.cmbOutlierParamFace.Size = new System.Drawing.Size(121, 21);
            this.cmbOutlierParamFace.TabIndex = 11;
            // 
            // lblTimePlotParamFace
            // 
            this.lblTimePlotParamFace.AutoSize = true;
            this.lblTimePlotParamFace.Location = new System.Drawing.Point(219, 49);
            this.lblTimePlotParamFace.Name = "lblTimePlotParamFace";
            this.lblTimePlotParamFace.Size = new System.Drawing.Size(93, 13);
            this.lblTimePlotParamFace.TabIndex = 20;
            this.lblTimePlotParamFace.Text = "Face Time Param:";
            // 
            // cmbTimePlotParamFace
            // 
            this.cmbTimePlotParamFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimePlotParamFace.FormattingEnabled = true;
            this.cmbTimePlotParamFace.Location = new System.Drawing.Point(323, 46);
            this.cmbTimePlotParamFace.Name = "cmbTimePlotParamFace";
            this.cmbTimePlotParamFace.Size = new System.Drawing.Size(121, 21);
            this.cmbTimePlotParamFace.TabIndex = 9;
            // 
            // lblFaceTypeZ
            // 
            this.lblFaceTypeZ.AutoSize = true;
            this.lblFaceTypeZ.Location = new System.Drawing.Point(219, 159);
            this.lblFaceTypeZ.Name = "lblFaceTypeZ";
            this.lblFaceTypeZ.Size = new System.Drawing.Size(70, 13);
            this.lblFaceTypeZ.TabIndex = 18;
            this.lblFaceTypeZ.Text = "Z-Faces Plot:";
            // 
            // cmbFaceTypeZ
            // 
            this.cmbFaceTypeZ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFaceTypeZ.FormattingEnabled = true;
            this.cmbFaceTypeZ.Location = new System.Drawing.Point(323, 156);
            this.cmbFaceTypeZ.Name = "cmbFaceTypeZ";
            this.cmbFaceTypeZ.Size = new System.Drawing.Size(121, 21);
            this.cmbFaceTypeZ.TabIndex = 15;
            // 
            // lblFaceTypeY
            // 
            this.lblFaceTypeY.AutoSize = true;
            this.lblFaceTypeY.Location = new System.Drawing.Point(219, 132);
            this.lblFaceTypeY.Name = "lblFaceTypeY";
            this.lblFaceTypeY.Size = new System.Drawing.Size(70, 13);
            this.lblFaceTypeY.TabIndex = 16;
            this.lblFaceTypeY.Text = "Y-Faces Plot:";
            // 
            // cmbFaceTypeY
            // 
            this.cmbFaceTypeY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFaceTypeY.FormattingEnabled = true;
            this.cmbFaceTypeY.Location = new System.Drawing.Point(323, 129);
            this.cmbFaceTypeY.Name = "cmbFaceTypeY";
            this.cmbFaceTypeY.Size = new System.Drawing.Size(121, 21);
            this.cmbFaceTypeY.TabIndex = 14;
            // 
            // lblFaceTypeX
            // 
            this.lblFaceTypeX.AutoSize = true;
            this.lblFaceTypeX.Location = new System.Drawing.Point(219, 105);
            this.lblFaceTypeX.Name = "lblFaceTypeX";
            this.lblFaceTypeX.Size = new System.Drawing.Size(70, 13);
            this.lblFaceTypeX.TabIndex = 14;
            this.lblFaceTypeX.Text = "X-Faces Plot:";
            // 
            // cmbFaceTypeX
            // 
            this.cmbFaceTypeX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFaceTypeX.FormattingEnabled = true;
            this.cmbFaceTypeX.Location = new System.Drawing.Point(323, 102);
            this.cmbFaceTypeX.Name = "cmbFaceTypeX";
            this.cmbFaceTypeX.Size = new System.Drawing.Size(121, 21);
            this.cmbFaceTypeX.TabIndex = 13;
            // 
            // splitContainerDataTables
            // 
            this.splitContainerDataTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDataTables.Location = new System.Drawing.Point(0, 0);
            this.splitContainerDataTables.Name = "splitContainerDataTables";
            this.splitContainerDataTables.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerDataTables.Panel1
            // 
            this.splitContainerDataTables.Panel1.Controls.Add(this.dataGridView1);
            this.splitContainerDataTables.Panel1.Controls.Add(this.lblRawCanData);
            // 
            // splitContainerDataTables.Panel2
            // 
            this.splitContainerDataTables.Panel2.Controls.Add(this.dataGridViewRaw);
            this.splitContainerDataTables.Panel2.Controls.Add(this.lblProcessedData);
            this.splitContainerDataTables.Size = new System.Drawing.Size(297, 531);
            this.splitContainerDataTables.SplitterDistance = 242;
            this.splitContainerDataTables.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(297, 229);
            this.dataGridView1.TabIndex = 3;
            // 
            // lblRawCanData
            // 
            this.lblRawCanData.AutoSize = true;
            this.lblRawCanData.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRawCanData.Location = new System.Drawing.Point(0, 0);
            this.lblRawCanData.Name = "lblRawCanData";
            this.lblRawCanData.Size = new System.Drawing.Size(80, 13);
            this.lblRawCanData.TabIndex = 13;
            this.lblRawCanData.Text = "Raw CAN Data";
            // 
            // dataGridViewRaw
            // 
            this.dataGridViewRaw.AllowUserToAddRows = false;
            this.dataGridViewRaw.AllowUserToDeleteRows = false;
            this.dataGridViewRaw.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRaw.Location = new System.Drawing.Point(0, 13);
            this.dataGridViewRaw.Name = "dataGridViewRaw";
            this.dataGridViewRaw.ReadOnly = true;
            this.dataGridViewRaw.RowHeadersWidth = 51;
            this.dataGridViewRaw.Size = new System.Drawing.Size(297, 272);
            this.dataGridViewRaw.TabIndex = 4;
            // 
            // lblProcessedData
            // 
            this.lblProcessedData.AutoSize = true;
            this.lblProcessedData.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProcessedData.Location = new System.Drawing.Point(0, 0);
            this.lblProcessedData.Name = "lblProcessedData";
            this.lblProcessedData.Size = new System.Drawing.Size(83, 13);
            this.lblProcessedData.TabIndex = 14;
            this.lblProcessedData.Text = "Processed Data";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(12, 41);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerDataTables);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerRight);
            this.splitContainerMain.Size = new System.Drawing.Size(943, 531);
            this.splitContainerMain.SplitterDistance = 297;
            this.splitContainerMain.SplitterWidth = 3;
            this.splitContainerMain.TabIndex = 15;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 607);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.btnLoadData);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Form1";
            this.Text = "Lab 4 - Мезенцев";
            this.splitContainerRight.Panel1.ResumeLayout(false);
            this.splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).EndInit();
            this.splitContainerRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sharpGLControl1)).EndInit();
            this.groupBoxControls.ResumeLayout(false);
            this.groupBoxControls.PerformLayout();
            this.splitContainerDataTables.Panel1.ResumeLayout(false);
            this.splitContainerDataTables.Panel1.PerformLayout();
            this.splitContainerDataTables.Panel2.ResumeLayout(false);
            this.splitContainerDataTables.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDataTables)).EndInit();
            this.splitContainerDataTables.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRaw)).EndInit();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.ComboBox cmbXAxis;
        private System.Windows.Forms.ComboBox cmbYAxis;
        private System.Windows.Forms.ComboBox cmbZAxis;
        private System.Windows.Forms.Label lblXAxis;
        private System.Windows.Forms.Label lblYAxis;
        private System.Windows.Forms.Label lblZAxis;
        private System.Windows.Forms.CheckBox chkShowCube;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private SharpGL.OpenGLControl sharpGLControl1;
        private System.Windows.Forms.GroupBox groupBoxControls;
        private System.Windows.Forms.SplitContainer splitContainerDataTables;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblRawCanData;
        private System.Windows.Forms.DataGridView dataGridViewRaw;
        private System.Windows.Forms.Label lblProcessedData;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Label lblFaceTypeZ;
        private System.Windows.Forms.ComboBox cmbFaceTypeZ;
        private System.Windows.Forms.Label lblFaceTypeY;
        private System.Windows.Forms.ComboBox cmbFaceTypeY;
        private System.Windows.Forms.Label lblFaceTypeX;
        private System.Windows.Forms.ComboBox cmbFaceTypeX;
        private System.Windows.Forms.Label lblOutlierParamFace;
        private System.Windows.Forms.ComboBox cmbOutlierParamFace;
        private System.Windows.Forms.Label lblTimePlotParamFace;
        private System.Windows.Forms.ComboBox cmbTimePlotParamFace;
        private System.Windows.Forms.CheckBox chkShowHistogramSurface;
    }
}