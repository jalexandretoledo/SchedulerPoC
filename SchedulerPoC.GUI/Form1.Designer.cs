namespace SchedulerPoC.GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gridTasks = new DataGridView();
            btnAddPyTask = new Button();
            btnAddXLTask = new Button();
            button3 = new Button();
            button4 = new Button();
            btnLoad = new Button();
            btnSave = new Button();
            lblTaskFile = new Label();
            dlgOpen = new OpenFileDialog();
            TaskId = new DataGridViewTextBoxColumn();
            ScheduleTime = new DataGridViewTextBoxColumn();
            Description = new DataGridViewTextBoxColumn();
            Status = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)gridTasks).BeginInit();
            SuspendLayout();
            // 
            // gridTasks
            // 
            gridTasks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridTasks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridTasks.Columns.AddRange(new DataGridViewColumn[] { TaskId, ScheduleTime, Description, Status });
            gridTasks.Location = new Point(12, 12);
            gridTasks.Name = "gridTasks";
            gridTasks.RowTemplate.Height = 25;
            gridTasks.Size = new Size(776, 344);
            gridTasks.TabIndex = 0;
            // 
            // btnAddPyTask
            // 
            btnAddPyTask.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddPyTask.Location = new Point(12, 386);
            btnAddPyTask.Name = "btnAddPyTask";
            btnAddPyTask.Size = new Size(203, 23);
            btnAddPyTask.TabIndex = 1;
            btnAddPyTask.Text = "Add Python Task";
            btnAddPyTask.UseVisualStyleBackColor = true;
            // 
            // btnAddXLTask
            // 
            btnAddXLTask.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddXLTask.Location = new Point(12, 415);
            btnAddXLTask.Name = "btnAddXLTask";
            btnAddXLTask.Size = new Size(203, 23);
            btnAddXLTask.TabIndex = 2;
            btnAddXLTask.Text = "Add Excel Task";
            btnAddXLTask.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button3.Location = new Point(221, 386);
            button3.Name = "button3";
            button3.Size = new Size(203, 23);
            button3.TabIndex = 3;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Location = new Point(221, 415);
            button4.Name = "button4";
            button4.Size = new Size(203, 23);
            button4.TabIndex = 4;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            btnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoad.Location = new Point(430, 386);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(203, 23);
            btnLoad.TabIndex = 5;
            btnLoad.Text = "Load tasks from file";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSave.Location = new Point(430, 415);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(203, 23);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save tasks to file";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // lblTaskFile
            // 
            lblTaskFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblTaskFile.AutoSize = true;
            lblTaskFile.Location = new Point(714, 359);
            lblTaskFile.Name = "lblTaskFile";
            lblTaskFile.Size = new Size(74, 15);
            lblTaskFile.TabIndex = 7;
            lblTaskFile.Text = "<not saved>";
            // 
            // dlgOpen
            // 
            dlgOpen.DefaultExt = "tasks";
            dlgOpen.FileName = "sample.tasks";
            dlgOpen.Filter = "Task Definition Files|*.tasks|All files|*.*";
            dlgOpen.Title = "Select a Task Definition file...";
            // 
            // TaskId
            // 
            TaskId.DataPropertyName = "TaskId";
            TaskId.Frozen = true;
            TaskId.HeaderText = "Task Id";
            TaskId.Name = "TaskId";
            TaskId.ReadOnly = true;
            // 
            // ScheduleTime
            // 
            ScheduleTime.DataPropertyName = "Time";
            ScheduleTime.Frozen = true;
            ScheduleTime.HeaderText = "Time";
            ScheduleTime.Name = "ScheduleTime";
            ScheduleTime.ReadOnly = true;
            // 
            // Description
            // 
            Description.DataPropertyName = "Description";
            Description.HeaderText = "Task Description";
            Description.Name = "Description";
            Description.ReadOnly = true;
            // 
            // Status
            // 
            Status.DataPropertyName = "Status";
            Status.HeaderText = "Status";
            Status.Name = "Status";
            Status.ReadOnly = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblTaskFile);
            Controls.Add(btnSave);
            Controls.Add(btnLoad);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(btnAddXLTask);
            Controls.Add(btnAddPyTask);
            Controls.Add(gridTasks);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)gridTasks).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView gridTasks;
        private Button btnAddPyTask;
        private Button btnAddXLTask;
        private Button button3;
        private Button button4;
        private Button btnLoad;
        private Button btnSave;
        private Label lblTaskFile;
        private OpenFileDialog dlgOpen;
        private DataGridViewTextBoxColumn TaskId;
        private DataGridViewTextBoxColumn ScheduleTime;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewTextBoxColumn Status;
    }
}