namespace WindowsFormsApplication1
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
      this.components = new System.ComponentModel.Container();
      this.drawingArea = new System.Windows.Forms.PictureBox();
      this.randomizeObstaclesButton = new System.Windows.Forms.Button();
      this.removeChainButton = new System.Windows.Forms.Button();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.showObstaclesCheckBox = new System.Windows.Forms.CheckBox();
      this.showSplineCheckBox = new System.Windows.Forms.CheckBox();
      this.showSplineHandlesCheckBox = new System.Windows.Forms.CheckBox();
      this.showClickPointsCheckBox = new System.Windows.Forms.CheckBox();
      this.showAdjustedJointLocationsCheckBox = new System.Windows.Forms.CheckBox();
      this.avoidObstaclesCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.drawingArea)).BeginInit();
      this.SuspendLayout();
      // 
      // drawingArea
      // 
      this.drawingArea.Location = new System.Drawing.Point(-4, 1);
      this.drawingArea.Name = "drawingArea";
      this.drawingArea.Size = new System.Drawing.Size(802, 572);
      this.drawingArea.TabIndex = 0;
      this.drawingArea.TabStop = false;
      this.drawingArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.drawingArea_MouseDown);
      this.drawingArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.drawingArea_MouseMove);
      this.drawingArea.MouseUp += new System.Windows.Forms.MouseEventHandler(this.drawingArea_MouseUp);
      // 
      // randomizeObstaclesButton
      // 
      this.randomizeObstaclesButton.Location = new System.Drawing.Point(870, 159);
      this.randomizeObstaclesButton.Name = "randomizeObstaclesButton";
      this.randomizeObstaclesButton.Size = new System.Drawing.Size(121, 23);
      this.randomizeObstaclesButton.TabIndex = 1;
      this.randomizeObstaclesButton.Text = "Randomize Obstacles";
      this.randomizeObstaclesButton.UseVisualStyleBackColor = true;
      this.randomizeObstaclesButton.Click += new System.EventHandler(this.randomizeObstaclesClick);
      // 
      // removeChainButton
      // 
      this.removeChainButton.Location = new System.Drawing.Point(870, 188);
      this.removeChainButton.Name = "removeChainButton";
      this.removeChainButton.Size = new System.Drawing.Size(121, 23);
      this.removeChainButton.TabIndex = 2;
      this.removeChainButton.Text = "Remove Chain";
      this.removeChainButton.UseVisualStyleBackColor = true;
      this.removeChainButton.Click += new System.EventHandler(this.removeChainButtonClick);
      // 
      // timer1
      // 
      this.timer1.Enabled = true;
      this.timer1.Interval = 10;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // vScrollBar1
      // 
      this.vScrollBar1.LargeChange = 1;
      this.vScrollBar1.Location = new System.Drawing.Point(818, 339);
      this.vScrollBar1.Minimum = -100;
      this.vScrollBar1.Name = "vScrollBar1";
      this.vScrollBar1.Size = new System.Drawing.Size(20, 163);
      this.vScrollBar1.TabIndex = 5;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(849, 135);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "label1";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(867, 236);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "label2";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(867, 249);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(35, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "label3";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(906, 271);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 9;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // showObstaclesCheckBox
      // 
      this.showObstaclesCheckBox.AutoSize = true;
      this.showObstaclesCheckBox.Checked = true;
      this.showObstaclesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showObstaclesCheckBox.Location = new System.Drawing.Point(822, 93);
      this.showObstaclesCheckBox.Name = "showObstaclesCheckBox";
      this.showObstaclesCheckBox.Size = new System.Drawing.Size(101, 17);
      this.showObstaclesCheckBox.TabIndex = 10;
      this.showObstaclesCheckBox.Text = "Show obstacles";
      this.showObstaclesCheckBox.UseVisualStyleBackColor = true;
      this.showObstaclesCheckBox.Click += new System.EventHandler(this.checkboxClick);
      // 
      // showSplineCheckBox
      // 
      this.showSplineCheckBox.AutoSize = true;
      this.showSplineCheckBox.Checked = true;
      this.showSplineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showSplineCheckBox.Location = new System.Drawing.Point(822, 47);
      this.showSplineCheckBox.Name = "showSplineCheckBox";
      this.showSplineCheckBox.Size = new System.Drawing.Size(83, 17);
      this.showSplineCheckBox.TabIndex = 11;
      this.showSplineCheckBox.Text = "Show spline";
      this.showSplineCheckBox.UseVisualStyleBackColor = true;
      this.showSplineCheckBox.Click += new System.EventHandler(this.checkboxClick);
      // 
      // showSplineHandlesCheckBox
      // 
      this.showSplineHandlesCheckBox.AutoSize = true;
      this.showSplineHandlesCheckBox.Checked = true;
      this.showSplineHandlesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showSplineHandlesCheckBox.Location = new System.Drawing.Point(822, 24);
      this.showSplineHandlesCheckBox.Name = "showSplineHandlesCheckBox";
      this.showSplineHandlesCheckBox.Size = new System.Drawing.Size(123, 17);
      this.showSplineHandlesCheckBox.TabIndex = 12;
      this.showSplineHandlesCheckBox.Text = "Show spline handles";
      this.showSplineHandlesCheckBox.UseVisualStyleBackColor = true;
      this.showSplineHandlesCheckBox.Click += new System.EventHandler(this.checkboxClick);
      // 
      // showClickPointsCheckBox
      // 
      this.showClickPointsCheckBox.AutoSize = true;
      this.showClickPointsCheckBox.Location = new System.Drawing.Point(822, 1);
      this.showClickPointsCheckBox.Name = "showClickPointsCheckBox";
      this.showClickPointsCheckBox.Size = new System.Drawing.Size(109, 17);
      this.showClickPointsCheckBox.TabIndex = 13;
      this.showClickPointsCheckBox.Text = "Show click points";
      this.showClickPointsCheckBox.UseVisualStyleBackColor = true;
      this.showClickPointsCheckBox.Click += new System.EventHandler(this.checkboxClick);
      // 
      // showAdjustedJointLocationsCheckBox
      // 
      this.showAdjustedJointLocationsCheckBox.AutoSize = true;
      this.showAdjustedJointLocationsCheckBox.Location = new System.Drawing.Point(822, 70);
      this.showAdjustedJointLocationsCheckBox.Name = "showAdjustedJointLocationsCheckBox";
      this.showAdjustedJointLocationsCheckBox.Size = new System.Drawing.Size(163, 17);
      this.showAdjustedJointLocationsCheckBox.TabIndex = 14;
      this.showAdjustedJointLocationsCheckBox.Text = "Show adjusted joint locations";
      this.showAdjustedJointLocationsCheckBox.UseVisualStyleBackColor = true;
      this.showAdjustedJointLocationsCheckBox.Click += new System.EventHandler(this.checkboxClick);
      // 
      // avoidObstaclesCheckBox
      // 
      this.avoidObstaclesCheckBox.AutoSize = true;
      this.avoidObstaclesCheckBox.Location = new System.Drawing.Point(822, 115);
      this.avoidObstaclesCheckBox.Name = "avoidObstaclesCheckBox";
      this.avoidObstaclesCheckBox.Size = new System.Drawing.Size(101, 17);
      this.avoidObstaclesCheckBox.TabIndex = 15;
      this.avoidObstaclesCheckBox.Text = "Avoid obstacles";
      this.avoidObstaclesCheckBox.UseVisualStyleBackColor = true;
      this.avoidObstaclesCheckBox.CheckStateChanged += new System.EventHandler(this.AvoidObstaclesCheckStateChanged);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1003, 585);
      this.Controls.Add(this.avoidObstaclesCheckBox);
      this.Controls.Add(this.showAdjustedJointLocationsCheckBox);
      this.Controls.Add(this.showClickPointsCheckBox);
      this.Controls.Add(this.showSplineHandlesCheckBox);
      this.Controls.Add(this.showSplineCheckBox);
      this.Controls.Add(this.showObstaclesCheckBox);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.vScrollBar1);
      this.Controls.Add(this.removeChainButton);
      this.Controls.Add(this.randomizeObstaclesButton);
      this.Controls.Add(this.drawingArea);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
      ((System.ComponentModel.ISupportInitialize)(this.drawingArea)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.PictureBox drawingArea;
      private System.Windows.Forms.Button randomizeObstaclesButton;
      private System.Windows.Forms.Button removeChainButton;
      private System.Windows.Forms.Timer timer1;
      private System.Windows.Forms.VScrollBar vScrollBar1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.CheckBox showObstaclesCheckBox;
      private System.Windows.Forms.CheckBox showSplineCheckBox;
      private System.Windows.Forms.CheckBox showSplineHandlesCheckBox;
      private System.Windows.Forms.CheckBox showClickPointsCheckBox;
      private System.Windows.Forms.CheckBox showAdjustedJointLocationsCheckBox;
      private System.Windows.Forms.CheckBox avoidObstaclesCheckBox;
   }
}

